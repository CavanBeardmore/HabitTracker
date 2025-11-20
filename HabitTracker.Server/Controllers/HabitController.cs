using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Services;
using HabitTracker.Server.Exceptions;
using System.Text.Json;
using HabitTracker.Server.SSE;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache _cache;

        public HabitController(ILogger<HabitController>  logger, IEventService<HabitTrackerEvent> eventService, IHabitService habitService, IMemoryCache cache)
        {
            _logger = logger;
            _habitService = habitService;
            _eventService = eventService;
            _cache = cache;
        }

        private int GetUserId()
        {
            if (HttpContext.Items.TryGetValue("userId", out var userIdObj) == false || userIdObj is not int userId)
            {
                throw new UnauthorizedException("Could not retrieve user id from JWT");
            }

            return userId;
        }

        private void AddToCache(int userId, int habitId, Habit value)
        {
            _cache.Set(GenerateCacheKey(userId, habitId), value, TimeSpan.FromMinutes(30));
        }

        private void AddToCache(int userId, IReadOnlyCollection<Habit?> value)
        {
            _cache.Set(GenerateCacheKey(userId), value, TimeSpan.FromMinutes(30));
        }

        private T? GetFromCache<T>(int userId, int? habitId = null)
        {
            return _cache.Get<T>(GenerateCacheKey(userId, habitId));
        }

        private void RemoveFromCache(int userId, int? habitId = null)
        {
            _cache.Remove(GenerateCacheKey(userId, habitId));
        }

        private string GenerateCacheKey(int userId, int? habitId = null)
        {
            return habitId != null ? $"habit_user:{userId}_habit:{habitId}" : $"habit_user:{userId}_habits";
        }

        [Authorize]
        [HttpGet("{habitId}")]
        public IActionResult GetHabit(int habitId)
        {
            _logger.LogInformation("HabitController - GetHabit - invoked");
            int userId = GetUserId();

            Habit? cachedHabit = GetFromCache<Habit>(userId, habitId);

            if (cachedHabit != null)
            {
                _logger.LogInformation("HabitController - GetHabit - returning cached habit for user - {@Userid}", userId);
                return Ok(cachedHabit);
            }

            _logger.LogInformation("HabitController - GetHabit - retrieving habit for user - {@Userid}", userId);
            Habit? habit = _habitService.GetById(habitId, userId);
            if (habit != null)
            {
                AddToCache(userId, habitId, habit);
            }

            return Ok(habit);
        }

        [Authorize]
        [HttpGet()]
        public IActionResult GetUserHabits()
        {
            _logger.LogInformation("HabitController - GetUserHabits - invoked");
            int userId = GetUserId();

            IReadOnlyCollection<Habit>? cachedUserHabits = GetFromCache<IReadOnlyCollection<Habit>>(userId);

            if (cachedUserHabits != null)
            {
                _logger.LogInformation("HabitController - GetUserHabits - retrieved cached user habits for user - {@Userid}", userId);
                return Ok(cachedUserHabits);
            }

            _logger.LogInformation("HabitController - GetUserHabits - retrieving user habits for user - {@Userid}", userId);
            IReadOnlyCollection<Habit?> habits = _habitService.GetAllByUserId(userId);
            if (habits.Any())
            {
                AddToCache(userId, habits);
            }

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
                AddToCache(userId, createdHabit.Id, createdHabit);
                RemoveFromCache(userId);
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
                AddToCache(userId, updatedHabit.Id, updatedHabit);
                RemoveFromCache(userId);
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
                RemoveFromCache(userId, habitId);
                RemoveFromCache(userId);
                return NoContent();
            }

            throw new AppException($"Could not delete habit ${habitId} - for user - {userId}");
        }
    }
}
