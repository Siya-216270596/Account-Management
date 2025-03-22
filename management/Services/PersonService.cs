using management.Interface;
using management.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

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
            foreach(var account in person.Account)
            {
                await _accountService.AddAccountAsync(account);
            }
          

            if (await _context.Persons.AnyAsync(p => p.id_number == person.id_number))
                throw new InvalidOperationException("ID Number already exists.");
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
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
            var person = await _context.Persons.FindAsync(id);
            if (person is not null)
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
