using HabitTracker.Server.Services;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Middleware
{
    public class AddAuthQueryToHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserIdMiddleware> _logger;

        public AddAuthQueryToHeaderMiddleware(ILogger<UserIdMiddleware> logger, RequestDelegate next)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            if (context.Request.Query.TryGetValue("token", out var jwtToken))
            {
                _logger.LogInformation("AddAuthQueryToHeaderMiddleware - InvokeAsync - Adding JWT to header - {jwtToken}", jwtToken);
                context.Request.Headers["Authorization"] = $"Bearer {jwtToken}";
            }

            await _next(context);
        }
    }
}
