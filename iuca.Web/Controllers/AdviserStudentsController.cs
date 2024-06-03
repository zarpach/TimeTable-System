using iuca.Application.Constants;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels.Courses;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class AdviserStudentsController : Controller
    {
        private readonly IAdviserStudentService _adviserStudentService;
        private readonly IOrganizationService _organizationService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IInstructorBasicInfoService _instructorBasicInfoService;
        private readonly IUserRolesService _userRolesService;
        private readonly IDeanService _deanService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserInfoService _userInfoService;
        private readonly ISemesterService _semesterService;
        private readonly IEnvarSettingService _envarSettingService;

        public AdviserStudentsController(IAdviserStudentService adviserStudentService,
            IOrganizationService organizationService,
            IDepartmentGroupService departmentGroupService,
            IInstructorBasicInfoService instructorBasicInfoService,
            IUserRolesService userRolesService,
            IDeanService deanService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserInfoService userInfoService,
            ISemesterService semesterService,
            IEnvarSettingService envarSettingService)
        {
            _adviserStudentService = adviserStudentService;
            _organizationService = organizationService;
            _departmentGroupService = departmentGroupService;
            _instructorBasicInfoService = instructorBasicInfoService;
            _userRolesService = userRolesService;
            _deanService = deanService;
            _userManager = userManager;
            _userInfoService = userInfoService;
            _semesterService = semesterService;
            _envarSettingService = envarSettingService;
        }

        
        [Authorize(Policy = Permissions.Advisers.Edit)]
        public IActionResult EditAdviserStudents(string deanUserId, string instructorUserId) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var user = _userManager.Users.FirstOrDefault(x => x.Id == instructorUserId);
            if (user == null)
                throw new Exception("User not found");
            
            ViewBag.AdviserName = user.FullNameEng;
            ViewBag.InstructorUserId = instructorUserId;
            ViewBag.OrganizationId = selectedOrganizationId;
            ViewBag.DeanUserId = deanUserId;

            return View(_adviserStudentService.GetAdviserStudentsByInstuctorId(selectedOrganizationId, instructorUserId)
                .OrderBy(x => x.Name).ToList());
        }

        [Authorize(Policy = Permissions.Advisers.Edit)]
        [HttpPost]
        public IActionResult EditAdviserStudents(string deanUserId, string instructorUserId, string[] studentUserIds)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _adviserStudentService.EditAdviserStudents(instructorUserId, selectedOrganizationId, studentUserIds.ToList());
                    return RedirectToAction("DeanAdvisers", "Deans", new { deanUserId = deanUserId });
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View("EditAdviserStudents", new { instructorUserId = instructorUserId, studentUserIds = studentUserIds });
        }

        [Authorize(Policy = Permissions.Advisers.Edit)]
        /// <summary>
        /// Get students for selection window
        /// </summary>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="excludedIds">Id of students that must be excluded</param>
        /// <returns>Students with recommendations</returns>
        public ViewResult GetStudentsForSelection(string deanUserId, int departmentGroupId, string[] excludedIds)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var departments = _deanService.GetDeanDepartments(selectedOrganization, deanUserId);

            var students = _adviserStudentService.GetStudentsForSelection(departments.Departments.Select(x => x.Id).ToArray(),
                departmentGroupId, selectedOrganization, excludedIds)
                .OrderBy(x => x.FullNameEng).ToList();

            ViewData["OrganizationId"] = selectedOrganization;
            ViewBag.DepartmentGroups = new SelectList(students.GroupBy(x => x.Group)
                .Select(x => new { Group = x.Key }).OrderBy(x => x.Group).ToList(),
                    "Group", "Group");

            return View("_SelectStudentsPartial", students);
        }

        [Authorize(Policy = Permissions.Advisers.Edit)]
        public ViewResult GetStudentsFromSelection(int organizationId, string[] studentUserIds)
        {
            return View("_StudentRow", _adviserStudentService.GetStudentsFromSelection(organizationId, studentUserIds));
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.View)]
        public IActionResult StudentRegistrations(int? departmentGroupId, int semesterId, bool activeOnly, bool withDebtsOnly, bool incompleteRegistration) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            ApplicationUser user = null;
            Task.Run(() => user = _userManager.GetUserAsync(User).GetAwaiter().GetResult()).Wait();
            if (user == null)
                return NotFound();

            var registrations = _adviserStudentService.GetAdviserStudentRegistrations(selectedOrganizationId, user.Id, 
                departmentGroupId, semesterId, activeOnly, withDebtsOnly, incompleteRegistration).ToList();

            ViewBag.ActiveOnly = activeOnly;
            ViewBag.WithDebtsOnly = withDebtsOnly;
            ViewBag.IncompleteRegistration = incompleteRegistration;
            var adviserStudents = _adviserStudentService.GetAdviserStudentsByInstuctorId(selectedOrganizationId, user.Id);
            var departmentGroupIds = adviserStudents.Select(x => x.DepartmentGroupId).Distinct().ToList();

            departmentGroupIds.AddRange(adviserStudents.Where(x => x.PrepDepartmentGroupId != null)
                .Select(x => x.PrepDepartmentGroupId.Value).Distinct().ToList());

            var departmentGroups = _departmentGroupService.GetDepartmentGroups(selectedOrganizationId)
                .Where(x => departmentGroupIds.Contains(x.Id)).OrderBy(x => x.DepartmentCode).ToList();

            ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode", departmentGroupId);
            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId)
                .OrderByDescending(x => x.Year).ThenBy(x => x.Season), "Id", "SeasonYear", semesterId);
            ViewBag.SemesterId = semesterId;

            return View(registrations);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.View)]
        public IActionResult CheckStudentCourses(int studentCourseRegistrationId) 
        {
           return View(_adviserStudentService.GetStudentRegistrationCheckModel(studentCourseRegistrationId));
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult CheckStudentCourses(CheckRegistrationPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _adviserStudentService.CheckStudentRegistration(model);
                    return RedirectToAction("StudentRegistrations", 
                        new { semesterId = model.SemesterId, activeOnly = true});
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View("CheckStudentCourses", new { studentCourseRegistrationId = model.StudentCourseRegistrationId });
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.View)]
        public IActionResult CheckStudentAddDropCourses(int studentCourseRegistrationId)
        {
            return View(_adviserStudentService.GetStudentAddDropCheckModel(studentCourseRegistrationId));
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult CheckStudentAddDropCourses(CheckRegistrationPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _adviserStudentService.CheckStudentAddDropForm(model);
                    return RedirectToAction("StudentRegistrations", 
                        new { semesterId = model.SemesterId, activeOnly = true });
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return View("CheckStudentAddDropCourses", new { studentCourseRegistrationId = model.StudentCourseRegistrationId });
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        public IActionResult ViewStudentCourses(int studentCourseRegistrationId)
        {
            return View(_adviserStudentService.GetStudentRegistrationCheckModel(studentCourseRegistrationId));
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        public IActionResult ViewStudentAddDropCourses(int studentCourseRegistrationId)
        {
            return View(_adviserStudentService.GetStudentAddDropCheckModel(studentCourseRegistrationId));
        }

    }
}
