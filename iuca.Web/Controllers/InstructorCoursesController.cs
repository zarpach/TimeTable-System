using iuca.Application.Constants;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class InstructorCoursesController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly ISemesterService _semesterService;
        private readonly IInstructorCourseService _instructorCourseService;
        private readonly IGradeService _gradeService;
        private readonly ISemesterPeriodService _semesterPeriodService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserRolesService _userRolesService;
        private readonly IUserInfoService _userInfoService;
        private readonly IInstructorInfoService _instructorInfoService;
        private readonly IEnvarSettingService _envarSettingService;

        public InstructorCoursesController(IOrganizationService organizationService,
            ISemesterService semesterService,
            IInstructorCourseService instructorCourseService,
            IGradeService gradeService,
            ISemesterPeriodService semesterPeriodService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService,
            IUserInfoService userInfoService,
            IInstructorInfoService instructorInfoService,
            IEnvarSettingService envarSettingService)
        {
            _organizationService = organizationService;
            _semesterService = semesterService;
            _instructorCourseService = instructorCourseService;
            _gradeService = gradeService;
            _semesterPeriodService = semesterPeriodService;
            _userManager = userManager;
            _userRolesService = userRolesService;
            _userInfoService = userInfoService;
            _instructorInfoService = instructorInfoService;
            _envarSettingService = envarSettingService;
        }

        [Authorize(Policy = Permissions.InstructorCourses.View)]
        public IActionResult Index(int semesterId, string instructorUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            if (semesterId == 0) 
                semesterId = _envarSettingService.GetCurrentSemester(selectedOrganizationId);

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), 
                "Id", "SeasonYear", semesterId);
            ViewBag.SemesterId = semesterId;

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();
            
            bool isAdmin = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Admin);
            bool isDean = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean);

            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsDean = isDean;

            if (isAdmin || isDean)
            {
                if (string.IsNullOrEmpty(instructorUserId))
                    instructorUserId = user.Id;
                ViewBag.InstructorName = _userManager.Users.FirstOrDefault(x => x.Id == instructorUserId)?.FullNameEng;
                ViewBag.Instructors = _instructorInfoService.GetInstructorSelectList(instructorUserId, selectedOrganizationId);
            }
            else
            {
                if (!_userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Instructor))
                    throw new Exception("User is not instructor");

                instructorUserId = user.Id;
                ViewBag.InstructorName = user.FullNameEng;
            }

            var instructorCourses = _instructorCourseService.GetInstructorCourses(selectedOrganizationId, semesterId, instructorUserId)
                .OrderBy(x => x.Code).ThenBy(x => x.Name).ToList();
            ViewBag.InstructorUserId = instructorUserId;

            return View(instructorCourses);
        }

        [Authorize(Policy = Permissions.InstructorCourses.View)]
        public IActionResult InstructorCourseStudents(int semesterId, int announcementSectionId, 
            bool onlyActiveStudents) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var announcementSection = _instructorCourseService.GetInstructorCourse(announcementSectionId);
            var students = _instructorCourseService.GetInstructorCourseStudents(selectedOrganizationId, announcementSectionId, onlyActiveStudents);

            ViewBag.AnnouncementSectionId = announcementSection.AnnouncementSetcionId;
            ViewBag.CourseCode = announcementSection.Code;
            ViewBag.CourseName = announcementSection.Name;
            ViewBag.CourseSection = announcementSection.Section;
            ViewBag.CourseId = announcementSection.ImportCode;

            ViewBag.SemesterId = semesterId;
            ViewBag.InstructorUserId = announcementSection.InstructorUserId;
            ViewBag.OnlyActiveStudents = onlyActiveStudents;

            return View(students.OrderBy(x => x.StudentName).ToList());
        }

        [Authorize(Policy = Permissions.InstructorCourses.View)]
        public IActionResult InstructorCourseStudentsPrint(int announcementSectionId, bool onlyActiveStudents)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var announcementSection = _instructorCourseService.GetInstructorCourse(announcementSectionId);
            var students = _instructorCourseService.GetInstructorCourseStudents(selectedOrganizationId, announcementSectionId, onlyActiveStudents);

            ViewBag.CourseCode = announcementSection.Code;
            ViewBag.CourseName = announcementSection.Name;
            ViewBag.CourseSection = announcementSection.Section;
            ViewBag.CourseId = announcementSection.ImportCode;

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.MarginTop = 50;
            converter.Options.MarginBottom = 50;
            var html = this.RenderViewAsync("InstructorCourseStudentsPrint", students.OrderBy(x => x.StudentName).ToList(), true).Result;

            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            PdfDocument doc = converter.ConvertHtmlString(html, baseUrl);
            using var ms = new MemoryStream();
            doc.Save(ms);

            return File(ms.ToArray(), "application/pdf");

            //return View(students.OrderBy(x => x.StudentName).ToList());
        }

        [Authorize(Policy = Permissions.InstructorCourses.View)]
        public IActionResult InstructorCourseStudentGrades(int semesterId, int announcementSectionId, string instructorUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            bool isAdmin = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Admin);
            bool isDean = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Dean);

            var announcementSection = _instructorCourseService.GetInstructorCourse(announcementSectionId);

            if (!isAdmin && !isDean && !(announcementSection.InstructorUserId == user.Id ||
                    announcementSection.ExtraInstructorIds.Contains(user.Id)))
                throw new Exception("Wrong instructor user id");

            var students = _instructorCourseService.GetInstructorCourseStudentGrades(selectedOrganizationId, announcementSectionId);

            ViewBag.AnnouncementSectionId = announcementSection.AnnouncementSetcionId;
            ViewBag.GradeSheetSubmitted = announcementSection.GradeSheetSubmitted;
            ViewBag.GradingPeriodOpened =
                _semesterPeriodService.GetSemesterPeriod(selectedOrganizationId, (int)enu_Period.Grading, DateTime.Now) != null;
            ViewBag.CourseCode = announcementSection.Code;
            ViewBag.CourseName = announcementSection.Name;
            ViewBag.CourseSection = announcementSection.Section;
            ViewBag.CourseId = announcementSection.ImportCode;
            ViewBag.SemesterId = semesterId;

            List<string> disabledGradeMarks = new List<string> { "*", "A+", "AH", "AU", "I", "PE", "W", "X" };
            var grades = _gradeService.GetGrades().OrderBy(x => x.GradeMark).ToList();
            List<SelectListItem> gradeItems = new List<SelectListItem>();
            foreach (var grade in grades)
            {
                gradeItems.Add(new SelectListItem()
                {
                    Text = grade.GradeMark,
                    Value = grade.Id.ToString(),
                    Disabled = disabledGradeMarks.Contains(grade.GradeMark)
                });
            }

            ViewBag.Grades = gradeItems;
            ViewBag.InstructorUserId = instructorUserId;

            return View(students.OrderBy(x => x.StudentName).ToList());
        }

        [Authorize(Policy = Permissions.InstructorCourses.Edit)]
        [HttpPost]
        public void SetStudentGrade(int studentCourseId, int? gradeId) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _instructorCourseService.SetStudentGrade(selectedOrganizationId, studentCourseId, gradeId);
        }

        [Authorize(Policy = Permissions.InstructorCourses.Edit)]
        [HttpPost]
        public IActionResult SubmitGradeSheet(int announcementSectionId)
        {
            try 
            {
                _instructorCourseService.SetGradeSheetSubmitted(announcementSectionId, true);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.InstructorCourses.Edit)]
        [HttpPost]
        public IActionResult ReturnGradeSheet(int announcementSectionId)
        {
            try
            {
                _instructorCourseService.SetGradeSheetSubmitted(announcementSectionId, false);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.InstructorCourses.View)]
        public IActionResult GradeSheet(int semesterId, int announcementSectionId) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            bool isInstructor = _userRolesService.IsUserInRole(user.Id, selectedOrganizationId, enu_Role.Instructor);

            var announcementSection = _instructorCourseService.GetInstructorCourse(announcementSectionId);

            if (!announcementSection.GradeSheetSubmitted)
                throw new Exception("Grade sheet is not submitted");

            if (isInstructor && !(announcementSection.InstructorUserId == user.Id ||
                    announcementSection.ExtraInstructorIds.Contains(user.Id)))
                throw new Exception("Wrong instructor user id");

            var students = _instructorCourseService.GetInstructorCourseStudentGrades(selectedOrganizationId, announcementSectionId);

            ViewBag.Semester = _semesterService.GetSemester(selectedOrganizationId, semesterId).SeasonYear;
            ViewBag.Course = announcementSection;
            ViewBag.OrganizationTitle = selectedOrganizationId == 1 ? "International University of Central Asia" :
                "Humanitarian-Technical College (HTC)";
            ViewBag.OrganizationId = selectedOrganizationId;

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.MarginTop = 50;
            converter.Options.MarginBottom = 50;
            var html = this.RenderViewAsync("GradeSheet", students.OrderBy(x => x.StudentName).ToList(), true).GetAwaiter().GetResult();

            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            PdfDocument doc = converter.ConvertHtmlString(html, baseUrl);
            using var ms = new MemoryStream();
            doc.Save(ms);

            return File(ms.ToArray(), "application/pdf");

            //return View(students.OrderBy(x => x.StudentName).ToList());
        }


        
    }

    
}
