using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Services;
using HabitTracker.Server.Exceptions;
using System.Text.Json;
using HabitTracker.Server.SSE;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("habits/[controller]")]
    public class HabitController : ControllerBase
    {

        private readonly IHabitService _habitService;
        private readonly IEventService<HabitTrackerEvent> _eventService;
        private readonly ILogger<HabitController> _logger;

        public HabitController(ILogger<HabitController>  logger, IEventService<HabitTrackerEvent> eventService, IHabitService habitService)
        {
            _logger = logger;
            _habitService = habitService;
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


        [Authorize]
        [HttpGet("{habitId}")]
        public IActionResult GetHabit(int habitId)
        {
            _logger.LogInformation("HabitController - GetHabit - invoked");
            int userId = GetUserId();

            _logger.LogInformation("HabitController - GetHabit - retrieving habit for user - {@Userid}", userId);
            Habit? habit = _habitService.GetById(habitId, userId);

            return Ok(habit);
        }

        [Authorize]
        [HttpGet()]
        public IActionResult GetUserHabits()
        {
            _logger.LogInformation("HabitController - GetUserHabits - invoked");
            int userId = GetUserId();

            _logger.LogInformation("HabitController - GetUserHabits - retrieving user habits for user - {@Userid}", userId);
            IReadOnlyCollection<Habit?> habits = _habitService.GetAllByUserId(userId);

            return Ok(habits);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateHabit([FromBody] PostHabit habit)
        {
            _logger.LogInformation("HabitController - CreateHabit - invoked");
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

            _logger.LogInformation("HabitController - CreateHabit - adding habit for - {@Userid}", userId);
            Habit? createdHabit = _habitService.Add(userId, habit);

            if (createdHabit != null)
            {
                _logger.LogInformation("HabitController - CreateHabit - successfully created habit for user -  {@Userid}", userId);
                _eventService.AddEvent(userId, new HabitTrackerEvent(HabitTrackerEventTypes.HABIT_ADDED, createdHabit));
                return Ok();
            }

            throw new AppException($"Could not create habit ${JsonSerializer.Serialize(habit)} - for user - {userId}");
        }

        [Authorize]
        [HttpPatch("update")]
        public IActionResult UpdateHabit([FromBody] PatchHabit habit)
        {
            _logger.LogInformation("HabitController - UpdateHabit - invoked");
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

            _logger.LogInformation("HabitController - UpdateHabit - updating habit for - {@Userid}", userId);
            Habit? updatedHabit = _habitService.Update(userId, habit);

            if (updatedHabit != null)
            {
                _logger.LogInformation("HabitController - UpdateHabit - successfully updated habit for - {@Userid}", userId);
                _eventService.AddEvent(userId, new HabitTrackerEvent(HabitTrackerEventTypes.HABIT_UPDATED, updatedHabit));
                return Ok();
            }

            throw new AppException($"Could not update habit ${JsonSerializer.Serialize(habit)} - for user - {userId}");
        }

        [Authorize]
        [HttpDelete("delete/{habitId}")]
        public IActionResult DeleteHabit(int habitId)
        {
            _logger.LogInformation("HabitController - DeleteHabit - invoked");
            int userId = GetUserId();

            _logger.LogInformation("HabitController - DeleteHabit - deleting habit for - {@Userid}", userId);
            bool success = _habitService.Delete(habitId, userId);

            if (success)
            {
                _logger.LogInformation("HabitController - DeleteHabit - successfully deleted habit for - {@Userid}", userId);
                _eventService.AddEvent(userId, new HabitTrackerEvent(HabitTrackerEventTypes.HABIT_DELETED, habitId));
                return NoContent();
            }

            throw new AppException($"Could not delete habit ${habitId} - for user - {userId}");
        }
    }
}
