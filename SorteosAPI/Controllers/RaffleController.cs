using Microsoft.AspNetCore.Mvc;
using SorteosAPI.Models;
using SorteosAPI.Services;

namespace SorteosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaffleController : ControllerBase
    {
        private readonly IRaffleService _raffleService;

        public RaffleController(IRaffleService raffleService)
        {
            _raffleService = raffleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRaffle([FromBody] RaffleCreate raffleCreate)
        {
            if (raffleCreate == null)
            {
                return BadRequest(new { success = false, message = "El sorteo no puede ser nulo." });
            }

            if (await _raffleService.RaffleExistsAsync(raffleCreate.Name))
            {
                return BadRequest(new { success = false, message = "Ya existe un sorteo con el mismo nombre." });
            }

            try
            {
                var result = await _raffleService.CreateRaffleAsync(raffleCreate);

                if (result)
                {
                    return Ok(new { success = true, message = "Sorteo creado exitosamente." });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Error al crear el sorteo." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRaffles()
        {
            try
            {
                var raffles = await _raffleService.GetAllRafflesAsync();

                if (raffles.Count == 0)
                {
                    return NotFound(new { success = false, message = "No se encontraron sorteos." });
                }

                return Ok(new { success = true, data = raffles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}