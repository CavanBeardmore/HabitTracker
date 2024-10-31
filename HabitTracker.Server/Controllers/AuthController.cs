using HabitTracker.Server.Auth;
using HabitTracker.Server.Models;
using HabitTracker.Server.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.Server.Controllers
{
    [ApiController]
    [Route("auth/[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthenticationService _authService;
        private readonly UserRepository _userRepository;

        public AuthController(AuthenticationService authService, UserRepository userService)
        {
            _authService = authService;
            _userRepository = userService;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] AuthUser user)
        {

            var foundUser = _userRepository.GetByUsername(user.Username);
            if (foundUser == null)
            {
                return NotFound();
            }
            Console.WriteLine(user.Password);
            PasswordService passwordService = new PasswordService(user.Password);
            Console.WriteLine(foundUser.Password);
            bool isPasswordCorrect = passwordService.VerifyPassword(foundUser.Password);

            if (!isPasswordCorrect)
            {
                return Unauthorized("Incorrect password");
            }

            var jwt = _authService.GenerateJWTToken(foundUser.Username);

            return Ok(jwt);
        }
    }
}
