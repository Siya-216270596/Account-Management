using Microsoft.AspNetCore.Mvc;
using management.Models;
using management.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace management.Controllers
{


    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;
        private readonly IAccountService _accountService;

        public PersonsController(IPersonService personService, IAccountService accountService)
        {
            _personService = personService;
            _accountService = accountService;
        }

        [Authorize]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? SearchString = null)
        {
            // Fetch all accounts asynchronously
            var persons = await _personService.GetAllPersonsAsync();

            // Filter by search string if provided
            if (!string.IsNullOrEmpty(SearchString))
            {
                persons = persons.Where(x => x.id_number.Contains(SearchString)).ToList();
            }

            // Update total items after filtering
            ViewBag.TotalItems = persons.Count();

            // Apply pagination
            var paginatedPersons = persons
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass pagination data to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            return View(paginatedPersons);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Person person)
        {
            try
            {
                await _personService.AddPersonAsync(person);
                return Json(new { success = true, message="New Person added successfully" });
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

        [HttpPost]
        public async Task<IActionResult> CreateAccount(Account account)
        {
            try
            {
                await _accountService.AddAccountAsync(account);
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

        [HttpPost]
        public async Task<IActionResult> Edit(Person person)
        {
            try 
            { 
               await _personService.UpdatePersonAsync(person);
            return Json(new { success = true, message = "Upadeted successfully" });
        }
            catch (InvalidOperationException ex)
            {
                // Return the exception message for duplicate account number
                return Json(new { success = false, message = ex.Message});
            }
            catch (Exception)
            {
                // Handle other exceptions
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Person person)
        {
            try 
            { 
            await _personService.DeletePersonAsync(person.Code);
            return Json(new { success = true, message = "Person Deleted successfully" });

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
