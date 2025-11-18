using HabitTracker.Server.DTOs;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Middleware;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace HabitTracker.Server.Tests.Middleware
{
    public class RateLimitingMiddlewareTests
    {
        private readonly ITestOutputHelper _output;
        public RateLimitingMiddlewareTests(ITestOutputHelper output) 
        {
            _output = output;
        }

        [Fact]
        public async Task RateLimitingMiddleware_ShouldThrowBadRequestExceptionWhenIpAddressIsMissing()
        {
            var context = new DefaultHttpContext();
            

            var loggerMock = new Mock<ILogger<RateLimitingMiddleware>>();
            var rateLimitServiceMock = new Mock<IRateLimitService>();

            RequestDelegate next = ctx =>
            {
                return Task.CompletedTask;
            };

            var middleware = new RateLimitingMiddleware(loggerMock.Object, next);

            await Assert.ThrowsAsync<BadRequestException>(async () => await middleware.InvokeAsync(context, rateLimitServiceMock.Object));
        }

        [Fact]
        public async Task RateLimitingMiddleware_ShouldCheckIfLimitedWithRemoteIpAddress()
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = new System.Net.IPAddress([1,1,1,1]);

            var loggerMock = new Mock<ILogger<RateLimitingMiddleware>>();
            var rateLimitServiceMock = new Mock<IRateLimitService>();
            rateLimitServiceMock.Setup(service => service.CheckRateLimitForIpAddress("1.1.1.1"));

            var calledNext = false;
            RequestDelegate next = ctx =>
            {
                calledNext = true;
                return Task.CompletedTask;
            };

            var middleware = new RateLimitingMiddleware(loggerMock.Object, next);
            await middleware.InvokeAsync(context, rateLimitServiceMock.Object);

            rateLimitServiceMock.Verify(service => service.CheckRateLimitForIpAddress("1.1.1.1"), Times.Once);
            Assert.True(calledNext);
        }

        [Fact]
        public async Task RateLimitingMiddleware_ShouldCheckIfLimitedWithIpFromHeaders()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("X-Forwarded-For", "2.2.2.2");

            var loggerMock = new Mock<ILogger<RateLimitingMiddleware>>();
            var rateLimitServiceMock = new Mock<IRateLimitService>();
            rateLimitServiceMock.Setup(service => service.CheckRateLimitForIpAddress("2.2.2.2"));

            var calledNext = false;
            RequestDelegate next = ctx =>
            {
                calledNext = true;
                return Task.CompletedTask;
            };

            var middleware = new RateLimitingMiddleware(loggerMock.Object, next);
            await middleware.InvokeAsync(context, rateLimitServiceMock.Object);

            rateLimitServiceMock.Verify(service => service.CheckRateLimitForIpAddress("2.2.2.2"), Times.Once);
            Assert.True(calledNext);
        }
    }
}
