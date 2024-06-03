using iuca.Application.Constants;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Students;
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
    public class DepartmentGroupsController : Controller
    {
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IOrganizationService _organizationService;
        private readonly IDepartmentService _departmentService;

        public DepartmentGroupsController(IDepartmentGroupService departmentGroupService,
            IOrganizationService organizationService,
            IDepartmentService departmentService)
        {
            _departmentGroupService = departmentGroupService;
            _organizationService = organizationService;
            _departmentService = departmentService;
        }

        [Authorize(Policy = Permissions.DepartmentGroups.View)]
        public IActionResult Index()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_departmentGroupService.GetDepartmentGroups(selectedOrganizationId)
                .OrderByDescending(x => x.Year).ThenBy(x => x.Department.NameRus));
        }

        [Authorize(Policy = Permissions.DepartmentGroups.View)]
        public IActionResult Details(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_departmentGroupService.GetDepartmentGroup(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.DepartmentGroups.Edit)]
        public IActionResult Create() 
        {
            FillSelectLists();
            return View();
        }

        [Authorize(Policy = Permissions.DepartmentGroups.Edit)]
        [HttpPost]
        public IActionResult Create(DepartmentGroupDTO departmentGroup)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            departmentGroup.OrganizationId = selectedOrganizationId;

            if (ModelState.IsValid)
            {
                try
                {
                    _departmentGroupService.Create(departmentGroup);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            FillSelectLists(departmentGroup);

            return View(departmentGroup);
        }

        [Authorize(Policy = Permissions.DepartmentGroups.Edit)]
        public IActionResult Edit(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var departmentGroup = _departmentGroupService.GetDepartmentGroup(selectedOrganizationId, id);
            FillSelectLists(departmentGroup);
            
            return View(departmentGroup);
        }

        [Authorize(Policy = Permissions.DepartmentGroups.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, DepartmentGroupDTO departmentGroup)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _departmentGroupService.Edit(selectedOrganizationId, id, departmentGroup);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            FillSelectLists(departmentGroup);

            return View(departmentGroup);
        }

        [Authorize(Policy = Permissions.DepartmentGroups.Edit)]
        public IActionResult Delete(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_departmentGroupService.GetDepartmentGroup(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.DepartmentGroups.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _departmentGroupService.Delete(selectedOrganizationId, id);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return RedirectToAction("Delete", new { id = id });
        }

        private void FillSelectLists(DepartmentGroupDTO departmentGroup = null)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var departments = _departmentService.GetDepartments(selectedOrganization).Where(x => x.IsActive);

            if (departmentGroup != null)
            {
                ViewBag.Departments = new SelectList(departments, "Id", "NameRus", departmentGroup.DepartmentId);
            }
            else
            {
                ViewBag.Departments = new SelectList(departments, "Id", "NameRus");
            }
        }

        [Authorize(Policy = Permissions.DepartmentGroups.Edit)]
        [HttpPost]
        public IActionResult GenerateDepartmentGroupsForYear(int year)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            
            try
            {
                _departmentGroupService.GenerateDepartmentGroupsForYear(selectedOrganizationId, year);
            }
            catch (ModelValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}

