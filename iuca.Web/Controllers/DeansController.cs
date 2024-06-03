using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class DeansController : Controller
    {
        private readonly IDeanService _deanService;
        private readonly IOrganizationService _organizationService;
        private readonly IUserBasicInfoService _userBasicInfoService;
        private readonly IUserRolesService _userRolesService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserInfoService _userInfoService;

        public DeansController(IDeanService deanService,
            IOrganizationService organizationService,
            IUserBasicInfoService userBasicInfoService,
            IUserRolesService userRolesService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserInfoService userInfoService)
        {
            _deanService = deanService;
            _organizationService = organizationService;
            _userBasicInfoService = userBasicInfoService;
            _userRolesService = userRolesService;
            _userManager = userManager;
            _userInfoService = userInfoService;
        }

        [Authorize(Policy = Permissions.Deans.View)]
        public IActionResult Index()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            return View(_deanService.GetDeans(selectedOrganizationId).OrderBy(x => x.DeanUser.FullNameEng).ToList());
        }

        [Authorize(Policy = Permissions.Deans.Edit)]
        public IActionResult EditDeanDepartments(string deanUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var user = _userManager.Users.FirstOrDefault(x => x.Id == deanUserId);
            if (user == null)
                throw new Exception("User not found");

            ViewBag.DeanName = user.FullNameEng;
            ViewBag.DeanUserId = deanUserId;
            ViewBag.OrganizationId = selectedOrganizationId;

            return View(_deanService.GetDeanDepartments(selectedOrganizationId, deanUserId));
        }

        [Authorize(Policy = Permissions.Deans.Edit)]
        [HttpPost]
        public IActionResult EditDeanDepartments(string deanUserId, int[] departmentIds)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _deanService.EditDeanDepartments(selectedOrganizationId, deanUserId, departmentIds.ToList());
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View("EditDeanDepartments", new { deanUserId = deanUserId, departmentIds = departmentIds });
        }

        [Authorize(Policy = Permissions.Deans.Edit)]
        public ViewResult GetBlankDepartmentEditorRow(int organizationId)
        {
            ViewBag.OrganizationId = organizationId;
            return View("_EditDeanDepartmentsPartial", new DepartmentDTO());
        }

        [Authorize(Policy = Permissions.Advisers.View)]
        public IActionResult DeanAdvisers(string deanUserId) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            
            ApplicationUser user = null;
            Task.Run(() => user = _userManager.GetUserAsync(User).GetAwaiter().GetResult()).Wait();
            if (user == null)
                return NotFound();

            bool isAdmin = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Admin);

            ViewBag.IsAdmin = isAdmin;

            if (isAdmin)
            {
                ViewBag.DeanName = _userManager.Users.FirstOrDefault(x => x.Id == deanUserId)?.FullNameEng;
                ViewBag.Deans = _userInfoService.GetUserSelectList(selectedOrganizationId, enu_Role.Dean, deanUserId);
            }
            else 
            {
                if (!_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean))
                    throw new Exception("User is not Dean");

                deanUserId = user.Id;
                ViewBag.DeanName = user.FullNameEng;
            }

            var model = _deanService.GetDeanAdvisers(selectedOrganizationId, deanUserId)
                    .OrderBy(x => x.Instructor.FullNameEng).ToList();

            ViewBag.DeanUserId = deanUserId;

            return View(model);
        }

        [Authorize(Policy = Permissions.Advisers.View)]
        public IActionResult AddDeanAdvisers(string deanUserId, bool onlyActive)
        {
            if (string.IsNullOrEmpty(deanUserId))
                throw new Exception("Dean is not selected");

            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ViewBag.DeanName = _userManager.Users.FirstOrDefault(x => x.Id == deanUserId)?.FullNameEng;
            ViewBag.DeanUserId = deanUserId;
            ViewBag.OnlyActive = onlyActive;

            var model = _deanService.GetFreeInstructorsForDean(selectedOrganizationId, deanUserId, onlyActive)
                    .OrderBy(x => x.Instructor.FullNameEng).ToList();

            return View(model);
        }

        [Authorize(Policy = Permissions.Advisers.Edit)]
        [HttpPost]
        public void AddDeanAdviser(string deanUserId, string instructorUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _deanService.SetDeanAdviser(selectedOrganizationId, deanUserId, instructorUserId, true);
        }

        [Authorize(Policy = Permissions.Advisers.Edit)]
        [HttpPost]
        public void DeleteDeanAdviser(string deanUserId, string instructorUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _deanService.SetDeanAdviser(selectedOrganizationId, deanUserId, instructorUserId, false);
        }
    }
}
