using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Services;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Exceptions;
using System.Text.Json;
using HabitTracker.Server.SSE;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("habitLogs/[controller]")]
    public class HabitLogController : ControllerBase
    {

        private readonly IHabitLogService _habitLogService; 
        private readonly ILogger<HabitLogController> _logger;
        private readonly IEventService<HabitTrackerEvent> _eventService;

        public HabitLogController(ILogger<HabitLogController>  logger, IHabitLogService habitLogService, IEventService<HabitTrackerEvent> eventService)
        {
            _logger = logger;
            _habitLogService = habitLogService;
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
        [HttpGet("{habitLogId}")]
        public IActionResult GetHabitLog(int habitLogId)
        {
            _logger.LogInformation("HabitLogController - GetHabitLog - invoked");
            int userId = GetUserId();

            _logger.LogInformation("HabitLogController - GetHabitLog - getting habit log for - {@Userid}", userId);
            HabitLog? habitLog = _habitLogService.GetById(habitLogId, userId);

            if (habitLog == null)
            {
                throw new NotFoundException($"Could not find habit log of id - {habitLogId} - for user - {userId}");
            }

            _logger.LogInformation("HabitLogController - GetHabitLog - successfully retrieved habit log for - {@Userid}", userId);
            return Ok(habitLog);
        }

        [Authorize]
        [HttpGet("habit/{habitId}")]
        public IActionResult GetHabitLogsFromHabit(int habitId, [FromQuery] int pageNumber)
        {
            _logger.LogInformation("HabitLogController - GetHabitLogsFromHabit - invoked");
            int userId = GetUserId();

            _logger.LogInformation("HabitLogController - GetHabitLogsFromHabit - getting habit logs from habit id for - {@Userid}", userId);
            IReadOnlyCollection<HabitLog?> habitLogs = _habitLogService.GetAllByHabitId(habitId, userId, pageNumber);

            if (habitLogs.Count() == 0)
            {
                throw new NotFoundException($"Could not find habit logs from habit id - {habitId} - for user - {userId} - from page - {pageNumber}");

            }

            _logger.LogInformation("HabitLogController - GetHabitLogsFromHabit - successfully retrieved habit logs from habit id for - {@Userid}", userId);
            return Ok(habitLogs);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateHabitLog([FromBody] PostHabitLog habitLog)
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

            _logger.LogInformation("HabitLogController - CreateHabitLog - invoked");
            int userId = GetUserId();

            _logger.LogInformation("HabitLogController - CreateHabitLog - adding habit log for - {@Userid}", userId);
            Tuple<Habit, HabitLog>? result = _habitLogService.Add(habitLog, userId);

            if (result == null)
            {
                throw new AppException($"Could not create habit log from - {habitLog}");
            }

            _logger.LogInformation("HabitLogController - CreateHabitLog - successfully added habit log for - {@Userid}", userId);
            _eventService.AddEvent(userId, new HabitTrackerEvent(HabitTrackerEventTypes.HABIT_LOG_ADDED, new { habit = result.Item1, habitLog = result.Item2}));
            return Ok();
        }

        [Authorize]
        [HttpPatch("update")]
        public IActionResult UpdateHabitLog([FromBody] PatchHabitLog habitLog)
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
            int userId = GetUserId();

            _logger.LogInformation("HabitLogController - UpdateHabitLog - invoked");
            _logger.LogInformation("HabitLogController - UpdateHabitLog - updating habit log");
            HabitLog? updatedLog = _habitLogService.Update(habitLog);

            if (updatedLog != null)
            {
                _logger.LogInformation("HabitLogController - UpdateHabitLog - successfully updated habit log");
                _eventService.AddEvent(userId, new HabitTrackerEvent(HabitTrackerEventTypes.HABIT_LOG_UPDATED, updatedLog));
                return Ok();
            }

            throw new AppException($"Could not update habit log from - {habitLog}");
        }

        [Authorize]
        [HttpDelete("delete/{habitLogId}")]
        public IActionResult DeleteHabitLog(int habitLogId)
        {
            _logger.LogInformation("HabitLogController - DeleteHabitLog - invoked");
            int userId = GetUserId();

            _logger.LogInformation("HabitLogController - DeleteHabitLog - deleting habit log for {@Userid}", userId);
            bool success = _habitLogService.Delete(habitLogId, userId);

            if (success)
            {
                _logger.LogInformation("HabitLogController - DeleteHabitLog - successfully deleted habit log for {@Userid}", userId);
                _eventService.AddEvent(userId, new HabitTrackerEvent(HabitTrackerEventTypes.HABIT_LOG_DELETED, habitLogId));
                return NoContent();
            }

            throw new AppException($"Could not delete habit log from - {habitLogId} - for user - {userId}");
        }
    }
}
