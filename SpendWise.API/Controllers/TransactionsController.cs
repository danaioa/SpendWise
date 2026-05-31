using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SpendWise.API.DTOs.Transactions;
using SpendWise.API.Services.Interfaces;

namespace SpendWise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll([FromQuery] DateTime? date)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transactions = await _service.GetAllAsync(userId!);

            if (date.HasValue)
            {
                transactions = transactions
                    .Where(t => t.Date.Date == date.Value.Date)
                    .ToList();
            }

            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetById(int id)
        {
            var transaction = await _service.GetByIdAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDto>> Create(
            CreateTransactionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var createdTransaction =
                await _service.CreateAsync(dto, userId!);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTransaction.Id },
                createdTransaction);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionDto>> Update(
            int id,
            UpdateTransactionDto dto)
        {
            var updatedTransaction =
                await _service.UpdateAsync(id, dto);

            if (updatedTransaction == null)
            {
                return NotFound();
            }

            return Ok(updatedTransaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("scan-product")]
        public async Task<ActionResult<TransactionDto>> CreateFromScannedProduct(
    ScanProductTransactionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var createdTransaction =
                await _service.CreateFromScannedProductAsync(dto, userId!);

            if (createdTransaction == null)
            {
                return NotFound("Product not found.");
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTransaction.Id },
                createdTransaction);
        }
    }
}