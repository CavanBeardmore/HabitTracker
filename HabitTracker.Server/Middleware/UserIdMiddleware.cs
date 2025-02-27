using HabitTracker.Server.Services;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Middleware
{
    public class UserIdMiddleware
    {
        private readonly RequestDelegate _next;

        public UserIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserService userService)
        {
            string? username = context.User.Identity?.Name;

            if (username != null)
            {
                User? user = userService.GetByUsername(username);

                if (user != null)
                {
                    int userId = user.Id;
                    Console.WriteLine($"GOT USER ID {userId}");   
                    context.Items.Add("userId", userId);
                }
            }

            await _next(context);
        }
    }
}
