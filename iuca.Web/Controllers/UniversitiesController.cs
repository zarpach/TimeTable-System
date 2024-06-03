using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class UniversitiesController : Controller
    {

        private readonly IUniversityService _universityService;
        private readonly ICountryService _countryService;
        public UniversitiesController(IUniversityService universityService,
            ICountryService countryService)
        {
            _universityService = universityService;
            _countryService = countryService;
        }

        [Authorize(Policy = Permissions.Universities.View)]
        public IActionResult Index()
        {
            return View(_universityService.GetUniversities());
        }

        [Authorize(Policy = Permissions.Universities.View)]
        public IActionResult Details(int id)
        {
            return View(_universityService.GetUniversity(id));
        }

        [Authorize(Policy = Permissions.Universities.Edit)]
        public IActionResult Create() 
        {
            ViewBag.Countries = new SelectList(_countryService.GetCountries(), "Id", "NameEng");
            return View();
        }

        [Authorize(Policy = Permissions.Universities.Edit)]
        [HttpPost]
        public IActionResult Create(UniversityDTO university)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _universityService.Create(university);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            ViewBag.Countries = new SelectList(_countryService.GetCountries(), "Id", "NameEng", university.CountryId);

            return View(university);
        }

        [Authorize(Policy = Permissions.Universities.Edit)]
        public IActionResult Edit(int id)
        {
            var model = _universityService.GetUniversity(id);
            ViewBag.Countries = new SelectList(_countryService.GetCountries(), "Id", "NameEng", model.CountryId);
            
            return View(model);
        }

        [Authorize(Policy = Permissions.Universities.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, UniversityDTO university)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _universityService.Edit(id, university);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            ViewBag.Countries = new SelectList(_countryService.GetCountries(), "Id", "NameEng", university.CountryId);

            return View(university);
        }

        [Authorize(Policy = Permissions.Universities.Edit)]
        public IActionResult Delete(int id)
        {
            return View(_universityService.GetUniversity(id));
        }

        [Authorize(Policy = Permissions.Universities.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _universityService.Delete(id);
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
