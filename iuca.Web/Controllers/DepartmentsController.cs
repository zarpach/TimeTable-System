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
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IOrganizationService _organizationService;

        public DepartmentsController(IDepartmentService departmentService,
            IOrganizationService organizationService)
        {
            _departmentService = departmentService;
            _organizationService = organizationService;
        }

        [Authorize(Policy = Permissions.Departments.View)]
        public IActionResult Index()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_departmentService.GetDepartments(selectedOrganizationId));
        }

        [Authorize(Policy = Permissions.Departments.View)]
        public IActionResult Details(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_departmentService.GetDepartment(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.Departments.Edit)]
        public IActionResult Create() => View();

        [Authorize(Policy = Permissions.Departments.Edit)]
        [HttpPost]
        public IActionResult Create(DepartmentDTO department)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            department.OrganizationId = selectedOrganizationId;
            if (ModelState.IsValid)
            {
                try
                {
                    _departmentService.Create(department);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View(department);
        }

        [Authorize(Policy = Permissions.Departments.Edit)]
        public IActionResult Edit(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_departmentService.GetDepartment(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.Departments.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, DepartmentDTO department)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _departmentService.Edit(selectedOrganizationId, id, department);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(department);
        }

        [Authorize(Policy = Permissions.Departments.Edit)]
        public IActionResult Delete(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_departmentService.GetDepartment(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.Departments.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _departmentService.Delete(selectedOrganizationId, id);
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
