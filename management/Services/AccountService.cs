using management.Interface;
using management.Models;
using management.Models.Response;
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

        public async Task<PersonAccountsResponse> GetAccountsByPersonIdAsync(int personId)
        {
            var accounts = await _context.Accounts.Where(a => a.PersonCode == personId).ToListAsync();
            return new PersonAccountsResponse()
            {
                Accounts = accounts
            };

        }

        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            return await _context.Accounts.FindAsync(accountId);
        }

        public async Task AddAccountAsync(Account account)
        {
            if (await _context.Accounts.AnyAsync(a => a.account_number == account.account_number))
                throw new Exception("Duplicate account number is not allowed.");

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccountAsync(Account account)
        {

            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task CloseAccountAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) throw new Exception("Account not found.");
            if (account.outstanding_balance != 0) throw new Exception("Cannot close an account with a nonzero balance.");

            await _context.SaveChangesAsync();
        }
    }
}
