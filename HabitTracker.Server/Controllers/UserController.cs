using HabitTracker.Server.Classes.PasswordService;
using HabitTracker.Server.Classes.User;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.Server.Controllers
{
    [ApiController]
    [Route("user/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{user_id}")]
        public IActionResult GetUser(int user_id)
        {
            var user = _userService.GetById(user_id);
            if (user == null)
            {
                return NotFound();
            }
            Console.WriteLine($"Fetched User: {user.user_id}, email: {user.email}");

            return Ok(user);
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] CreateUserRequest data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                PasswordService passwordService = new PasswordService(data.Password);
                string hashedPassword = passwordService.HashPassword();
                User user = new User(data.UserId, data.Username, data.Email, hashedPassword);
                _userService.AddUser(user);
                return CreatedAtAction(nameof(GetUser), new { user_id = user.user_id }, user);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"An error occured when creating the user: {ex.Message}");
            }
        }

        [HttpPut("update")]
        public IActionResult UpdateUser([FromBody] CreateUserRequest data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string password = data.Password;
                if (data.Password != null)
                {
                    PasswordService passwordService = new PasswordService(data.Password);
                    password = passwordService.HashPassword();
                }
                User user = new User(data.UserId, data.Username, data.Email, password);
                _userService.UpdateUser(user);

                return Ok(user);
            }
            catch
            {
                return StatusCode(500, "An error occured when updating the user");
            }
        }

        [HttpDelete("delete/{user_id}")]
        public IActionResult DeleteUser(int user_id)
        {
            try
            {
                _userService.DeleteUser(user_id);

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "An error occured while deleting the user");
            }
        }
    }
}
