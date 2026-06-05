using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.API.Services.Interfaces;
using System.Security.Claims;

namespace SpendWise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MonthlySavingsController : ControllerBase
    {
        private readonly IMonthlySavingsService _monthlySavingsService;

        public MonthlySavingsController(IMonthlySavingsService monthlySavingsService)
        {
            _monthlySavingsService = monthlySavingsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMySavings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var savings = await _monthlySavingsService.GetMySavingsAsync(userId);

            return Ok(savings);
        }

        [HttpPost("close-month")]
        public async Task<IActionResult> CloseMonth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _monthlySavingsService.CloseMonthAsync(userId);

            if (result == null)
                return BadRequest("This month is already saved.");

            return Ok(result);
        }
    }
}