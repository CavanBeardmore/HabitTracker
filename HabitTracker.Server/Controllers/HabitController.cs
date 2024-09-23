using HabitTracker.Server.Classes.Habit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("habits/[controller]")]
    public class HabitController : ControllerBase
    {

        private readonly HabitService _habitService;

        public HabitController(HabitService habitService)
        {
            _habitService = habitService;
        }

        [Authorize]
        [HttpGet("{habit_id}")]
        public IActionResult GetHabit(int habit_id)
        {
            var habit = _habitService.GetHabitByHabitId(habit_id);
            if (habit == null)
            {
                return NotFound();
            }
            Console.WriteLine($"Fetched habit: {habit.name}, Id: {habit.habit_id}");

            return Ok(habit);
        }

        [Authorize]
        [HttpGet("user/{username}")]
        public IActionResult GetUserHabits(string username)
        {
            var habits = _habitService.GetAllHabitsByUsername(username);
            if (habits == null || !habits.Any())
            {
                return NotFound();
                
            }
            foreach (Habit habit in habits)
            {
                Console.WriteLine($"Fetched habit: {habit.name}, Id: {habit.habit_id}");
            }

            return Ok(habits);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateHabit([FromBody] CreateHabitRequest data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Habit habit = new Habit(data.HabitId, data.username, data.Name);
                _habitService.AddHabit(habit);
                return CreatedAtAction(nameof(GetHabit), new { habit_id = habit.habit_id }, habit);
            }
            catch
            {
                return StatusCode(500, "An error occurred while creating the habit.");
            }
        }

        [Authorize]
        [HttpPut("update")]
        public IActionResult UpdateHabit([FromBody] CreateHabitRequest data)
        {
            try
            { 
                Habit habit = new Habit(data.HabitId, data.username, data.Name);
                _habitService.UpdateHabit(habit);

                return Ok(habit);
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
                _habitService.DeleteHabit(habit_id);

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "An error occurred while deleting the habit.");
            }
        }
    }
}
