using management.Interface;
using management.Models;
using Microsoft.Data.SqlClient;
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
             var transactions= await GetAllTransactionsAsync();
             var codelist=transactions.Select(c => c.code).ToList();
            foreach(int code in codelist)
            {
                  transactions = transactions.Where(t => t.account_code == accountId).ToList();
            }
            return transactions;
        }


        public async Task AddTransactionAsync(Transaction transaction)
        {
            var account = await _context.Accounts.FindAsync(transaction.account_code);
            if (account == null) throw new InvalidOperationException("Account not found.");
            if (transaction.amount == 0.0000m) throw new InvalidOperationException("Transaction amount cannot be zero.");
            if (transaction.transaction_date > DateTime.Now) throw new InvalidOperationException("Transaction date cannot be in the future.");
            account.outstanding_balance += transaction.amount;
            var parameters = new []
                       {

                            new SqlParameter("@account_code", transaction.account_code),
                            new SqlParameter("@amount", transaction.amount),
                            new SqlParameter("@description", transaction.description),
                        };

            // Call the stored procedure using _context
             await _context.Database.ExecuteSqlRawAsync("EXEC AddTransaction  @account_code, @amount, @description", parameters);
             await _context.SaveChangesAsync(); // Save changes asynchronously

        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            // Validate input
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            var Account = await _context.Accounts.FirstOrDefaultAsync(a => a.code ==transaction.account_code);

            if (Account == null)
            {
                throw new InvalidOperationException("Account with the specified Code does not exist.");
            }

            // Update the necessary fields
            Account.outstanding_balance += transaction.amount;
            transaction.account_code = Account.code;

                var updatedAccount = _context.Accounts.Update(Account); // Mark the entity as updated
            if(updatedAccount is not null && updatedAccount is not null)
            {
                var parameters = new[]
           {

                            new SqlParameter("@account_code", transaction.account_code),
                            new SqlParameter("@amount", transaction.amount),
                            new SqlParameter("@description", transaction.description),
                        };

                // Call the stored procedure using _context
                await _context.Database.ExecuteSqlRawAsync("EXEC UpdateTransaction  @account_code, @amount, @description", parameters);
                await _context.SaveChangesAsync(); // Save changes asynchronously
            } else
            {
                throw new InvalidOperationException("Not Updated.");
            }
        }

        public async Task DeletetTransactionByIdAsync(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC DeleteTransactionByCode @code = {0}", id);
        }



    }
}
