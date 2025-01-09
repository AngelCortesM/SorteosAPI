using Microsoft.AspNetCore.Mvc;
using SorteosAPI.Services;

namespace SorteosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListNumberController : ControllerBase
    {
        private readonly IListNumberService _listNumberService;

        public ListNumberController(IListNumberService listNumberService)
        {
            _listNumberService = listNumberService;
        }

        [HttpGet("GetAssignedNumbersPaged")]
        public async Task<IActionResult> GetAssignedNumbersPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string clientFilter = "",
            [FromQuery] string raffleFilter = "",
            [FromQuery] string userFilter = "")
        {
            var result = await _listNumberService.GetAssignedNumbersPagedAsync(pageNumber, pageSize, clientFilter, raffleFilter, userFilter);

            if (result.Success)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Numbers,
                    totalCount = result.TotalCount,
                    message = result.Message
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Message
            });
        }
    }
}