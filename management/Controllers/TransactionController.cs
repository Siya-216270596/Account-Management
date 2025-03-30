using management.Interface;
using management.Models;
using management.Services;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
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
        public async Task<IActionResult> Edit(Transaction transaction)
        {
            try
            {
                await _transactionService.UpdateTransactionAsync(transaction);
                return Json(new { success = true, message = "Upadeted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                // Return the exception message for duplicate account number
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                // Handle other exceptions
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

    }
}
