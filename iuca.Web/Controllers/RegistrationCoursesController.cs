using iuca.Application.Constants;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Users.UserInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class RegistrationCoursesController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly ISemesterService _semesterService;
        private readonly IRegistrationCourseService _registrationCourseService;
        private readonly IUserInfoService _userInfoService;
        private readonly IStudentCourseTempService _studentCourseService;
        private readonly IEnvarSettingService _envarSettingService;

        public RegistrationCoursesController(IOrganizationService organizationService,
            ISemesterService semesterService,
            IRegistrationCourseService registrationCourseService,
            IUserInfoService userInfoService,
            IStudentCourseTempService studentCourseService,
            IEnvarSettingService envarSettingService)
        {
            _organizationService = organizationService;
            _semesterService = semesterService;
            _registrationCourseService = registrationCourseService;
            _userInfoService = userInfoService;
            _studentCourseService = studentCourseService;
            _envarSettingService = envarSettingService;
        }

        [Authorize(Policy = Permissions.StudyCards.Edit)]
        public IActionResult Index(int searchSemesterId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semesterId = SemesterSelectList(selectedOrganization, searchSemesterId);

            var announcementSections = _registrationCourseService.GetRegistrationCoursesInfo(semesterId)
                    .OrderBy(x => x.AnnouncementSection.Course.Abbreviation).ThenBy(x => x.AnnouncementSection.Course.Number)
                    .ThenBy(x => x.AnnouncementSection.Course.Name)
                    .ToList();

            return View(announcementSections);
        }

        [Authorize(Policy = Permissions.StudyCards.Edit)]
        [HttpPost]
        public void MarkRegistrationCourseDeleted (int courseDetId, bool isDeleted)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _registrationCourseService.MarkRegistrationCourseDeleted(selectedOrganizationId, courseDetId, isDeleted);
        }

        [Authorize(Policy = Permissions.StudyCards.View)]
        public IActionResult RegistrationCourseDetails(int registrationCourseId) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var model = _registrationCourseService.GetRegistrationCourseDetails(selectedOrganizationId, registrationCourseId);
            var semester = _semesterService.GetSemester(selectedOrganizationId, model.RegistrationCourse.Year, model.RegistrationCourse.Season);
            ViewBag.SemesterId = semester.Id;

            ViewBag.ReturnUrl = HttpContext.Request.Headers["Referer"].ToString();

            return View(model);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.Edit)]
        /// <summary>
        /// Get students for selection window
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="excludedIds">Id of students that must be excluded</param>
        /// <returns>Students with recommendations</returns>
        public ViewResult GetStudentsForSelection(int semesterId, string[] excludedIds)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var students = _registrationCourseService.GetStudentsForSelection(selectedOrganization, semesterId, excludedIds, true)
                .OrderBy(x => x.FullNameEng).ToList();

            ViewBag.DepartmentGroups = new SelectList(students.GroupBy(x => x.Group)
                .Select(x => new { Group = x.Key }).OrderBy(x => x.Group).ToList(),
                    "Group", "Group");

            return View("_SelectStudentsPartial", students);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.Edit)]
        public ViewResult AddStudentsFromSelection(int semesterId, int registrationCourseId, string[] studentUserIds)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View("_StudentRow", _registrationCourseService.AddStudentsFromSelection(selectedOrganization, semesterId,
                    registrationCourseId, studentUserIds));
        }

        [Authorize(Policy = Permissions.StudyCards.View)]
        public IActionResult PrintRegistrationCourseDetails(int registrationCourseId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var model = _registrationCourseService.GetRegistrationCourseDetails(selectedOrganizationId, registrationCourseId);

            return View(model);
        }

        [Authorize(Policy = Permissions.StudyCards.Edit)]
        [HttpPost]
        public void MarkStudentCourseDeleted(int studentCourseId, bool isDeleted) 
        {
            _studentCourseService.MarkDeleted(studentCourseId, isDeleted);
        }

        private int SemesterSelectList(int selectedOrganization, int searchSemesterId)
        {
            var semesters = _semesterService.GetSemesters(selectedOrganization);

            if (searchSemesterId == -1)
                searchSemesterId = _envarSettingService.GetUpcomingSemester(selectedOrganization);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", searchSemesterId);

            return searchSemesterId;
        }
    }
}
