using Microsoft.AspNetCore.Mvc;
using SorteosAPI.Models;
using SorteosAPI.Services;

namespace SorteosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignedNumbersController : ControllerBase
    {
        private readonly IAssignedNumberService _assignedNumberService;

        public AssignedNumbersController(IAssignedNumberService assignedNumberService)
        {
            _assignedNumberService = assignedNumberService;
        }

        [HttpPost("AssignRandomNumber")]
        public async Task<IActionResult> AssignRandomNumber([FromBody] AssignedNumberRaffer model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _assignedNumberService.AssignRandomNumberAsync(model);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        data = result.Model,
                        message = result.Message
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Ocurrió un error: {ex.Message}"
                });
            }
        }
    }
}