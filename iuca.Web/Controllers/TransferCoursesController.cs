using iuca.Application.Constants;
using iuca.Application.DTO.Courses;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
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
    public class TransferCoursesController : Controller
    {
        private readonly ITransferCourseService _transferCourseService;
        private readonly IOrganizationService _organizationService;
        private readonly IUniversityService _universityService;
        private readonly IStudentInfoService _studentInfoService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IStudentBasicInfoService _studentBasicInfoService;
        private readonly ICourseService _courseService;
        private readonly IStudentOrgInfoService _studentOrgInfoService;
        private readonly IDepartmentService _departmentService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public TransferCoursesController(ITransferCourseService transferCourseService,
            IOrganizationService organizationService,
            IUniversityService universityService,
            IStudentInfoService studentInfoService,
            IDepartmentGroupService departmentGroupService,
            IStudentBasicInfoService studentBasicInfoService,
            ICourseService courseService,
            IStudentOrgInfoService studentOrgInfoService,
            IDepartmentService departmentService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _transferCourseService = transferCourseService;
            _organizationService = organizationService;
            _universityService = universityService;
            _studentInfoService = studentInfoService;
            _departmentGroupService = departmentGroupService;
            _studentBasicInfoService = studentBasicInfoService;
            _courseService = courseService;
            _studentOrgInfoService = studentOrgInfoService;
            _departmentService = departmentService;
            _userManager = userManager;
        }

        [Authorize(Policy = Permissions.TransferCourses.View)]
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

        [Authorize(Policy = Permissions.TransferCourses.Edit)]
        public IActionResult EditTransferCourses(string studentUserId) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var user = _userManager.Users
                .Include(x => x.StudentBasicInfo).ThenInclude(x => x.StudentOrgInfo).ThenInclude(x => x.DepartmentGroup)
                .Include(x => x.StudentBasicInfo).ThenInclude(x => x.StudentOrgInfo).ThenInclude(x => x.PrepDepartmentGroup)
                .FirstOrDefault(x => x.Id == studentUserId);

            if (user == null)
                throw new Exception("User not found");

            ViewBag.StudentName = user.FullNameEng;
            ViewBag.StudentUserId = user.Id;

            if (user.StudentBasicInfo != null && user.StudentBasicInfo.StudentOrgInfo != null) 
            {
                var studentOrgInfo = user.StudentBasicInfo
                    .StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == selectedOrganizationId);

                if (studentOrgInfo != null) 
                {
                    ViewBag.DepartmentId = studentOrgInfo.DepartmentGroup.DepartmentId;
                    ViewBag.Year = studentOrgInfo.DepartmentGroup.Year;
                }
            }
           
            return View(_transferCourseService.GetTransferCourses(selectedOrganizationId, studentUserId));
        }

        [Authorize(Policy = Permissions.TransferCourses.Edit)]
        [HttpPost]
        public IActionResult EditTransferCourses(string studentUserId, List<TransferCourseDTO> transferCourses)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            _transferCourseService.EditStudentTransferCourses(selectedOrganization, studentUserId, transferCourses);
            
            return RedirectToAction("Index");
        }

        [Authorize(Policy = Permissions.TransferCourses.Edit)]
        public ViewResult GetBlankEditTransferCourseRow(int universityId, int year)
        {
            TransferCourseDTO model = new TransferCourseDTO();
            model.UniversityId = universityId;
            model.Year = year;

            return View("_EditRowPartial", model);
        }

        [Authorize(Policy = Permissions.TransferCourses.Edit)]
        public ViewResult GetCoursesForSelection(int rowIndex, int departmentId, int year, int courseId, int[] excludedIds)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var courses = _courseService.GetCyclePartCoursesWithExclusion(selectedOrganization, departmentId, year, courseId, excludedIds).ToList();

            ViewBag.RowIndex = rowIndex;
            ViewBag.Departments = new SelectList(_departmentService.GetDepartments(selectedOrganization).OrderBy(x => x.Code), "Id", "Code", departmentId);
            ViewBag.Year = year;
            ViewBag.CourseId = courseId;

            return View("_SelectCoursesPartial", courses);
        }
    }
}
