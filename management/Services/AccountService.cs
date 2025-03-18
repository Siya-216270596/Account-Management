using management.Interface;
using management.Models;
using Microsoft.EntityFrameworkCore;

namespace management.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;

        public AccountService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAccountsByPersonIdAsync(int personId)
        {
            return await _context.Account.Where(a => a.PersonCode == personId).ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            return await _context.Account.FindAsync(accountId);
        }

        public async Task AddAccountAsync(Account account)
        {
            if (await _context.Account.AnyAsync(a => a.AccountNumber == account.AccountNumber))
                throw new Exception("Duplicate account number is not allowed.");

            await _context.Account.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task CloseAccountAsync(int accountId)
        {
            var account = await _context.Account.FindAsync(accountId);
            if (account == null) throw new Exception("Account not found.");
            if (account.OutstandingBalance != 0) throw new Exception("Cannot close an account with a nonzero balance.");

            account.IsClosed = true;
            await _context.SaveChangesAsync();
        }
    }
}
