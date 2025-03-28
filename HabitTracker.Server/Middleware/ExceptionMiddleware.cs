using HabitTracker.Server.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace HabitTracker.Server.Middleware
{

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebApplicationBuilder _builder;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, WebApplicationBuilder builder, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _builder = builder;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - FORBIDDEN");
                await HandleExceptionAsync(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (ConflictException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - CONFLICT");
                await HandleExceptionAsync(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - BAD REQUEST");
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (TooManyRequestsException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - TOO MANY REQUESTS");
                await HandleExceptionAsync(context, HttpStatusCode.TooManyRequests, ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - UNAUTHORIZED");
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - NOT FOUND");
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (AppException ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - APP EXCEPTION");
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("ExceptionMiddleware - InvokeAsync - EXCEPTION");
                _logger.LogError(ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An error occurred");
            }
        }

        private Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = statusCode,
                Message = message
            };

            string jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }

}
