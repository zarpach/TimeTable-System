using iuca.Application.Constants;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Courses;
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
    public class OldStudyCardsController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly ISemesterService _semesterService;
        private readonly IInstructorInfoService _instructorInfoService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IOldStudyCardService _studyCardService;
        private readonly ICourseService _courseService;
        private readonly IAcademicPlanService _academicPlanService;


        public OldStudyCardsController(
            IOrganizationService organizationService,
            ISemesterService semesterService,
            IInstructorInfoService instructorInfoService,
            IDepartmentGroupService departmentGroupService,
            IOldStudyCardService studyCardService,
            ICourseService courseService,
            IAcademicPlanService academicPlanService)
        {
            _organizationService = organizationService;
            _semesterService = semesterService;
            _instructorInfoService = instructorInfoService;
            _departmentGroupService = departmentGroupService;
            _studyCardService = studyCardService;
            _courseService = courseService;
            _academicPlanService = academicPlanService;
        }

        [Authorize(Policy = Permissions.AcademicPlans.View)]
        public IActionResult Index(int? departmentGroupId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            IEnumerable<OldStudyCardDTO> studyCards;
            if (departmentGroupId != null && departmentGroupId != 0)
                studyCards = _studyCardService.GetStudyCards(selectedOrganizationId, departmentGroupId.Value);
            else
                studyCards = _studyCardService.GetStudyCards(selectedOrganizationId);

            ViewBag.DepartmentGroupId = departmentGroupId;
            ViewBag.DepartmentGroups = new SelectList(_departmentGroupService.GetDepartmentGroups(selectedOrganizationId).OrderBy(x => x.DepartmentCode),
                "Id", "DepartmentCode", departmentGroupId);

            return View(studyCards.OrderBy(x => x.DepartmentGroup.Code)
                .ThenBy(x => x.Semester.Year).ThenBy(x => x.Semester.Season));
        }

        [Authorize(Policy = Permissions.AcademicPlans.View)]
        public IActionResult Details(int id, int departmentGroupId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ViewBag.DepartmentGroupId = departmentGroupId;

            return View(_studyCardService.GetStudyCard(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public IActionResult Create(int departmentGroupId)
        {
            FillSelectLists();

            ViewBag.DepartmentGroupId = departmentGroupId;

            return View();
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        [HttpPost]
        public IActionResult Create(OldStudyCardDTO studyCard, int depGroupId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            studyCard.OrganizationId = selectedOrganizationId;
            if (ModelState.IsValid)
            {
                try
                {
                    _studyCardService.Create(studyCard);
                    return RedirectToAction("Index", new { departmentGroupId = depGroupId });
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            FillSelectLists(studyCard);
            ViewBag.DepartmentGroupId = depGroupId;

            return View(studyCard);

        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public IActionResult Edit(int id, int departmentGroupId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var studyCard = _studyCardService.GetStudyCard(selectedOrganizationId, id);
            FillSelectLists(studyCard);

            ViewBag.DepartmentGroupId = departmentGroupId;

            return View(studyCard);
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, OldStudyCardDTO studyCard, int departmentGroupId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _studyCardService.Edit(selectedOrganizationId, id, studyCard);
                    return RedirectToAction("Index", new { departmentGroupId = departmentGroupId });
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            FillSelectLists(studyCard);

            ViewBag.DepartmentGroupId = departmentGroupId;

            return View(studyCard);
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public IActionResult Delete(int id, int departmentGroupId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ViewBag.DepartmentGroupId = departmentGroupId;

            return View(_studyCardService.GetStudyCard(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id, int departmentGroupId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _studyCardService.Delete(selectedOrganizationId, id);
                    return RedirectToAction("Index", new { departmentGroupId = departmentGroupId });
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return RedirectToAction("Delete", new { id = id, departmentGroupId = departmentGroupId });
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public IActionResult EditStudyCardCourses(int id, int departmentGroupId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var studyCard = _studyCardService.GetStudyCard(selectedOrganization, id);

            ViewBag.DepartmentGroupId = departmentGroupId;

            return View(studyCard);
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        [HttpPost]
        public IActionResult EditStudyCardCourses(int studyCardId, List<OldStudyCardCourseDTO> modelCourses, int departmentGroupId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _studyCardService.EditStudyCardCourses(selectedOrganizationId, studyCardId, modelCourses);
                    return RedirectToAction("Index", new { departmentGroupId = departmentGroupId });
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            ViewBag.DepartmentGroupId = departmentGroupId;

            return View("EditStudyCardCourses", new { studyCardId = studyCardId, modelCourses = modelCourses, departmentGroupId = departmentGroupId });
        }

        private void FillSelectLists(OldStudyCardDTO studyCard = null)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semesters = _semesterService.GetSemesters(selectedOrganization).Select(x => new { Id = x.Id, Name = $"{(enu_Season)x.Season}  {x.Year}" });
            var instructors = _instructorInfoService.GetInstructorInfoList(selectedOrganization, enu_InstructorState.Active, null)
                .Where(x => x.BasicInfoExists);
            var departmentGroups = _departmentGroupService.GetDepartmentGroups(selectedOrganization);

            if (studyCard != null)
            {
                ViewBag.Semesters = new SelectList(semesters, "Id", "Name", studyCard.SemesterId);
                ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode", studyCard.DepartmentGroupId);

            }
            else
            {
                ViewBag.Semesters = new SelectList(semesters, "Id", "Name");
                ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode");
            }
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        /// <summary>
        /// Get courses for selection window
        /// </summary>
        /// <param name="year">Year of study vard</param>
        /// <param name="season">Season of study card</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="excludedIds">Id of courses that must be excluded</param>
        /// <returns>Courses with recommendations</returns>
        public ViewResult GetCoursesForSelection(int year, int season, int departmentGroupId, int studyCardId, int[] excludedIds)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semester = _semesterService.GetSemester(selectedOrganization, year, season);
            var departmentGroup = _departmentGroupService.GetDepartmentGroup(selectedOrganization, departmentGroupId);
            var academicPlan = _academicPlanService.GetAcademicPlanForDepartmentGroup(selectedOrganization, departmentGroup);
            var courses = _studyCardService.GetCoursesForSelection(semester, departmentGroup, academicPlan, excludedIds);

            ViewData["StudyCardId"] = studyCardId;

            return View("_SelectCoursesPartial", courses);
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public ViewResult GetCoursesFromSelection(int studyCardId, int[] ids)
        {
            return View("_CourseRow", _studyCardService.GetCoursesFromSelection(studyCardId, ids));
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public IActionResult SetStudyCardPlaces(int? year, int? season) 
        {
            if (year == null)
                year = DateTime.Now.Year;

            if (season == null)
                season = (int)enu_Season.Fall;

            List<StudyCardPlacesViewModel> studyCardCourses = new List<StudyCardPlacesViewModel>();

            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semester = _semesterService.GetSemester(selectedOrganization, year.Value, season.Value, false);
            if (semester != null) 
            {
                studyCardCourses = _studyCardService.GetStudyCardPlaces(selectedOrganization, semester.Id)
                        .OrderBy(x => x.CyclePartCourse.Course.Name)
                        .ToList();
                ViewBag.SemesterId = semester.Id;
            }

            ViewBag.Year = year;
            ViewBag.Season = season;

            return View(studyCardCourses);
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        [HttpPost]
        public IActionResult SetStudyCardPlaces(int semesterId, List<StudyCardPlacesViewModel> courses)
        {

            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var semester = _semesterService.GetSemester(selectedOrganizationId, semesterId);

            ViewBag.Year = semester.Year;
            ViewBag.Season = semester.Season;
            ViewBag.SemesterId = semester.Id;

            string errorMsg = "";

            if (ModelState.IsValid)
            {
                try
                {
                    _studyCardService.SetStudyCardPlaces(selectedOrganizationId, semesterId, courses);
                    TempData["SuccessResult"] = "Успешно сохранено";
                    return RedirectToAction("SetStudyCardPlaces", new { year = semester.Year, season = semester.Season });
                }
                catch (ModelValidationException ex)
                {
                    errorMsg = ex.Message;
                }
            }

            TempData["FailResult"] = "Ошибка при сохранении\n" + errorMsg;


            return RedirectToAction("SetStudyCardPlaces", new { year = semester.Year, season = semester.Season });
        }

    }
}
