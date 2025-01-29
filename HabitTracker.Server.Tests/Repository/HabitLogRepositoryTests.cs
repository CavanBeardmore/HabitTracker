using HabitTracker.Server.DTOs;
using HabitTracker.Server.Facade;
using HabitTracker.Server.Models;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Transformer;
using Moq;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HabitTracker.Server.Tests.Repository
{
    public class HabitLogRepositoryTests
    {
        private readonly HabitLogRepository _repository;
        private readonly Mock<IStorage> _mockFacade;
        private readonly Mock<ITransformer<HabitLog, IDataReader>> _mockTransformer;

        public HabitLogRepositoryTests()
        {
            _mockTransformer = new Mock<ITransformer<HabitLog, IDataReader>>();
            _mockFacade = new Mock<IStorage>();
            _repository = new HabitLogRepository(_mockFacade.Object, _mockTransformer.Object);
        }

        [Fact]
        public void GetAllByHabitId_ReturnsCollectionOfHabitLogs()
        {
            int id = 1234;
            int userId = 4321;
            int pageNumber = 1;

            var expectedHabitLogs = new List<HabitLog> { new HabitLog(id, 2341, DateTime.Now, true, 1) };

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Habit_id = @id AND u.Id = @userId ORDER BY Start_date DESC LIMIT 30 OFFSET @offset;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", id },
                { "@userId", userId },
                {"@offset", pageNumber }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(expectedHabitLogs);

            var result = _repository.GetAllByHabitId(id, userId, pageNumber);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, hl => hl.Id == expectedHabitLogs[0].Id
                                          && hl.Habit_id == expectedHabitLogs[0].Habit_id
                                          && hl.Start_date == expectedHabitLogs[0].Start_date
                                          && hl.Habit_logged == expectedHabitLogs[0].Habit_logged
                                          && hl.LengthInDays == expectedHabitLogs[0].LengthInDays);
        }

        [Fact]
        public void GetById_ReturnsHabitLog()
        {
            int habitLogId = 1234;
            int userId = 4321;

            DateTime date = DateTime.Now;

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Id = @id AND u.Id = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitLogId },
                { "@userId", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(new List<HabitLog> { new HabitLog(habitLogId, 2341, date, true, 1) });

            var result = _repository.GetById(habitLogId, userId);

            Assert.NotNull(result);
            Assert.True(result.Id == habitLogId);
            Assert.True(result.Habit_id == 2341);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged);
            Assert.True(result.LengthInDays == 1);
        }
        [Fact]
        public void GetById_ReturnsNull()
        {
            int habitLogId = 1234;
            int userId = 4321;

            DateTime date = DateTime.Now;

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Id = @id AND u.Id = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitLogId },
                { "@userId", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(new List<HabitLog>());

            var result = _repository.GetById(habitLogId, userId);

            Assert.Null(result);
        }

        [Fact]
        public void GetMostRecentHabitLog_ReturnsHabitLog()
        {
            int habitId = 1234;
            int userId = 4321;

            DateTime date = DateTime.Now;

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id ORDER BY Start_date DESC LIMIT 1";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(new List<HabitLog> { new HabitLog(1234, 2341, date, true, 1) });

            var result = _repository.GetMostRecentHabitLog(habitId, userId);

            Assert.NotNull(result);
            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.Habit_id == 2341);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged);
            Assert.True(result.LengthInDays == 1);
        }

        [Fact]
        public void GetMostRecentHabitLog_ReturnsNull()
        {
            int habitId = 1234;
            int userId = 4321;

            DateTime date = DateTime.Now;

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id ORDER BY Start_date DESC LIMIT 1";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(new List<HabitLog>());

            var result = _repository.GetMostRecentHabitLog(habitId, userId);

            Assert.Null(result);
        }

        [Fact]
        public void Add_ReturnsTrue()
        {
            DateTime date = DateTime.Now;

            PostHabitLog habitLog = new PostHabitLog(1234, 2341, date, true, 1);

            string query = "INSERT INTO HabitLogs (Habit_id, Start_date, Habit_logged, Length_in_days) VALUES (@Habit_id, @Start_date, @Habit_logged, @Length_in_days);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Habit_id", habitLog.Habit_id },
                { "@Start_date", habitLog.Start_date },
                { "@Habit_logged", habitLog.Habit_logged },
                { "@Length_in_days", habitLog.Length_in_days }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Add(habitLog);

            Assert.True(result);
        }

        [Fact]
        public void Add_ReturnsFalse()
        {
            DateTime date = DateTime.Now;

            PostHabitLog habitLog = new PostHabitLog(1234, 2341, date, true, 1);

            string query = "INSERT INTO HabitLogs (Habit_id, Start_date, Habit_logged, Length_in_days) VALUES (@Habit_id, @Start_date, @Habit_logged, @Length_in_days);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Habit_id", habitLog.Habit_id },
                { "@Start_date", habitLog.Start_date },
                { "@Habit_logged", habitLog.Habit_logged },
                { "@Length_in_days", habitLog.Length_in_days }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Add(habitLog);

            Assert.False(result);
        }

        [Fact]
        public void Update_ReturnsTrue()
        {
            DateTime date = DateTime.Now;

            PatchHabitLog habitLog = new PatchHabitLog(1234, 2341, 4321, true);

            string query = "UPDATE HabitLogs SET Habit_logged = @Habit_logged WHERE Id = @Id;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Id", habitLog.Id },
                { "@Habit_logged", habitLog.Habit_logged },
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Update(habitLog);

            Assert.True(result);
        }

        [Fact]
        public void Update_ReturnsFalse()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1234, 2341, 4321, true);

            string query = "UPDATE HabitLogs SET Habit_logged = @Habit_logged WHERE Id = @Id;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Id", habitLog.Id },
                { "@Habit_logged", habitLog.Habit_logged },
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Update(habitLog);

            Assert.False(result);
        }

        [Fact]
        public void Delete_ReturnsTrue()
        {
            int habitLogId = 1234;
            int userId = 4321;

            string query = "DELETE FROM HabitLogs WHERE Id = @id AND Habit_id IN (SELECT h.Id FROM Habits h INNER JOIN Users u ON h.User_id = u.Id WHERE u.Id = @userId);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitLogId },
                { "@userId", userId },
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Delete(habitLogId, userId);

            Assert.True(result);
        }

        [Fact]
        public void Delete_ReturnsFalse()
        {
            int habitLogId = 1234;
            int userId = 4321;

            string query = "DELETE FROM HabitLogs WHERE Id = @id AND Habit_id IN (SELECT h.Id FROM Habits h INNER JOIN Users u ON h.User_id = u.Id WHERE u.Id = @userId);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitLogId },
                { "@userId", userId },
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Delete(habitLogId, userId);

            Assert.False(result);
        }

        [Fact]
        public void DeleteByHabitIdAndStartDate_ReturnsTrue()
        {
            int habitId = 1234;
            DateTime startDate = DateTime.Now;

            string query = "DELETE FROM HabitLogs WHERE HabitLogs.Habit_id = @habitId AND HabitLogs.Start_date = @startDate;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@habitId", habitId },
                { "@startDate", startDate },
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.DeleteByHabitIdAndStartDate(habitId, startDate);

            Assert.True(result);
        }

        [Fact]
        public void DeleteByHabitIdAndStartDate_ReturnsFalse()
        {
            int habitId = 1234;
            DateTime startDate = DateTime.Now;

            string query = "DELETE FROM HabitLogs WHERE HabitLogs.Habit_id = @habitId AND HabitLogs.Start_date = @startDate;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@habitId", habitId },
                { "@startDate", startDate },
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.DeleteByHabitIdAndStartDate(habitId, startDate);

            Assert.False(result);
        }
    }
}
