using management.Models;

namespace management.Interface
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId);
        Task AddTransactionAsync(Transaction transaction);
    }
}
