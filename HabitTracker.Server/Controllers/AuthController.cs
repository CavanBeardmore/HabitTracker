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
        private readonly Authentication _auth;
        private readonly UserRepository _userRepository;

        public AuthController(Authentication auth, UserRepository userService)
        {
            _auth = auth;
            _userRepository = userService;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] AuthUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var foundUser = _userRepository.GetByUsername(user.Username);
            if (foundUser == null)
            {
                return NotFound();
            }

            PasswordService passwordService = new PasswordService(user.Password);

            bool isPasswordCorrect = passwordService.VerifyPassword(foundUser.Password);

            if (!isPasswordCorrect)
            {
                return Unauthorized("Incorrect password");
            }

            var jwt = _auth.GenerateJWTToken(foundUser.Username);

            return Ok(jwt);
        }
    }
}
