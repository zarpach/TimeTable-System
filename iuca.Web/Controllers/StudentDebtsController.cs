using iuca.Application.Constants;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Users.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class StudentDebtsController : Controller
    {
        private readonly IStudentDebtService _studentDebtService;
        private readonly IOrganizationService _organizationService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly ISemesterService _semesterService;
        private readonly ISemesterPeriodService _semesterPeriodService;
        private readonly IEnvarSettingService _envarSettingService;

        public StudentDebtsController(IStudentDebtService studentDebtService,
            IOrganizationService organizationService,
            IDepartmentGroupService departmentGroupService,
            ISemesterService semesterService,
            ISemesterPeriodService semesterPeriodService,
            IEnvarSettingService envarSettingService)
        {
            _studentDebtService = studentDebtService;
            _organizationService = organizationService;
            _departmentGroupService = departmentGroupService;
            _semesterService = semesterService;
            _semesterPeriodService = semesterPeriodService;
            _envarSettingService = envarSettingService;
        }


        [Authorize(Policy = Permissions.DebtsAccounting.View)]
        public IActionResult ManageAccountingDebts(int semesterId, int departmentGroupId, string lastName, string firstName, 
            int studentId, enu_DebtorType debtorType, bool activeOnly, bool noRegistrations)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            if (semesterId == 0) 
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            var studentDebts = _studentDebtService.GetStudentDebtList(selectedOrganizationId, enu_DebtType.Accounting, semesterId, departmentGroupId, 
                lastName, firstName, studentId, debtorType, activeOnly, noRegistrations);

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), "Id", "SeasonYear", semesterId);
            ViewBag.SemesterId = semesterId;
            ViewBag.DepartmentGroups = new SelectList(_departmentGroupService.GetDepartmentGroups(selectedOrganizationId).OrderBy(x => x.DepartmentCode),
                                                                                            "Id", "DepartmentCode", departmentGroupId);
            ViewBag.DepartmentGroupId = departmentGroupId;
            ViewBag.LastName = lastName;
            ViewBag.FirstName = firstName;
            ViewBag.StudentId = studentId;
            ViewBag.DebtorTypes =  debtorType.ToSelectList((int)debtorType);
            ViewBag.DebtorType = debtorType;
            ViewBag.ActiveOnly = activeOnly;
            ViewBag.NoRegistrations = noRegistrations;
            ViewBag.DebtType = enu_DebtType.Accounting;

            return View(studentDebts.OrderBy(x => x.StudentName));
        }

        [Authorize(Policy = Permissions.DebtsAccounting.Edit)]
        [HttpPost]
        public IActionResult SetAccountingDebts(int semesterId, int departmentGroupId, string lastName, string firstName,
            int studentId, enu_DebtorType debtorType, bool activeOnly, bool noRegistrations, List<StudentDebtViewModel> debtList) 
        {
            try
            {
                _studentDebtService.SetStudentDebts(debtList);
                TempData["Success"] = "Saved successfully";
            }
            catch (Exception ex) 
            {
                TempData["Fail"] = "Failed to save:\n" + ex.Message;
            }

            return RedirectToAction("ManageAccountingDebts", new { semesterId = semesterId, 
                departmentGroupId = departmentGroupId, lastName = lastName, firstName = firstName, studentId = studentId,
                    debtorType = debtorType, activeOnly = activeOnly, noRegistrations = noRegistrations
            });
        }


        [Authorize(Policy = Permissions.DebtsLibrary.View)]
        public IActionResult ManageLibraryDebts(int semesterId, int departmentGroupId, string lastName, string firstName,
            int studentId, enu_DebtorType debtorType, bool activeOnly, bool noRegistrations)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            var studentDebts = _studentDebtService.GetStudentDebtList(selectedOrganizationId, enu_DebtType.Library, semesterId, departmentGroupId,
                lastName, firstName, studentId, debtorType, activeOnly, noRegistrations);

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), "Id", "SeasonYear", semesterId);
            ViewBag.SemesterId = semesterId;
            ViewBag.DepartmentGroups = new SelectList(_departmentGroupService.GetDepartmentGroups(selectedOrganizationId).OrderBy(x => x.DepartmentCode),
                                                                                            "Id", "DepartmentCode", departmentGroupId);
            ViewBag.DepartmentGroupId = departmentGroupId;
            ViewBag.LastName = lastName;
            ViewBag.FirstName = firstName;
            ViewBag.StudentId = studentId;
            ViewBag.DebtorTypes = debtorType.ToSelectList((int)debtorType);
            ViewBag.DebtorType = debtorType;
            ViewBag.ActiveOnly = activeOnly;
            ViewBag.NoRegistrations = noRegistrations;
            ViewBag.DebtType = enu_DebtType.Library;

            return View(studentDebts.OrderBy(x => x.StudentName));
        }

        [Authorize(Policy = Permissions.DebtsLibrary.Edit)]
        [HttpPost]
        public IActionResult SetLibraryDebts(int semesterId, int departmentGroupId, string lastName, string firstName,
            int studentId, enu_DebtorType debtorType, bool activeOnly, bool noRegistrations, List<StudentDebtViewModel> debtList)
        {
            try
            {
                _studentDebtService.SetStudentDebts(debtList);
                TempData["Success"] = "Saved successfully";
            }
            catch (Exception ex)
            {
                TempData["Fail"] = "Failed to save:\n" + ex.Message;
            }

            return RedirectToAction("ManageLibraryDebts", new
            {
                semesterId = semesterId,
                departmentGroupId = departmentGroupId,
                lastName = lastName,
                firstName = firstName,
                studentId = studentId,
                debtorType = debtorType,
                activeOnly = activeOnly,
                noRegistrations = noRegistrations
            });
        }

        [Authorize(Policy = Permissions.DebtsDormitory.View)]
        public IActionResult ManageDormitoryDebts(int semesterId, int departmentGroupId, string lastName, string firstName,
            int studentId, enu_DebtorType debtorType, bool activeOnly, bool noRegistrations)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            var studentDebts = _studentDebtService.GetStudentDebtList(selectedOrganizationId, enu_DebtType.Dormitory, semesterId, departmentGroupId,
                lastName, firstName, studentId, debtorType, activeOnly, noRegistrations);

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), "Id", "SeasonYear", semesterId);
            ViewBag.SemesterId = semesterId;
            ViewBag.DepartmentGroups = new SelectList(_departmentGroupService.GetDepartmentGroups(selectedOrganizationId).OrderBy(x => x.DepartmentCode),
                                                                                            "Id", "DepartmentCode", departmentGroupId);
            ViewBag.DepartmentGroupId = departmentGroupId;
            ViewBag.LastName = lastName;
            ViewBag.FirstName = firstName;
            ViewBag.StudentId = studentId;
            ViewBag.DebtorTypes = debtorType.ToSelectList((int)debtorType);
            ViewBag.DebtorType = debtorType;
            ViewBag.ActiveOnly = activeOnly;
            ViewBag.NoRegistrations = noRegistrations;
            ViewBag.DebtType = enu_DebtType.Dormitory;

            return View(studentDebts.OrderBy(x => x.StudentName));
        }

        [Authorize(Policy = Permissions.DebtsDormitory.Edit)]
        [HttpPost]
        public IActionResult SetDormitoryDebts(int semesterId, int departmentGroupId, string lastName, string firstName,
            int studentId, enu_DebtorType debtorType, bool activeOnly, bool noRegistrations, List<StudentDebtViewModel> debtList)
        {
            try
            {
                _studentDebtService.SetStudentDebts(debtList);
                TempData["Success"] = "Saved successfully";
            }
            catch (Exception ex)
            {
                TempData["Fail"] = "Failed to save:\n" + ex.Message;
            }

            return RedirectToAction("ManageDormitoryDebts", new
            {
                semesterId = semesterId,
                departmentGroupId = departmentGroupId,
                lastName = lastName,
                firstName = firstName,
                studentId = studentId,
                debtorType = debtorType,
                activeOnly = activeOnly,
                noRegistrations = noRegistrations
            });
        }

        [Authorize(Policy = Permissions.DebtsRegistarOffice.View)]
        public IActionResult ManageRegistarOfficeDebts(int semesterId, int departmentGroupId, string lastName, string firstName,
            int studentId, enu_DebtorType debtorType, bool activeOnly, bool noRegistrations)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            var studentDebts = _studentDebtService.GetStudentDebtList(selectedOrganizationId, enu_DebtType.RegistarOffice, semesterId, departmentGroupId,
                lastName, firstName, studentId, debtorType, activeOnly, noRegistrations);

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), "Id", "SeasonYear", semesterId);
            ViewBag.SemesterId = semesterId;
            ViewBag.DepartmentGroups = new SelectList(_departmentGroupService.GetDepartmentGroups(selectedOrganizationId).OrderBy(x => x.DepartmentCode),
                                                                                            "Id", "DepartmentCode", departmentGroupId);
            ViewBag.DepartmentGroupId = departmentGroupId;
            ViewBag.LastName = lastName;
            ViewBag.FirstName = firstName;
            ViewBag.StudentId = studentId;
            ViewBag.DebtorTypes = debtorType.ToSelectList((int)debtorType);
            ViewBag.DebtorType = debtorType;
            ViewBag.ActiveOnly = activeOnly;
            ViewBag.NoRegistrations = noRegistrations;
            ViewBag.DebtType = enu_DebtType.RegistarOffice;

            return View(studentDebts.OrderBy(x => x.StudentName));
        }

        [Authorize(Policy = Permissions.DebtsRegistarOffice.Edit)]
        [HttpPost]
        public IActionResult SetRegistarOfficeDebts(int semesterId, int departmentGroupId, string lastName, string firstName,
            int studentId, enu_DebtorType debtorType, bool activeOnly, bool noRegistrations, List<StudentDebtViewModel> debtList)
        {
            try
            {
                _studentDebtService.SetStudentDebts(debtList);
                TempData["Success"] = "Saved successfully";
            }
            catch (Exception ex)
            {
                TempData["Fail"] = "Failed to save:\n" + ex.Message;
            }

            return RedirectToAction("ManageRegistarOfficeDebts", new
            {
                semesterId = semesterId,
                departmentGroupId = departmentGroupId,
                lastName = lastName,
                firstName = firstName,
                studentId = studentId,
                debtorType = debtorType,
                activeOnly = activeOnly,
                noRegistrations = noRegistrations
            });
        }

        [Authorize(Policy = Permissions.DebtsMedicineOffice.View)]
        public IActionResult ManageMedicineOfficeDebts(int semesterId, int departmentGroupId, string lastName, string firstName,
            int studentId, enu_DebtorType debtorType, bool activeOnly, bool noRegistrations)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            var studentDebts = _studentDebtService.GetStudentDebtList(selectedOrganizationId, enu_DebtType.MedicineOffice, semesterId, departmentGroupId,
                lastName, firstName, studentId, debtorType, activeOnly, noRegistrations);

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), "Id", "SeasonYear", semesterId);
            ViewBag.SemesterId = semesterId;
            ViewBag.DepartmentGroups = new SelectList(_departmentGroupService.GetDepartmentGroups(selectedOrganizationId).OrderBy(x => x.DepartmentCode),
                                                                                            "Id", "DepartmentCode", departmentGroupId);
            ViewBag.DepartmentGroupId = departmentGroupId;
            ViewBag.LastName = lastName;
            ViewBag.FirstName = firstName;
            ViewBag.StudentId = studentId;
            ViewBag.DebtorTypes = debtorType.ToSelectList((int)debtorType);
            ViewBag.DebtorType = debtorType;
            ViewBag.ActiveOnly = activeOnly;
            ViewBag.NoRegistrations = noRegistrations;
            ViewBag.DebtType = enu_DebtType.MedicineOffice;

            return View(studentDebts.OrderBy(x => x.StudentName));
        }

        [Authorize(Policy = Permissions.DebtsMedicineOffice.Edit)]
        [HttpPost]
        public IActionResult SetMedicineOfficeDebts(int semesterId, int departmentGroupId, string lastName, string firstName,
            int studentId, enu_DebtorType debtorType, bool activeOnly, bool noRegistrations, List<StudentDebtViewModel> debtList)
        {
            try
            {
                _studentDebtService.SetStudentDebts(debtList);
                TempData["Success"] = "Saved successfully";
            }
            catch (Exception ex)
            {
                TempData["Fail"] = "Failed to save:\n" + ex.Message;
            }

            return RedirectToAction("ManageMedicineOfficeDebts", new
            {
                semesterId = semesterId,
                departmentGroupId = departmentGroupId,
                lastName = lastName,
                firstName = firstName,
                studentId = studentId,
                debtorType = debtorType,
                activeOnly = activeOnly,
                noRegistrations = noRegistrations
            });
        }
    }
}
