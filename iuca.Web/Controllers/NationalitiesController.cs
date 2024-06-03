using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class NationalitiesController : Controller
    {
        private readonly INationalityService _nationalityService;
        
        public NationalitiesController(INationalityService nationalityService)
        {
            _nationalityService = nationalityService;
        }

        [Authorize(Policy = Permissions.Nationalities.View)]
        public IActionResult Index()
        {
            return View(_nationalityService.GetNationalities().OrderByDescending(x => x.SortNum).ThenBy(x => x.NameEng));
        }

        [Authorize(Policy = Permissions.Nationalities.View)]
        public IActionResult Details(int id)
        {
            return View(_nationalityService.GetNationality(id));
        }

        [Authorize(Policy = Permissions.Nationalities.Edit)]
        public IActionResult Create() => View();

        [Authorize(Policy = Permissions.Nationalities.Edit)]
        [HttpPost]
        public IActionResult Create(NationalityDTO nationality)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _nationalityService.Create(nationality);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View(nationality);
        }

        [Authorize(Policy = Permissions.Nationalities.Edit)]
        public IActionResult Edit(int id)
        {
            return View(_nationalityService.GetNationality(id));
        }

        [Authorize(Policy = Permissions.Nationalities.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, NationalityDTO nationality)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _nationalityService.Edit(id, nationality);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(nationality);
        }

        [Authorize(Policy = Permissions.Nationalities.Edit)]
        public IActionResult Delete(int id)
        {
            return View(_nationalityService.GetNationality(id));
        }

        [Authorize(Policy = Permissions.Nationalities.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _nationalityService.Delete(id);
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
