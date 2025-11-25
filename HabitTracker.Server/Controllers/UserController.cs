using HabitTracker.Server.Auth;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Models;
using HabitTracker.Server.Exceptions;
using System.Text.Json;
using HabitTracker.Server.SSE;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IEventService<HabitTrackerEvent> _eventService;

        public UserController(ILogger<UserController> logger, IUserService userService, IEventService<HabitTrackerEvent> eventService)
        {
            _logger = logger;
            _userService = userService;
            _eventService = eventService;
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
        [HttpGet()]
        public IActionResult GetUser()
        {
            _logger.LogInformation("UserController - GetUser - invoked");
            
            int userId = GetUserId();
            User? user = _userService.Get(userId);

            if (user == null)
            {
                throw new AppException("Could not get user.");
            }
            
            _logger.LogInformation("UserController - GetUser - successfully retrieved user");
            return Ok(user);
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
            UpdatedUserResult? response = _userService.Update(userId, user);

            if (response != null)
            {
                _logger.LogInformation("UserController - AddUser - successfully updated user record and created new JWT");
                _eventService.AddEvent(userId, new HabitTrackerEvent(HabitTrackerEventTypes.USER_UPDATED, new { jwt = response.Jwt, user = response.User }));
                return Ok();
            }

            throw new AppException("Could not update user.");
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
                _eventService.AddEvent(userId, new HabitTrackerEvent(HabitTrackerEventTypes.USER_DELETED, userId));
                return NoContent();
            }

            throw new AppException($"Unable to delete user {userId}");
        }
    }
}
