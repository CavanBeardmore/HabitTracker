using HabitTracker.Server.Classes.Auth;
using HabitTracker.Server.Classes.PasswordService;
using HabitTracker.Server.Classes.User;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.Server.Controllers
{
    [ApiController]
    [Route("auth/[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthenticationService _authService;
        private readonly UserService _userService;

        public AuthController(AuthenticationService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] CreateUserRequest data)
        {
            var user = _userService.GetByUsername(data.Username);
            if (user == null)
            {
                return NotFound();
            }

            PasswordService passwordService = new PasswordService(user.password);
            bool isPasswordCorrect = passwordService.VerifyPassword(data.Password);

            if (!isPasswordCorrect)
            {
                return Unauthorized("Incorrect password");
            }

            var jwt = _authService.GenerateJWTToken(data.Username);
            Console.WriteLine("jwt secret", jwt);

            return Ok(jwt);
        }
    }
}
