using HabitTracker.Server.Services;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Exceptions;

namespace HabitTracker.Server.Middleware
{
    public class UserIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserIdMiddleware> _logger;

        public UserIdMiddleware(ILogger<UserIdMiddleware> logger, RequestDelegate next)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            string? username = context.User.Identity?.Name;

            if (username != null)
            {
                User? user = userService.Get(username);

                if (user != null)
                {
                    int userId = user.Id;
                    context.Items.Add("userId", userId);
                }
            }
                    
            await _next(context);
        }
    }
}
