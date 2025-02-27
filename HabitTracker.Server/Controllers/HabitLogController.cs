using HabitTracker.Server.Repository;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Services;
using HabitTracker.Server.DTOs;
using HabitTracker.Server.Exceptions;
using HabitTracker.Server.Database.Entities;
using System.Text.Json;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("habitLogs/[controller]")]
    public class HabitLogController : ControllerBase
    {

        private readonly HabitLogService _habitLogService; 

        public HabitLogController(HabitLogService habitLogService)
        {
            _habitLogService = habitLogService;
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
            int userId = GetUserId();

            HabitLog? habitLog = _habitLogService.GetById(habitLogId, userId);
            if (habitLog == null)
            {
                throw new NotFoundException($"Could not find habit log of id - {habitLogId} - for user - {userId}");
            }

            return Ok(habitLog);
        }

        [Authorize]
        [HttpGet("habit/{habitId}")]
        public IActionResult GetHabitLogsFromHabit(int habitId, [FromQuery] int pageNumber)
        {
            int userId = GetUserId();

            IReadOnlyCollection<HabitLog?> habitLogs = _habitLogService.GetAllByHabitId(habitId, userId, pageNumber);
            if (habitLogs.Count() == 0)
            {
                throw new NotFoundException($"Could not find habit logs from habit id - {habitId} - for user - {userId} - from page - {pageNumber}");

            }

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

            IReadOnlyCollection<HabitLog?> habitLogs = _habitLogService.Add(habitLog);

            if (habitLogs.Count() == 0)
            {
                throw new AppException($"Could not create habit log from - {habitLog}");
            }

            return Created($"habitLogs/habit/{habitLog.Habit_id}", habitLogs);
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

            bool success = _habitLogService.Update(habitLog);

            if (success)
            {
                return Ok(habitLog);
            }

            throw new AppException($"Could not update habit log from - {habitLog}");
        }

        [Authorize]
        [HttpDelete("delete/{habitLogId}")]
        public IActionResult DeleteHabitLog(int habitLogId)
        {
            int userId = GetUserId();

            bool success = _habitLogService.Delete(habitLogId, userId);

            if (success)
            {
                return NoContent();
            }
            throw new AppException($"Could not delete habit log from - {habitLogId} - for user - {userId}");
        }
    }
}
