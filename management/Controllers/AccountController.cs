using management.Interface;
using management.Models;
using management.Services;
using Microsoft.AspNetCore.Mvc;

namespace management.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;


        public AccountsController(IAccountService accountService, ITransactionService transactionService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? SearchString = null)
        {
            // Fetch all accounts asynchronously
            var account = await _accountService.GetAllPersonsAsync();

            // Filter by search string if provided
            if (!string.IsNullOrEmpty(SearchString))
            {
                account = account.Where(x => x.account_number.Contains(SearchString)).ToList();
            }

            // Update total items after filtering
            ViewBag.TotalItems = account.Count();

            // Apply pagination
            var paginatedPersons = account
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass pagination data to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            return View(paginatedPersons);
        }

        public async Task<IActionResult> Edit(Account account)
        {
            try
            {
                await _accountService.UpdateAccountAsync(account);
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

        [HttpPost]
        public async Task<IActionResult> CreateAccount(Transaction transaction)
        {
            try
            {
                await _transactionService.AddTransactionAsync(transaction);
                return Json(new { success = true, message = "New Account added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                // Return the exception message for duplicate account number
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                // Handle other exceptions
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

    }
}
