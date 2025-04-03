using HabitTracker.Server.Auth;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Models;
using HabitTracker.Server.Exceptions;
using System.Text.Json;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("user/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        private int GetUserId()
        {
            if (HttpContext.Items.TryGetValue("userId", out var userIdObj) == false || userIdObj is not int userId)
            {
                throw new UnauthorizedException("Could not retrieve user id from JWT");
            }

            return userId;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddUser([FromBody] PostUser user)
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

            _logger.LogInformation("UserController - AddUser - invoked");
            _logger.LogInformation("UserController - AddUser - adding user");
            bool success = _userService.Add(user);

            if (success)
            {
                _logger.LogInformation("UserController - AddUser - successfully added user");
                return Created();
            }

            throw new AppException("Could not create user.");
        }

        [Authorize]
        [HttpPatch("update")]
        public IActionResult UpdateUser([FromBody] PatchUser user)
        {
            _logger.LogInformation("UserController - UpdateUser - invoked");
            int userId = GetUserId();

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

            _logger.LogInformation("UserController - AddUser - updating user");
            string? jwt = _userService.Update(userId, user);

            _logger.LogInformation("UserController - AddUser - successfully updated user record and created new JWT");
            return Ok(jwt);
        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult DeleteUser([FromBody] AuthUser user)
        {
            _logger.LogInformation("UserController - DeleteUser - invoked");
            int userId = GetUserId();

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

            _logger.LogInformation("UserController - DeleteUser - deleting user");
            bool success = _userService.Delete(userId, user);

            if (success)
            {
                _logger.LogInformation("UserController - DeleteUser - successfully deleted user");
                return NoContent();
            }

            throw new AppException($"Unable to delete user {userId}");
        }
    }
}
