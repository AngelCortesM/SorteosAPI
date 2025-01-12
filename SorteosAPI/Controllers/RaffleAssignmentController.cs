using Microsoft.AspNetCore.Mvc;
using SorteosAPI.Models;
using SorteosAPI.Services;

namespace SorteosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaffleAssignmentController : ControllerBase
    {
        private readonly IRaffleAssignmentService _raffleAssignmentService;

        public RaffleAssignmentController(IRaffleAssignmentService raffleAssignmentService)
        {
            _raffleAssignmentService = raffleAssignmentService;
        }

        [HttpPost]
        public async Task<IActionResult> AssignRaffleToClient([FromBody] RaffleByClientAssign assignment)
        {
            if (assignment == null)
            {
                return BadRequest(new { success = false, message = "Los datos del sorteo no pueden ser nulos." });
            }

            try
            {
                var result = await _raffleAssignmentService.AssignRaffleToClientAsync(assignment);

                if (result)
                {
                    return Ok(new { success = true, message = "Sorteo asignado al cliente exitosamente." });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Error al asignar el sorteo al cliente." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListRafflesByClient([FromQuery] int? idClient, [FromQuery] int? idRaffle)
        {
            try
            {
                var assignments = await _raffleAssignmentService.ListRafflesByClientAsync(idClient, idRaffle);

                if (assignments.Count == 0)
                {
                    return NotFound(new { success = false, message = "No se encontraron sorteos asignados." });
                }

                return Ok(new { success = true, data = assignments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}


