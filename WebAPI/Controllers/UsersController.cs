using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;
        public UsersController(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var userDtos = await _userService.GetAllUsersAsync();
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto == null)
            {
                return NotFound();
            }
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            try
            {
                await _userService.AddUserAsync(userDto);
                return CreatedAtAction(nameof(GetUserById), new { id = userDto.UserId }, userDto);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("Username already exists"))
            {
                return BadRequest("Username already exists");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UserDto userDto, [FromForm] IFormFile? photo)
        {
            if (id != userDto.UserId)
            {
                return BadRequest("User ID mismatch");
            }

            if (photo != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }
                userDto.PhotoUrl = "/uploads/" + uniqueFileName;
            }

            try
            {
                await _userService.UpdateUserAsync(id, userDto);
                return NoContent();
            }
            catch (ArgumentException ex) when (ex.Message.Contains("Username already exists"))
            {
                return BadRequest("Username already exists");
            }
            catch (ArgumentException ex) when (ex.Message.Contains("User not found"))
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
