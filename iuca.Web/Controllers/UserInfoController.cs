using iuca.Application.Constants;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels;
using iuca.Application.ViewModels.Users.UserInfo;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iuca.Web.Controllers
{

    [Authorize]
    public class UserInfoController : Controller
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IUserTypeOrganizationService _userTypeOrganizationService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IOrganizationService _organizationService;

        public UserInfoController(IUserInfoService userInfoService,
            IUserTypeOrganizationService userTypeOrganizationService,
            ApplicationUserManager<ApplicationUser> userManager,
            IOrganizationService organizationService)
        {
            _userInfoService = userInfoService;
            _userTypeOrganizationService = userTypeOrganizationService;
            _userManager = userManager;
            _organizationService = organizationService;
        }

        [Authorize(Policy = Permissions.Users.View)]
        public IActionResult Index()
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            List<UserInfoOrgTypesViewModel> userInfoList = _userInfoService.GetUserInfoWithOrgTypesList(selectedOrganization)
                            .OrderBy(x => x.ApplicationUser.FullNameEng).ToList();
            
            return View(userInfoList);
        }

        [Authorize(Policy = Permissions.Users.View)]
        public IActionResult Details(string id) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            UserInfoViewModel model = _userInfoService.GetUserInfoByApplicationUserId(selectedOrganizationId, id);

            return View(model);
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        public IActionResult Create() 
        {
            return View();
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPost]
        public IActionResult Create(UserInfoViewModel model) 
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                if (ModelState.IsValid)
                {
                    _userInfoService.Create(selectedOrganization, model);
                    return RedirectToAction("Index");
                }
            }
            catch (ModelValidationException ex) 
            {
                TempData["Error"] = ex.Message;
            }

            return View(model);
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        public IActionResult Edit(string id) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            UserInfoViewModel model = _userInfoService.GetUserInfoByApplicationUserId(selectedOrganizationId, id);

            return View(model);
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPost]
        public IActionResult Edit(string id, UserInfoViewModel model)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            try
            {
                if (ModelState.IsValid)
                {
                    _userInfoService.Edit(selectedOrganizationId, id, model);
                    return RedirectToAction("Index");
                }
            }
            catch (ModelValidationException ex) 
            {
                TempData["Error"] = ex.Message;
            }

            return View(model);
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        public IActionResult Delete(string id) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            UserInfoViewModel model = _userInfoService.GetUserInfoByApplicationUserId(selectedOrganizationId, id);

            return View(model);
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(string id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            try
            {
                _userInfoService.Delete(selectedOrganizationId, id);
            }
            catch (ModelValidationException ex) 
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Delete", new { id = id });
            }

            return RedirectToAction("Index");
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPost]
        public void SetUserType(int organizationId, string applicationUserId, int userType, bool isActive)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            if (organizationId != selectedOrganization)
                throw new Exception("Wrong organization");

            if (isActive)
                _userTypeOrganizationService.Create(organizationId, applicationUserId, userType);
            else
                _userTypeOrganizationService.Delete(organizationId, applicationUserId, userType);
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPost]
        public async Task ActivateUser(string id) 
        {
            IdentityResult result = await _userManager.SetUserActive(id, true);
            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
        }

        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPost]
        public async Task DeactivateUser(string id)
        {
            IdentityResult result = await _userManager.SetUserActive(id, false);
            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));

            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");
            
            result = await _userManager.UpdateSecurityStampAsync(user);
            
            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
        }
    }
}
