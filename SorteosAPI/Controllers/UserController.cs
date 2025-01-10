using Microsoft.AspNetCore.Mvc;
using SorteosAPI.Models;
using SorteosAPI.Services;

namespace SorteosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreate userCreate)
        {
            if (userCreate == null)
            {
                return BadRequest(new { success = false, message = "El usuario no puede ser nulo." });
            }

            if (!await _userService.ClientExistsAsync(userCreate.IdClient))
            {
                return BadRequest(new { success = false, message = "El cliente con el Id proporcionado no existe." });
            }

            if (await _userService.UserExistsAsync(userCreate.IdClient, userCreate.Name))
            {
                return BadRequest(new { success = false, message = "Ya existe un usuario con el mismo nombre para este cliente." });
            }

            try
            {
                var result = await _userService.CreateUserAsync(userCreate);

                if (result)
                {
                    return Ok(new { success = true, message = "Usuario creado exitosamente." });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Error al crear el usuario." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetUsersByClientId([FromQuery] string? name = null)
        {
            try
            {
                var users = await _userService.GetUsersByClientIdAsync(name);

                if (users.Count == 0)
                {
                    return NotFound(new { success = false, message = "No se encontraron usuarios para el cliente proporcionado." });
                }

                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
