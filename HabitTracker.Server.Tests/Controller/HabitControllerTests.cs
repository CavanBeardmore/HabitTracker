using HabitTracker.Server.Controllers;
using HabitTracker.Server.Database.Entities;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Models;
using HabitTracker.Server.Services;
using HabitTracker.Server.SSE;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Server.Tests.Controller
{
    public class HabitControllerTests
    {
        private readonly Mock<IHabitService> _mockHabitService;
        private readonly Mock<ILogger<HabitController>> _mockLogger;
        private HabitController? _controller;
        private readonly Mock<IEventService<HabitTrackerEvent>> _mockEventService;
        private IMemoryCache? _memoryCache;

        public HabitControllerTests()
        {
            _mockHabitService = new Mock<IHabitService>();
            _mockLogger = new Mock<ILogger<HabitController>>();
            _mockEventService = new Mock<IEventService<HabitTrackerEvent>>();
        }

        [Fact]
        public void GetHabit_ReturnsOkObjectResult()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);

            int habitId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            Habit habit = new Habit(1234, 4321, "test", 7);

            _mockHabitService.Setup(service => service.GetById(habitId, 1234)).Returns(habit);

            var result = _controller.GetHabit(habitId);

            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1");

            Assert.Equal(habit, cachedHabit);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetHabit_ThrowsUnauthorizedException()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            int habitId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1");

            Assert.Null(cachedHabit);
            Assert.Throws<UnauthorizedException>(() => _controller.GetHabit(habitId));
        }

        [Fact]
        public void GetHabit_ReturnsHabitFromCache()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);

            Habit habit = new Habit(1234, 4321, "test", 7);
            _memoryCache.Set("habit_user:1234_habit:1", habit);
            int habitId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _controller.GetHabit(habitId);
            var result = _controller.GetHabit(habitId);

            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1");

            Assert.Equal(habit, cachedHabit);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetUserHabits_ReturnsOkObjectResult()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.GetAllByUserId(1234)).Returns(new List<Habit> { new Habit(1234, 4321, "test", 7) });

            var result = _controller.GetUserHabits();

            IReadOnlyCollection<Habit>? cachedHabits = _memoryCache.Get<IReadOnlyCollection<Habit>>("habit_user:1234_habits");

            Assert.NotNull(cachedHabits);
            Assert.IsType<List<Habit>>(cachedHabits);
            Assert.True(cachedHabits.Any());
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetUserHabits_ThrowsUnauthorizedException()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.GetUserHabits());
            IReadOnlyCollection<Habit>? cachedHabits = _memoryCache.Get<IReadOnlyCollection<Habit>>("habit_user:1234_habits");

            Assert.Null(cachedHabits);
        }

        [Fact]
        public void GetUserHabits_ReturnsCollectionOfHabitsFromCache()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _memoryCache.Set("habit_user:1234_habits", new List<Habit> { new Habit(1234, 4321, "test", 7) });

            var result = _controller.GetUserHabits();

            IReadOnlyCollection<Habit>? cachedHabits = _memoryCache.Get<IReadOnlyCollection<Habit>>("habit_user:1234_habits");

            Assert.NotNull(cachedHabits);
            Assert.IsType<List<Habit>>(cachedHabits);
            Assert.True(cachedHabits.Any());
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void CreateHabit_ReturnsCreatedResult()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            PostHabit habit = new PostHabit("test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            Habit habit1 = new Habit(1234, 4321, "test", 7);

            _mockHabitService.Setup(service => service.Add(1234, habit)).Returns(habit1);

            var result = _controller.CreateHabit(habit);

            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.Equal(habit1, cachedHabit);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void CreateHabit_ThrowsUnauthorizedException()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            PostHabit habit = new PostHabit("test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.CreateHabit(habit));

            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.Null(cachedHabit);
        }

        [Fact]
        public void CreateHabit_ThrowsBadRequestException()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            PostHabit habit = new PostHabit("test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _controller.ModelState.Clear();
            _controller.ModelState.AddModelError("Username", "The Username field is required.");

            Assert.Throws<BadRequestException>(() => _controller.CreateHabit(habit));
            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.Null(cachedHabit);
        }

        [Fact]
        public void CreateHabit_ThrowsAppException()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            PostHabit habit = new PostHabit("test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.Add(1234, habit)).Returns((Habit)null);

            Assert.Throws<AppException>(() => _controller.CreateHabit(habit));
            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.Null(cachedHabit);
        }

        [Fact]
        public void UpdateHabit_ReturnsOkResult()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            PatchHabit habit = new PatchHabit(1234, "test", 7);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            Habit habit1 = new Habit(1234, 4321, "test", 7);

            _mockHabitService.Setup(service => service.Update(1234, habit)).Returns(habit1);

            var result = _controller.UpdateHabit(habit);

            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.Equal(habit1, cachedHabit);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void UpdateHabit_ThrowsUnauthorizedException()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            PatchHabit habit = new PatchHabit(1234, "test", 7);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.UpdateHabit(habit));
            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.Null(cachedHabit);
        }

        [Fact]
        public void UpdateHabit_ThrowsBadRequestException()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            PatchHabit habit = new PatchHabit(1234, "test", 7);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _controller.ModelState.AddModelError("name", "habit name is a required field");

            Assert.Throws<BadRequestException>(() => _controller.UpdateHabit(habit));
            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.Null(cachedHabit);
        }
        [Fact]
        public void UpdateHabit_ThrowsAppException()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            PatchHabit habit = new PatchHabit(1234, "test", 7);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.Update(1234, habit)).Returns((Habit)null);

            Assert.Throws<AppException>(() => _controller.UpdateHabit(habit));
            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.Null(cachedHabit);
        }

        [Fact]
        public void DeleteHabit_ReturnsNoContentResult()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            int habitId = 1234;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            _memoryCache.Set("habit_user:1234_habit:1234", new Habit(1234, 1234, "test", 7));

            _mockHabitService.Setup(service => service.Delete(1234, 1234)).Returns(true);

            var result = _controller.DeleteHabit(habitId);

            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.Null(cachedHabit);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteHabit_ThrowsUnauthorizedException()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            int habitId = 1234;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _memoryCache.Set("habit_user:1234_habit:1234", new Habit(1234, 1234, "test", 7));

            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.NotNull(cachedHabit);
            Assert.Throws<UnauthorizedException>(() => _controller.DeleteHabit(habitId));
        }

        [Fact]
        public void DeleteHabit_ThrowsAppException()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object, _memoryCache);
            int habitId = 1234;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            _memoryCache.Set("habit_user:1234_habit:1234", new Habit(1234, 1234, "test", 7));

            _mockHabitService.Setup(service => service.Delete(1234, 1234)).Returns(false);

            var cachedHabit = _memoryCache.Get("habit_user:1234_habit:1234");

            Assert.NotNull(cachedHabit);
            Assert.Throws<AppException>(() => _controller.DeleteHabit(habitId));
        }
    }
}
