using HabitTracker.Server.Services;
using Microsoft.Extensions.Logging;
using Moq;
using HabitTracker.Server.Controllers;
using Microsoft.AspNetCore.Http;
using HabitTracker.Server.DTOs;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Tests.Controller
{
    public class HabitLogControllerTests
    {
        private readonly Mock<IHabitLogService> _mockHabitLogService;
        private readonly Mock<ILogger<HabitLogController>> _mockLogger;
        private readonly HabitLogController _controller;

        public HabitLogControllerTests()
        {
            _mockHabitLogService = new Mock<IHabitLogService>();
            _mockLogger = new Mock<ILogger<HabitLogController>>();
            _controller = new HabitLogController(_mockLogger.Object, _mockHabitLogService.Object);
        }

        [Fact]
        public void GetHabitLog_ReturnsOkObjectResult()
        {
            int habitLogId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.GetById(habitLogId, 1234)).Returns(new HabitLog(1, 2, DateTime.UtcNow, true, 7));

            var result = _controller.GetHabitLog(habitLogId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetHabitLog_ThrowsUnauthorizedException()
        {
            int habitLogId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.GetHabitLog(habitLogId));
        }

        [Fact]
        public void GetHabitLog_ThrowsNotFoundException()
        {
            int habitLogId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.GetById(habitLogId, 1234)).Returns((HabitLog)null);

            Assert.Throws<NotFoundException>(() => _controller.GetHabitLog(habitLogId));
        }

        [Fact]
        public void GetHabitLogsFromHabit_ReturnsOkObjectResult()
        {
            int habitId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.GetAllByHabitId(habitId, 1234, 1)).Returns(new List<HabitLog> { new HabitLog(1, 2, DateTime.UtcNow, true, 7)});

            var result = _controller.GetHabitLogsFromHabit(habitId, 1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetHabitLogsFromHabit_ThrowsUnauthorizedException()
        {
            int habitLogId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.GetHabitLogsFromHabit(habitLogId, 1));
        }

        [Fact]
        public void GetHabitLogsFromHabit_ThrowsNotFoundException()
        {
            int habitId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.GetAllByHabitId(habitId, 1234, 1)).Returns(new List<HabitLog>());

            Assert.Throws<NotFoundException>(() => _controller.GetHabitLogsFromHabit(habitId, 1));
        }

        [Fact]
        public void CreateHabitLog_ReturnsCreatedResult()
        {
            PostHabitLog habitlog = new PostHabitLog(1, DateTime.UtcNow, true, 7);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.Add(habitlog, 1234)).Returns(new HabitLog(1, 2, DateTime.UtcNow, true, 7));

            var result = _controller.CreateHabitLog(habitlog);

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public void CreateHabitLog_ThrowsBadRequestException()
        {
            PostHabitLog habitlog = new PostHabitLog(1, DateTime.UtcNow, true, 7);

            _controller.ModelState.AddModelError("date", "test");

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.Add(habitlog, 1234)).Returns(new HabitLog(1, 2, DateTime.UtcNow, true, 7));

            Assert.Throws<BadRequestException>(() => _controller.CreateHabitLog(habitlog));
        }

        [Fact]
        public void CreateHabitLog_ThrowsUnauthorizedException()
        {
            PostHabitLog habitlog = new PostHabitLog(1, DateTime.UtcNow, true, 7);

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            _mockHabitLogService.Setup(service => service.Add(habitlog, 1234)).Returns(new HabitLog(1, 2, DateTime.UtcNow, true, 7));

            Assert.Throws<UnauthorizedException>(() => _controller.CreateHabitLog(habitlog));
        }

        [Fact]
        public void CreateHabitLog_ThrowsAppException()
        {
            PostHabitLog habitlog = new PostHabitLog(1, DateTime.UtcNow, true, 7);

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.Add(habitlog, 1234)).Returns((HabitLog)null);

            Assert.Throws<AppException>(() => _controller.CreateHabitLog(habitlog));
        }

        [Fact]
        public void UpdateHabitLog_ReturnsOkObjectResult()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1, true);

            _mockHabitLogService.Setup(service => service.Update(habitLog)).Returns(true);

            var result = _controller.UpdateHabitLog(habitLog);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void UpdateHabitLog_ThrowsBadRequestException()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1, true);
            _controller.ModelState.AddModelError("test", "test");

            Assert.Throws<BadRequestException>(() => _controller.UpdateHabitLog(habitLog));
        }

        [Fact]
        public void UpdateHabitLog_ThrowsAppException()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1, true);

            _mockHabitLogService.Setup(service => service.Update(habitLog)).Returns(false);

            Assert.Throws<AppException>(() => _controller.UpdateHabitLog(habitLog));
        }

        [Fact]
        public void DeleteHabitLog_ReturnsNoContentResult()
        {
            int habitLogId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.Delete(habitLogId, 1234)).Returns(true);

            var result = _controller.DeleteHabitLog(habitLogId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteHabitLog_ThrowsUnauthorizedException()
        {
            int habitLogId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.DeleteHabitLog(habitLogId));
        }

        [Fact]
        public void DeleteHabitLog_ThrowsAppException()
        {
            int habitLogId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.Delete(habitLogId, 1234)).Returns(false);

            Assert.Throws<AppException>(() => _controller.DeleteHabitLog(habitLogId));
        }
    }
}
