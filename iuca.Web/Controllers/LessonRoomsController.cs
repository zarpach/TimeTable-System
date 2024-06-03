using System;
using iuca.Application.DTO.Slots;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Slots;
using iuca.Application.Services.Slots;
using Microsoft.AspNetCore.Mvc;

namespace iuca.Web.Controllers
{
	public class LessonRoomsController : Controller
	{
		private readonly ILessonRoomService _lessonRoomService;

		public LessonRoomsController(
			ILessonRoomService lessonRoomService)
		{
			_lessonRoomService = lessonRoomService;
		}

		public IActionResult Index()
		{
			return View(_lessonRoomService.GetLessonRooms());
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(LessonRoomDTO lessonRoom)
		{
            if (ModelState.IsValid)
            {
                try
                {
                    _lessonRoomService.Create(lessonRoom);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(lessonRoom);
		}
	}
}

