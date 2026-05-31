using SpendWise.API.Models;

namespace SpendWise.API.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync();

        Task<Transaction?> GetByIdAsync(int id);

        Task<Transaction> CreateAsync(Transaction transaction);

        Task<Transaction?> UpdateAsync(Transaction transaction);

        Task<bool> DeleteAsync(int id);
    }
}