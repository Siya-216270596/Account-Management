using management.Models;
using management.Models.Response;

namespace management.Interface
{
    public interface IAccountService
    {
        Task<PersonAccountsResponse> GetAccountsByPersonIdAsync(int personId);
        Task<Account> GetAccountByIdAsync(int accountId);
        Task AddAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task CloseAccountAsync(int accountId);
    }

}
