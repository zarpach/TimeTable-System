using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class EducationTypesController : Controller
    {
        private readonly IEducationTypeService _educationTypeService;
        public EducationTypesController(IEducationTypeService educationTypeService)
        {
            _educationTypeService = educationTypeService;
        }

        [Authorize(Policy = Permissions.EducationTypes.View)]
        public IActionResult Index()
        {
            return View(_educationTypeService.GetEducationTypes());
        }

        [Authorize(Policy = Permissions.EducationTypes.View)]
        public IActionResult Details(int id)
        {
            return View(_educationTypeService.GetEducationType(id));
        }

        [Authorize(Policy = Permissions.EducationTypes.Edit)]
        public IActionResult Create() => View();

        [Authorize(Policy = Permissions.EducationTypes.Edit)]
        [HttpPost]
        public IActionResult Create(EducationTypeDTO educationType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _educationTypeService.Create(educationType);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View(educationType);
        }

        [Authorize(Policy = Permissions.EducationTypes.Edit)]
        public IActionResult Edit(int id)
        {
            return View(_educationTypeService.GetEducationType(id));
        }

        [Authorize(Policy = Permissions.EducationTypes.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, EducationTypeDTO educationType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _educationTypeService.Edit(id, educationType);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(educationType);
        }

        [Authorize(Policy = Permissions.EducationTypes.Edit)]
        public IActionResult Delete(int id)
        {
            return View(_educationTypeService.GetEducationType(id));
        }

        [Authorize(Policy = Permissions.EducationTypes.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _educationTypeService.Delete(id);
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
