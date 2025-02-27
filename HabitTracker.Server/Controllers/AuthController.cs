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
        private readonly UserService _userService;

        public AuthController(IAuthentication auth, UserService userService)
        {
            _auth = auth;
            _userService = userService;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] AuthUser user)
        {
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
                string jwt = _auth.GenerateJWTToken(user.Username);

                return Ok(jwt);
            }

            throw new ForbiddenException("Incorrect credentials");
        }
    }
}
