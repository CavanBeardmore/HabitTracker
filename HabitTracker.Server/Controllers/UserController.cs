using HabitTracker.Server.Auth;
using HabitTracker.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HabitTracker.Server.Models;
using HabitTracker.Server.Services.Responses;

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

        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddUser([FromBody] PostUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IServiceResponseWithStatusCode response = _userService.Add(user);

                if (response.Success)
                {
                    return StatusCode((int)response.StatusCode);
                }

                return StatusCode((int)response.StatusCode, response.Error);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("update")]
        public IActionResult UpdateUser([FromBody] PatchUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IServiceResponseWithStatusCode response = _userService.Update(user);

                if (response.Success)
                {
                    return Ok();
                }

                return StatusCode((int)response.StatusCode, response.Error);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult DeleteUser([FromBody] AuthUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IServiceResponseWithStatusCode response = _userService.Delete(user);

                if (response.Success)
                {
                    return NoContent();
                }

                return StatusCode((int)response.StatusCode, response.Error);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
