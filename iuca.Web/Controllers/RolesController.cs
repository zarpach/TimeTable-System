using iuca.Application.Constants;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Roles;
using iuca.Application.ViewModels.Users.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace iuca.Web.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [Authorize(Policy = Permissions.Roles.View)]
        public ActionResult Index()
        {
            return View(_roleService.GetRoles());
        }

        [Authorize(Policy = Permissions.Roles.Edit)]
        public IActionResult Create() => View();

        [Authorize(Policy = Permissions.Roles.Edit)]
        [HttpPost]
        public ActionResult Create(string name)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IdentityResult result = _roleService.Create(name);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else 
                    {
                        TempData["Error"] = "Error";
                        Errors(result);
                    }
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View(model:name);
        }

        [Authorize(Policy = Permissions.Roles.Edit)]
        public ActionResult Edit(string roleNamePrefix) 
        {
            return View(_roleService.GetRoleByNamePrefix(roleNamePrefix));
        }

        [Authorize(Policy = Permissions.Roles.Edit)]
        [HttpPost]
        public ActionResult Edit(RoleViewModel role)
        {
            IdentityResult result = _roleService.Edit(role.RoleName, role.RoleIds);
            if (result.Succeeded)
                return RedirectToAction("Index");
            else 
            {
                TempData["Error"] = "Error";
                Errors(result);
            }
  
            return View(role);
        }

        [Authorize(Policy = Permissions.Roles.Edit)]
        public ActionResult Delete(string roleNamePrefix)
        {
            return View(_roleService.GetRoleByNamePrefix(roleNamePrefix));
        }

        [Authorize(Policy = Permissions.Roles.Edit)]
        [HttpPost]
        public ActionResult DeleteConfirm(RoleViewModel role)
        {
            try
            {
                IdentityResult result = _roleService.Delete(role.RoleIds);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else 
                {
                    TempData["Error"] = "Error";
                    Errors(result);
                }
            }
            catch (ModelValidationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return View("Delete", model: role);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
