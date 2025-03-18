using management.Interface;
using management.Models;
using Microsoft.EntityFrameworkCore;

namespace management.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId)
        {
            return await _context.Transaction.Where(t => t.AccountCode == accountId).ToListAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            var account = await _context.Account.FindAsync(transaction.AccountCode);
            if (account == null) throw new Exception("Account not found.");
            if (account.IsClosed) throw new Exception("Cannot add transactions to a closed account.");
            if (transaction.Amount == 0) throw new Exception("Transaction amount cannot be zero.");
            if (transaction.TransactionDate > DateTime.Now) throw new Exception("Transaction date cannot be in the future.");

            account.OutstandingBalance += transaction.Amount;
            await _context.Transaction.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
