using Microsoft.AspNetCore.Mvc;
using management.Models;
using management.Interface;

namespace management.Controllers
{


    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;

        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        public async Task<IActionResult> Index(int pageNumber, int pageSize, string SearchString)
        {
            pageSize = 10;
            pageNumber = 1;
            // Fetch all persons
            var persons = await _personService.GetAllPersonsAsync();

            if (!String.IsNullOrEmpty(SearchString))
            {
                persons = persons.Where(x => x.id_number.Contains(SearchString)).ToList();
            }
            // Apply pagination
            var paginatedPersons = persons
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass pagination data to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = persons.Count();

            return View(paginatedPersons);
        }

        // Controller Action to Display Modal Popup
        public ActionResult ShowModal()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Person person)
        {
            if (ModelState.IsValid)
            {
                await _personService.AddPersonAsync(person);
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null) return NotFound();
            return View(person);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Person person)
        {
            if (ModelState.IsValid)
            {
                await _personService.UpdatePersonAsync(person);
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _personService.DeletePersonAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
