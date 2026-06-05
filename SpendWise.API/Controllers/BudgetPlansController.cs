using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.API.Services.Interfaces;
using System.Security.Claims;

namespace SpendWise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetPlansController : ControllerBase
    {
        private readonly IBudgetPlanService _budgetPlanService;

        public BudgetPlansController(IBudgetPlanService budgetPlanService)
        {
            _budgetPlanService = budgetPlanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var plans = await _budgetPlanService.GetAllAsync(userId);

            return Ok(plans);
        }

        [HttpPost("{id}/apply")]
        public async Task<IActionResult> ApplyPlan(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var applied = await _budgetPlanService.ApplyPlanAsync(id, userId);

            if (!applied)
                return NotFound();

            return Ok("Budget plan applied successfully.");
        }
    }
}