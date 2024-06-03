using iuca.Application.Constants;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using iuca.Application.Interfaces.Users.Instructors;
using System.Text.Json;
using iuca.Application.Exceptions;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class GroupTransferOrdersController : Controller
    {
        private readonly IGroupTransferOrderService _groupTransferOrderService;
        private readonly IOrganizationService _organizationService;
        private readonly IStudentInfoService _studentInfoService;
        private readonly ISemesterService _semesterService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IAdviserStudentService _adviserStudentService;

        public GroupTransferOrdersController(IGroupTransferOrderService groupTransferOrderService,
            IOrganizationService organizationService,
            IStudentInfoService studentInfoService,
            ISemesterService semesterService,
            IDepartmentGroupService departmentGroupService,
            IAdviserStudentService adviserStudentService)
        {
            _groupTransferOrderService = groupTransferOrderService;
            _organizationService = organizationService;
            _studentInfoService = studentInfoService;
            _semesterService = semesterService;
            _departmentGroupService = departmentGroupService;
            _adviserStudentService = adviserStudentService;
        }

        [Authorize(Policy = Permissions.StudentOrders.View)]
        public IActionResult Index(int searchIsApplied)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var orders = _groupTransferOrderService
                .GetGroupTransferOrders(selectedOrganization, searchIsApplied);

            StatusSelectList(searchIsApplied);

            return View(orders.OrderByDescending(x => x.Date));
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public IActionResult Create()
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            StudentSelectList();
            SemesterSelectList(selectedOrganization);
            DepartmentGroupSelectList(selectedOrganization);
            AdviserSelectList(selectedOrganization);

            return View(new GroupTransferOrderDTO());
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        [HttpPost]
        public IActionResult Create(GroupTransferOrderDTO newGroupTransferOrderDTO)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            StudentSelectList();
            SemesterSelectList(selectedOrganization);
            DepartmentGroupSelectList(selectedOrganization);
            AdviserSelectList(selectedOrganization);

            newGroupTransferOrderDTO.OrganizationId = selectedOrganization;

            if (ModelState.IsValid)
            {
                try
                {
                    int id = newGroupTransferOrderDTO.Id = _groupTransferOrderService.CreateGroupTransferOrder(newGroupTransferOrderDTO);
                    TempData["GroupTransferOrdersSuccessMessage"] = "Group transfer order creation succeeded!";
                    return RedirectToAction("Edit", new
                    {
                        id = id
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["GroupTransferOrdersError"] = ex.Message;
                }
            }
            else
            {
                TempData["GroupTransferOrdersErrorMessage"] = "Group transfer order creation failed.";
            }

            return View(newGroupTransferOrderDTO);
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public IActionResult Edit(int id)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            StudentSelectList();
            SemesterSelectList(selectedOrganization);
            DepartmentGroupSelectList(selectedOrganization);
            AdviserSelectList(selectedOrganization);

            var groupTransferOrder = _groupTransferOrderService.GetGroupTransferOrder(id);

            return View(groupTransferOrder);
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, GroupTransferOrderDTO newGroupTransferOrderDTO)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            StudentSelectList();
            SemesterSelectList(selectedOrganization);
            DepartmentGroupSelectList(selectedOrganization);
            AdviserSelectList(selectedOrganization);

            if (ModelState.IsValid)
            {
                try
                {
                    _groupTransferOrderService.EditGroupTransferOrder(id, newGroupTransferOrderDTO);
                    TempData["GroupTransferOrdersSuccessMessage"] = "Group transfer order saving succeeded!";
                    return RedirectToAction("Edit", new
                    {
                        id = id
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["GroupTransferOrdersError"] = ex.Message;
                }
            }
            else
            {
                TempData["GroupTransferOrdersErrorMessage"] = "Group transfer order saving failed.";
            }

            return View(newGroupTransferOrderDTO);
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public IActionResult Apply(int id)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            try
            {
                var message = _groupTransferOrderService.SetOrderApplicationStatus(selectedOrganization, id, true);
                TempData["GroupTransferOrdersSuccessMessage"] = message == "" ? null : message;
            }
            catch (ModelValidationException ex)
            {
                TempData["GroupTransferOrdersError"] = ex.Message;
            }

            return Json(Url.Action("Index"));
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public void Cancel(int id)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            try
            {
                var message = _groupTransferOrderService.SetOrderApplicationStatus(selectedOrganization, id, false);
                TempData["GroupTransferOrdersSuccessMessage"] = message == "" ? null : message;
            }
            catch (ModelValidationException ex)
            {
                TempData["GroupTransferOrdersError"] = ex.Message;
            }
        }

        [Authorize(Policy = Permissions.StudentOrders.Edit)]
        public void Delete(int id)
        {
            try
            {
                _groupTransferOrderService.DeleteGroupTransferOrder(id);
            }
            catch (ModelValidationException ex)
            {
                TempData["GroupTransferOrdersError"] = ex.Message;
            }
        }

        public string GetStudentGroupCode(string studentUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            return _studentInfoService.GetStudentDepartmentGroup(selectedOrganization, studentUserId).Code;
        }

        public IEnumerable<string> GetStudentAdviserFullnames(string studentUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            return _adviserStudentService.GetStudentAdvisers(selectedOrganization, studentUserId)
                .Select(x => x.FullName);
        }

        private void StudentSelectList()
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var students = _studentInfoService.GetStudentInfoList(selectedOrganization, new int[] { (int)enu_StudentState.Active });

            ViewBag.Students = new SelectList(students, "StudentUserId", "StudentInfo");
        }

        private void StatusSelectList(int searchIsApplied)
        {
            var statuses = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = ((int)enu_Status.True).ToString(),
                    Value = enu_Status.True.ToString()
                },
                new SelectListItem
                {
                    Text = ((int)enu_Status.False).ToString(),
                    Value = enu_Status.False.ToString()
                }
            };

            ViewBag.ApplicationStatuses = new SelectList(statuses, "Text", "Value", searchIsApplied);
        }

        private void SemesterSelectList(int selectedOrganization)
        {
            var semesters = _semesterService.GetSemesters(selectedOrganization).OrderByDescending(x => x.Year).ThenBy(x => x.Season);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear");
        }

        private void DepartmentGroupSelectList(int selectedOrganization)
        {
            var departmentGroups = _departmentGroupService.GetDepartmentGroups(selectedOrganization)
                .OrderBy(x => x.Department.Code).ThenByDescending(x => x.Year);

            ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode");
        }

        private void AdviserSelectList(int selectedOrganization)
        {
            var advisers = _adviserStudentService.GetAdvisers(selectedOrganization)
                .Select(x => new { Value = x.Id, Text = x.FullName })
                .OrderBy(x => x.Text);

            ViewBag.Advisers = new SelectList(advisers, "Value", "Text");
        }
    }
}
