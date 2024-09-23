using HabitTracker.Server.Classes.PasswordService;
using HabitTracker.Server.Classes.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("user/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("{username}")]
        public IActionResult GetUser(string username)
        {
            var user = _userService.GetByUsername(username);
            if (user == null)
            {
                return NotFound();
            }
            Console.WriteLine($"Fetched User: {user.username}, email: {user.email}");

            return Ok(user);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddUser([FromBody] CreateUserRequest data)
        {
            var existingUser = _userService.GetByUsername(data.Username);

            if (existingUser != null)
            {
                return BadRequest("User exists");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                PasswordService passwordService = new PasswordService(data.Password);
                string hashedPassword = passwordService.HashPassword();
                User user = new User(data.Username, data.Email, hashedPassword);
                _userService.AddUser(user);
                return CreatedAtAction(nameof(GetUser), new { username = user.username}, user);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"An error occured when creating the user: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("update")]
        public IActionResult UpdateUser([FromBody] UpdateUserRequest data)
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
                User user = new User(data.NewUsername, data.Email, password);
                _userService.UpdateUser(user, data.OldUsername);

                return Ok(user);
            }
            catch
            {
                return StatusCode(500, "An error occured when updating the user");
            }
        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult DeleteUser([FromBody] CreateUserRequest data)
        {

            var user = _userService.GetByUsername(data.Username);
            if (user == null)
            {
                return NotFound();
            }

            PasswordService passwordService = new PasswordService(user.password);
            bool isPasswordCorrect = passwordService.VerifyPassword(data.Password);

            if (!isPasswordCorrect)
            {
                return Unauthorized("Incorrect password");
            }

            try
            {
                _userService.DeleteUser(data.Username);

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "An error occured while deleting the user");
            }
        }
    }
}
