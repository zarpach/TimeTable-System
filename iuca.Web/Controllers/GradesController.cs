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
    public class GradesController : Controller
    {
        private readonly IGradeService _gradeService;
        public GradesController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [Authorize(Policy = Permissions.Grades.View)]
        public IActionResult Index()
        {
            return View(_gradeService.GetGrades().OrderBy(x => x.GradeMark));
        }

        [Authorize(Policy = Permissions.Grades.View)]
        public IActionResult Details(int id)
        {
            return View(_gradeService.GetGrade(id));
        }

        [Authorize(Policy = Permissions.Grades.Edit)]
        public IActionResult Create() => View();

        [Authorize(Policy = Permissions.Grades.Edit)]
        [HttpPost]
        public IActionResult Create(GradeDTO grade)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _gradeService.Create(grade);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View(grade);
        }

        [Authorize(Policy = Permissions.Grades.Edit)]
        public IActionResult Edit(int id)
        {
            return View(_gradeService.GetGrade(id));
        }

        [Authorize(Policy = Permissions.Grades.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, GradeDTO grade)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _gradeService.Edit(id, grade);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(grade);
        }

        [Authorize(Policy = Permissions.Grades.Edit)]
        public IActionResult Delete(int id)
        {
            return View(_gradeService.GetGrade(id));
        }

        [Authorize(Policy = Permissions.Grades.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _gradeService.Delete(id);
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
