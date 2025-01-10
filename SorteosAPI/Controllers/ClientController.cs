using Microsoft.AspNetCore.Mvc;
using SorteosAPI.Models;
using SorteosAPI.Services;

namespace SorteosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientCreate clientCreate)
        {
            if (clientCreate == null)
            {
                return BadRequest(new { success = false, message = "El cliente no puede ser nulo." });
            }

            try
            {
                var result = await _clientService.CreateClientAsync(clientCreate);

                if (result)
                {
                    return Ok(new { success = true, message = "Cliente creado exitosamente." });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Error al crear el cliente." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetClients()
        {
            try
            {
                var clients = await _clientService.GetClientsAsync();

                if (clients.Count == 0)
                {
                    return NotFound(new { success = false, message = "No se encontraron clientes." });
                }

                return Ok(new { success = true, data = clients });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}