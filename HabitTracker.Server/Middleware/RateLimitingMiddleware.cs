using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Services;

namespace HabitTracker.Server.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        public RateLimitingMiddleware(ILogger<RateLimitingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRateLimitService rateLimitService)
        {
            _logger.LogInformation("RateLimitingMiddleware - InvokeAsync - Obtaining IP Address from Http Context - {IpAddress}", context.Connection.RemoteIpAddress);
            string? clientIp = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(clientIp))
            {
                _logger.LogInformation("RateLimitingMiddleware - InvokeAsync - Obtaining IP Address from X-Forwarded-For header instead - {IpAddress}", context.Request.Headers["X-Forwarded-For"].FirstOrDefault());
                clientIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            if (string.IsNullOrEmpty(clientIp))
            {
                _logger.LogInformation("RateLimitingMiddleware - InvokeAsync - Could not obtain IP address");
                throw new BadRequestException("IP Address is missing from the request");
            }

            rateLimitService.CheckRateLimitForIpAddress(clientIp);                
            await _next(context);
        }
    }
}
