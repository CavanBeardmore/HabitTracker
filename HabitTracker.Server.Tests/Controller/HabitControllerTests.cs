using HabitTracker.Server.Controllers;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Models;
using HabitTracker.Server.Services;
using HabitTracker.Server.SSE;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Server.Tests.Controller
{
    public class HabitControllerTests
    {
        private readonly Mock<IHabitService> _mockHabitService;
        private readonly Mock<ILogger<HabitController>> _mockLogger;
        private readonly HabitController _controller;
        private readonly Mock<IEventService<HabitTrackerEvent>> _mockEventService;

        public HabitControllerTests()
        {
            _mockHabitService = new Mock<IHabitService>();
            _mockLogger = new Mock<ILogger<HabitController>>();
            _mockEventService = new Mock<IEventService<HabitTrackerEvent>>();
            _controller = new HabitController(_mockLogger.Object, _mockEventService.Object, _mockHabitService.Object);
        }

        [Fact]
        public void GetHabit_ReturnsOkObjectResult()
        {
            int habitId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.GetById(habitId, 1234)).Returns(new Habit(1234, 4321, "test"));

            var result = _controller.GetHabit(habitId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetHabit_ThrowsUnauthorizedException()
        {
            int habitId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.GetHabit(habitId));
        }

        [Fact]
        public void GetUserHabits_ReturnsOkObjectResult()
        {
            int habitId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.GetAllByUserId(1234)).Returns(new List<Habit> { new Habit(1234, 4321, "test") });

            var result = _controller.GetUserHabits();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetUserHabits_ThrowsUnauthorizedException()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.GetUserHabits());
        }

        [Fact]
        public void CreateHabit_ReturnsCreatedResult()
        {
            PostHabit habit = new PostHabit("test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.Add(1234, habit)).Returns(new Habit(1234, 4321, "test"));

            var result = _controller.CreateHabit(habit);

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public void CreateHabit_ThrowsUnauthorizedException()
        {
            PostHabit habit = new PostHabit("test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.CreateHabit(habit));
        }

        [Fact]
        public void CreateHabit_ThrowsBadRequestException()
        {
            PostHabit habit = new PostHabit("test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _controller.ModelState.Clear();
            _controller.ModelState.AddModelError("Username", "The Username field is required.");

            Assert.Throws<BadRequestException>(() => _controller.CreateHabit(habit));
        }

        [Fact]
        public void CreateHabit_ThrowsAppException()
        {
            PostHabit habit = new PostHabit("test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.Add(1234, habit)).Returns((Habit)null);

            Assert.Throws<AppException>(() => _controller.CreateHabit(habit));
        }

        [Fact]
        public void UpdateHabit_ReturnsOkResult()
        {
            PatchHabit habit = new PatchHabit(1234, "test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.Update(1234, habit)).Returns(true);

            var result = _controller.UpdateHabit(habit);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void UpdateHabit_ThrowsUnauthorizedException()
        {
            PatchHabit habit = new PatchHabit(1234, "test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.UpdateHabit(habit));
        }

        [Fact]
        public void UpdateHabit_ThrowsBadRequestException()
        {
            PatchHabit habit = new PatchHabit(1234, "test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _controller.ModelState.AddModelError("name", "habit name is a required field");

            Assert.Throws<BadRequestException>(() => _controller.UpdateHabit(habit));
        }
        [Fact]
        public void UpdateHabit_ThrowsAppException()
        {
            PatchHabit habit = new PatchHabit(1234, "test");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.Update(1234, habit)).Returns(false);

            Assert.Throws<AppException>(() => _controller.UpdateHabit(habit));
        }

        [Fact]
        public void DeleteHabit_ReturnsNoContentResult()
        {
            int habitId = 1234;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.Delete(1234, 1234)).Returns(true);

            var result = _controller.DeleteHabit(habitId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteHabit_ThrowsUnauthorizedException()
        {
            int habitId = 1234;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.DeleteHabit(habitId));
        }

        [Fact]
        public void DeleteHabit_ThrowsAppException()
        {
            int habitId = 1234;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitService.Setup(service => service.Delete(1234, 1234)).Returns(false);

            Assert.Throws<AppException>(() => _controller.DeleteHabit(habitId));
        }
    }
}
