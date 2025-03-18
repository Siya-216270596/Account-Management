using management.Models;

namespace management.Interface
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAccountsByPersonIdAsync(int personId);
        Task<Account> GetAccountByIdAsync(int accountId);
        Task AddAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task CloseAccountAsync(int accountId);
    }

}
