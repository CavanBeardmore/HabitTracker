using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Storage;
using HabitTracker.Server.Transformer;
using Moq;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HabitTracker.Server.Tests.Repository
{
    public class HabitLogRepositoryTests
    {
        private readonly HabitLogRepository _repository;
        private readonly Mock<IStorage> _mockFacade;
        private readonly Mock<ITransformer<IReadOnlyCollection<HabitLog>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>> _mockTransformer;

        public HabitLogRepositoryTests()
        {
            _mockTransformer = new Mock<ITransformer<IReadOnlyCollection<HabitLog>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>>();
            _mockFacade = new Mock<IStorage>();
            _repository = new HabitLogRepository(_mockFacade.Object, _mockTransformer.Object);
        }

        [Fact]
        public void GetByHabitIdAndStartDate_ReturnsHabitLog()
        {
            int habitId = 1;
            int userId = 2;
            DateTime date = DateTime.UtcNow;

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Habit_id = @habitId AND hl.Start_date = @date AND u.Id = @userId AND u.IsDeleted = 0"; ;

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@habitId", habitId },
                { "@userId", userId },
                { "@date", date }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            facadeData.Add(new Dictionary<string, object>{
                { "Id", 1234 },
                { "Habit_id", habitId },
                { "Start_date", date},
                { "Habit_logged", true },
                { "Length_in_days", 7 }
            });

            List<HabitLog> transformerData = new List<HabitLog> { new HabitLog(1234, habitId, date, true, 7) };


            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetByHabitIdAndStartDate(habitId, userId, date);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.Habit_id == habitId);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged == true);
            Assert.True(result.LengthInDays == 7);
        }

        [Fact]
        public void GetByHabitIdAndStartDate_ReturnsNull()
        {
            int habitId = 1;
            int userId = 2;
            DateTime date = DateTime.UtcNow;

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Habit_id = @habitId AND hl.Start_date = @date AND u.Id = @userId AND u.IsDeleted = 0"; ;

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@habitId", habitId },
                { "@userId", userId },
                { "@date", date }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();

            List<HabitLog> transformerData = new List<HabitLog>();


            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetByHabitIdAndStartDate(habitId, userId, date);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllByHabitId_ReturnsCollectionOfHabitLogs()
        {
            int id = 1234;
            int userId = 4321;
            uint pageNumber = 1;
            uint limit = 2;
            uint offset = limit * pageNumber;
            DateTime date = DateTime.UtcNow;

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>
            {
                new Dictionary<string, object>{
                    { "Id", 1 },
                    { "Habit_id", id },
                    { "Start_date", date},
                    { "Habit_logged", true },
                    { "Length_in_days", 7 }
                }
            };

            IReadOnlyCollection<HabitLog> transformerData = new List<HabitLog> { new HabitLog(1, id, date, true, 7) };

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Habit_id = @id AND u.Id = @userId AND u.IsDeleted = 0 ORDER BY Start_date DESC LIMIT @limit OFFSET @offset;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", id },
                { "@userId", userId },
                { "@offset", offset },
                { "@limit", limit + 1 }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(It.IsAny<IReadOnlyCollection<IReadOnlyDictionary<string, object>>>())).Returns(transformerData);

            var result = _repository.GetAllByHabitId(id, userId, pageNumber, 2);

            Assert.NotNull(result);
            Assert.True(result.Item1.Any());
            Assert.Contains(result.Item1, hl => hl.Id == 1
                                          && hl.Habit_id == id
                                          && hl.Start_date == date
                                          && hl.Habit_logged == true
                                          && hl.LengthInDays == 7);
        }

        [Fact]
        public void GetAllByHabitId_ReturnsMoreAsTrue()
        {
            int id = 1234;
            int userId = 4321;
            uint pageNumber = 1;
            uint limit = 2;
            uint offset = limit * pageNumber;
            DateTime date = DateTime.UtcNow;

            IReadOnlyCollection<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>
            {
                new Dictionary<string, object>{
                    { "Id", 1 },
                    { "Habit_id", id },
                    { "Start_date", date},
                    { "Habit_logged", true },
                    { "Length_in_days", 7 }
                },
                new Dictionary<string, object>{
                    { "Id", 2 },
                    { "Habit_id", id },
                    { "Start_date", date},
                    { "Habit_logged", true },
                    { "Length_in_days", 7 }
                },
                new Dictionary<string, object>{
                    { "Id", 3 },
                    { "Habit_id", id },
                    { "Start_date", date},
                    { "Habit_logged", true },
                    { "Length_in_days", 7 }
                }
            };

            IReadOnlyCollection<HabitLog> transformerData = new List<HabitLog> { new HabitLog(1, id, date, true, 7), new HabitLog(2, id, date, true, 7), new HabitLog(3, id, date, true, 7) };

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Habit_id = @id AND u.Id = @userId AND u.IsDeleted = 0 ORDER BY Start_date DESC LIMIT @limit OFFSET @offset;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", id },
                { "@userId", userId },
                { "@offset", offset },
                { "@limit", limit + 1 }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(It.IsAny<IReadOnlyCollection<IReadOnlyDictionary<string, object>>>())).Returns(transformerData);

            var result = _repository.GetAllByHabitId(id, userId, pageNumber, 2);

            Assert.NotNull(result);
            Assert.True(result.Item2);
        }

        [Fact]
        public void GetById_ReturnsHabitLog()
        {
            int habitLogId = 1234;
            int userId = 4321;

            DateTime date = DateTime.UtcNow;

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Id = @id AND u.Id = @userId AND u.IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitLogId },
                { "@userId", userId }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();

            facadeData.Add(new Dictionary<string, object>{
                { "Id", habitLogId },
                { "Habit_id", 2341 },
                { "Start_date", date},
                { "Habit_logged", true },
                { "Length_in_days", 7 }
            });

            List<HabitLog> transformerData = new List<HabitLog> { new HabitLog(habitLogId, 2341, date, true, 7) };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);

            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetById(habitLogId, userId);

            Assert.NotNull(result);
            Assert.True(result.Id == habitLogId);
            Assert.True(result.Habit_id == 2341);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged);
            Assert.True(result.LengthInDays == 7);
        }

        [Fact]
        public void GetById_ReturnsNull()
        {
            int habitLogId = 1234;
            int userId = 4321;

            DateTime date = DateTime.UtcNow;

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE hl.Id = @id AND u.Id = @userId AND u.IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitLogId },
                { "@userId", userId }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();

            List<HabitLog> transformerData = new List<HabitLog>();

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetById(habitLogId, userId);

            Assert.Null(result);
        }

        [Fact]
        public void GetMostRecentHabitLog_ReturnsHabitLog()
        {
            int habitId = 1234;
            int userId = 4321;

            DateTime date = DateTime.UtcNow;

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE u.IsDeleted = 0 AND h.Id = @id AND u.Id = @userId ORDER BY Start_date DESC LIMIT 1";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();

            facadeData.Add(new Dictionary<string, object>{
                { "Id", 1234 },
                { "Habit_id", 2341 },
                { "Start_date", date},
                { "Habit_logged", true },
                { "Length_in_days", 7 }
            });

            List<HabitLog> transformerData = new List<HabitLog> { new HabitLog(1234, 2341, date, true, 7) };

            using var connection = new SQLiteConnection("Data Source=:memory:");
            connection.Open();

            using var transaction = connection.BeginTransaction();

            _mockFacade.Setup(facade => facade.ExecuteQueryInTransaction(query, parameters, connection, transaction)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetMostRecentHabitLog(habitId, userId, connection, transaction);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.Habit_id == 2341);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged);
            Assert.True(result.LengthInDays == 7);
        }

        [Fact]
        public void GetMostRecentHabitLog_ReturnsNull()
        {
            int habitId = 1234;
            int userId = 4321;

            DateTime date = DateTime.UtcNow;

            string query = "SELECT hl.* FROM HabitLogs hl INNER JOIN Habits h ON hl.Habit_id = h.Id INNER JOIN Users u ON h.User_id = u.Id WHERE u.IsDeleted = 0 AND h.Id = @id AND u.Id = @userId ORDER BY Start_date DESC LIMIT 1";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();

            List<HabitLog> transformerData = new List<HabitLog>();

            using var connection = new SQLiteConnection("Data Source=:memory:");
            connection.Open();

            using var transaction = connection.BeginTransaction();

            _mockFacade.Setup(facade => facade.ExecuteQueryInTransaction(query, parameters, connection, transaction)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetMostRecentHabitLog(habitId, userId, connection, transaction);

            Assert.Null(result);
        }

        [Fact]
        public void Add_ReturnsHabitLog()
        {
            DateTime date = DateTime.UtcNow;

            PostHabitLog habitLog = new PostHabitLog(1234, date, true, 1);

            string query = "INSERT INTO HabitLogs (Habit_id, Start_date, Habit_logged, Length_in_days) VALUES (@Habit_id, @Start_date, @Habit_logged, @Length_in_days) RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Habit_id", habitLog.Habit_id },
                { "@Start_date", habitLog.Start_date },
                { "@Habit_logged", habitLog.Habit_logged },
                { "@Length_in_days", habitLog.Length_in_days }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();

            facadeData.Add(new Dictionary<string, object>{
                { "Id", 1234 },
                { "Habit_id", 2341 },
                { "Start_date", date},
                { "Habit_logged", true },
                { "Length_in_days", 7 }
            });

            using var connection = new SQLiteConnection("Data Source=:memory:");
            connection.Open();

            using var transaction = connection.BeginTransaction();

            List<HabitLog> transformerData = new List<HabitLog> { new HabitLog(1234, 2341, date, true, 7) };

            _mockFacade.Setup(facade => facade.ExecuteQueryInTransaction(query, parameters, connection, transaction)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);


            var result = _repository.Add(habitLog, connection, transaction);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.Habit_id == 2341);
            Assert.True(result.Start_date == date);
            Assert.True(result.Habit_logged);
            Assert.True(result.LengthInDays == 7);
        }

        [Fact]
        public void Add_ReturnsNull()
        {
            DateTime date = DateTime.UtcNow;

            PostHabitLog habitLog = new PostHabitLog(1234, date, true, 1);

            string query = "INSERT INTO HabitLogs (Habit_id, Start_date, Habit_logged, Length_in_days) VALUES (@Habit_id, @Start_date, @Habit_logged, @Length_in_days) RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Habit_id", habitLog.Habit_id },
                { "@Start_date", habitLog.Start_date },
                { "@Habit_logged", habitLog.Habit_logged },
                { "@Length_in_days", habitLog.Length_in_days }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            List<HabitLog> transformerData = new List<HabitLog>();

            using var connection = new SQLiteConnection("Data Source=:memory:");
            connection.Open();

            using var transaction = connection.BeginTransaction();

            _mockFacade.Setup(facade => facade.ExecuteQueryInTransaction(query, parameters, connection, transaction)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.Add(habitLog, connection, transaction);

            Assert.Null(result);
        }

        [Fact]
        public void Update_ReturnsHabitLog()
        {
            DateTime date = DateTime.UtcNow;

            PatchHabitLog habitLog = new PatchHabitLog(1234, true);

            string query = "UPDATE HabitLogs SET Habit_logged = @Habit_logged WHERE Id = @Id RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Id", habitLog.Id },
                { "@Habit_logged", habitLog.Habit_logged },
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();

            facadeData.Add(new Dictionary<string, object>{
                { "Id", 1234 },
                { "Habit_id", 2341 },
                { "Start_date", date},
                { "Habit_logged", true },
                { "Length_in_days", 7 }
            });

            List<HabitLog> transformerData = new List<HabitLog> { new HabitLog(1234, 2341, date, true, 7) };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.Update(habitLog);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.Habit_id == 2341);
            Assert.True(result.Habit_logged == true);
            Assert.True(result.Start_date == date);
            Assert.True(result.LengthInDays == 7);
        }

        [Fact]
        public void Update_ReturnsNull()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1234, true);

            string query = "UPDATE HabitLogs SET Habit_logged = @Habit_logged WHERE Id = @Id RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Id", habitLog.Id },
                { "@Habit_logged", habitLog.Habit_logged },
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();

            List<HabitLog> transformerData = new List<HabitLog>();

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.Update(habitLog);

            Assert.Null(result);
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
            DateTime startDate = DateTime.UtcNow;

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
            DateTime startDate = DateTime.UtcNow;

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
