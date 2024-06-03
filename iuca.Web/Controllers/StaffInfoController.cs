using iuca.Application.Constants;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Staff;
using iuca.Application.ViewModels;
using iuca.Application.ViewModels.Users.Staff;
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
    public class StaffInfoController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly IStaffInfoService _staffInfoService;

        public StaffInfoController(IOrganizationService organizationService,
                IStaffInfoService staffInfoService)
        {
            _organizationService = organizationService;
            _staffInfoService = staffInfoService;
        }

        [Authorize(Policy = Permissions.Staff.View)]
        public IActionResult Index()
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View(_staffInfoService.GetStaffInfoList(selectedOrganization).ToList());
        }

        [Authorize(Policy = Permissions.Staff.View)]
        public IActionResult Details(string staffUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View(_staffInfoService.GetStaffFullInfo(selectedOrganization, staffUserId));
        }

        [Authorize(Policy = Permissions.Staff.Edit)]
        public IActionResult Create(string staffUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var emptyStaffList = _staffInfoService.GetEmptyStaff(selectedOrganization);

            ViewBag.Users = new SelectList(emptyStaffList, "Id", "FullName");

            return View();
        }

        [Authorize(Policy = Permissions.Staff.Edit)]
        [HttpPost]
        public IActionResult Create(StaffInfoDetailsViewModel model)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                if (ModelState.IsValid)
                {
                    _staffInfoService.Create(selectedOrganization, model);
                    return RedirectToAction("Index");
                }
            }
            catch (ModelValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            var emptyStaffList = _staffInfoService.GetEmptyStaff(selectedOrganization);

            ViewBag.Users = new SelectList(emptyStaffList, "Id", "FullNameEng", model.StaffUserId);

            return View(model);
        }

        [Authorize(Policy = Permissions.Staff.Edit)]
        public IActionResult Edit(string staffUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View(_staffInfoService.GetStaffFullInfo(selectedOrganization, staffUserId));
        }

        [Authorize(Policy = Permissions.Staff.Edit)]
        [HttpPost]
        public IActionResult Edit(StaffInfoDetailsViewModel model)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                if (ModelState.IsValid)
                {
                    _staffInfoService.Edit(selectedOrganization, model);
                    return RedirectToAction("Index");
                }
            }
            catch (ModelValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return View(model);
        }

        [Authorize(Policy = Permissions.Staff.Edit)]
        public IActionResult Delete(string staffUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View(_staffInfoService.GetStaffFullInfo(selectedOrganization, staffUserId));
        }

        [Authorize(Policy = Permissions.Staff.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int userBasicInfoId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                _staffInfoService.Delete(selectedOrganization, userBasicInfoId);
            }
            catch (ModelValidationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Delete", new { userBasicInfoId = userBasicInfoId });
            }

            return RedirectToAction("Index");
        }
    }
}
