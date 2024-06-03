using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class PoliciesController : Controller
    {
        private readonly IPolicyService _policyService;

        public PoliciesController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        [Authorize(Policy = Permissions.Policies.View)]
        public IActionResult Index() => View(_policyService.GetPolicies());

        [Authorize(Policy = Permissions.Policies.View)]
        public IActionResult Details(int id) => View(_policyService.GetPolicy(id));

        [Authorize(Policy = Permissions.Policies.Edit)]
        public IActionResult Create() => View(new PolicyDTO());

        [Authorize(Policy = Permissions.Policies.Edit)]
        [HttpPost]
        public IActionResult Create(PolicyDTO newPolicy)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _policyService.CreatePolicy(newPolicy);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["PolicyError"] = ex.Message;
                }
            }
            else
            {
                TempData["PolicyErrorMessage"] = "Policy creation failed.";
            }

            return View(newPolicy);
        }

        [Authorize(Policy = Permissions.Policies.Edit)]
        public IActionResult Edit(int id) => View(_policyService.GetPolicy(id));

        [Authorize(Policy = Permissions.Policies.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, PolicyDTO newPolicy)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _policyService.EditPolicy(id, newPolicy);
                    TempData["PolicySuccessMessage"] = "Policy saving succeeded!";
                    return RedirectToAction("Edit", new {
                        id = id
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["PolicyError"] = ex.Message;
                }
            }
            else
            {
                TempData["PolicyErrorMessage"] = "Policy saving failed.";
            }

            return View(newPolicy);
        }

        [Authorize(Policy = Permissions.Policies.Edit)]
        public IActionResult Delete(int id) => View(_policyService.GetPolicy(id));

        [Authorize(Policy = Permissions.Policies.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _policyService.DeletePolicy(id);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["PolicyError"] = ex.Message;
                }
            }

            return RedirectToAction("Delete", new { 
                id = id
            });
        }
    }
}
