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
    public class LanguagesController : Controller
    {
        private readonly ILanguageService _languageService;
        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [Authorize(Policy = Permissions.Languages.View)]
        public IActionResult Index()
        {
            return View(_languageService.GetLanguages().OrderByDescending(x => x.SortNum).ThenBy(x => x.NameEng));
        }

        [Authorize(Policy = Permissions.Languages.View)]
        public IActionResult Details(int id)
        {
            return View(_languageService.GetLanguage(id));
        }

        [Authorize(Policy = Permissions.Languages.Edit)]
        public IActionResult Create() => View();

        [Authorize(Policy = Permissions.Languages.Edit)]
        [HttpPost]
        public IActionResult Create(LanguageDTO language)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _languageService.Create(language);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View(language);
        }

        [Authorize(Policy = Permissions.Languages.Edit)]
        public IActionResult Edit(int id)
        {
            return View(_languageService.GetLanguage(id));
        }

        [Authorize(Policy = Permissions.Languages.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, LanguageDTO language)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _languageService.Edit(id, language);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(language);
        }

        [Authorize(Policy = Permissions.Languages.Edit)]
        public IActionResult Delete(int id)
        {
            return View(_languageService.GetLanguage(id));
        }

        [Authorize(Policy = Permissions.Languages.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _languageService.Delete(id);
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
