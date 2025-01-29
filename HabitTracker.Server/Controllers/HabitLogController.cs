using HabitTracker.Server.Repository;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Services;
using HabitTracker.Server.DTOs;

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

        [Authorize]
        [HttpGet("{habitLogId}")]
        public IActionResult GetHabitLog(int habitLogId, [FromQuery] int userId)
        {
            try
            {
                IServiceResponseWithData<HabitLog?> response = _habitLogService.GetById(habitLogId, userId);
                if (response.Success == false)
                {
                    return NotFound(response.Error);
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("habit/{habitId}")]
        public IActionResult GetHabitLogsFromHabit(int habitId, [FromQuery] int userId, int pageNumber)
        {
            try
            {
                IServiceResponseWithData<IReadOnlyCollection<HabitLog?>> response = _habitLogService.GetAllByHabitId(habitId, userId, pageNumber);
                if (response.Success == false)
                {
                    return NotFound(response.Error);

                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateHabitLog([FromBody] PostHabitLog habitLog)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IServiceResponse response = _habitLogService.Add(habitLog);
                
                if (response.Success)
                {
                    return StatusCode(200);
                }

                return StatusCode(500, response.Error);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("update")]
        public IActionResult UpdateHabitLog([FromBody] PatchHabitLog habitLog)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IServiceResponse response = _habitLogService.Update(habitLog);

                if (response.Success)
                {
                    return Ok(habitLog);
                }

                return StatusCode(500, response.Error);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("delete/{habitLogId}")]
        public IActionResult DeleteHabitLog(int habitLogId, [FromQuery] int userId)
        {
            try
            {
                IServiceResponse response = _habitLogService.Delete(habitLogId, userId);

                if (response.Success)
                {
                    return NoContent();
                }

                return StatusCode(500, response.Error);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
