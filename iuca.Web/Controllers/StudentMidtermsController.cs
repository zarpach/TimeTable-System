using iuca.Application.Constants;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels.Courses;
using iuca.Application.ViewModels.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class StudentMidtermsController : Controller
    {
        private readonly IApplicationDbContext _db;
        private readonly IOrganizationService _organizationService;
        private readonly IStudentMidtermService _studentMidtermService;
        private readonly IGradeService _gradeService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserRolesService _userRolesService;
        private readonly ISemesterPeriodService _semesterPeriodService;
        private readonly ISemesterService _semesterService;
        private readonly IDepartmentService _departmentService;
        private readonly IInstructorInfoService _instructorInfoService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IDeanService _deanService;
        private readonly IAdviserStudentService _adviserStudentService;
        private readonly IEnvarSettingService _envarSettingService;

        public StudentMidtermsController(IApplicationDbContext db,
            IOrganizationService organizationService,
            IStudentMidtermService studentMidtermService,
            IGradeService gradeService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService,
            ISemesterPeriodService semesterPeriodService,
            ISemesterService semesterService,
            IDepartmentService departmentService,
            IInstructorInfoService instructorInfoService,
            IDepartmentGroupService departmentGroupService,
            IDeanService deanService,
            IAdviserStudentService adviserStudentService,
            IEnvarSettingService envarSettingService)
        {
            _db = db;
            _organizationService = organizationService;
            _studentMidtermService = studentMidtermService;
            _gradeService = gradeService;
            _userManager = userManager;
            _userRolesService = userRolesService;
            _semesterPeriodService = semesterPeriodService;
            _semesterService = semesterService;
            _departmentService = departmentService;
            _instructorInfoService = instructorInfoService;
            _departmentGroupService = departmentGroupService;
            _deanService = deanService;
            _adviserStudentService = adviserStudentService;
            _envarSettingService = envarSettingService;
        }

        [Authorize(Policy = Permissions.InstructorCourses.View)]
        public IActionResult Index(int semesterId, int registrationCourseId, string instructorUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            bool isAdmin = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Admin);
            bool isDean = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean);

            var registrationCourse = _db.AnnouncementSections.Include(x => x.Course)
                .Include(x => x.ExtraInstructors).FirstOrDefault(x => x.Id == registrationCourseId);

            if (!isAdmin && !isDean && !(registrationCourse.InstructorUserId == user.Id ||
                    registrationCourse.ExtraInstructors != null && 
                    registrationCourse.ExtraInstructors.Select(s => s.InstructorUserId).Contains(user.Id) ||
                    registrationCourse.ExtraInstructorsJson != null && registrationCourse.ExtraInstructorsJson.Contains(user.Id)))
                throw new Exception("Wrong instructor user id");

            var students = _studentMidtermService.GetStudentMidterms(selectedOrganizationId, registrationCourseId)
                .OrderBy(x => x.StudentName).ToList();

            ViewBag.CourseName = $"{registrationCourse.Course.NameEng} / {registrationCourse.Course.NameRus} / " +
                $"{registrationCourse.Course.NameKir} ({registrationCourse.Section})";
            ViewBag.SemesterId = semesterId;
            ViewBag.InstructorUserId = instructorUserId;
            
            var semesterPeriod = _semesterPeriodService.GetSemesterPeriod(selectedOrganizationId, (int)enu_Period.Midterm, semesterId);
            ViewBag.IsPeriodEnabled = semesterPeriod != null && semesterPeriod.IsEnabed();

            FillSelectLists();

            return View(students);
        }

        [Authorize(Policy = Permissions.InstructorCourses.Edit)]
        [HttpPost]
        public PartialViewResult SaveStudentMidterm(int semesterId, int num, StudentMidtermViewModel model) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var isPeriodEnabled = false;

            try
            {
                var semesterPeriod = _semesterPeriodService.GetSemesterPeriod(selectedOrganizationId, (int)enu_Period.Midterm, semesterId);
                isPeriodEnabled = semesterPeriod != null && semesterPeriod.IsEnabed();
                if (!isPeriodEnabled)
                    throw new ModelValidationException("Midterm period is closed", "");

                if (ModelState.IsValid)
                {
                    if (model.StudentMidterm.Id != 0)
                        _studentMidtermService.Edit(model.StudentMidterm.Id, model.StudentMidterm);
                    else
                        model.StudentMidterm.Id = _studentMidtermService.Create(model.StudentMidterm);
                    TempData["SuccessMessage"] = "Saved successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Saving failed";
                }
            }
            catch (ModelValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                TempData["ErrorMessage"] = $"Saving failed: {ex.Message}";
            }

            ViewData["Num"] = num;
            ViewBag.SemesterId = semesterId;
            ViewBag.IsPeriodEnabled = isPeriodEnabled;

            FillSelectLists();

            return PartialView("_EditStudentMidtermPartial", model);
        }

        [Authorize(Policy = Permissions.StudentMidterms.View)]
        [HttpPost]
        public IActionResult SetAdviserComment(int studentMidtermId, string adviserComment)
        {
            try
            {
                _studentMidtermService.SetStudentMidtermAdviserComment(studentMidtermId, adviserComment);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private void FillSelectLists() 
        {
            List<string> allowedGradeMarks = new List<string> { "A", "A-", "B+", "B", "B-", "C+", "C", "C-", "D+", "D", "D-", "F", "I" };
            var grades = _gradeService.GetGrades().Where(x => allowedGradeMarks.Contains(x.GradeMark)).OrderBy(x => x.GradeMark).ToList();
            ViewBag.Grades = new SelectList(grades, "Id", "GradeMark");
        }

        [Authorize(Policy = Permissions.StudentMidterms.View)]
        public IActionResult MidtermStatisticsReport(int semesterId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetCurrentSemester(selectedOrganizationId);

            ViewBag.SemesterId = semesterId;
            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);

            var model = _studentMidtermService.MidtermStatisticsReport(selectedOrganizationId, semesterId);
            return View(model);
        }

        [Authorize(Policy = Permissions.StudentMidterms.View)]
        public IActionResult MidtermStatisticsDetailedReport(int semesterId, int? departmentId, 
            string instructorUserId, int? courseId, enu_MidtermReportState state = enu_MidtermReportState.NotSelected)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetCurrentSemester(selectedOrganizationId);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);
            ViewBag.Departments = new SelectList(_departmentService.GetDepartments(selectedOrganizationId, true), "Id", "Code", departmentId);

            var instructors = _instructorInfoService.GetInstructorInfoList(selectedOrganizationId, enu_InstructorState.Active, null);
            ViewBag.Instructors = new SelectList(instructors, "InstructorUserId", "FullNameEng", instructorUserId);
            ViewBag.CourseId = courseId;
            ViewBag.States = new SelectList(Enum.GetValues(typeof(enu_MidtermReportState)).Cast<enu_MidtermReportState>().OrderBy(x => (int)x)
                .Select(v => new SelectListItem
                {
                    Text = EnumExtentions.GetDisplayName(v),
                    Value = ((int)v).ToString()
                }).ToList(), "Value", "Text", ((int)state).ToString());

            var model = _studentMidtermService.MidtermStatisticsDetailedReport(selectedOrganizationId, semesterId,
                departmentId, instructorUserId, courseId, state);
            return View(model);
        }

        [Authorize(Policy = Permissions.StudentMidterms.View)]
        public IActionResult MidtermAdviserReport(int semesterId, string adviserUserId, 
            int? departmentGroupId, enu_MidtermReportSorting sorting, bool onlyActiveStudents)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetCurrentSemester(selectedOrganizationId);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);

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

            ViewBag.DepartmentGroups = new SelectList(
                _adviserStudentService.GetAdviserDepartmentGroupsByInstuctorId(selectedOrganizationId, adviserUserId)
                .OrderBy(x => x.DepartmentCode), "Id", "DepartmentCode", departmentGroupId);

            ViewBag.Sorting = new SelectList(Enum.GetValues(typeof(enu_MidtermReportSorting)).Cast<enu_MidtermReportSorting>()
                .Select(v => new SelectListItem
                {
                    Text = v.GetDisplayName(),
                    Value = ((int)v).ToString()
                }).ToList(), "Value", "Text", ((int)sorting).ToString());

            ViewBag.OnlyActiveStudents = onlyActiveStudents;

            var model = _studentMidtermService.MidtermAdviserReport(selectedOrganizationId, 
                semesterId, adviserUserId, departmentGroupId, onlyActiveStudents);

            if (sorting == enu_MidtermReportSorting.Group)
                model.AllStudents = model.AllStudents.OrderBy(x => x.StudentInfo.Group).ThenBy(x => x.StudentInfo.Name).ToList();
            else if (sorting == enu_MidtermReportSorting.StudentName)
                model.AllStudents = model.AllStudents.OrderBy(x => x.StudentInfo.Name).ToList();
            else if (sorting == enu_MidtermReportSorting.Attention)
                model.AllStudents = model.AllStudents.OrderByDescending(x => x.AttentionCount)
                    .ThenBy(x => x.StudentInfo.Group).ThenBy(x => x.StudentInfo.Name).ToList();

            return View(model);
        }

        public JsonResult GetAdviserDepartmentGroups(string adviserUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return Json(new SelectList(_adviserStudentService.GetAdviserDepartmentGroupsByInstuctorId(selectedOrganizationId, adviserUserId)
                .OrderBy(x => x.DepartmentCode), "Id", "DepartmentCode"));
        }

        [Authorize(Policy = Permissions.StudentMidterms.View)]
        public IActionResult MidtermStudentReport(int semesterId, string studentUserId, int? courseId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);

            var adviserStudents = new List<AdviserStudentViewModel>();

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            bool isDean = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean);
            if (isDean)
            {
                var adviserIds = _deanService.GetDeanAdvisers(selectedOrganizationId, user.Id)
                    .Select(x => x.Instructor.Id)
                    .ToList();
                adviserStudents = _adviserStudentService.GetAdviserStudentsByInstuctorId(selectedOrganizationId, adviserIds);
            }
            else if (_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Adviser))
            {
                adviserStudents = _adviserStudentService.GetAdviserStudentsByInstuctorId(selectedOrganizationId, user.Id);
            }

            ViewBag.Students = new SelectList(adviserStudents.OrderBy(x => x.Group).ThenBy(x => x.Name)
                .Select(x => new { Name =  $"{x.Group} - {x.Name} - {x.StudentId}", Id = x.StudentUserId }), "Id", "Name", studentUserId);
            ViewBag.CourseId = courseId;
            
            var model = _studentMidtermService.MidtermStudentReport(selectedOrganizationId,
                semesterId, studentUserId, courseId);

            return View(model);
        }
    }
}
