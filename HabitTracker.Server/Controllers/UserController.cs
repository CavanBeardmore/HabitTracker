using HabitTracker.Server.Auth;
using HabitTracker.Server.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Models;

namespace HabitTracker.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("user/[controller]")]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userService)
        {
            _userRepository = userService;
        }

        [Authorize]
        [HttpGet("{User_id}")]
        public IActionResult GetUser(string username)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddUser([FromBody] PostUser user)
        {
            var existingUser = _userRepository.GetByUsername(user.Username);

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
                PasswordService passwordService = new PasswordService(user.Password);
                string hashedPassword = passwordService.HashPassword();
                user.Password = hashedPassword;

                bool success = _userRepository.Add(user);

                if (success)
                {
                    return StatusCode(201);
                }

                return StatusCode(500, "0 records updated");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"An error occured when creating the user: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPatch("update")]
        public IActionResult UpdateUser([FromBody] PatchUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string password = user.Password;
                if (user.Password != null)
                {
                    PasswordService passwordService = new PasswordService(user.Password);
                    password = passwordService.HashPassword();
                }

                bool success = _userRepository.Update(user);

                if (success)
                {
                    return Ok();
                }

                return StatusCode(500, "0 records updated");
            }
            catch
            {
                return StatusCode(500, "An error occured when updating the user");
            }
        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult DeleteUser([FromBody] AuthUser user)
        {

            var existingUser = _userRepository.GetByUsername(user.Username);
            if (user == null)
            {
                return NotFound();
            }

            PasswordService passwordService = new PasswordService(user.Password);
            string hashedPassword = passwordService.HashPassword();
            bool isPasswordCorrect = passwordService.VerifyPassword(hashedPassword);

            if (!isPasswordCorrect)
            {
                return Unauthorized("Incorrect password");
            }

            try
            {
                bool success = _userRepository.Delete(user.Username);

                if (success)
                {
                    return NoContent();
                }

                return StatusCode(500, "0 records updated");
            }
            catch
            {
                return StatusCode(500, "An error occured while deleting the user");
            }
        }
    }
}
