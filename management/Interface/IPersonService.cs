using management.Models;

namespace management.Interface
{
    public interface IPersonService
    {
        Task<Person> GetPersonByIdAsync(int id);
        Task<List<Person>> GetAllPersonsAsync(); // Add this method
        Task AddPersonAsync(Person person);
        Task UpdatePersonAsync(Person person);
        Task DeletePersonAsync(int id);
    }
}
