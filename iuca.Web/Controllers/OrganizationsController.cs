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
    public class OrganizationsController : Controller
    {
        private readonly IOrganizationService _organizationService;
        
        public OrganizationsController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [Authorize(Policy = Permissions.Organizations.View)]
        public IActionResult Index()
        {
            return View(_organizationService.GetOrganizations());
        }

        [Authorize(Policy = Permissions.Organizations.View)]
        public IActionResult Details(int id) 
        {
            return View(_organizationService.GetOrganization(id));
        }

        [Authorize(Policy = Permissions.Organizations.Edit)]
        public IActionResult Create() => View();

        [Authorize(Policy = Permissions.Organizations.Edit)]
        [HttpPost]
        public IActionResult Create(OrganizationDTO organization)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _organizationService.Create(organization);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex) 
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View(organization);
        }

        [Authorize(Policy = Permissions.Organizations.Edit)]
        public IActionResult Edit(int id)
        {
            return View(_organizationService.GetOrganization(id));
        }

        [Authorize(Policy = Permissions.Organizations.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, OrganizationDTO organization)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _organizationService.Edit(id, organization);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex) 
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(organization);
        }

        [Authorize(Policy = Permissions.Organizations.Edit)]
        public IActionResult Delete(int id)
        {
            return View(_organizationService.GetOrganization(id));
        }

        [Authorize(Policy = Permissions.Organizations.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _organizationService.Delete(id);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return RedirectToAction("Delete", new { id = id });
        }

        [HttpPost]
        public IActionResult ChangeUserOrganization(string userId, int organizationId) 
        {
            _organizationService.ChangeUserOrganization(userId, organizationId);
            return RedirectToAction("Index", "Home");
        }
    }
}
