using SpendWise.API.DTOs.Transactions;
using SpendWise.API.Models;
using SpendWise.API.Repositories.Interfaces;
using SpendWise.API.Services.Interfaces;

namespace SpendWise.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        private readonly IProductRepository _productRepository;

        public TransactionService(
            ITransactionRepository repository,
            IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllAsync(string userId)
        {
            var transactions = await _repository.GetAllAsync();

            transactions = transactions
                .Where(t => t.UserId == userId)
                .ToList();

            return transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Description = t.Description,
                Date = t.Date,
                CategoryName = t.Category?.Name ?? string.Empty,
                IsExpense = t.Category?.IsExpense ?? false
            });
        }

        public async Task<TransactionDto?> GetByIdAsync(int id)
        {
            var transaction = await _repository.GetByIdAsync(id);

            if (transaction == null)
                return null;

            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                CategoryName = transaction.Category?.Name ?? string.Empty,
                IsExpense = transaction.Category?.IsExpense ?? false
            };
        }

        public async Task<TransactionDto> CreateAsync(
            CreateTransactionDto dto,
            string userId)
        {
            var transaction = new Transaction
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date,
                CategoryId = dto.CategoryId,
                UserId = userId
            };

            var createdTransaction = await _repository.CreateAsync(transaction);

            return new TransactionDto
            {
                Id = createdTransaction.Id,
                Amount = createdTransaction.Amount,
                Description = createdTransaction.Description,
                Date = createdTransaction.Date,
                CategoryName = createdTransaction.Category?.Name ?? string.Empty,
                IsExpense = createdTransaction.Category?.IsExpense ?? false
            };
        }

        public async Task<TransactionDto?> UpdateAsync(
            int id,
            UpdateTransactionDto dto)
        {
            var transaction = new Transaction
            {
                Id = id,
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date,
                CategoryId = dto.CategoryId
            };

            var updatedTransaction = await _repository.UpdateAsync(transaction);

            if (updatedTransaction == null)
                return null;

            return new TransactionDto
            {
                Id = updatedTransaction.Id,
                Amount = updatedTransaction.Amount,
                Description = updatedTransaction.Description,
                Date = updatedTransaction.Date,
                CategoryName = updatedTransaction.Category?.Name ?? string.Empty,
                IsExpense = updatedTransaction.Category?.IsExpense ?? false
            };
        }

        public async Task<TransactionDto?> CreateFromScannedProductAsync(
            ScanProductTransactionDto dto,
            string userId)
        {
            var product = await _productRepository.GetByCodeAsync(dto.ProductCode);

            if (product == null)
                return null;

            var totalAmount = product.UnitPrice * dto.Quantity;

            var transaction = new Transaction
            {
                Amount = totalAmount,
                Description = $"Scanned product: {product.Name} x {dto.Quantity}",
                Date = DateTime.UtcNow,
                CategoryId = product.CategoryId,
                UserId = userId
            };

            var createdTransaction = await _repository.CreateAsync(transaction);

            return new TransactionDto
            {
                Id = createdTransaction.Id,
                Amount = createdTransaction.Amount,
                Description = createdTransaction.Description,
                Date = createdTransaction.Date,
                CategoryName = product.Category?.Name ?? string.Empty,
                IsExpense = product.Category?.IsExpense ?? false
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}