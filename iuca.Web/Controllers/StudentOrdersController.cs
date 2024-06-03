using iuca.Application.Constants;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class StudentOrdersController : Controller
    {
        private readonly IReinstatementExpulsionOrderService _reinstatementExpulsionOrderService;
        private readonly IAcademicLeaveOrderService _academicLeaveOrderService;
        private readonly IOrganizationService _organizationService;
        private readonly IStudentInfoService _studentInfoService;

        public StudentOrdersController(IReinstatementExpulsionOrderService reinstatementExpulsionOrderService,
            IAcademicLeaveOrderService academicLeaveOrderService,
            IOrganizationService organizationService,
            IStudentInfoService studentInfoService)
        {
            _reinstatementExpulsionOrderService = reinstatementExpulsionOrderService;
            _academicLeaveOrderService = academicLeaveOrderService;
            _organizationService = organizationService;
            _studentInfoService = studentInfoService;
        }

        [Authorize(Policy = Permissions.StudentOrders.View)]
        public IActionResult Index(int searchType, int searchIsApplied)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var orders = _reinstatementExpulsionOrderService
                .GetReinstatementExpulsionOrders(selectedOrganization, searchType, searchIsApplied);

            if (searchType == (int)enu_OrderType.AcadLeave || searchType == 0)
            {
                orders = orders.Concat(_academicLeaveOrderService
                .GetAcademicLeaveOrders(selectedOrganization, searchIsApplied));
            }

            StatusSelectList(searchIsApplied);
            OrderTypeSelectList(searchType);
            
            return View(orders.OrderByDescending(x => x.Date));
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public IActionResult CreateReinstatementExpulsionOrder(int type)
        {
            StudentSelectList(type);
            ViewBag.Type = type;

            return View(new ReinstatementExpulsionOrderDTO() { Type = type });
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        [HttpPost]
        public IActionResult CreateReinstatementExpulsionOrder(ReinstatementExpulsionOrderDTO newReinstatementExpulsionOrder, int type)
        {
            StudentSelectList(type);
            ViewBag.Type = type;

            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            newReinstatementExpulsionOrder.OrganizationId = selectedOrganization;

            if (ModelState.IsValid)
            {
                try
                {
                    int id = _reinstatementExpulsionOrderService.CreateReinstatementExpulsionOrder(newReinstatementExpulsionOrder);
                    TempData["StudentOrdersSuccessMessage"] = "Reinstatement/Expulsion order creation succeeded!";
                    return RedirectToAction("EditReinstatementExpulsionOrder", new
                    {
                        id = id
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["StudentOrdersError"] = ex.Message;
                }
            }
            else
            {
                TempData["StudentOrdersErrorMessage"] = "Reinstatement/Expulsion order creation failed.";
            }

            return View(newReinstatementExpulsionOrder);
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public IActionResult CreateAcademicLeaveOrder()
        {
            StudentSelectList();

            return View(new AcademicLeaveOrderDTO());
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        [HttpPost]
        public IActionResult CreateAcademicLeaveOrder(AcademicLeaveOrderDTO newAcademicLeaveOrder)
        {
            StudentSelectList();

            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            newAcademicLeaveOrder.OrganizationId = selectedOrganization;

            if (ModelState.IsValid)
            {
                try
                {
                    int id = _academicLeaveOrderService.CreateAcademicLeaveOrder(newAcademicLeaveOrder);
                    TempData["StudentOrdersSuccessMessage"] = "Academic Leave order creation succeeded!";
                    return RedirectToAction("EditAcademicLeaveOrder", new
                    {
                        id = id
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["StudentOrdersError"] = ex.Message;
                }
            }
            else
            {
                TempData["StudentOrdersErrorMessage"] = "Academic Leave order creation failed.";
            }

            return View(newAcademicLeaveOrder);
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public IActionResult EditReinstatementExpulsionOrder(int id)
        {
            var reinstatementExpulsionOrder = _reinstatementExpulsionOrderService.GetReinstatementExpulsionOrder(id);
            StudentSelectList(reinstatementExpulsionOrder.Type);

            return View(reinstatementExpulsionOrder);
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        [HttpPost]
        public IActionResult EditReinstatementExpulsionOrder(int id, ReinstatementExpulsionOrderDTO newReinstatementExpulsionOrder)
        {
            StudentSelectList(newReinstatementExpulsionOrder.Type);

            if (ModelState.IsValid)
            {
                try
                {
                    _reinstatementExpulsionOrderService.EditReinstatementExpulsionOrder(id, newReinstatementExpulsionOrder);
                    TempData["StudentOrdersSuccessMessage"] = "Reinstatement/Expulsion order saving succeeded!";
                    return RedirectToAction("EditReinstatementExpulsionOrder", new
                    {
                        id = id
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["StudentOrdersError"] = ex.Message;
                }
            }
            else
            {
                TempData["StudentOrdersErrorMessage"] = "Reinstatement/Expulsion order saving failed.";
            }

            return View(newReinstatementExpulsionOrder);
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public IActionResult EditAcademicLeaveOrder(int id)
        {
            StudentSelectList();
            var academicLeaveOrders = _academicLeaveOrderService.GetAcademicLeaveOrder(id);

            return View(academicLeaveOrders);
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        [HttpPost]
        public IActionResult EditAcademicLeaveOrder(int id, AcademicLeaveOrderDTO newAcademicLeaveOrder)
        {
            StudentSelectList();

            if (ModelState.IsValid)
            {
                try
                {
                    _academicLeaveOrderService.EditAcademicLeaveOrder(id, newAcademicLeaveOrder);
                    TempData["StudentOrdersSuccessMessage"] = "Academic Leave order saving succeeded!";
                    return RedirectToAction("EditAcademicLeaveOrder", new
                    {
                        id = id
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["StudentOrdersError"] = ex.Message;
                }
            }
            else
            {
                TempData["StudentOrdersErrorMessage"] = "Academic Leave order saving failed.";
            }

            return View(newAcademicLeaveOrder);
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public IActionResult ApplyReinstatementExpulsionOrder(int id)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            try
            {
                _reinstatementExpulsionOrderService.SetOrderApplicationStatus(selectedOrganization, id, true);
            }
            catch (ModelValidationException ex)
            {
                TempData["StudentOrdersError"] = ex.Message;
            }

            return Json(Url.Action("Index"));
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public IActionResult ApplyAcademicLeaveOrder(int id)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            try
            {
                _academicLeaveOrderService.SetOrderApplicationStatus(selectedOrganization, id, true);
            }
            catch (ModelValidationException ex)
            {
                TempData["StudentOrdersError"] = ex.Message;
            }

            return Json(Url.Action("Index"));
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public void CancelReinstatementExpulsionOrder(int id)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            try
            {
                _reinstatementExpulsionOrderService.SetOrderApplicationStatus(selectedOrganization, id, false);
            }
            catch (ModelValidationException ex)
            {
                TempData["StudentOrdersError"] = ex.Message;
            }
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public void CancelAcademicLeaveOrder(int id)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            try
            {
                _academicLeaveOrderService.SetOrderApplicationStatus(selectedOrganization, id, false);
            }
            catch (ModelValidationException ex)
            {
                TempData["StudentOrdersError"] = ex.Message;
            }
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public void DeleteReinstatementExpulsionOrder(int id)
        {
            _reinstatementExpulsionOrderService.DeleteReinstatementExpulsionOrder(id);
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public void DeleteAcademicLeaveOrder(int id)
        {
            _academicLeaveOrderService.DeleteAcademicLeaveOrder(id);
        }

        private void StudentSelectList(int type = 0)
        {
            int[] studentStates = (type == (int)enu_OrderType.Reinstatement) ?
                new int[] { (int)enu_StudentState.Dismissed, (int)enu_StudentState.AcadLeave } : new int[] { (int)enu_StudentState.Active };

            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var students = _studentInfoService.GetStudentInfoList(selectedOrganization, studentStates);

            ViewBag.Students = new SelectList(students, "StudentUserId", "StudentInfo");
        }

        private void StatusSelectList(int searchIsApplied)
        {
            var statuses = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Applied",
                    Value = enu_Status.True.ToString()
                },
                new SelectListItem
                {
                    Text = "Not Applied",
                    Value = enu_Status.False.ToString()
                }
            };

            ViewBag.ApplicationStatuses = new SelectList(statuses, "Text", "Value", searchIsApplied);
        }

        private void OrderTypeSelectList(int searchType)
        {
            var types = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = ((int)enu_OrderType.Reinstatement).ToString(),
                    Value = enu_OrderType.Reinstatement.ToString()
                },
                new SelectListItem
                {
                    Text = ((int)enu_OrderType.Expulsion).ToString(),
                    Value = enu_OrderType.Expulsion.ToString()
                },
                new SelectListItem
                {
                    Text = ((int)enu_OrderType.AcadLeave).ToString(),
                    Value = enu_OrderType.AcadLeave.ToString()
                }
            };

            ViewBag.Types = new SelectList(types, "Text", "Value", searchType);
        }
    }
} 
