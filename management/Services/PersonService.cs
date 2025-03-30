using management.Interface;
using management.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Globalization;
using System.Linq;
using System.Web.Helpers;

namespace management.Services
{
    public class PersonService : IPersonService
    {
        private readonly AppDbContext _context;
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;


        public PersonService(AppDbContext context, IAccountService accountService, ITransactionService transactionService)
        {
            _context = context;
            _accountService = accountService;
            _transactionService = transactionService;
        }

        public async Task<Person> GetPersonByIdAsync(int id)
        {
            var accountperson = await _accountService.GetAccountByIdAsync(id);
            var result =  await _context.Persons.FindAsync(accountperson.PersonCode);
             
            return result;
        }

        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            try {
           
            // Fetch all persons from the database
            var persons = await _context.Persons.ToListAsync();

            // Gather all person codes
            var personCodes = persons.Select(p => p.Code).ToList();

            // Fetch all accounts for the persons in a single query
            var accounts = await _context.Accounts.ToListAsync();

                // Map accounts to their respective persons
                foreach (var person in persons)
                {
                    person.Account = accounts.Where(a => a.PersonCode == person.Code).ToList();
                }

                // Return the list of persons with their accounts
                return persons;
            }
            catch (Exception ex) {

                throw;
            }
        }



        public async Task AddPersonAsync(Person person)
        {
            if (!ValidateSouthAfricanID(person.id_number))
            {
                throw new InvalidOperationException("Invalid South African ID number.");
            }

            foreach (var account in person.Account)
            {
                await _accountService.AddAccountAsync(account);
            }
          

            if (await _context.Persons.AnyAsync(p => p.id_number == person.id_number))
                throw new InvalidOperationException("ID Number already exists.");
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
        }

        public bool ValidateSouthAfricanID(string idNumber)
        {
            if (idNumber.Length != 13 || !idNumber.All(char.IsDigit))
                throw new InvalidOperationException("ID Number must have 13 degits");


            string birthDate = idNumber.Substring(0, 6);
            if (!DateTime.TryParseExact(birthDate, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) 
                throw new InvalidOperationException("DOB is incorrect format");


            int checksum = CalculateLuhnChecksum(idNumber.Substring(0, 12));
            return checksum == int.Parse(idNumber[12].ToString());
        }

        private int CalculateLuhnChecksum(string number)
        {
            int sum = 0;
            bool alternate = false;

            for (int i = number.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(number[i].ToString());
                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                        n -= 9;
                }
                sum += n;
                alternate = !alternate;
            }

            return (sum % 10);
        }

        public async Task UpdatePersonAsync(Person person)
        {
            foreach (var account in person.Account)
            {
                await _accountService.UpdateAccountAsync(account);
            }
            var existingPerson = await _context.Persons.FirstOrDefaultAsync(p => p.Code == person.Code);

            if (existingPerson == null)
            {
                throw new InvalidOperationException("The specified person does not exist.");
            }
            // Update all fields apart from the Code
            existingPerson.id_number = person.id_number;
            existingPerson.Name = person.Name;
            existingPerson.Surname = person.Surname;

            await _context.SaveChangesAsync();
        }

        public async Task DeletePersonAsync(int id)
        {
            var deleteTransaction = await _transactionService.GetTransactionsByAccountIdAsync(id);
            var tran = deleteTransaction.FirstOrDefault(a => a.code == id);
            var deleteAccount = await _accountService.GetAllPersonsAccountAsync();
            var personacc = deleteAccount.Where(a => a.PersonCode == id && a.outstanding_balance > 0.0000M).ToList();
            var closedAccount =  deleteAccount.Where(a => a.account_number.StartsWith("999-"));

            if (personacc.Count() > 0) throw new InvalidOperationException("Can not delete a person with an OutStanding Balance.");
            if (closedAccount.Count() == 0) throw new InvalidOperationException("Can not delete a person with an open account.");

                await _context.Database.ExecuteSqlRawAsync("EXEC DeletePersonByCode @code = {0}", id);
        }
    }
}
