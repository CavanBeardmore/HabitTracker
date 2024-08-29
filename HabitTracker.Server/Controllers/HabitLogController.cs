using HabitTracker.Server.Classes.Habit;
using HabitTracker.Server.Classes.HabitLog;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.Server.Controllers
{
    [ApiController]
    [Route("habitLogs/[controller]")]
    public class HabitLogController : ControllerBase
    {

        private readonly HabitLogService _habitLogService;

        public HabitLogController(HabitLogService habitLogService)
        {
            _habitLogService = habitLogService;
        }

        [HttpGet("{habitLog_id}")]
        public IActionResult GetHabitLog(int habitLog_id)
        {
            var habitLog = _habitLogService.GetById(habitLog_id);
            if (habitLog == null)
            {
                return NotFound();
            }
            Console.WriteLine($"Fetched habit: {habitLog.start_date}, Id: {habitLog.habitLog_id}, habitId: {habitLog.habit_id}");

            return Ok(habitLog);
        }

        [HttpGet("habit/{habit_id}")]
        public IActionResult GetHabitLogsFromHabit(int habit_id)
        {
            var habitlogs = _habitLogService.GetAllHabitlogsByHabitId(habit_id);
            if (habitlogs == null || !habitlogs.Any())
            {
                return NotFound();

            }
            foreach (HabitLog habitLog in habitlogs)
            {
                Console.WriteLine($"Fetched habit: {habitLog.start_date}, Id: {habitLog.habitLog_id}, habitId: {habitLog.habit_id}");
            }

            return Ok(habitlogs);
        }

        [HttpPost]
        public IActionResult CreateHabitLog([FromBody] CreateHabitLogRequest data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                HabitLog habitLog = new HabitLog(data.habitLog_id, data.habit_id, data.start_date, data.habit_logged, data.period_type);
                _habitLogService.AddHabitLog(habitLog);
                return CreatedAtAction(nameof(GetHabitLog), new { habitlog_id = habitLog.habitLog_id }, habitLog);
            }
            catch
            {
                return StatusCode(500, "An error occurred while creating the habit log.");
            }
        }

        [HttpPut("update")]
        public IActionResult UpdateHabit([FromBody] CreateHabitLogRequest data)
        {
            try
            {
                HabitLog habitLog = new HabitLog(data.habitLog_id, data.habit_id, data.start_date, data.habit_logged, data.period_type);
                _habitLogService.UpdateHabitLog(habitLog);

                return Ok(habitLog);
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the habit log.");
            }
        }

        [HttpDelete("delete/{habitLog_id}")]
        public IActionResult DeleteHabit(int habitLog_id)
        {
            try
            {
                _habitLogService.DeleteHabitLog(habitLog_id);

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "An error occurred while deleting the habit log.");
            }
        }
    }
}
