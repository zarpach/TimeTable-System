using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.Services.Common;
using iuca.Application.Services.Users.UserInfo;
using iuca.Application.ViewModels.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class StudentTranscriptsController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IStudentInfoService _studentInfoService;
        private readonly IStudentTranscriptService _studentTranscriptService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IStudentBasicInfoService _studentBasicInfoService;
        private readonly IStudentOrgInfoService _studentOrgInfoService;
        private readonly IInstructorBasicInfoService _instructorBasicInfoService;
        private readonly IInstructorOrgInfoService _instructorOrgInfoService;
        private readonly IDeanService _deanDepartmentService;
        private readonly IAdviserStudentService _adviserStudentService;
        private readonly IUserRolesService _userRolesService;
        private readonly ISemesterService _semesterService;
        private readonly IEnvarSettingService _envarSettingService;
        private readonly IDepartmentService _departmentService;

        public StudentTranscriptsController(IOrganizationService organizationService,
            IDepartmentGroupService departmentGroupService,
            IStudentInfoService studentInfoService,
            IStudentTranscriptService studentTranscriptService,
            ApplicationUserManager<ApplicationUser> userManager,
            IStudentBasicInfoService studentBasicInfoService,
            IStudentOrgInfoService studentOrgInfoService,
            IInstructorBasicInfoService instructorBasicInfoService,
            IInstructorOrgInfoService instructorOrgInfoService,
            IDeanService deanDepartmentService,
            IAdviserStudentService adviserStudentService,
            IUserRolesService userRolesService,
            ISemesterService semesterService,
            IEnvarSettingService envarSettingService,
            IDepartmentService departmentService)
        {
            _organizationService = organizationService;
            _departmentGroupService = departmentGroupService;
            _studentInfoService = studentInfoService;
            _studentTranscriptService = studentTranscriptService;
            _userManager = userManager;
            _studentBasicInfoService = studentBasicInfoService;
            _studentOrgInfoService = studentOrgInfoService;
            _instructorBasicInfoService = instructorBasicInfoService;
            _instructorOrgInfoService = instructorOrgInfoService;
            _deanDepartmentService = deanDepartmentService;
            _adviserStudentService = adviserStudentService;
            _userRolesService = userRolesService;
            _semesterService = semesterService;
            _envarSettingService = envarSettingService;
            _departmentService = departmentService;
        }

        [Authorize(Policy = Permissions.TranscriptsAdmin.View)]
        public IActionResult Index(int departmentGroupId, string lastName, string firstName, int studentId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var students = _studentInfoService.GetStudentSelectList(selectedOrganizationId, new List<int>(), departmentGroupId,
                lastName, firstName, studentId, false).OrderBy(x => x.FullNameEng).ToList();

            ViewBag.DepartmentGroups = new SelectList(_departmentGroupService.GetDepartmentGroups(selectedOrganizationId).OrderBy(x => x.DepartmentCode),
                    "Id", "DepartmentCode", departmentGroupId);

            ViewBag.LastName = lastName;
            ViewBag.FirstName = firstName;
            ViewBag.StudentId = studentId;

            return View(students);
        }

        [Authorize(Policy = Permissions.TranscriptsAdmin.View)]
        public IActionResult GetStudentTranscript(string studentUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_studentTranscriptService.GetTranscript(selectedOrganizationId, studentUserId));
        }

        [Authorize(Policy = Permissions.TranscriptsStudent.View)]
        public IActionResult StudentTranscript()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = null;
            Task.Run(() => user = _userManager.GetUserAsync(User).GetAwaiter().GetResult()).Wait();
            if (user == null)
                return NotFound();

            return View(_studentTranscriptService.GetTranscript(selectedOrganizationId, user.Id));
        }

        [Authorize(Policy = Permissions.TranscriptsDean.View)]
        public IActionResult StudentTranscriptsForDean(int departmentGroupId, string lastName, string firstName, int studentId,
            bool onlyActive)
        {
            List<SelectStudentViewModel> students = new List<SelectStudentViewModel>();

            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            var departmentIds = _deanDepartmentService.GetDeanDepartments(selectedOrganizationId, user.Id)
                .Departments.Select(x => x.Id).ToList();

            if (departmentIds.Count != 0) 
            {
                students = _studentInfoService.GetStudentSelectList(selectedOrganizationId, departmentIds, departmentGroupId,
                    lastName, firstName, studentId, onlyActive).OrderBy(x => x.FullNameEng).ToList();

                ViewBag.DepartmentGroups = new SelectList(_departmentGroupService.GetDepartmentGroups(selectedOrganizationId)
                    .Where(x => departmentIds.Contains(x.DepartmentId)).OrderBy(x => x.DepartmentCode),
                        "Id", "DepartmentCode", departmentGroupId);
            }

            ViewBag.LastName = lastName;
            ViewBag.FirstName = firstName;
            ViewBag.StudentId = studentId;
            ViewBag.OnlyActive = onlyActive;

            return View(students);
        }

        [Authorize(Policy = Permissions.TranscriptsDean.View)]
        public IActionResult GetStudentTranscriptForDean(string studentUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = null;
            Task.Run(() => user = _userManager.GetUserAsync(User).GetAwaiter().GetResult()).Wait();
            if (user == null)
                return NotFound();

            var departmentIds = _deanDepartmentService.GetDeanDepartments(selectedOrganizationId, user.Id)
                .Departments.Select(x => x.Id).ToList();

            var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(selectedOrganizationId, studentUserId);
            if (!departmentIds.Contains(studentOrgInfo.DepartmentGroup.DepartmentId))
                throw new Exception("Student info access denied for instucor");

            return View(_studentTranscriptService.GetTranscript(selectedOrganizationId, studentUserId));
        }

        [Authorize(Policy = Permissions.TranscriptsAdviser.View)]
        public IActionResult StudentTranscriptsForAdviser(int departmentGroupId, string lastName, string firstName, 
            int studentId, bool onlyActive)
        {
            List<SelectStudentViewModel> students = new List<SelectStudentViewModel>();

            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            if (!_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Adviser))
                throw new Exception("User is not an Adviser");

            students = _adviserStudentService.GetStudentSelectList(selectedOrganizationId, user.Id, departmentGroupId,
                lastName, firstName, studentId, onlyActive).OrderBy(x => x.FullNameEng).ToList();

            ViewBag.DepartmentGroups = new SelectList(
                _adviserStudentService.GetAdviserDepartmentGroupsByInstuctorId(selectedOrganizationId, user.Id)
                .OrderBy(x => x.DepartmentCode), "Id", "DepartmentCode", departmentGroupId);
            
            ViewBag.LastName = lastName;
            ViewBag.FirstName = firstName;
            ViewBag.StudentId = studentId;
            ViewBag.OnlyActive = onlyActive;

            return View(students);
        }

        [Authorize(Policy = Permissions.TranscriptsAdviser.View)]
        public IActionResult GetStudentTranscriptForAdviser(string studentUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = null;
            Task.Run(() => user = _userManager.GetUserAsync(User).GetAwaiter().GetResult()).Wait();
            if (user == null)
                return NotFound();

            if (!_adviserStudentService.HasStudent(selectedOrganizationId, user.Id, studentUserId))
                throw new Exception("Adviser does not have student");

            return View(_studentTranscriptService.GetTranscript(selectedOrganizationId, studentUserId));
        }

        [Authorize(Policy = Permissions.TranscriptsStudent.View)]
        public void RecalcStudentsGPA() 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _studentTranscriptService.RecalcStudentsGPA(selectedOrganizationId);
        }

        [Authorize(Policy = Permissions.TranscriptsAdmin.View)]
        public IActionResult GPAReport(int semesterId, int? departmentId, int? departmentGroupId,
            bool onlyActiveStudents, float minGPA, float maxGPA = 5)
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

            ViewBag.MinGPA = minGPA;
            ViewBag.MaxGPA = maxGPA;
            ViewBag.OnlyActiveStudents = onlyActiveStudents;

            var model = _studentTranscriptService.GPAReport(selectedOrganizationId, semesterId, 
                departmentId, departmentGroupId, minGPA, maxGPA, onlyActiveStudents);

            return View(model);
        }
    }
}
