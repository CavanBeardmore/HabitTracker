using HabitTracker.Server.Repository;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("habits/[controller]")]
    public class HabitController : ControllerBase
    {

        private readonly HabitRepository _habitRepository;

        public HabitController(HabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
        }

        [Authorize]
        [HttpGet("{habit_id}")]
        public IActionResult GetHabit(int habit_id)
        {
            var habit = _habitRepository.GetById(habit_id);
            if (habit == null)
            {
                return NotFound();
            }

            return Ok(habit);
        }

        [Authorize]
        [HttpGet("user/{user_id}")]
        public IActionResult GetUserHabits(int user_id)
        {
            var habits = _habitRepository.GetAllByUserId(user_id);
            if (habits == null || !habits.Any())
            {
                return NotFound();
                
            }

            return Ok(habits);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateHabit([FromBody] PostHabit habit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool success = _habitRepository.Add(habit);

                if (success)
                {
                    return StatusCode(201);
                }

                return StatusCode(500, "0 records updated");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"error when adding {ex.Message}");
                return StatusCode(500, "An error occurred while creating the habit.");
            }
        }

        [Authorize]
        [HttpPatch("update")]
        public IActionResult UpdateHabit([FromBody] PatchHabit habit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            { 
                bool success = _habitRepository.Update(habit);

                if (success)
                {
                    return Ok();
                }

                return StatusCode(500, "0 records updated");
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the habit.");
            }
        }

        [Authorize]
        [HttpDelete("delete/{habit_id}")]
        public IActionResult DeleteHabit(int habit_id)
        {
            try
            {
                bool success = _habitRepository.Delete(habit_id);

                if (success)
                {
                    return NoContent();
                }

                return StatusCode(500, "0 records updated");
            }
            catch
            {
                return StatusCode(500, "An error occurred while deleting the habit.");
            }
        }
    }
}
