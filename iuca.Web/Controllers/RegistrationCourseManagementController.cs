using iuca.Application.Constants;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.Interfaces.Roles;
using System.Collections.Generic;
using iuca.Application.DTO.Common;

namespace iuca.Web.Controllers
{
    public class RegistrationCourseManagementController : Controller
    {
        private readonly ISemesterService _semesterService;
        private readonly IOrganizationService _organizationService;
        private readonly IRegistrationCourseService _registrationCourseService;
        private readonly IRegistrationCourseManagementService _registrationCourseManagementService;
        private readonly IEnvarSettingService _envarSettingService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserRolesService _userRolesService;
        private readonly IDeanService _deanService;

        public RegistrationCourseManagementController(ISemesterService semesterService,
            IOrganizationService organizationService,
            IRegistrationCourseService registrationCourseService,
            IRegistrationCourseManagementService registrationCourseManagementService,
            IEnvarSettingService envarSettingService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService,
            IDeanService deanService)
        {
            _semesterService = semesterService;
            _organizationService = organizationService;
            _registrationCourseService = registrationCourseService;
            _registrationCourseManagementService = registrationCourseManagementService;
            _envarSettingService = envarSettingService;
            _userManager = userManager;
            _userRolesService = userRolesService;
            _deanService = deanService;
        }

        public IActionResult TransferCourseStudents()
        {
            int organizationId = _organizationService.GetSelectedOrganization(User);

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(organizationId), "Id", "SeasonYear");
            return View();
        }

        [Authorize(Policy = Permissions.StudentsInSections.View)]
        public IActionResult AnnouncementsForAssigning(int searchSemesterId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semesters = _semesterService.GetSemesters(selectedOrganization);

            if (searchSemesterId == -1)
                searchSemesterId = _envarSettingService.GetUpcomingSemester(selectedOrganization);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", searchSemesterId);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            List<DepartmentDTO> departments = _userRolesService.IsUserInRole(user.Id, selectedOrganization, enu_Role.Dean) ? 
                _deanService.GetDeanDepartments(selectedOrganization, user.Id).Departments : null;

            var announcements = _registrationCourseManagementService.GetAnnouncementsForAssigning(searchSemesterId, departments);

            return View(announcements);
        }

        [Authorize(Policy = Permissions.StudentsInSections.View)]
        public IActionResult AssignStudentsToSections(int announcementId, int searchSemesterId, string courseName)
        {
            var studentsInSecctions = _registrationCourseManagementService.GetSectionsWithStudents(announcementId);

            ViewBag.SemesterId = searchSemesterId == 0 ? -1 : searchSemesterId;
            ViewBag.CourseName = courseName;

            ViewBag.ReturnUrl = HttpContext.Request.Headers["Referer"].ToString();

            return View(studentsInSecctions);
        }

        [Authorize(Policy = Permissions.StudentsInSections.Edit)]
        [HttpPost]
        public IActionResult SetStudentSection(string studentUserId, int oldAnnouncementSectionId, int newAnnouncementSectionId)
        {
            try
            {
                _registrationCourseManagementService.SetStudentSection(studentUserId, oldAnnouncementSectionId, newAnnouncementSectionId);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get registration courses for selection window
        /// </summary>
        /// <param name="semesterId">Semester Id</param>
        /// <returns>Registration courses list</returns>
        public ViewResult GetCoursesForSelection(int semesterId, bool isFrom)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semester = _semesterService.GetSemester(selectedOrganization, semesterId);

            var registrationCourses = _registrationCourseService.GetRegistrationCourses(selectedOrganization, 
                                        semester.Year, semester.Season).OrderBy(x => x.Course.NameEng).ToList();

            ViewBag.IsFrom = isFrom;

            return View("_SelectRegistrationCoursesPartial", registrationCourses);
        }

        /// <summary>
        /// Get registration course students
        /// </summary>
        /// <param name="registrationCourseId">Registration course Id</param>
        /// <returns>Student list</returns>
        public ViewResult GetRegistrationCourseStudents(int registrationCourseId, bool isFrom)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var registrationCourse = _registrationCourseService.GetRegistrationCourse(selectedOrganization, registrationCourseId);

            if (registrationCourse == null)
                throw new Exception("Registration course not found");

            ViewBag.CourseId = registrationCourseId;
            ViewBag.CourseName = $"{registrationCourse.Course.Name} ({registrationCourse.Section}) " +
                $"{registrationCourse.Course.Abbreviation}{registrationCourse.Course.Number} ID: {registrationCourse.Course.ImportCode} " +
                $"credits: {registrationCourse.Points}"; 

            var students = _registrationCourseManagementService.GetRegistrationCourseStudents(registrationCourseId)
                .OrderBy(x => x.Name).ThenBy(x => x.Group).ToList();

            ViewBag.IsFrom = isFrom;

            return View("_CourseStudentRows", students);
        }

        [HttpPost]
        public void SaveTransferCourseStudents(int courseIdFrom, int courseIdTo, string[] transferStudentUserIds)
        {
            _registrationCourseManagementService.SaveTransferCourseStudents(courseIdFrom, courseIdTo, transferStudentUserIds);
        }
    }
}
