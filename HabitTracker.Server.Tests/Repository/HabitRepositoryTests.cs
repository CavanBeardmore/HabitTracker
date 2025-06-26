using Moq;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Storage;
using HabitTracker.Server.Transformer;
using HabitTracker.Server.DTOs;
using System.Data;
using HabitTracker.Server.Database.Entities;
using System.Xml.Linq;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Tests.Repository
{

    public class HabitRepositoryTests
    {

        private readonly HabitRepository _repository;
        private readonly Mock<IStorage> _mockFacade;
        private readonly Mock<ITransformer<IReadOnlyCollection<Habit>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>> _mockTransformer;

        public HabitRepositoryTests()
        {
            _mockTransformer = new Mock<ITransformer<IReadOnlyCollection<Habit>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>>();
            _mockFacade = new Mock<IStorage>();
            _repository = new HabitRepository(_mockFacade.Object, _mockTransformer.Object);
        }

        [Fact]
        public void GetAllByUserId_ReturnsHabitCollection()
        {
            int userId = 1234;
            string query = "SELECT h.* FROM Habits h INNER JOIN Users u ON h.User_id = u.Id WHERE h.User_id = @User_id AND u.IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", userId }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();

            facadeData.Add(new Dictionary<string, object>{
                { "Id", 1234 },
                { "User_id", 4321 },
                { "Name", "test" },
            });

            List<Habit> transformerData = new List<Habit> { new Habit(1234, 4321, "test", 7) };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetAllByUserId(userId);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, h => h.Id == 1234
                                      && h.User_id == 4321
                                      && h.Name == "test");
        }

        [Fact]
        public void GetById_ReturnsHabit()
        {
            int habitId = 4321;
            int userId = 1234;

            string query = "SELECT h.* FROM Habits h INNER JOIN Users u On h.User_id WHERE h.Id = @id AND h.User_id = @userId AND u.IsDeleted = 0;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();

            facadeData.Add(new Dictionary<string, object>{
                { "Id", 1234 },
                { "User_id", 4321 },
                { "Name", "test" },
            });

            List<Habit> transformerData = new List<Habit> { new Habit(1234, 4321, "test", 7) };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetById(habitId, userId, null, null);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.User_id == 4321);
            Assert.True(result.Name == "test");
        }

        [Fact]
        public void GetById_ReturnsNull()
        {
            int habitId = 4321;
            int userId = 1234;

            string query = "SELECT h.* FROM Habits h INNER JOIN Users u On h.User_id WHERE h.Id = @id AND h.User_id = @userId AND u.IsDeleted = 0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            List<Habit> transformerData = new List<Habit>();

            facadeData.Add(new Dictionary<string, object>{
                { "Id", 1234 },
                { "User_id", 4321 },
                { "Name", "test" },
            });

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.GetById(habitId, userId, null, null);

            Assert.Null(result);
        }

        [Fact]
        public void Add_ReturnsHabit() 
        {
            PostHabit habit = new PostHabit("test");
            int userId = 1234;

            string query = "INSERT INTO Habits (User_id, name, StreakCount) VALUES (@User_id, @name, @StreakCount) RETURNING *;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", userId },
                { "@name", habit.Name },
                { "@StreakCount", 0 }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            List<Habit> transformerData = new List<Habit> { new Habit(1234, 4321, "test", 7) };

            facadeData.Add(new Dictionary<string, object>{
                { "Id", 1234 },
                { "User_id", 4321 },
                { "Name", "test" },
            });

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.Add(userId, habit);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.User_id == 4321);
            Assert.True(result.Name == "test");
        }

        [Fact]
        public void Add_ReturnsNull()
        {
            PostHabit habit = new PostHabit("test");

            string query = "INSERT INTO Habits (User_id, name, StreakCount) VALUES (@User_id, @name, @StreakCount) RETURNING *;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", 1234 },
                { "@name", habit.Name },
                { "@StreakCount", 0 }
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            List<Habit> transformerData = new List<Habit>();

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.Add(1234, habit);

            Assert.Null(result);
        }

        [Fact]
        public void Delete_ReturnsTrue()
        {
            int habitId = 1234;
            int userId = 4321;

            string query = "DELETE FROM Habits WHERE Habits.Id = @id AND Habits.User_id = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Delete(habitId, userId);

            Assert.True(result);
        }

        [Fact]
        public void Delete_ReturnsFalse()
        {
            int habitId = 1234;
            int userId = 4321;

            string query = "DELETE FROM Habits WHERE Habits.Id = @id AND Habits.User_id = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Delete(habitId, userId);

            Assert.False(result);
        }

        [Fact]
        public void Update_ReturnsHabit()
        {
            PatchHabit habit = new PatchHabit(1234, "test", 7);

            string query = "UPDATE Habits SET name = @name, StreakCount = @streakCount WHERE Id = @id AND User_id = @userId RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habit.Id },
                { "@name", habit.Name },
                { "@userId", 1234 },
                { "@streakCount", habit.StreakCount },
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            List<Habit> transformerData = new List<Habit> { new Habit(1234, 4321, "test", 7) };

            facadeData.Add(new Dictionary<string, object>{
                { "Id", 1234 },
                { "User_id", 4321 },
                { "Name", "test" },
                { "StreakCount", 7 },
            });

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.Update(1234, habit, null, null);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.User_id == 4321);
            Assert.True(result.Name == "test");
            Assert.True(result.StreakCount == 7);
        }

        [Fact]
        public void Update_ReturnsNull()
        {
            PatchHabit habit = new PatchHabit(1234, "test", 7);

            string query = "UPDATE Habits SET name = @name, StreakCount = @streakCount WHERE Id = @id AND User_id = @userId RETURNING *;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habit.Id },
                { "@name", habit.Name },
                { "@userId", 1234 },
                { "@streakCount", habit.StreakCount },
            };

            List<IReadOnlyDictionary<string, object>> facadeData = new List<IReadOnlyDictionary<string, object>>();
            List<Habit> transformerData = new List<Habit>();

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, parameters)).Returns(facadeData);
            _mockTransformer.Setup(transformer => transformer.Transform(facadeData)).Returns(transformerData);

            var result = _repository.Update(1234, habit, null, null);

            Assert.Null(result);
        }
    }
}