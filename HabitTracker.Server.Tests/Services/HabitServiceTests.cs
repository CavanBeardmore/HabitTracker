using Moq;
using HabitTracker.Server.Repository;
using HabitTracker.Server.Services;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using Microsoft.Extensions.Logging;
using HabitTracker.Server.Exceptions;

namespace HabitTracker.Server.Tests.Services
{

    public class HabitServiceTests
    {

        private readonly HabitService _service;
        private readonly Mock<ILogger<HabitService>> _mockLogger;
        private readonly Mock<IHabitRepository> _mockRepository;

        public HabitServiceTests()
        {
            _mockRepository = new Mock<IHabitRepository>();
            _mockLogger = new Mock<ILogger<HabitService>>();
            _service = new HabitService(_mockLogger.Object, _mockRepository.Object);
        }

        [Fact]
        public void GetAllByUserId_ReturnsTrueAndHabitCollection()
        {
            int userId = 1234;
            var expectedHabits = new List<Habit> { new Habit(1, 1, "Test") };

            _mockRepository.Setup(repo => repo.GetAllByUserId(userId)).Returns(expectedHabits);

            var result = _service.GetAllByUserId(userId);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, h => h.Id == expectedHabits[0].Id
                                      && h.User_id == expectedHabits[0].User_id
                                      && h.Name == expectedHabits[0].Name);
        }

        [Fact]
        public void GetAllByUserId_ThrowsNotFoundException()
        {
            int userId = 1234;

            _mockRepository.Setup(repo => repo.GetAllByUserId(userId)).Returns(new List<Habit>());

            Assert.Throws<NotFoundException>(() => _service.GetAllByUserId(userId));
        }

        [Fact]
        public void GetAllByUserId_ThrowsAppException()
        {
            int userId = 1234;

            _mockRepository.Setup(repo => repo.GetAllByUserId(userId)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.GetAllByUserId(userId));
        }

        [Fact]
        public void GetById_ReturnsHabitAndTrue()
        {
            int habitId = 12345;
            int userId = 54321;

            var expectedHabit = new Habit(1, 2, "Test");

            _mockRepository.Setup(repo => repo.GetById(habitId, userId)).Returns(expectedHabit);

            var result = _service.GetById(habitId, userId);

            Assert.NotNull(result);
            Assert.True(expectedHabit.Id == result.Id);
            Assert.True(expectedHabit.User_id == result.User_id);
            Assert.True(expectedHabit.Name == result.Name);
        }

        [Fact]
        public void GetById_ThrowsNotFoundException()
        {
            int habitId = 12345;
            int userId = 54321;

            _mockRepository.Setup(repo => repo.GetById(habitId, userId)).Returns((Habit)null);

            Assert.Throws<NotFoundException>(() => _service.GetById(habitId, userId));
        }

        [Fact]
        public void GetById_ThrowsAppException()
        {
            int habitId = 12345;
            int userId = 54321;

            _mockRepository.Setup(repo => repo.GetById(habitId, userId)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.GetById(habitId, userId));
        }

        [Fact]
        public void Add_Returnshabit()
        {
            PostHabit habit = new PostHabit("Test");

            _mockRepository.Setup(repo => repo.Add(1234, habit)).Returns(new Habit(1234, 4321, "Test"));

            var result = _service.Add(1234, habit);

            Assert.NotNull(result);
            Assert.True(result.Id == 1234);
            Assert.True(result.User_id == 4321);
            Assert.True(result.Name == "Test");
        }

        [Fact]
        public void Add_ThrowsAppException()
        {
            PostHabit habit = new PostHabit("Test");

            _mockRepository.Setup(repo => repo.Add(1234, habit)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.Add(1234, habit));
        }

        [Fact]
        public void Delete_ReturnsTrue()
        {
            int habitId = 12345;
            int userId = 54321;

            _mockRepository.Setup(repo => repo.Delete(habitId, userId)).Returns(true);

            var result = _service.Delete(habitId, userId);

            Assert.True(result);
        }

        [Fact]
        public void Delete_ReturnsFalse()
        {
            int habitId = 12345;
            int userId = 54321;

            _mockRepository.Setup(repo => repo.Delete(habitId, userId)).Returns(false);

            var result = _service.Delete(habitId, userId);

            Assert.False(result);
        }

        [Fact]
        public void Delete_ThrowsAppException()
        {
            int habitId = 12345;
            int userId = 54321;

            _mockRepository.Setup(repo => repo.Delete(habitId, userId)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.Delete(habitId, userId));
        }

        [Fact]
        public void Update_CallsRepositoryUpdateMethod()
        {
            PatchHabit habit = new PatchHabit(1, "test");

            _mockRepository.Setup(repo => repo.Update(1234, habit)).Returns(true);

            var result = _service.Update(1234, habit);

            Assert.True(result);
        }

        [Fact]
        public void Update_ReturnsFalse()
        {
            PatchHabit habit = new PatchHabit(1, "test");

            _mockRepository.Setup(repo => repo.Update(1234, habit)).Returns(false);

            var result = _service.Update(1234, habit);

            Assert.False(result);
        }

        [Fact]
        public void Update_ThrowsAppException()
        {
            PatchHabit habit = new PatchHabit(1, "test");

            _mockRepository.Setup(repo => repo.Update(1234, habit)).Throws<AppException>(() => throw new AppException("test"));

            Assert.Throws<AppException>(() => _service.Update(1234, habit));
        }
    }
}