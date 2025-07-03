using HabitTracker.Server.Middleware;
using HabitTracker.Server.Services;
using HabitTracker.Server.SSE;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.DTOs;
using System.Text;

namespace HabitTracker.Server.Tests.Middleware
{
    public class ExceptionMiddlewareTests
    {
        public ExceptionMiddlewareTests() { }

        [Fact]
        public async Task ExceptionMiddleware_CallsNextAndDoesntThrow()
        {
            var context = new DefaultHttpContext();

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            Assert.True(wasNextCalled);
        }

        [Fact]
        public async Task ExceptionMiddleware_ReturnsForbiddenResponseWhenForbiddenExceptionIsThrown()
        {
            var context = new DefaultHttpContext();

            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new ForbiddenException("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
            var response = await reader.ReadToEndAsync();

            Assert.True(wasNextCalled);
            Assert.True(context.Response.StatusCode == 403);
            Assert.True(response == "\"test\"");
        }

        [Fact]
        public async Task ExceptionMiddleware_ReturnsConflictResponseWhenConflictExceptionIsThrown()
        {
            var context = new DefaultHttpContext();

            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new ConflictException("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
            var response = await reader.ReadToEndAsync();

            Assert.True(wasNextCalled);
            Assert.True(context.Response.StatusCode == 409);
            Assert.True(response == "\"test\"");
        }

        [Fact]
        public async Task ExceptionMiddleware_ReturnsBadRequestResponseWhenBadRequestExceptionIsThrown()
        {
            var context = new DefaultHttpContext();

            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new BadRequestException("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
            var response = await reader.ReadToEndAsync();

            Assert.True(wasNextCalled);
            Assert.True(context.Response.StatusCode == 400);
            Assert.True(response == "\"test\"");
        }

        [Fact]
        public async Task ExceptionMiddleware_ReturnsTooManyRequestsResponseWhenTooManyRequestsExceptionIsThrown()
        {
            var context = new DefaultHttpContext();

            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new TooManyRequestsException("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
            var response = await reader.ReadToEndAsync();

            Assert.True(wasNextCalled);
            Assert.True(context.Response.StatusCode == 429);
            Assert.True(response == "\"test\"");
        }

        [Fact]
        public async Task ExceptionMiddleware_ReturnsUnauthorisedResponseWhenUnauthorisedExceptionIsThrown()
        {
            var context = new DefaultHttpContext();

            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new UnauthorizedException("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
            var response = await reader.ReadToEndAsync();

            Assert.True(wasNextCalled);
            Assert.True(context.Response.StatusCode == 401);
            Assert.True(response == "\"test\"");
        }

        [Fact]
        public async Task ExceptionMiddleware_ReturnsNotFoundsResponseWhenNotFoundExceptionIsThrown()
        {
            var context = new DefaultHttpContext();

            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new NotFoundException("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
            var response = await reader.ReadToEndAsync();

            Assert.True(wasNextCalled);
            Assert.True(context.Response.StatusCode == 404);
            Assert.True(response == "\"test\"");
        }

        [Fact]
        public async Task ExceptionMiddleware_ReturnsInternalServerErrorResponseWhenAppExceptionIsThrown()
        {
            var context = new DefaultHttpContext();

            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new AppException("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
            var response = await reader.ReadToEndAsync();

            Assert.True(wasNextCalled);
            Assert.True(context.Response.StatusCode == 500);
            Assert.True(response == "\"test\"");
        }

        [Fact]
        public async Task ExceptionMiddleware_ReturnsInternalServerErrorResponseWhenExceptionIsThrown()
        {
            var context = new DefaultHttpContext();

            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new Exception("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
            var response = await reader.ReadToEndAsync();

            Assert.True(wasNextCalled);
            Assert.True(context.Response.StatusCode == 500);
            Assert.True(response == "\"An error occurred\"");
        }

        [Fact]
        public async Task ExceptionMiddleware_CallsAddEventWhenUserIdExistsAndEventsAreActiveForUser()
        {
            var context = new DefaultHttpContext();
            context.Items.Add("userId", 1);

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();
            eventServiceMock.Setup(service => service.IsActiveForUser(1)).Returns(true);

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new Exception("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            eventServiceMock.Verify(service => service.AddEvent(1, It.IsAny<HabitTrackerEvent>()), Times.Once());
            Assert.True(wasNextCalled);
        }

        [Fact]
        public async Task ExceptionMiddleware_DoesntCallsAddEventWhenUserIdDoesntExist()
        {
            var context = new DefaultHttpContext();

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();
            eventServiceMock.Setup(service => service.IsActiveForUser(1)).Returns(true);

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new Exception("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            eventServiceMock.Verify(service => service.AddEvent(1, It.IsAny<HabitTrackerEvent>()), Times.Never());
            Assert.True(wasNextCalled);
        }

        [Fact]
        public async Task ExceptionMiddleware_DoesntCallsAddEventWhenUserDoesntHaveEventsActive()
        {
            var context = new DefaultHttpContext();
            context.Items.Add("userId", 1);

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var eventServiceMock = new Mock<IEventService<HabitTrackerEvent>>();
            eventServiceMock.Setup(service => service.IsActiveForUser(1)).Returns(false);

            var wasNextCalled = false;
            RequestDelegate next = ctx =>
            {
                wasNextCalled = true;
                throw new Exception("test");
            };

            var middleware = new ExceptionMiddleware(next, loggerMock.Object, eventServiceMock.Object);

            await middleware.InvokeAsync(context);

            eventServiceMock.Verify(service => service.IsActiveForUser(1), Times.Once());
            eventServiceMock.Verify(service => service.AddEvent(1, It.IsAny<HabitTrackerEvent>()), Times.Never());
            Assert.True(wasNextCalled);
        }
    }
}
