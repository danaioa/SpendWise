using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.API.Models;
using SpendWise.API.Services.Interfaces;
using System.Security.Claims;

namespace SpendWise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService categoryService,
            ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var categories = await _categoryService.GetAllAsync(userId, isAdmin);

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var category = await _categoryService.GetByIdAsync(id, userId, isAdmin);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var createdCategory = await _categoryService.CreateAsync(category, userId);

            if (createdCategory == null)
                return BadRequest("Category name is required.");

            _logger.LogInformation("Category was created by user {UserId}.", userId);

            return Ok(createdCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _categoryService.UpdateAsync(id, category, userId, isAdmin);

            if (result == null)
                return NotFound();

            if (result is string message && message == "FORBID")
                return Forbid();

            if (result is string badRequest && badRequest == "BAD_REQUEST")
                return BadRequest("Category name is required.");

            _logger.LogInformation("Category {CategoryId} was updated.", id);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _categoryService.DeleteAsync(id, userId, isAdmin);

            if (result == null)
                return NotFound();

            if (result == false)
                return Forbid();

            _logger.LogInformation("Category {CategoryId} was deleted.", id);

            return NoContent();
        }
    }
}