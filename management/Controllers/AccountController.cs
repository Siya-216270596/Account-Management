using management.Interface;
using Microsoft.AspNetCore.Mvc;

namespace management.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(int personId)
        {
            var accounts = await _accountService.GetAccountsByPersonIdAsync(personId);
            return View(accounts);
        }
    }
}
