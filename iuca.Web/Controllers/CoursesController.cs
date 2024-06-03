using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.Models;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IOrganizationService _organizationService;
        private readonly IDepartmentService _departmentService;
        private readonly ILanguageService _languageService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserRolesService _userRolesService;
        private readonly IDeanService _deanService;

        public CoursesController(ICourseService courseService,
            IOrganizationService organizationService,
            IDepartmentService departmentService,
            ILanguageService languageService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService,
            IDeanService deanService)
        {
            _courseService = courseService;
            _organizationService = organizationService;
            _departmentService = departmentService;
            _languageService = languageService;
            _userManager = userManager;
            _userRolesService = userRolesService;
            _deanService = deanService;
        }

        [Authorize(Policy = Permissions.Courses.View)]
        public IActionResult Index(QueryStringParameters parameters, int? departmentId, string courseName,
            string courseAbbr, string courseNum, int? courseId, bool isDeleted = false)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            bool isDean = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean);

            ViewBag.IsDean = isDean;

            List<int> filterDepartmentIds = departmentId != null ? new List<int> { departmentId.Value } : null;
            List<DepartmentDTO> departments = new List<DepartmentDTO>();
            
            if (isDean)
            {
                departments = _deanService.GetDeanDepartments(selectedOrganizationId, user.Id).Departments;
                if (departmentId is null)
                    filterDepartmentIds = departments.Select(x => x.Id).ToList();
            }
            else
            { 
                departments = _departmentService.GetDepartments(selectedOrganizationId, true).ToList();
            }


            ViewBag.Departments = new SelectList(departments,
                "Id", "Code", departmentId);

            ViewBag.DepartmentId = departmentId;
            ViewBag.CourseName = courseName;
            ViewBag.CourseAbbr = courseAbbr;
            ViewBag.CourseNum = courseNum;
            ViewBag.CourseId = courseId;
            ViewBag.IsDeleted = isDeleted;

            return View(_courseService.GetCourses(selectedOrganizationId, parameters,
                    filterDepartmentIds, courseName, courseAbbr, courseNum, courseId, isDeleted));
        }

        [Authorize(Policy = Permissions.Courses.View)]
        public IActionResult Details(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_courseService.GetCourse(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.Courses.Edit)]
        public IActionResult Create()
        {
            FillSelectLists();
            return View();
        }

        [Authorize(Policy = Permissions.Courses.Edit)]
        [HttpPost]
        public IActionResult Create(CourseDTO course)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            course.OrganizationId = selectedOrganizationId;
            if (ModelState.IsValid)
            {
                try
                {
                    _courseService.Create(course);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            FillSelectLists(course);

            return View(course);

        }

        [Authorize(Policy = Permissions.Courses.Edit)]
        public IActionResult Edit(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var course = _courseService.GetCourse(selectedOrganizationId, id);
            FillSelectLists(course);

            return View(course);
        }

        [Authorize(Policy = Permissions.Courses.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, CourseDTO course)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _courseService.Edit(selectedOrganizationId, id, course);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            FillSelectLists(course);
            return View(course);
        }


        [Authorize(Policy = Permissions.Courses.Edit)]
        public ViewResult GetCoursesForSelection(int[] excludedCourseIds)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var courses = _courseService.GetCoursesForSelection(selectedOrganization, excludedCourseIds).ToList();

            return View("_SelectCoursesPartial", courses);
        }

        [Authorize(Policy = Permissions.Courses.Edit)]
        public IActionResult GetCoursesFromSelection(int[] selectedCourseIds)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var courses = _courseService.GetCourses(selectedOrganizationId, selectedCourseIds).ToList();

            return PartialView("_EditCoursePrerequsitePartial", courses);
        }
        

        [Authorize(Policy = Permissions.Courses.Edit)]
        public IActionResult Delete(int id, bool isDelete)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            ViewBag.IsDelete = isDelete;

            return View(_courseService.GetCourse(selectedOrganizationId, id, true));
        }

        [Authorize(Policy = Permissions.Courses.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _courseService.Delete(selectedOrganizationId, id);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return RedirectToAction("Delete", new { id = id, isDelete = true });
        }

        [Authorize(Policy = Permissions.Courses.Edit)]
        [HttpPost]
        public IActionResult UnDeleteConfirm(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _courseService.UnDelete(selectedOrganizationId, id);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return RedirectToAction("Delete", new { id = id, isDelete = false });
        }

        private void FillSelectLists(CourseDTO course = null) 
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var departments = _departmentService.GetDepartments(selectedOrganization, true);
            var languges = _languageService.GetLanguages().OrderByDescending(x => x.SortNum).ThenBy(x => x.NameEng);

            if (course != null)
            {
                ViewBag.Departments = new SelectList(departments, "Id", "Code", course.DepartmentId);
                ViewBag.Languages = new SelectList(languges, "Id", "NameEng", course.LanguageId);
            }
            else 
            {
                ViewBag.Departments = new SelectList(departments, "Id", "Code");
                ViewBag.Languages = new SelectList(languges, "Id", "NameEng");
            }
        }
    }
}
