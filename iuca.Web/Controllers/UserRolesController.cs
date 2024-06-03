using iuca.Application.Constants;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Roles;
using iuca.Application.ViewModels.Users.Roles;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IUserRolesService _userRolesService;
        private readonly IOrganizationService _organizationService;

        public UserRolesController(IUserRolesService userRolesService,
                                   IOrganizationService organizationService)
        {
            _userRolesService = userRolesService;
            _organizationService = organizationService;
        }

        [Authorize(Policy = Permissions.UserRoles.View)]
        public IActionResult Index()
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View(_userRolesService.GetUserList(selectedOrganization).OrderBy(x => x.FullNameEng).ToList());
        }

        [Authorize(Policy = Permissions.UserRoles.Edit)]
        public IActionResult ManageRoles(string id) 
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            UserRolesViewModel model = _userRolesService.GetUserRolesInfo(selectedOrganization, id);
            return View(model);
        }

        [Authorize(Policy = Permissions.UserRoles.Edit)]
        /// <summary>
        /// Add or remove role from user for specific organization
        /// </summary>
        /// <param name="userId"> Application userId</param>
        /// <param name="organizationId">Id of organization</param>
        /// <param name="roleName">Role name</param>
        /// <param name="isActive">true - add role, false - remove role</param>
        [HttpPost]
        public void SetRole(string userId, int organizationId, string roleName, bool isActive)
        {
            if (_userRolesService.IsUserType(roleName))
                throw new Exception("This role can only be changed by changing the user type");

            if (isActive)
                _userRolesService.AddToRole(userId, $"{roleName}_{organizationId}");
            else
                _userRolesService.RemoveFromRole(userId, $"{roleName}_{organizationId}");
        }
    }
}
