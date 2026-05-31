using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.API.DTOs.Products;
using SpendWise.API.Services.Interfaces;
using System.Security.Claims;

namespace SpendWise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Products list requested.");

            var products = await _productService.GetAllAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Product requested with id {ProductId}.", id);

            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product with id {ProductId} was not found.", id);
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            _logger.LogInformation("Product requested with code {ProductCode}.", code);

            var product = await _productService.GetByCodeAsync(code);

            if (product == null)
            {
                _logger.LogWarning("Product with code {ProductCode} was not found.", code);
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            _logger.LogInformation(
                "User {UserId} is creating a product. IsAdmin: {IsAdmin}.",
                userId,
                isAdmin
            );

            var createdProduct = await _productService.CreateAsync(
                dto,
                userId!,
                isAdmin
            );

            _logger.LogInformation(
                "Product with id {ProductId} was created by user {UserId}.",
                createdProduct.Id,
                userId
            );

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                createdProduct
            );
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            _logger.LogInformation("Admin is updating product with id {ProductId}.", id);

            var updated = await _productService.UpdateAsync(id, dto);

            if (!updated)
            {
                _logger.LogWarning("Update failed. Product with id {ProductId} was not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Product with id {ProductId} was updated.", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Admin is deleting product with id {ProductId}.", id);

            var deleted = await _productService.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Delete failed. Product with id {ProductId} was not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Product with id {ProductId} was deleted.", id);

            return NoContent();
        }
    }
}