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

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return _context.Transaction
                .Select(t => new Transaction
                {
                    code = t.code,
                    description = t.description,
                    amount = t.amount,
                    transaction_date = t.transaction_date,
                    account_code = t.account_code,
                    capture_date = t.capture_date
                })
                .ToList();
        }
        public async Task<List<Transaction>> GetTransactionsByAccountIdAsync(int accountId)
        {
            return await _context.Transaction.Where(t => t.account_code == accountId).ToListAsync();
        }


        public async Task AddTransactionAsync(Transaction transaction)
        {
            var account = await _context.Accounts.FindAsync(transaction.account_code);
            if (account == null) throw new Exception("Account not found.");
            if (transaction.amount == 0) throw new Exception("Transaction amount cannot be zero.");
            if (transaction.transaction_date > DateTime.Now) throw new Exception("Transaction date cannot be in the future.");

            account.outstanding_balance += transaction.amount;
            await _context.Transaction.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
