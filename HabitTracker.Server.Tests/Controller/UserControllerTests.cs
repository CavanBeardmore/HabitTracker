using Moq;
using HabitTracker.Server.Services;
using Microsoft.Extensions.Logging;
using HabitTracker.Server.Controllers;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Exceptions;
using Microsoft.AspNetCore.Http;
using HabitTracker.Server.SSE;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Tests.Controller
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly Mock<IEventService<HabitTrackerEvent>> _mockEventService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockEventService = new Mock<IEventService<HabitTrackerEvent>>();
            _controller = new UserController(_mockLogger.Object, _mockUserService.Object, _mockEventService.Object);
        }

        [Fact]
        public void GetUser_ReturnsOkResult()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);   
            _mockUserService.Setup(service => service.Get(1234)).Returns(new User(1234, "test-username", "test-email", "test-password"));

            var result = _controller.GetUser();

            Assert.IsType<OkObjectResult>(result);
        }
        
        [Fact]
        public void GetUser_ThrowsAppException()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);   
            _mockUserService.Setup(service => service.Get(1234)).Returns((User)null);
            
            Assert.Throws<AppException>(() => _controller.GetUser());
        }

        [Fact]
        public void AddUser_ReturnsCreatedResult()
        {
            PostUser user = new PostUser("test1", "test2", "test3");

            _mockUserService.Setup(service => service.Add(user)).Returns(true);

            var result = _controller.AddUser(user);

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public void AddUser_ReturnsBadRequestResult()
        {
            PostUser user = new PostUser("test1", "test2", "test3");

            user.Username = null;

            _controller.ModelState.Clear();
            _controller.ModelState.AddModelError("Username", "The Username field is required.");

            Assert.Throws<BadRequestException>(() => _controller.AddUser(user));
        }

        [Fact]
        public void AddUser_ThrowsAppException()
        {
            PostUser user = new PostUser("test1", "test2", "test3");

            _mockUserService.Setup(service => service.Add(user)).Returns(false);

            Assert.Throws<AppException>(() => _controller.AddUser(user));

        }

        [Fact]
        public void UpdateUser_ReturnsOkResult()
        {
            PatchUser user = new PatchUser("test1", "test2", "test3", "test4");

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockUserService.Setup(service => service.Update(1234, user)).Returns(new UpdatedUserResult("test1233", new User(1234, "test1", "test2", "test3")));

            var result = _controller.UpdateUser(user);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void UpdateUser_ReturnsBadRequestException()
        {
            PatchUser user = new PatchUser("test1", "test2", "test3", "test4");

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _controller.ModelState.Clear();
            _controller.ModelState.AddModelError("Username", "The Username field is required.");

            _mockUserService.Setup(service => service.Update(1234, user)).Returns(new UpdatedUserResult("test1233", new User(1234, "test1", "test2", "test3")));

            Assert.Throws<BadRequestException>(() => _controller.UpdateUser(user));
        }

        [Fact]
        public void DeleteUser_ReturnsNoContentResult()
        {
            AuthUser user = new AuthUser("test1", "test2");

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockUserService.Setup(service => service.Delete(1234, user)).Returns(true);

            var result = _controller.DeleteUser(user);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteUser_ThrowsUnauthorizedException()
        {
            AuthUser user = new AuthUser("test1", "test2");

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            _mockUserService.Setup(service => service.Delete(1234, user)).Returns(true);

            Assert.Throws<UnauthorizedException>(() => _controller.DeleteUser(user));
        }

        [Fact]
        public void DeleteUser_ThrowsBadRequestException()
        {
            AuthUser user = new AuthUser("test1", "test2");

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _controller.ModelState.Clear();
            _controller.ModelState.AddModelError("Username", "The Username field is required.");

            _mockUserService.Setup(service => service.Delete(1234, user)).Returns(true);

            Assert.Throws<BadRequestException>(() => _controller.DeleteUser(user));
        }

        [Fact]
        public void DeleteUser_ThrowsAppException()
        {
            AuthUser user = new AuthUser("test1", "test2");

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Items.Add("userId", 1234);

            _mockUserService.Setup(service => service.Delete(1234, user)).Returns(false);

            Assert.Throws<AppException>(() => _controller.DeleteUser(user));
        }
    }
}
