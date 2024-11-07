using HabitTracker.Server.Repository;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("habitLogs/[controller]")]
    public class HabitLogController : ControllerBase
    {

        private readonly HabitLogRepository _habitLogRepository;

        public HabitLogController(HabitLogRepository habitLogRepository)
        {
            _habitLogRepository = habitLogRepository;
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetHabitLog(int id)
        {
            var habitLog = _habitLogRepository.GetById(id);
            if (habitLog == null)
            {
                return NotFound();
            }

            return Ok(habitLog);
        }

        [Authorize]
        [HttpGet("habit/{habit_id}")]
        public IActionResult GetHabitLogsFromHabit(int habit_id)
        {
            var habitlogs = _habitLogRepository.GetAllByHabitId(habit_id);
            if (habitlogs == null || !habitlogs.Any())
            {
                return NotFound();

            }

            return Ok(habitlogs);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateHabitLog([FromBody] PostHabitLog habitLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool success = _habitLogRepository.Add(habitLog);

                if (success)
                {
                    return StatusCode(200);
                }

                return StatusCode(500, "0 records updated");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("update")]
        public IActionResult UpdateHabit([FromBody] PatchHabitLog habitLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool success = _habitLogRepository.Update(habitLog);

                if (success)
                {
                    return Ok(habitLog);
                }

                return StatusCode(500, "0 records updated");
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the habit log.");
            }
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteHabit(int id)
        {
            try
            {
                bool success = _habitLogRepository.Delete(id);

                if (success)
                {
                    return NoContent();
                }

                return StatusCode(500, "0 records updated");
            }
            catch
            {
                return StatusCode(500, "An error occurred while deleting the habit log.");
            }
        }
    }
}
