using management.Interface;
using management.Services;
using Microsoft.AspNetCore.Mvc;

namespace management.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            // Fetch all accounts asynchronously
            var transactions = await _transactionService.GetAllTransactionsAsync();



            // Update total items after filtering
            ViewBag.TotalItems = transactions.Count();

            // Apply pagination
            var paginatedPersons = transactions
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass pagination data to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            return View(paginatedPersons);
        }

    }
}
