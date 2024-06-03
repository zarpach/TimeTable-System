using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class SemesterPeriodsController : Controller
    {
        private readonly ISemesterPeriodService _semesterPeriodService;
        private readonly IOrganizationService _organizationService;
        private readonly ISemesterService _semesterService;

        public SemesterPeriodsController(ISemesterPeriodService semesterPeriodService,
            IOrganizationService organizationService,
            ISemesterService semesterService)
        {
            _semesterPeriodService = semesterPeriodService;
            _organizationService = organizationService;
            _semesterService = semesterService;
        }

        [Authorize(Policy = Permissions.SemesterPeriods.View)]
        public IActionResult Index()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_semesterPeriodService.GetSemesterPeriods(selectedOrganizationId));
        }

        [Authorize(Policy = Permissions.SemesterPeriods.View)]
        public IActionResult Details(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_semesterPeriodService.GetSemesterPeriod(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.SemesterPeriods.Edit)]
        public IActionResult Create()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), "Id", "SeasonYear");
    
            return View();
        }

        [Authorize(Policy = Permissions.SemesterPeriods.Edit)]
        [HttpPost]
        public IActionResult Create(SemesterPeriodDTO semesterPeriod)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            semesterPeriod.OrganizationId = selectedOrganizationId;
            if (ModelState.IsValid)
            {
                try
                {
                    _semesterPeriodService.Create(semesterPeriod);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), "Id", "SeasonYear", semesterPeriod.SemesterId);

            return View(semesterPeriod);
        }

        [Authorize(Policy = Permissions.SemesterPeriods.Edit)]
        public IActionResult Edit(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var semesterPeriod = _semesterPeriodService.GetSemesterPeriod(selectedOrganizationId, id);

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), "Id", "SeasonYear", semesterPeriod.Semester.Id);

            return View(semesterPeriod);
        }

        [Authorize(Policy = Permissions.SemesterPeriods.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, SemesterPeriodDTO semesterPeriod)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _semesterPeriodService.Edit(selectedOrganizationId, id, semesterPeriod);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), "Id", "SeasonYear", semesterPeriod.Semester.Id);

            return View(semesterPeriod);
        }

        [Authorize(Policy = Permissions.SemesterPeriods.Edit)]
        public IActionResult Delete(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_semesterPeriodService.GetSemesterPeriod(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.SemesterPeriods.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _semesterPeriodService.Delete(selectedOrganizationId, id);
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
