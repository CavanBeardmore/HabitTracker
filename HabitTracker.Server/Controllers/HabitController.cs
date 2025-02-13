using HabitTracker.Server.DTOs;
using HabitTracker.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Services;
using HabitTracker.Server.Services.Responses;

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

        [Authorize]
        [HttpGet("{habitId}")]
        public IActionResult GetHabit(int habitId)
        {
            try
            {
                if (HttpContext.Items.TryGetValue("userId", out var userIdObj) == false || userIdObj is not int userId)
                {
                    return Unauthorized("Could not retrieve user id from JWT");
                }

                IServiceResponseWithData<Habit?> response = _habitService.GetById(habitId, userId);
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
        [HttpGet()]
        public IActionResult GetUserHabits()
        {
            try
            {
                if (HttpContext.Items.TryGetValue("userId", out var userIdObj) == false || userIdObj is not int userId)
                {
                    return Unauthorized("Could not retrieve user id from JWT");
                }

                IServiceResponseWithData<IReadOnlyCollection<Habit>> response = _habitService.GetAllByUserId(userId);
                
                Console.WriteLine($"GET HABITS BY USER ID RES - {response.Success}");

                if (response.Success == false)
                {
                    Console.WriteLine($"FAILED TO GET HABITS BY USER ID - {userId}");
                    return NotFound(response.Error);
                }

                Console.WriteLine($"SUCCESFULLY GOT HABITS BY USER ID - {userId}");
                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateHabit([FromBody] PostHabit habit)
        {
            if (HttpContext.Items.TryGetValue("userId", out var userIdObj) == false || userIdObj is not int userId)
            {
                return Unauthorized("Could not retrieve user id from JWT");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                IServiceResponse response = _habitService.Add(userId, habit);

                if (response.Success)
                {
                    return StatusCode(201);
                }

                return StatusCode(500, response.Error);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("update")]
        public IActionResult UpdateHabit([FromBody] PatchHabit habit)
        {
            if (HttpContext.Items.TryGetValue("userId", out var userIdObj) == false || userIdObj is not int userId)
            {
                return Unauthorized("Could not retrieve user id from JWT");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                IServiceResponse response = _habitService.Update(userId, habit);

                if (response.Success)
                {
                    return Ok();
                }

                return StatusCode(500, response.Error);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("delete/{habitId}")]
        public IActionResult DeleteHabit(int habitId)
        {
            try
            {
                if (HttpContext.Items.TryGetValue("userId", out var userIdObj) == false || userIdObj is not int userId)
                {
                    return Unauthorized("Could not retrieve user id from JWT");
                }

                IServiceResponse response = _habitService.Delete(habitId, userId);

                if (response.Success)
                {
                    return NoContent();
                }

                return StatusCode(500, response.Error);
            }
            catch
            {
                return StatusCode(500, "An error occurred while deleting the habit.");
            }
        }
    }
}
