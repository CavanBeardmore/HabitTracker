using HabitTracker.Server.Auth;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Services;
using HabitTracker.Server.Exceptions;
using System.Text.Json;

namespace HabitTracker.Server.Controllers
{
    [ApiController]
    [Route("auth/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthentication _auth;
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, IAuthentication auth, IUserService userService)
        {
            _auth = auth;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] AuthUser user)
        {
            _logger.LogInformation("AuthController - Login - endpoint invoked");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
   
                throw new BadRequestException(JsonSerializer.Serialize(errors));
            }

            bool areCredentialsCorrect = _userService.AreUserCredentialsCorrect(user.Username, user.Password);

            if (areCredentialsCorrect)
            {
                _logger.LogInformation("AuthController - Login - Credentials are correct generating JWT token");
                string jwt = _auth.GenerateJWTToken(user.Username);

                return Ok(new { token = jwt });
            }

            _logger.LogInformation("AuthController - Login - credentials are incorrect");

            throw new ForbiddenException("Incorrect credentials");
        }
    }
}
