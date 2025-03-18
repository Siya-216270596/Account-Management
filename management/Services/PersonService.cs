using management.Interface;
using management.Models;
using Microsoft.EntityFrameworkCore;

namespace management.Services
{
    public class PersonService : IPersonService
    {
        private readonly AppDbContext _context;

        public PersonService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Person> GetPersonByIdAsync(int id)
        {
            return await _context.Persons.FindAsync(id);
        }

        public async Task<List<Person>> GetAllPersonsAsync()
        {
            return await _context.Persons.ToListAsync(); // Fetch all persons from the database
        }

        public async Task AddPersonAsync(Person person)
        {
            if (await _context.Persons.AnyAsync(p => p.id_number == person.id_number))
                throw new InvalidOperationException("ID Number already exists.");
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePersonAsync(Person person)
        {
            _context.Persons.Update(person);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePersonAsync(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person != null)
            {
                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
            }
        }
    }
}
