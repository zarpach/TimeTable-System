using iuca.Application.Constants;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly IAttendanceService _attendanceService;
        private readonly ISemesterService _semesterService;
        private readonly IEnvarSettingService _envarSettingService;
        private readonly IStudentInfoService _studentInfoService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserRolesService _userRolesService;
        private readonly IAdviserStudentService _adviserStudentService;
        private readonly IAttendanceFolderService _attendanceFolderService;

        public AttendanceController(IOrganizationService organizationService, 
            IAttendanceService attendanceService,
            ISemesterService semesterService,
            IEnvarSettingService envarSettingService,
            IStudentInfoService studentInfoService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService,
            IAdviserStudentService adviserStudentService,
            IAttendanceFolderService attendanceFolderService)
        {
            _organizationService = organizationService;
            _attendanceService = attendanceService;
            _semesterService = semesterService;
            _envarSettingService = envarSettingService;
            _studentInfoService = studentInfoService;
            _userManager = userManager;
            _userRolesService = userRolesService;
            _adviserStudentService = adviserStudentService;
            _attendanceFolderService = attendanceFolderService;
        }

        public IActionResult Index(int searchSemesterId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semesterId = SemesterSelectList(selectedOrganization, searchSemesterId);

            var anouncementSections = _attendanceService.GetAnnouncementSections(semesterId);

            ViewBag.LastParsing = _envarSettingService.GetLastAttendanceUpdate(1);
            ViewBag.MainSpreadsheetLink = _attendanceFolderService.GetAttendanceMainSpreadsheetLink(semesterId);

            return View(anouncementSections);
        }

        public IActionResult Details(int id, int searchSemesterId)
        {
            var attendances = _attendanceService.GetAnnouncementSectionAttendance(id);

            ViewBag.SemesterId = searchSemesterId;
            ViewBag.ReturnUrl = HttpContext.Request.Headers["Referer"].ToString();

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            bool isStudent = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Student);

            if (isStudent)
                attendances = attendances.Where(x => x.StudentUserId == user.Id);

            return View(attendances);
        }

        [Authorize(Policy = Permissions.Attendance.Edit)]
        public IActionResult ParseAttendanceTables()
        {
            try
            {
                int selectedOrganization = _organizationService.GetSelectedOrganization(User);
                int semesterId = _envarSettingService.GetCurrentSemester(selectedOrganization);
                _attendanceService.ParseAttendanceSpreadsheets(semesterId);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Attendance.Edit)]
        public IActionResult ParseAttendanceTable(int announcementId)
        {
            try
            {
                _attendanceService.ParseAttendanceSpreadsheet(announcementId);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Attendance.Edit)]
        public IActionResult GenerateAttendanceTables()
        {
            try
            {
                int selectedOrganization = _organizationService.GetSelectedOrganization(User);
                int semesterId = _envarSettingService.GetCurrentSemester(selectedOrganization);
                _attendanceService.GenerateAttendanceSpreadsheets(semesterId);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Attendance.Edit)]
        public IActionResult GenerateAttendanceTable(int announcementId)
        {
            try
            {
                _attendanceService.GenerateAttendanceSpreadsheet(announcementId);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public IActionResult RedirectToAttendanceSpreadsheet(string attendanceSpreadsheetId)
        {
            string attendanceSpreadsheetLink = _attendanceService.GetSpreadsheetLink(attendanceSpreadsheetId);

            return Redirect(attendanceSpreadsheetLink);
        }

        [Authorize(Policy = Permissions.Attendance.Edit)]
        public IActionResult DeleteAttendanceSpreadsheet(int announcementId)
        {
            try
            {
                _attendanceService.DeleteAttendanceSpreadsheet(announcementId);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Attendance.Edit)]
        public IActionResult DeleteAttendance(int announcementSectionId)
        {
            try
            {
                _attendanceService.DeleteAttendance(announcementSectionId);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Attendance.View)]
        public IActionResult AdviserStudentsAttendanceReport(string adviserUserId, int semesterId, string group)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            adviserUserId = AdviserSelectList(selectedOrganizationId, adviserUserId);
            semesterId = SemesterSelectList(selectedOrganizationId, semesterId);

            var adviserStudentsAttendance = _attendanceService.GetAdvisorStudentsAttendance(adviserUserId, semesterId);

            ViewBag.Groups = new SelectList(adviserStudentsAttendance.OrderBy(x => x.Student.Group).Select(x => x.Student.Group).Distinct(), group);

            if (!string.IsNullOrEmpty(group))
                adviserStudentsAttendance = adviserStudentsAttendance.Where(x => x.Student.Group == group);

            return View(adviserStudentsAttendance);
        }

        public ViewResult StudentCoursesAttendance(string studentUserId, int semesterId, string studentName)
        {
            if (semesterId == 0)
                return View("_StudentCoursesAttendance", null);

            if (string.IsNullOrEmpty(studentUserId))
            {
                ApplicationUser user = _userManager.GetUserAsync(User).Result;
                if (user == null)
                    return View("_StudentCoursesAttendance", null);

                studentUserId = user.Id;
                studentName = $"{user.FirstNameEng} {user.LastNameEng}";
            }

            var studentCoursesAttendance = _attendanceService.GetStudentCoursesAttendance(studentUserId, semesterId);

            ViewBag.StudentName = studentName;

            return View("_StudentCoursesAttendance", studentCoursesAttendance);
        }

        private int SemesterSelectList(int selectedOrganization, int searchSemesterId)
        {
            var semesters = _semesterService.GetSemesters(selectedOrganization);

            if (searchSemesterId == -1)
                searchSemesterId = _envarSettingService.GetCurrentSemester(selectedOrganization);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", searchSemesterId);
            ViewBag.SemesterId = searchSemesterId;

            return searchSemesterId;
        }

        private void StudentSelectList(string studentUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var students = _studentInfoService.GetStudentInfoList(selectedOrganization, new int[] { (int)enu_StudentState.Active }).ToList();

            ViewBag.Students = new SelectList(students, "StudentUserId", "StudentInfo", studentUserId);
            ViewBag.Student = studentUserId;
        }

        private string AdviserSelectList(int selectedOrganization, string adviserUserId)
        {
            var advisers = _adviserStudentService.GetAdvisers(selectedOrganization);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                throw new Exception("User not found");

            bool isAdviser = _userRolesService.IsUserInRole(user.Id, selectedOrganization, enu_Role.Adviser);

            if (isAdviser)
            {
                adviserUserId = user.Id;
                advisers = advisers.Where(x => x.Id == adviserUserId);
            }

            ViewBag.Advisers = new SelectList(advisers.OrderBy(x => x.FullName), "Id", "FullName", adviserUserId);

            return adviserUserId;
        }
    }
}
