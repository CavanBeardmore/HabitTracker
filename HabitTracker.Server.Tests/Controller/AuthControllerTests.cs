using HabitTracker.Server.Auth;
using HabitTracker.Server.Controllers;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Models;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Server.Tests.Controller
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthentication> _mockAuth;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuth = new Mock<IAuthentication>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockLogger.Object, _mockAuth.Object, _mockUserService.Object);
        }

        [Fact]
        public void Login_ReturnsOkObjectResult()
        {
            AuthUser user = new AuthUser("test1", "test2");

            _mockUserService.Setup(service => service.AreUserCredentialsCorrect("test1", "test2")).Returns(true);

            _mockAuth.Setup(auth => auth.GenerateJWTToken("test1")).Returns("test1234");

            var result = _controller.Login(user);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Login_ThrowsBadRequestException()
        {
            AuthUser user = new AuthUser("test1", "test2");

            _controller.ModelState.Clear();
            _controller.ModelState.AddModelError("Username", "The Username field is required.");

            _mockUserService.Setup(service => service.AreUserCredentialsCorrect("test1", "test2")).Returns(true);

            _mockAuth.Setup(auth => auth.GenerateJWTToken("test1")).Returns("test1234");

            Assert.Throws<BadRequestException>(() => _controller.Login(user));
        }

        [Fact]
        public void Login_ThrowsForbiddenException()
        {
            AuthUser user = new AuthUser("test1", "test2");

            _mockUserService.Setup(service => service.AreUserCredentialsCorrect("test1", "test2")).Returns(false);

            _mockAuth.Setup(auth => auth.GenerateJWTToken("test1")).Returns("test1234");

            Assert.Throws<ForbiddenException>(() => _controller.Login(user));
        }
    }
}
