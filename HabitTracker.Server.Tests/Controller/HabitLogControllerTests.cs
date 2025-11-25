using HabitTracker.Server.Controllers;
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
    public class HabitLogControllerTests
    {
        private readonly Mock<IHabitLogService> _mockHabitLogService;
        private readonly Mock<ILogger<HabitLogController>> _mockLogger;
        private readonly Mock<IEventService<HabitTrackerEvent>> _mockEventService;
        private HabitLogController _controller;

        public HabitLogControllerTests()
        {
            _mockHabitLogService = new Mock<IHabitLogService>();
            _mockLogger = new Mock<ILogger<HabitLogController>>();
            _mockEventService = new Mock<IEventService<HabitTrackerEvent>>();
            _controller = new HabitLogController(_mockLogger.Object, _mockHabitLogService.Object, _mockEventService.Object, new MemoryCache(new MemoryCacheOptions()));
        }
        
        [Fact]
        public void GetMostRecentLogFromHabit_ReturnsOkObjectResult()
        {
            int habitId = 2;
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new HabitLogController(_mockLogger.Object, _mockHabitLogService.Object, _mockEventService.Object, cache);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            HabitLog log = new HabitLog(1, 2, DateTime.UtcNow, true, 7);

            _mockHabitLogService.Setup(service => service.GetMostRecentByHabitId(habitId, 1234)).Returns(log);

            var result = controller.GetMostRecentLogFromHabit(habitId);

            var cachedLog = cache.Get("mostRecentHabitLog_user:1234_habit:2");

            Assert.Equal(cachedLog, log);
            Assert.IsType<OkObjectResult>(result);
        }
        
        [Fact]
        public void GetMostRecentLogFromHabit_ReturnsCachedHabitLog()
        {
            int habitId = 2;
            HabitLog habitLog = new HabitLog(1, habitId, DateTime.UtcNow, true, 7);
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new HabitLogController(_mockLogger.Object, _mockHabitLogService.Object, _mockEventService.Object, cache);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            cache.Set("mostRecentHabitLog_user:1234_habit:2", habitLog);
            
            var result = controller.GetMostRecentLogFromHabit(habitId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetMostRecentLogFromHabit_ThrowsUnauthorizedException()
        {
            int habitLogId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Assert.Throws<UnauthorizedException>(() => _controller.GetHabitLog(habitLogId));
        }

        [Fact]
        public void GetMostRecentLogFromHabit_ThrowsNotFoundException()
        {
            int habitLogId = 1;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.GetById(habitLogId, 1234)).Returns((HabitLog)null);

            Assert.Throws<NotFoundException>(() => _controller.GetHabitLog(habitLogId));
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

            _mockHabitLogService.Setup(service => service.GetAllByHabitId(habitId, 1234, 1)).Returns(new PaginatedHabitLogs(new List<HabitLog> { new HabitLog(1, 2, DateTime.UtcNow, true, 7)}, true));

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

            _mockHabitLogService.Setup(service => service.GetAllByHabitId(habitId, 1234, 1)).Returns((PaginatedHabitLogs)null);

            Assert.Throws<NotFoundException>(() => _controller.GetHabitLogsFromHabit(habitId, 1));
        }

        [Fact]
        public void CreateHabitLog_ReturnsCreatedResult()
        {
            PostHabitLog habitlog = new PostHabitLog(1, DateTime.UtcNow, true, 7);
            HabitLog cacheableLog = new HabitLog(1, 2, DateTime.Now, true, 1);
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new HabitLogController(_mockLogger.Object, _mockHabitLogService.Object, _mockEventService.Object, cache);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            var log = new HabitLog(1, 2, DateTime.Now, true, 7);
            var habit = new Habit(2, 5, "This is a test habit", 1);
            cache.Set("mostRecentHabitLog_user:1234_habit:2", cacheableLog);

            _mockHabitLogService.Setup(service => service.Add(habitlog, 1234)).Returns(new AddedHabitLogResult(
                log,
                habit
            ));

            var result = controller.CreateHabitLog(habitlog);

            var cachedLog = cache.Get("mostRecentHabitLog_user:1234_habit:2");
            
            Assert.Null(cachedLog);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void CreateHabitLog_ThrowsBadRequestException()
        {
            PostHabitLog habitlog = new PostHabitLog(1, DateTime.UtcNow, true, 7);

            _controller.ModelState.AddModelError("date", "test");

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockHabitLogService.Setup(service => service.Add(habitlog, 1234)).Returns(It.IsAny<AddedHabitLogResult>());

            Assert.Throws<BadRequestException>(() => _controller.CreateHabitLog(habitlog));
        }

        [Fact]
        public void CreateHabitLog_ThrowsUnauthorizedException()
        {
            PostHabitLog habitlog = new PostHabitLog(1, DateTime.UtcNow, true, 7);

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            _mockHabitLogService.Setup(service => service.Add(habitlog, 1234)).Returns(It.IsAny<AddedHabitLogResult>());

            Assert.Throws<UnauthorizedException>(() => _controller.CreateHabitLog(habitlog));
        }

        [Fact]
        public void CreateHabitLog_ThrowsAppException()
        {
            PostHabitLog habitlog = new PostHabitLog(1, DateTime.UtcNow, true, 7);
            HabitLog log = new HabitLog(1, 2, DateTime.Now, true, 1);
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new HabitLogController(_mockLogger.Object, _mockHabitLogService.Object, _mockEventService.Object, cache);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            cache.Set("mostRecentHabitLog_user:1234_habit:2", log);

            _mockHabitLogService.Setup(service => service.Add(habitlog, 1234)).Returns(It.IsAny<AddedHabitLogResult>());

            Assert.Throws<AppException>(() => controller.CreateHabitLog(habitlog));
            var cachedLog = cache.Get("mostRecentHabitLog_user:1234_habit:2");
            Assert.NotNull(cachedLog);
            Assert.Equal(log, cachedLog);
        }

        [Fact]
        public void UpdateHabitLog_ReturnsOkObjectResult()
        {
            PatchHabitLog habitLog = new PatchHabitLog(1, true);
            HabitLog log = new HabitLog(1, 2, DateTime.Now, true, 1);
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new HabitLogController(_mockLogger.Object, _mockHabitLogService.Object, _mockEventService.Object, cache);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            cache.Set("mostRecentHabitLog_user:1234_habit:2", log);

            _mockHabitLogService.Setup(service => service.Update(habitLog)).Returns(log);

            var result = controller.UpdateHabitLog(habitLog);

            var cachedLog = cache.Get("mostRecentHabitLog_user:1234_habit:2");
            
            Assert.IsType<OkResult>(result);
            Assert.Null(cachedLog);
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
            HabitLog log = new HabitLog(1, 2, DateTime.Now, true, 7); 
            PatchHabitLog habitLog = new PatchHabitLog(1, true);
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new HabitLogController(_mockLogger.Object, _mockHabitLogService.Object, _mockEventService.Object, cache);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            cache.Set("mostRecentHabitLog_user:1234_habit:2", log);

            _mockHabitLogService.Setup(service => service.Update(habitLog)).Returns((HabitLog)null);

            Assert.Throws<AppException>(() => controller.UpdateHabitLog(habitLog));
            var cachedLog = cache.Get("mostRecentHabitLog_user:1234_habit:2");
            Assert.NotNull(cachedLog);
            Assert.Equal(log, cachedLog);
        }

        [Fact]
        public void DeleteHabitLog_ReturnsNoContentResult()
        {
            int habitLogId = 1;
            HabitLog log = new HabitLog(habitLogId, 2, DateTime.Now, true, 1);
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new HabitLogController(_mockLogger.Object, _mockHabitLogService.Object, _mockEventService.Object, cache);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            cache.Set("mostRecentHabitLog_user:1234_habit:2", log);

            _mockHabitLogService.Setup(service => service.Delete(habitLogId, 1234)).Returns(new DeleteHabitLogResult(true, log));

            var result = controller.DeleteHabitLog(habitLogId);
            
            var cachedLog = cache.Get("mostRecentHabitLog_user:1234_habit:2");

            Assert.IsType<NoContentResult>(result);
            Assert.Null(cachedLog);
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
            HabitLog log = new HabitLog(habitLogId, 2, DateTime.Now, true, 1);
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new HabitLogController(_mockLogger.Object, _mockHabitLogService.Object, _mockEventService.Object, cache);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Items.Add("userId", 1234);
            cache.Set("mostRecentHabitLog_user:1234_habit:2", log);

            _mockHabitLogService.Setup(service => service.Delete(habitLogId, 1234)).Returns(new DeleteHabitLogResult(false, log));

            Assert.Throws<AppException>(() => controller.DeleteHabitLog(habitLogId));
            var cachedLog = cache.Get("mostRecentHabitLog_user:1234_habit:2");
            Assert.NotNull(cachedLog);
            Assert.Equal(log, cachedLog);
        }
    }
}
