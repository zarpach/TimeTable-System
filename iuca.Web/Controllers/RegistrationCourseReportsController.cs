using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Reports;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.Services.Reports;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class RegistrationCourseReportsController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly ISemesterService _semesterService;
        private readonly IStudentRegistrationReportService _studentRegistrationReportService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserRolesService _userRolesService;
        private readonly IUserInfoService _userInfoService;
        private readonly IDepartmentService _departmentService;
        private readonly IDeanService _deanService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IAdviserStudentService _adviserStudentService;
        private readonly IEnvarSettingService _envarSettingService;

        public RegistrationCourseReportsController(IOrganizationService organizationService,
            ISemesterService semesterService,
            IStudentRegistrationReportService studentRegistrationReportService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService,
            IUserInfoService userInfoService,
            IDepartmentService departmentService,
            IDeanService deanService,
            IDepartmentGroupService departmentGroupService,
            IAdviserStudentService adviserStudentService,
            IEnvarSettingService envarSettingService)
        {
            _organizationService = organizationService;
            _semesterService = semesterService;
            _studentRegistrationReportService = studentRegistrationReportService;
            _userManager = userManager;
            _userRolesService = userRolesService;
            _userInfoService = userInfoService;
            _departmentService = departmentService;
            _deanService = deanService;
            _departmentGroupService = departmentGroupService;
            _adviserStudentService = adviserStudentService;
            _envarSettingService = envarSettingService;
        }

        [Authorize(Policy = Permissions.RegistrationCoursesReport.View)]
        public IActionResult StudentRegistrationsReport(int semesterId, string deanUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
                return NotFound();

            bool isAdmin = false;
            if (_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean))
            {
                deanUserId = user.Id;
                ViewBag.DeanName = user.FullNameEng;
            }
            else 
            {
                isAdmin = true;
                ViewBag.DeanName = _userManager.Users.FirstOrDefault(x => x.Id == deanUserId)?.FullNameEng;
                ViewBag.Deans = _userInfoService.GetUserSelectList(selectedOrganizationId, enu_Role.Dean, deanUserId);
            }
            ViewBag.IsAdmin = isAdmin;

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            ViewBag.SemesterId = semesterId;
            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);

            var model = _studentRegistrationReportService.GetStudentRegistrationsReport(selectedOrganizationId, semesterId, deanUserId);
            return View(model);
        }

        [Authorize(Policy = Permissions.RegistrationCoursesDetailedReport.View)]
        public IActionResult StudentRegistrationsDetailedReport(int semesterId, string deanUserId, 
            int? departmentId, enu_RegistrationState registrationState)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
                return NotFound();

            bool isAdmin = false;
            if (_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean))
            {
                deanUserId = user.Id;
                ViewBag.DeanName = user.FullNameEng;
            }
            else
            {
                isAdmin = true;
                ViewBag.DeanName = _userManager.Users.FirstOrDefault(x => x.Id == deanUserId)?.FullNameEng;
                ViewBag.Deans = _userInfoService.GetUserSelectList(selectedOrganizationId, enu_Role.Dean, deanUserId);
            }
            ViewBag.IsAdmin = isAdmin;

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);

            var departments = new List<DepartmentDTO>();
            if (isAdmin)
                departments = _departmentService.GetDepartments(selectedOrganizationId, true).ToList();
            else 
                departments = _deanService.GetDeanDepartments(selectedOrganizationId, deanUserId).Departments.ToList();
            ViewBag.Departments = new SelectList(departments.OrderBy(x => x.Code), "Id", "Code", departmentId);

            ViewBag.RegistrationStates = new SelectList(Enum.GetValues(typeof(enu_RegistrationState)).Cast<enu_RegistrationState>()
                .Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList(), "Value", "Text", ((int)registrationState).ToString());

            var model = _studentRegistrationReportService.GetStudentRegistrationsDetailedReport(selectedOrganizationId, 
                semesterId, deanUserId, departmentId, registrationState);
            return View(model);
        }

        public IActionResult DeanAdviserStudentReport(string deanUserId, string adviserUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
                return NotFound();

            ViewBag.IsAdmin = false;
            ViewBag.IsDean = false;
            ViewBag.IsAdviser = false;

            if (_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Adviser))
            {
                ViewBag.IsAdviser = true;
                if (string.IsNullOrEmpty(adviserUserId))
                    adviserUserId = user.Id;
            }
            else if (_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean))
            {
                ViewBag.IsDean = true;
                if (string.IsNullOrEmpty(deanUserId))
                    deanUserId = user.Id;
                var advisers = _deanService.GetAdvisersByDeanUserId(selectedOrganizationId, deanUserId);
                ViewBag.Advisers = new SelectList(advisers, "Id", "FullNameEng", adviserUserId).OrderBy(x => x.Text);
            }
            else if (_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Admin))
            {
                ViewBag.IsAdmin = true;
                var deans = _userInfoService.GetUserSelectList(selectedOrganizationId, enu_Role.Dean, deanUserId);
                ViewBag.Deans = deans;
                var advisers = new List<ApplicationUser>();
                if (deans.Count > 0)
                    advisers = _deanService.GetAdvisersByDeanUserId(selectedOrganizationId, deanUserId ?? deans.FirstOrDefault().Value);
                ViewBag.Advisers = new SelectList(advisers, "Id", "FullNameEng", adviserUserId).OrderBy(x => x.Text);
            }

            ViewBag.UserId = user.Id;

            var model = _studentRegistrationReportService.DeanAdviserStudentsReport(selectedOrganizationId, deanUserId, adviserUserId);

            return View(model);
        }

        public IActionResult StudentsWithoutAdviserReport(string deanUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user =  _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
                return NotFound();

            ViewBag.IsAdmin = false;
            ViewBag.IsDean = false;

            if (_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean))
            {
                ViewBag.IsDean = true;
                if (string.IsNullOrEmpty(deanUserId))
                    deanUserId = user.Id;
            }
            else if (_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Admin))
            {
                ViewBag.IsAdmin = true;
                var deans = _userInfoService.GetUserSelectList(selectedOrganizationId, enu_Role.Dean, deanUserId);
                ViewBag.Deans = deans;
            }

            ViewBag.UserId = user.Id;

            var model = _studentRegistrationReportService.StudentsWithoutAdviser(selectedOrganizationId, deanUserId);

            return View(model);
        }

        public JsonResult GetDeanAdvisers(string deanUserId, string adviserUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            return Json(new SelectList(_deanService.GetAdvisersByDeanUserId(selectedOrganizationId,
                deanUserId), "Id", "FullNameEng").OrderBy(x => x.Text), adviserUserId);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.View)]
        public IActionResult CourseRegistrationAdviserReport(int semesterId, string adviserUserId,
            int? departmentGroupId, bool onlyActiveStudents)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            bool isAdmin = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Admin);
            ViewBag.IsAdmin = isAdmin;

            if (isAdmin) 
            {
                var advisers = _adviserStudentService.GetAdvisers(selectedOrganizationId).OrderBy(x => x.FullName).ToList();
                ViewBag.Advisers = new SelectList(advisers, "Id", "FullName", adviserUserId);
                if (string.IsNullOrEmpty(adviserUserId) && advisers.Any())
                    adviserUserId = advisers.FirstOrDefault().Id;
            }
            else
            {
                bool isDean = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean);
                ViewBag.IsDean = isDean;

                if (isDean) 
                {
                    var advisers = _deanService.GetAdvisersByDeanUserId(selectedOrganizationId, user.Id)
                        .OrderBy(x => x.FullNameEng).ToList();
                    ViewBag.Advisers = new SelectList(advisers, "Id", "FullNameEng", adviserUserId);
                    if (string.IsNullOrEmpty(adviserUserId) && advisers.Any())
                        adviserUserId = advisers.FirstOrDefault().Id;
                }
                else
                    adviserUserId = user.Id;
            }

            var departmentGroups = _adviserStudentService.GetAdviserDepartmentGroupsByInstuctorId(selectedOrganizationId, adviserUserId)
                .OrderBy(x => x.DepartmentCode);

            if (departmentGroupId == null)
            {
                var departmentGroup = departmentGroups.FirstOrDefault();
                if (departmentGroup != null)
                    departmentGroupId = departmentGroup.Id;
                else
                    departmentGroupId = 0;
            }

            ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode", departmentGroupId);
            ViewBag.OnlyActiveStudents = onlyActiveStudents;

            var model = _studentRegistrationReportService.CourseRegistrationAdviserReport(selectedOrganizationId, semesterId, adviserUserId,
                departmentGroupId.Value, onlyActiveStudents);

            return View(model);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.View)]
        public JsonResult GetAdviserDepartmentGroups(string adviserUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return Json(new SelectList(_adviserStudentService.GetAdviserDepartmentGroupsByInstuctorId(selectedOrganizationId, adviserUserId)
                .OrderBy(x => x.DepartmentCode), "Id", "DepartmentCode"));
        }

        [Authorize(Policy = Permissions.RegistrationCoursesReport.View)]
        public IActionResult RegistrationCoursesReport(int semesterId, int? departmentId, 
            List<enu_CourseType> courseTypes, int maxQty)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            if (courseTypes.Count == 0)
                courseTypes = Enum.GetValues(typeof(enu_CourseType)).Cast<enu_CourseType>().ToList();

            ViewBag.SemesterId = semesterId;
            ViewBag.Semester = _semesterService.GetSemester(semesterId).SeasonYear;
            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);
            ViewBag.DepartmentId = departmentId;
            ViewBag.Departments = new SelectList(_departmentService.GetDepartments(selectedOrganizationId, true)
                .OrderBy(x => x.Code), "Id", "Code", departmentId);
            ViewBag.CourseTypes = courseTypes;
            ViewBag.MaxQty = maxQty;

            var model = _studentRegistrationReportService.GetRegistrationCoursesReport(semesterId, departmentId, courseTypes, maxQty);
            return View(model);
        }

        [Authorize(Policy = Permissions.RegistrationCoursesReport.View)]
        public IActionResult PrintRegistrationCoursesReport(int semesterId, int? departmentId, 
            List<enu_CourseType> courseTypes, int maxQty)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            ViewBag.Semester = _semesterService.GetSemester(semesterId).SeasonYear;

            var model = _studentRegistrationReportService.GetRegistrationCoursesReport(semesterId, departmentId, courseTypes, maxQty);
            return View(model);
        }
    }
}
