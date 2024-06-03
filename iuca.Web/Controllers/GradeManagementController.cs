using iuca.Application.Constants;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Grades;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.UserInfo;
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
    public class GradeManagementController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly ISemesterService _semesterService;
        private readonly IGradeManagementService _gradeManagementService;
        private readonly IGradeService _gradeService;
        private readonly IDepartmentService _departmentService;
        private readonly IEnvarSettingService _envarSettingService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserRolesService _userRolesService;
        private readonly IDeanService _deanService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IAdviserStudentService _adviserStudentService;

        public GradeManagementController(IOrganizationService organizationService,
            ISemesterService semesterService,
            IGradeManagementService gradeManagementService,
            IGradeService gradeService,
            IDepartmentService departmentService,
            IEnvarSettingService envarSettingService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService,
            IDeanService deanService,
            IDepartmentGroupService departmentGroupService,
            IAdviserStudentService adviserStudentService) 
        {
            _organizationService = organizationService;
            _semesterService = semesterService;
            _gradeManagementService = gradeManagementService;
            _gradeService = gradeService;
            _departmentService = departmentService;
            _envarSettingService = envarSettingService;
            _userManager = userManager;
            _userRolesService = userRolesService;
            _deanService = deanService;
            _departmentGroupService = departmentGroupService;
            _adviserStudentService = adviserStudentService;
        }

        [Authorize(Policy = Permissions.GradeManagement.View)]
        public IActionResult GradeReport(int semesterId, int? departmentId, int? courseImportCode, 
            int? studentId, int? gradeId, enu_GradeReportStatus status)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var semesters = _semesterService.GetSemesters(selectedOrganizationId);

            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            var model = _gradeManagementService.GetGradeReport(selectedOrganizationId, semesterId,
                departmentId, courseImportCode, studentId, gradeId, status);

            ViewBag.Departments = new SelectList(_departmentService.GetDepartments(selectedOrganizationId, true), "Id", "Code", departmentId);
            ViewBag.CourseImportCode = courseImportCode;
            ViewBag.StudentId = studentId;

            var grades = _gradeService.GetGrades().OrderBy(x => x.GradeMark);
            ViewBag.GradesFilter = new SelectList(grades, "Id", "GradeMark", gradeId);
            ViewBag.Grades = new SelectList(grades, "Id", "GradeMark");

            ViewBag.SemesterId = semesterId;
            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);

            ViewBag.GradeReportStatuses = new SelectList(Enum.GetValues(typeof(enu_GradeReportStatus)).Cast<enu_GradeReportStatus>()
                .Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList(), "Value", "Text", ((int)status).ToString());

            return View(model.OrderBy(x => x.CourseName));
        }

        [Authorize(Policy = Permissions.GradeManagement.View)]
        public IActionResult DepartmentGradeReport(int semesterId, int? departmentId, int? departmentGroupId,
            bool onlyActiveStudents, int? gradeId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            ViewBag.Semester = semesters.FirstOrDefault(x => x.Id == semesterId).SeasonYear;
            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);

            var departments = _departmentService.GetDepartments(selectedOrganizationId, true).ToList();
            ViewBag.Departments = new SelectList(departments.OrderBy(x => x.Code), "Id", "Code", departmentId);

            var departmentGroups = _departmentGroupService.GetDepartmentGroups(selectedOrganizationId)
                .OrderBy(x => x.DepartmentCode).ToList();
            ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode", departmentGroupId);

            ViewBag.Grades = new SelectList(_gradeService.GetGrades().OrderBy(x => x.GradeMark), "Id", "GradeMark", gradeId);

            ViewBag.OnlyActiveStudents = onlyActiveStudents;

            var model = _gradeManagementService.DepartmentGradeReport(selectedOrganizationId, semesterId,
                departmentId, departmentGroupId, gradeId, onlyActiveStudents);

            return View(model);
        }

        [Authorize(Policy = Permissions.GradeManagement.Edit)]
        [HttpPost]
        public void SetStudentGrade(int announcementSectionId, string studentUserId, int? gradeId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _gradeManagementService.SetStudentGrade(selectedOrganizationId, announcementSectionId, studentUserId, gradeId);
        }

        [Authorize(Policy = Permissions.GradeReport.View)]
        public IActionResult StudentGradesAdviserReport(int semesterId, string adviserUserId,
            int? departmentGroupId, int? gradeId, bool onlyActiveStudents)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetCurrentSemester(selectedOrganizationId);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);
            ViewBag.SemesterId = semesterId;

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            bool isDean = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean);

            ViewBag.IsDean = isDean;

            if (isDean)
                ViewBag.Advisers = new SelectList(_deanService.GetAdvisersByDeanUserId(selectedOrganizationId, user.Id),
                    "Id", "FullNameEng", adviserUserId);
            else
                adviserUserId = user.Id;

            var departmentroups = _departmentGroupService.GetDepartmentGroups(selectedOrganizationId)
                .OrderBy(x => x.DepartmentCode);

            var departmentGroups = _adviserStudentService.GetAdviserDepartmentGroupsByInstuctorId(selectedOrganizationId, adviserUserId)
                .OrderBy(x => x.DepartmentCode);
            ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode", departmentGroupId);
            ViewBag.DepartmentGroupId = departmentGroupId;

            ViewBag.Grades = new SelectList(
                _gradeService.GetGrades().OrderBy(x => x.GradeMark), "Id", "GradeMark", gradeId);
            ViewBag.GradeId = gradeId;

            ViewBag.OnlyActiveStudents = onlyActiveStudents;

            var model = _gradeManagementService.GradeAdviserReport(selectedOrganizationId,
                semesterId, adviserUserId, departmentGroupId, gradeId, onlyActiveStudents);
            
            model.AllStudents = model.AllStudents.OrderBy(x => x.Group).ThenBy(x => x.Name).ToList();

            return View(model);
        }

        [Authorize(Policy = Permissions.GradeReport.View)]
        public IActionResult StudentGradesAdviserReportPrint(int semesterId, string adviserUserId,
            int? departmentGroupId, int? gradeId, bool onlyActiveStudents)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var model = _gradeManagementService.GradeAdviserReport(selectedOrganizationId,
                semesterId, adviserUserId, departmentGroupId, gradeId, onlyActiveStudents);

            model.AllStudents = model.AllStudents.OrderBy(x => x.Group).ThenBy(x => x.Name).ToList();

            ViewBag.Semester = _semesterService.GetSemester(selectedOrganizationId, semesterId).SeasonYear;
            ViewBag.DepartmentGroupNames = String.Join(", ", model.AllStudents.Select(x => x.Group).Distinct());

            return View(model);
        }

        public JsonResult GetAdviserDepartmentGroups(string adviserUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return Json(new SelectList(_adviserStudentService.GetAdviserDepartmentGroupsByInstuctorId(selectedOrganizationId, adviserUserId)
                .OrderBy(x => x.DepartmentCode), "Id", "DepartmentCode"));
        }

        [Authorize(Policy = Permissions.FFXXReport.View)]
        public IActionResult FFXXReport(bool onlyActiveStudents)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var departmentIds = GetDepartmentIds(selectedOrganizationId);

            ViewBag.OnlyActiveStudents = onlyActiveStudents;

            return View(_gradeManagementService.FFXXReport(selectedOrganizationId, departmentIds, onlyActiveStudents));
        }

        private List<int> GetDepartmentIds(int selectedOrganizationId) 
        {
            List<int> departmentIds = new List<int>();
            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user is null)
                throw new Exception("User not found");

            if (_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean))
            {
                var deanDepartments = _deanService.GetDeanDepartments(selectedOrganizationId, user.Id);
                if (deanDepartments != null)
                    departmentIds = deanDepartments.Departments.Select(x => x.Id).ToList();
            }

            return departmentIds;
        }
    }
}
