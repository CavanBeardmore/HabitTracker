using Moq;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Services;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using HabitTracker.Server.Database.Entities;

namespace HabitTracker.Server.Tests.Services
{

    public class HabitServiceTests
    {

        private readonly HabitService _service;
        private readonly Mock<IHabitRepository> _mockRepository;

        public HabitServiceTests()
        {
            _mockRepository = new Mock<IHabitRepository>();
            _service = new HabitService(_mockRepository.Object);
        }

        [Fact]
        public void GetAllByUserId_ReturnsTrueAndHabitCollection()
        {
            int userId = 1234;
            var expectedHabits = new List<Habit> { new Habit(1, 1, "Test") };

            _mockRepository.Setup(repo => repo.GetAllByUserId(userId)).Returns(expectedHabits);

            var result = _service.GetAllByUserId(userId);

            Assert.NotNull(result.Data);
            Assert.True(result.Data.Any());
            Assert.True(result.Success);
            Assert.Contains(result.Data, h => h.Id == expectedHabits[0].Id
                                      && h.User_id == expectedHabits[0].User_id
                                      && h.Name == expectedHabits[0].Name);
            Assert.Null(result.Error);
        }

        [Fact]
        public void GetAllByUserId_ReturnsFalseAndNull()
        {
            int userId = 1234;

            _mockRepository.Setup(repo => repo.GetAllByUserId(userId)).Returns(new List<Habit>());

            var result = _service.GetAllByUserId(userId);

            Assert.True(result.Data.Count() == 0);
            Assert.False(result.Success);
            Assert.True(result.Error == "0 records found");
        }

        [Fact]
        public void GetAllByUserId_ReturnsFalseNullAndExceptionMessage()
        {
            int userId = 1234;

            _mockRepository.Setup(repo => repo.GetAllByUserId(userId)).Throws(new Exception("test"));

            var result = _service.GetAllByUserId(userId);

            Assert.Null(result.Data);
            Assert.False(result.Success);
            Assert.True(result.Error == "test");
        }

        [Fact]
        public void GetById_ReturnsHabitAndTrue()
        {
            int habitId = 12345;
            int userId = 54321;

            var expectedHabit = new Habit(1, 2, "Test");

            _mockRepository.Setup(repo => repo.GetById(habitId, userId)).Returns(expectedHabit);

            var result = _service.GetById(habitId, userId);

            Assert.NotNull(result.Data);
            Assert.True(result.Success);
            Assert.True(expectedHabit.Id == result.Data.Id);
            Assert.True(expectedHabit.User_id == result.Data.User_id);
            Assert.True(expectedHabit.Name == result.Data.Name);
            Assert.Null(result.Error);
        }

        [Fact]
        public void GetById_ReturnsNullAndFalse()
        {
            int habitId = 12345;
            int userId = 54321;

            var expectedHabit = new Habit(1, 2, "Test");

            _mockRepository.Setup(repo => repo.GetById(habitId, userId)).Returns((Habit)null);

            var result = _service.GetById(habitId, userId);

            Assert.Null(result.Data);
            Assert.False(result.Success);
            Assert.Null(result.Error);
        }

        [Fact]
        public void GetById_ReturnsNullFalseAndExceptionMessage()
        {
            int habitId = 12345;
            int userId = 54321;

            var expectedHabit = new Habit(1, 2, "Test");

            _mockRepository.Setup(repo => repo.GetById(habitId, userId)).Throws(new Exception("test"));

            var result = _service.GetById(habitId, userId);

            Assert.Null(result.Data);
            Assert.False(result.Success);
            Assert.True(result.Error == "test");
        }

        [Fact]
        public void Add_ReturnsTrue()
        {
            PostHabit habit = new PostHabit(1, "Test");

            _mockRepository.Setup(repo => repo.Add(habit)).Returns(true);

            var result = _service.Add(habit);

            Assert.True(result.Success);
            Assert.Null(result.Error);
        }

        [Fact]
        public void Add_ReturnsFalseAndExceptionMessage()
        {
            PostHabit habit = new PostHabit(1, "Test");

            _mockRepository.Setup(repo => repo.Add(habit)).Throws(new Exception("test"));

            var result = _service.Add(habit);

            Assert.False(result.Success);
            Assert.True(result.Error == "test");
        }

        [Fact]
        public void Delete_CallsRepositoryDeleteMethod()
        {
            int habitId = 12345;
            int userId = 54321;

            _mockRepository.Setup(repo => repo.Delete(habitId, userId)).Returns(true);

            var result = _service.Delete(habitId, userId);

            Assert.True(result.Success);
            Assert.Null(result.Error);
        }

        [Fact]
        public void Delete_ReturnsFalseAndExceptionMessage()
        {
            int habitId = 12345;
            int userId = 54321;

            _mockRepository.Setup(repo => repo.Delete(habitId, userId)).Throws(new Exception("test"));

            var result = _service.Delete(habitId, userId);

            Assert.False(result.Success);
            Assert.True(result.Error == "test");
        }

        [Fact]
        public void Update_CallsRepositoryUpdateMethod()
        {
            PatchHabit habit = new PatchHabit(1, "test", 2);

            _mockRepository.Setup(repo => repo.Update(habit)).Returns(true);

            var result = _service.Update(habit);

            Assert.True(result.Success);
            Assert.Null(result.Error);
        }

        [Fact]
        public void Update_ReturnsFalseAndExceptionMessage()
        {
            PatchHabit habit = new PatchHabit(1, "test", 2);

            _mockRepository.Setup(repo => repo.Update(habit)).Throws(new Exception("test"));

            var result = _service.Update(habit);

            Assert.False(result.Success);
            Assert.True(result.Error == "test");
        }
    }
}