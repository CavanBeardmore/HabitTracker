using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Services;
using HabitTracker.Server.Exceptions;
using System.Text.Json;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("habits/[controller]")]
    public class HabitController : ControllerBase
    {

        private readonly HabitService _habitService;
        private readonly UserService _userService;

        public HabitController(HabitService habitService, UserService userService)
        {
            _habitService = habitService;
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


        [Authorize]
        [HttpGet("{habitId}")]
        public IActionResult GetHabit(int habitId)
        {
            int userId = GetUserId();

            Habit? habit = _habitService.GetById(habitId, userId);

            return Ok(habit);
        }

        [Authorize]
        [HttpGet()]
        public IActionResult GetUserHabits()
        {
            int userId = GetUserId();

            IReadOnlyCollection<Habit?> habits = _habitService.GetAllByUserId(userId);

            return Ok(habits);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateHabit([FromBody] PostHabit habit)
        {
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

            Habit? createdHabit = _habitService.Add(userId, habit);

            if (createdHabit != null)
            { 
                return Created($"habits/{createdHabit.Id}", createdHabit);
            }

            throw new AppException($"Could not create habit ${JsonSerializer.Serialize(habit)} - for user - {userId}");
        }

        [Authorize]
        [HttpPatch("update")]
        public IActionResult UpdateHabit([FromBody] PatchHabit habit)
        {
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

            bool success = _habitService.Update(userId, habit);

            if (success)
            {
                return Ok();
            }

            throw new AppException($"Could not update habit ${JsonSerializer.Serialize(habit)} - for user - {userId}");
        }

        [Authorize]
        [HttpDelete("delete/{habitId}")]
        public IActionResult DeleteHabit(int habitId)
        {
            int userId = GetUserId();

            bool success = _habitService.Delete(habitId, userId);

            if (success)
            {
                return NoContent();
            }

            throw new AppException($"Could not delete habit ${habitId} - for user - {userId}");
        }
    }
}
