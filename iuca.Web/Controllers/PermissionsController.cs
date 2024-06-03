using iuca.Application.Constants;
using iuca.Application.Interfaces.Roles;
using iuca.Application.ViewModels.Users.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class PermissionsController : Controller
    {
        private readonly IPermissionService _permissionService;
        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [Authorize(Policy = Permissions.Roles.Edit)]
        public ActionResult Index(string roleNamePrefix)
        {
            return View(_permissionService.GetPermissionsByRole(roleNamePrefix));
        }

        [Authorize(Policy = Permissions.Roles.Edit)]
        public ActionResult Update(PermissionViewModel model)
        {
            _permissionService.UpdatePermissions(model);
            return RedirectToAction("Index", new { roleNamePrefix = model.RoleNamePrefix });
        }
    }
}
