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
    public class SemestersController : Controller
    {
        private readonly ISemesterService _semesterService;
        private readonly IOrganizationService _organizationService;

        public SemestersController(ISemesterService semesterService,
            IOrganizationService organizationService)
        {
            _semesterService = semesterService;
            _organizationService = organizationService;
        }

        [Authorize(Policy = Permissions.Semesters.View)]
        public IActionResult Index()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_semesterService.GetSemesters(selectedOrganizationId).OrderBy(x => x.Year).ThenBy(x => x.Season));
        }

        [Authorize(Policy = Permissions.Semesters.View)]
        public IActionResult Details(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_semesterService.GetSemester(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.Semesters.Edit)]
        public IActionResult Create() => View();

        [Authorize(Policy = Permissions.Semesters.Edit)]
        [HttpPost]
        public IActionResult Create(SemesterDTO semester)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            semester.OrganizationId = selectedOrganizationId;

            if (ModelState.IsValid)
            {
                try
                {
                    _semesterService.Create(semester);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View(semester);
        }

        [Authorize(Policy = Permissions.Semesters.Edit)]
        public IActionResult Edit(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_semesterService.GetSemester(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.Semesters.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, SemesterDTO semester)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _semesterService.Edit(selectedOrganizationId, id, semester);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(semester);
        }

        [Authorize(Policy = Permissions.Semesters.Edit)]
        public IActionResult Delete(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_semesterService.GetSemester(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.Semesters.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _semesterService.Delete(selectedOrganizationId, id);
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
