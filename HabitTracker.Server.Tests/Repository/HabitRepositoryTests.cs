using Moq;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Facade;
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
        private readonly Mock<ITransformer<Habit, IDataReader>> _mockTransformer;

        public HabitRepositoryTests()
        {
            _mockTransformer = new Mock<ITransformer<Habit, IDataReader>>();
            _mockFacade = new Mock<IStorage>();
            _repository = new HabitRepository(_mockFacade.Object, _mockTransformer.Object);
        }

        [Fact]
        public void GetAllByUserId_ReturnsHabitCollection()
        {
            int userId = 1234;
            string query = "SELECT h.* FROM Habits h INNER JOIN Users u ON h.User_id = u.Id WHERE h.User_id = @User_id;";
            var expectedHabits = new List<Habit> { new Habit(1, 1, "Test") };
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(expectedHabits);

            var result = _repository.GetAllByUserId(userId);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, h => h.Id == expectedHabits[0].Id
                                      && h.User_id == expectedHabits[0].User_id
                                      && h.Name == expectedHabits[0].Name);
        }

        [Fact]
        public void GetById_ReturnsHabit()
        {
            int habitId = 4321;
            int userId = 1234;
            string name = "Test";
            string query = "SELECT h.* FROM Habits h INNER JOIN Users u On h.User_id WHERE h.Id = @id AND h.User_id = @userId;";
            var expectedHabits = new List<Habit> { new Habit(1, 1, "Test") };
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(expectedHabits);

            var result = _repository.GetById(habitId, userId);

            Assert.NotNull(result);
            Assert.True(result.Id == expectedHabits[0].Id);
            Assert.True(result.User_id == expectedHabits[0].User_id);
            Assert.True(result.Name == expectedHabits[0].Name);
        }

        [Fact]
        public void GetById_ReturnsNull()
        {
            int habitId = 4321;
            int userId = 1234;
            string name = "Test";
            string query = "SELECT h.* FROM Habits h INNER JOIN Users u On h.User_id WHERE h.Id = @id AND h.User_id = @userId;";
            var expectedHabits = new List<Habit>();
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habitId },
                { "@userId", userId }
            };

            _mockFacade.Setup(facade => facade.ExecuteQuery(query, _mockTransformer.Object.Transform, parameters)).Returns(expectedHabits);

            var result = _repository.GetById(habitId, userId);

            Assert.Null(result);
        }

        [Fact]
        public void Add_ReturnsTrue()
        {
            PostHabit habit = new PostHabit(1234, "test");

            string query = "INSERT INTO Habits (User_id, name) VALUES (@User_id, @name);";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", habit.User_id },
                { "@name", habit.Name }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Add(habit);

            Assert.True(result);
        }

        [Fact]
        public void Add_ReturnsFalse()
        {
            PostHabit habit = new PostHabit(1234, "test");

            string query = "INSERT INTO Habits (User_id, name) VALUES (@User_id, @name);";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@User_id", habit.User_id },
                { "@name", habit.Name }
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Add(habit);

            Assert.False(result);
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
        public void Update_ReturnsTrue()
        {
            PatchHabit habit = new PatchHabit(1234, "test", 4321);

            string query = "UPDATE Habits SET name = @name WHERE Id = @id AND User_id = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habit.Id },
                { "@name", habit.Name },
                { "@userId", habit.UserId },
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(1);

            var result = _repository.Update(habit);

            Assert.True(result);
        }

        [Fact]
        public void Update_ReturnsFalse()
        {
            PatchHabit habit = new PatchHabit(1234, "test", 4321);

            string query = "UPDATE Habits SET name = @name WHERE Id = @id AND User_id = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", habit.Id },
                { "@name", habit.Name },
                { "@userId", habit.UserId },
            };

            _mockFacade.Setup(facade => facade.ExecuteNonQuery(query, parameters)).Returns(0);

            var result = _repository.Update(habit);

            Assert.False(result);
        }
    }
}