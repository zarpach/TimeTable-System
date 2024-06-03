using iuca.Application.Constants;
using iuca.Application.DTO.Courses;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class CyclesController : Controller
    {
        private readonly ICycleService _cycleService;
        public CyclesController(ICycleService cycleService)
        {
            _cycleService = cycleService;
        }

        [Authorize(Policy = Permissions.Cycles.View)]
        public IActionResult Index()
        {
            return View(_cycleService.GetCycles());
        }

        [Authorize(Policy = Permissions.Cycles.View)]
        public IActionResult Details(int id)
        {
            return View(_cycleService.GetCycle(id));
        }

        [Authorize(Policy = Permissions.Cycles.Edit)]
        public IActionResult Create() => View();

        [Authorize(Policy = Permissions.Cycles.Edit)]
        [HttpPost]
        public IActionResult Create(CycleDTO cycle)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _cycleService.Create(cycle);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View(cycle);
        }

        [Authorize(Policy = Permissions.Cycles.Edit)]
        public IActionResult Edit(int id)
        {
            return View(_cycleService.GetCycle(id));
        }

        [Authorize(Policy = Permissions.Cycles.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, CycleDTO cycle)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _cycleService.Edit(id, cycle);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(cycle);
        }

        [Authorize(Policy = Permissions.Cycles.Edit)]
        public IActionResult Delete(int id)
        {
            return View(_cycleService.GetCycle(id));
        }

        [Authorize(Policy = Permissions.Cycles.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _cycleService.Delete(id);
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
