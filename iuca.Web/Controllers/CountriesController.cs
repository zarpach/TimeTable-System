using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class CountriesController : Controller
    {
        private readonly ICountryService _countryService;
        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [Authorize(Policy = Permissions.Countries.View)]
        public IActionResult Index()
        {
            return View(_countryService.GetCountries().OrderByDescending(x => x.SortNum).ThenBy(x => x.NameEng));
        }

        [Authorize(Policy = Permissions.Countries.View)]
        public IActionResult Details(int id)
        {
            return View(_countryService.GetCountry(id));
        }

        [Authorize(Policy = Permissions.Countries.Edit)]
        public IActionResult Create() => View();

        [Authorize(Policy = Permissions.Countries.Edit)]
        [HttpPost]
        public IActionResult Create(CountryDTO country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _countryService.Create(country);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View(country);
        }

        [Authorize(Policy = Permissions.Countries.Edit)]
        public IActionResult Edit(int id)
        {
            return View(_countryService.GetCountry(id));
        }

        [Authorize(Policy = Permissions.Countries.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, CountryDTO country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _countryService.Edit(id, country);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(country);
        }

        [Authorize(Policy = Permissions.Countries.Edit)]
        public IActionResult Delete(int id)
        {
            return View(_countryService.GetCountry(id));
        }

        [Authorize(Policy = Permissions.Countries.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _countryService.Delete(id);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return RedirectToAction("Delete", new { id = id });
        }
    }
}
