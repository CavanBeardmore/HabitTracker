using HabitTracker.Server.Exceptions;
using HabitTracker.Server.SSE;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Middleware
{

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IEventService<HabitTrackerEvent> _eventService;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IEventService<HabitTrackerEvent> eventService)
        {
            _next = next;
            _logger = logger;
            _eventService = eventService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - FORBIDDEN - {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (ConflictException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - CONFLICT - {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - BAD REQUEST - {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (TooManyRequestsException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - TOO MANY REQUESTS - {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.TooManyRequests, ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - UNAUTHORIZED - {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - NOT FOUND - {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (AppException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - APP EXCEPTION - {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - EXCEPTION - {Message}", ex.Message);
                _logger.LogError(ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An error occurred");
            }
        }

        private Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            int? userId = GetUserId(context);
            _logger.LogDebug("ExceptionMiddleware - HandleExceptionAsync - user id: {UserId}", userId);
            _logger.LogDebug("ExceptionMiddleware - HandleExceptionAsync - is active: {Active}", _eventService.IsActiveForUser((int)userId));
            if (userId != null && _eventService.IsActiveForUser((int)userId))
            {
                _logger.LogError("ExceptionMiddleware - HandleExceptionAsync - adding error event to event stream");
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                _eventService.AddEvent((int)userId, new HabitTrackerEvent(HabitTrackerEventTypes.ERROR, new Error((int)statusCode, message)));
                return Task.CompletedTask;
            }

            _logger.LogError("ExceptionMiddleware - HandleExceptionAsync - returning error response");
            string jsonResponse = JsonSerializer.Serialize(message);

            return context.Response.WriteAsync(jsonResponse);
        }

        private int? GetUserId(HttpContext context)
        {
            if (context.Items.TryGetValue("userId", out var userIdObj) == false || userIdObj is not int userId)
            {
                return null;
            }

            return userId;
        }
    }

}
