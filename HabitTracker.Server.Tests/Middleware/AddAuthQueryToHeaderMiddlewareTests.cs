

using HabitTracker.Server.Middleware;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace HabitTracker.Server.Tests.Middleware
{
    public class AddAuthQueryToHeaderMiddlewareTests
    {
        public AddAuthQueryToHeaderMiddlewareTests() { }

        [Fact]
        public async Task InvokeAsync_AddsAuthHeaderWhenQueryParamExists()
        {
            var token = "mockJwtToken";
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString($"?token={token}");

            var loggerMock = new Mock<ILogger<UserIdMiddleware>>();
            var userServiceMock = new Mock<IUserService>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new AddAuthQueryToHeaderMiddleware(loggerMock.Object, next);

            await middleware.InvokeAsync(context, userServiceMock.Object);

            Assert.True(context.Request.Headers.ContainsKey("Authorization"));
            Assert.Equal($"Bearer {token}", context.Request.Headers["Authorization"]);
            Assert.True(wasNextCalled);
        }

        [Fact]
        public async Task InvokeAsync_AuthHeaderIsMissingWhenQueryParamDoesntExist()
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

            var middleware = new AddAuthQueryToHeaderMiddleware(loggerMock.Object, next);

            await middleware.InvokeAsync(context, userServiceMock.Object);

            Assert.False(context.Request.Headers.ContainsKey("Authorization"));
            Assert.True(wasNextCalled);
        }
    }
}
