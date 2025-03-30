using management.Interface;
using management.Models;
using management.Models.Response;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Data;

namespace management.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly ITransactionService _transactionService;

        public AccountService(AppDbContext context, ITransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;       
        }
        public async Task<IEnumerable<Account>> GetAllPersonsAccountAsync()
        {
            try
            {
                // Fetch all persons from the database
                var accounts = await _context.Accounts.Where(a => !a.account_number.StartsWith("999-")).ToListAsync();

                // Gather all person codes
                var account_codes = accounts.Select(a => a.code).ToList();

                // Fetch all accounts for the persons in a single query
                var transactions = await _transactionService.GetAllTransactionsAsync();

                // Map accounts to their respective persons
                foreach (var account in accounts)
                {
                    account.Transaction = transactions.Where(a => a.account_code == account.code).ToList();
                }

                // Return the list of persons with their accounts
                return accounts;
            }
            catch (Exception ex)
            {

                throw;
            }
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
                throw new InvalidOperationException("Duplicate account number is not allowed.");
            // Generate a random number between 1 and 9999, padded to 4 digits
            var random = new Random();
            var randomNumber = random.Next(1, 10000).ToString("D10");
            account.account_number = randomNumber;
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task GenerateAccountNumber()
        {
            // Generate a random number between 1 and 9999, padded to 4 digits
            var random = new Random();
            var randomNumber = random.Next(1, 10000).ToString("D10");
        }

        public async Task UpdateAccountAsync(Account account)
        {
            // Fetch the existing account from the database using the Code
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.code == account.code);

            if (existingAccount == null)
            {
                throw new InvalidOperationException("Account with the specified Code does not exist.");
            }
                existingAccount.account_number = account.account_number;

            // Save the changes
            await _context.SaveChangesAsync();
        }


        public async Task CloseAccountAsync(int accountId)
        {
            var acc = await GetAccountByIdAsync(accountId);
            var transactions = await _transactionService.GetTransactionsByAccountIdAsync(accountId);
            var transaction = transactions.Where(t => t.account_code == accountId).ToList();
            if (acc?.outstanding_balance == 0 && acc.Transaction?.Count() >= 0)
            {
                acc.account_number ="999-"+ acc.account_number;
                await _context.SaveChangesAsync();
            }
            if (acc?.outstanding_balance > 0) throw new InvalidOperationException(" Account can not be closed with an outshanding Balance.");
            if (acc == null) throw new InvalidOperationException("Account not found.");
        }

    }
}
