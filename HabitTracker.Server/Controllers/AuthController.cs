using HabitTracker.Server.Auth;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Services;
using HabitTracker.Server.Services.Responses;

namespace HabitTracker.Server.Controllers
{
    [ApiController]
    [Route("auth/[controller]")]
    public class AuthController : Controller
    {
        private readonly Authentication _auth;
        private readonly UserService _userService;

        public AuthController(Authentication auth, UserService userService)
        {
            _auth = auth;
            _userService = userService;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] AuthUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IServiceResponse response = _userService.AreUserCredentialsCorrect(user.Username, user.Password);

                if (response.Success == false && response.Error != null)
                {
                    return StatusCode(500, response.Error);
                }

                if (response.Success == false)
                {
                    return BadRequest("Invalid Credentials");
                }

                var jwt = _auth.GenerateJWTToken(user.Username);

                return Ok(jwt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
