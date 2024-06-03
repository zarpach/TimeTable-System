using iuca.Application.Constants;
using iuca.Application.DTO.Courses;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;
using iuca.Application.Interfaces.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class StudyCardsController : Controller
    {
        private readonly IStudyCardService _studyCardService;
        private readonly ISemesterService _semesterService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IOrganizationService _organizationService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserRolesService _userRolesService;
        private readonly IDepartmentService _departmentService;
        private readonly IDeanService _deanService;

        public StudyCardsController(IStudyCardService studyCardService,
            ISemesterService semesterService,
            IDepartmentGroupService departmentGroupService,
            IOrganizationService organizationService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService,
            IDepartmentService departmentService,
            IDeanService deanService
            )
        {
            _studyCardService = studyCardService;
            _semesterService = semesterService;
            _departmentGroupService = departmentGroupService;
            _organizationService = organizationService;
            _userManager = userManager;
            _userRolesService = userRolesService;
            _departmentService = departmentService;
            _deanService = deanService;
        }

        [Authorize(Policy = Permissions.StudyCards.View)]
        public IActionResult Index(int searchSemesterId, int searchDepartmentId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganization);
            var departments = _departmentService.GetDepartments(selectedOrganization, true);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            if (_userRolesService.IsUserInRole(user.Id, selectedOrganization, enu_Role.Dean))
                departments = _deanService.GetDeanDepartments(selectedOrganization, user.Id).Departments;

            if (searchSemesterId == -1)
            {
                var lastSemester = semesters.OrderByDescending(x => x.Year).ThenBy(x => x.Season).FirstOrDefault();
                if (lastSemester != null)
                    searchSemesterId = lastSemester.Id; 
            }

            var studyCards = _studyCardService.GetStudyCards(searchSemesterId, searchDepartmentId, departments);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", searchSemesterId);
            ViewBag.Departments = new SelectList(departments, "Id", "Code", searchDepartmentId);

            ViewBag.SemesterId = searchSemesterId;
            ViewBag.DepartmentId = searchDepartmentId;

            return View(studyCards);
        }

        [Authorize(Policy = Permissions.StudyCards.View)]
        public IActionResult Details(int id, int searchSemesterId, int searchDepartmentId)
        {
            var studyCard = _studyCardService.GetStudyCard(id);

            ViewBag.SemesterId = searchSemesterId;
            ViewBag.DepartmentId = searchDepartmentId;

            return View(studyCard);
        }

        [Authorize(Policy = Permissions.StudyCards.Edit)]
        public IActionResult Create(int searchSemesterId, int searchDepartmentId)
        {
            FillSelectLists(ref searchSemesterId, ref searchDepartmentId);

            var newStudyCard = new StudyCardDTO();
            newStudyCard.SemesterId = searchSemesterId;

            return View(newStudyCard);
        }

        [Authorize(Policy = Permissions.StudyCards.Edit)]
        [HttpPost]
        public IActionResult Create(StudyCardDTO newStudyCard, int searchSemesterId, int searchDepartmentId)
        {
            ViewBag.SemesterId = searchSemesterId;
            ViewBag.DepartmentId = searchDepartmentId;
            ViewBag.OrganizationId = _organizationService.GetSelectedOrganization(User);

            if (ModelState.IsValid)
            {
                try
                {
                    _studyCardService.CreateStudyCard(newStudyCard);
                    TempData["StudyCardSuccessMessage"] = "Study card creation succeeded!";
                    return RedirectToAction("Index", new
                    {
                        searchSemesterId = searchSemesterId,
                        searchDepartmentId = searchDepartmentId
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["StudyCardError"] = ex.Message;
                }
            } else
            {
                TempData["StudyCardErrorMessage"] = "Study card creation failed.";
            }

            return View(newStudyCard);
        }

        [Authorize(Policy = Permissions.StudyCards.Edit)]
        public IActionResult Edit(int id, int searchSemesterId, int searchDepartmentId)
        {
            FillSelectLists(ref searchSemesterId, ref searchDepartmentId);

            var studyCard = _studyCardService.GetStudyCard(id);

            return View(studyCard);
        }

        [Authorize(Policy = Permissions.StudyCards.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, StudyCardDTO newStudyCard, int searchSemesterId, int searchDepartmentId)
        {
            ViewBag.SemesterId = searchSemesterId;
            ViewBag.DepartmentId = searchDepartmentId;
            ViewBag.OrganizationId = _organizationService.GetSelectedOrganization(User);

            if (ModelState.IsValid)
            {
                try
                {
                    _studyCardService.EditStudyCard(id, newStudyCard);
                    _studyCardService.EditStudyCardCourses(id, newStudyCard.StudyCardCourses);
                    TempData["StudyCardSuccessMessage"] = "Study card saving succeeded!";
                    return RedirectToAction("Edit", new
                    {
                        id = id,
                        searchSemesterId = searchSemesterId,
                        searchDepartmentId = searchDepartmentId
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["StudyCardError"] = ex.Message;
                }
            } else
            {
                TempData["StudyCardErrorMessage"] = "Study card saving failed.";
            }

            return View(newStudyCard);
        }

        [Authorize(Policy = Permissions.StudyCards.Edit)]
        [HttpPost]
        public void Delete(int id)
        {
            _studyCardService.DeleteStudyCard(id);
        }

        [Authorize(Policy = Permissions.StudyCards.Edit)]
        public ViewResult GetCoursesForSelection(int semesterId, int[] excludedCourseIds, int departmentGroupId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semester = _semesterService.GetSemester(selectedOrganization, semesterId);
            var departmentGroup = _departmentGroupService.GetDepartmentGroup(selectedOrganization, departmentGroupId);

            var courses = _studyCardService.GetCoursesForSelection(selectedOrganization, semester.Year, semester.Season, excludedCourseIds);

            ViewBag.Semester = semester.SeasonYear;
            ViewBag.DepartmentGroup = departmentGroup.DepartmentCode;

            return View("_SelectCoursesPartial", courses);
        }

        [Authorize(Policy = Permissions.StudyCards.Edit)]
        public IActionResult GetCourseFromSelection(int selectedCourseId)
        {
            var course = _studyCardService.GetCourseFromSelection(selectedCourseId);

            var newStudyCardCourse =  new StudyCardCourseDTO
            {
                RegistrationCourseId = course.Id,
                AnnouncementSection = course
            };

            return PartialView("_EditStudyCardCoursesPartial", newStudyCardCourse);
        }

        private void FillSelectLists(ref int searchSemesterId, ref int searchDepartmentId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganization);
            var departments = _departmentService.GetDepartments(selectedOrganization, true);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            if (_userRolesService.IsUserInRole(user.Id, selectedOrganization, enu_Role.Dean))
                departments = _deanService.GetDeanDepartments(selectedOrganization, user.Id).Departments;

            var departmentGroups = _departmentGroupService.GetDepartmentGroups(selectedOrganization);
            departmentGroups = departmentGroups.Where(departmentGroup => departments.Select(x => x.Id)
            .Contains(departmentGroup.DepartmentId)).OrderBy(x => x.Department.Code).ThenByDescending(x => x.Year);
           
            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", searchSemesterId);
            ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode");

            ViewBag.SemesterId = searchSemesterId;
            ViewBag.DepartmentId = searchDepartmentId;
            ViewBag.OrganizationId = _organizationService.GetSelectedOrganization(User);
        }
    }
}
