using HabitTracker.Server.Middleware;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using HabitTracker.Server.DTOs;
using System.Security.Claims;
using HabitTracker.Server.Exceptions;

namespace HabitTracker.Server.Tests.Middleware
{
    public class UserIdMiddlewareTests
    {
        public UserIdMiddlewareTests() 
        {
        }

        [Fact]
        public async Task UserIdMiddleware_DoesntAddUserIdItemWhenIdentityDoesntExist()
        {
            var context = new DefaultHttpContext();

            var loggerMock = new Mock<ILogger<UserIdMiddleware>>();
            var userServiceMock = new Mock<IUserService>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new UserIdMiddleware(loggerMock.Object, next);

            await middleware.InvokeAsync(context, userServiceMock.Object);

            Assert.False(context.Items.ContainsKey("userId"));
            Assert.True(wasNextCalled);
        }

        [Fact]
        public async Task UserIdMiddleware_AddsUserIdItemWhenIdentityExists()
        {
            var context = new DefaultHttpContext();
            var identity = new ClaimsIdentity(
                new List<Claim> { new Claim(ClaimTypes.Name, "test") },
                authenticationType: "CustomAuthType",
                nameType: ClaimTypes.Name,
                roleType: ClaimTypes.Role
              );
            context.User = new ClaimsPrincipal(identity);

            var loggerMock = new Mock<ILogger<UserIdMiddleware>>();
            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(service => service.Get("test")).Returns(new User(1, "test", "test", "test"));

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new UserIdMiddleware(loggerMock.Object, next);

            await middleware.InvokeAsync(context, userServiceMock.Object);

            Assert.True(context.Items.ContainsKey("userId"));
            context.Items.TryGetValue("userId", out var userId);
            Assert.True((int)userId == 1);
            Assert.True(wasNextCalled);
        }
    }
}
