using System;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Slots;
using iuca.Application.DTO.Slots;
using Microsoft.AspNetCore.Mvc;

namespace iuca.Web.Controllers
{
	public class LessonPeriodsController : Controller
	{
        private ILessonPeriodService _lessonPeriodService;

		public LessonPeriodsController(ILessonPeriodService lessonPeriodService)
		{
            _lessonPeriodService = lessonPeriodService;
		}

		public IActionResult Index()
		{
			return View(_lessonPeriodService.GetLessonPeriods());
		}

        public IActionResult Create()
        { 
            return View();
        }

        [HttpPost]
        public IActionResult Create(LessonPeriodDTO lessonPeriodDTO)
		{
            if (ModelState.IsValid)
            {
                try
                {
                    _lessonPeriodService.Create(lessonPeriodDTO);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(lessonPeriodDTO);
        }
	}
}

