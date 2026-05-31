using Microsoft.EntityFrameworkCore;
using SpendWise.API.Data;
using SpendWise.API.Models;
using SpendWise.API.Repositories.Interfaces;

namespace SpendWise.API.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction?> UpdateAsync(Transaction transaction)
        {
            var existingTransaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transaction.Id);

            if (existingTransaction == null)
            {
                return null;
            }

            existingTransaction.Amount = transaction.Amount;
            existingTransaction.Description = transaction.Description;
            existingTransaction.Date = transaction.Date;
            existingTransaction.CategoryId = transaction.CategoryId;

            await _context.SaveChangesAsync();

            return existingTransaction;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                return false;
            }

            _context.Transactions.Remove(transaction);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}