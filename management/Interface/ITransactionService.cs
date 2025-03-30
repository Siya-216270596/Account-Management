using management.Models;

namespace management.Interface
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsByAccountIdAsync(int accountId);
        Task AddTransactionAsync(Transaction transaction);
        Task<List<Transaction>> GetAllTransactionsAsync();
        Task UpdateTransactionAsync(Transaction transaction);
        Task DeletetTransactionByIdAsync(int id);
    }
}
