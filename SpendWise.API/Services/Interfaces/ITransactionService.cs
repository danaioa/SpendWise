using SpendWise.API.DTOs.Transactions;

namespace SpendWise.API.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllAsync(string userId);

        Task<TransactionDto?> GetByIdAsync(int id);

        Task<TransactionDto> CreateAsync(
            CreateTransactionDto dto,
            string userId);

        Task<TransactionDto?> UpdateAsync(
            int id,
            UpdateTransactionDto dto);

        Task<bool> DeleteAsync(int id);

        Task<TransactionDto?> CreateFromScannedProductAsync(
        ScanProductTransactionDto dto,
        string userId);
    }
}