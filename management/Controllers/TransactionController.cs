using management.Interface;
using Microsoft.AspNetCore.Mvc;

namespace management.Controllers
{
    public class TransactionController : Controller
    {
        public class TransactionsController : Controller
        {
            private readonly ITransactionService _transactionService;

            public TransactionsController(ITransactionService transactionService)
            {
                _transactionService = transactionService;
            }

            public async Task<IActionResult> Index(int accountId)
            {
                var transactions = await _transactionService.GetTransactionsByAccountIdAsync(accountId);
                return View(transactions);
            }
        }
    }
}
