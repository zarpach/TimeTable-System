using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using iuca.Application.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using iuca.Application.Constants;
using iuca.Web.Extensions;
using SelectPdf;
using System.IO;
using System.Linq;
using iuca.Application.Exceptions;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class StudentCourseRegistrationsController : Controller
    {
        private readonly IStudentCourseRegistrationService _studentCourseRegistrationService;
        private readonly IOrganizationService _organizationService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IStudentBasicInfoService _studentBasicInfoService;
        private readonly IStudentOrgInfoService _studentOrgInfoService;
        private readonly ISemesterService _semesterService;
        private readonly ISemesterPeriodService _semesterPeriodService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IOldStudyCardService _studyCardService;
        private readonly IStudentCourseService _studentCourseService;
        private readonly IStudentCourseTempService _studentCourseTempService;
        private readonly IRegistrationCourseService _registrationCourseService;
        private readonly IDepartmentService _departmentService;
        private readonly IStudentInfoService _studentInfoService;
        private readonly ISyllabusService _syllabusService;
        private readonly ILogger<StudentCourseRegistrationsController> _logger;
        private readonly IEnvarSettingService _envarSettingService;

        public StudentCourseRegistrationsController(IStudentCourseRegistrationService studentCourseRegistrationService,
            IOrganizationService organizationService,
            ApplicationUserManager<ApplicationUser> userManager,
            IStudentBasicInfoService studentBasicInfoService,
            IStudentOrgInfoService studentOrgInfoService,
            ISemesterService semesterService,
            ISemesterPeriodService semesterPeriodService,
            IDepartmentGroupService departmentGroupService,
            IOldStudyCardService studyCardService,
            IStudentCourseService studentCourseService,
            IStudentCourseTempService studentCourseTempService,
            IRegistrationCourseService registrationCourseService,
            IDepartmentService departmentService,
            IStudentInfoService studentInfoService,
            ISyllabusService syllabusService,
            ILogger<StudentCourseRegistrationsController> logger,
            IEnvarSettingService envarSettingService)
        {
            _studentCourseRegistrationService = studentCourseRegistrationService;
            _organizationService = organizationService;
            _userManager = userManager;
            _studentBasicInfoService = studentBasicInfoService;
            _studentOrgInfoService = studentOrgInfoService;
            _semesterService = semesterService;
            _semesterPeriodService = semesterPeriodService;
            _departmentGroupService = departmentGroupService;
            _studyCardService = studyCardService;
            _studentCourseService = studentCourseService;
            _studentCourseTempService = studentCourseTempService;
            _registrationCourseService = registrationCourseService;
            _departmentService = departmentService;
            _studentInfoService = studentInfoService;
            _syllabusService = syllabusService;
            _logger = logger;
            _envarSettingService = envarSettingService;
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.View)]
        public IActionResult Index(int semesterId, int? departmentId, int? minCredits, 
            enu_RegistrationState registrationState, enu_StudentState studentState)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            if (semesterId == 0)
                semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganizationId);

            var registrations = _studentCourseRegistrationService.GetStudentCourseRegistrations(selectedOrganizationId,
                semesterId, departmentId, minCredits, registrationState, studentState);

            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId),
                "Id", "SeasonYear", semesterId);
            ViewBag.Departments = new SelectList(_departmentService.GetDepartments(selectedOrganizationId, true)
                .OrderBy(x => x.Code), "Id", "Code", departmentId);
            
            ViewBag.RegistrationStates = new SelectList(Enum.GetValues(typeof(enu_RegistrationState)).Cast<enu_RegistrationState>()
                .Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList(), "Value", "Text", ((int)registrationState).ToString());
            
            ViewBag.StudentStates = new SelectList(Enum.GetValues(typeof(enu_StudentState)).Cast<enu_StudentState>()
                .Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList(), "Value", "Text", ((int)studentState).ToString());
            
            ViewBag.MinCredits = minCredits;
            ViewBag.RegistrationState = registrationState;

            return View(registrations);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.Edit)]
        [HttpPost]
        public IActionResult SetNoCreditsLimitation(int registrationId, bool noCreditsLimitation)
        {
            _studentCourseRegistrationService.SetNoCreditsLimitation(registrationId, noCreditsLimitation);
            return Ok();
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.Edit)]
        public ViewResult SelectStudentsForRegistrationAdding()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var students = _studentInfoService.GetStudentInfoList(selectedOrganizationId, true).ToList();

            return View("_AddStudentRegistration", students);
        }


        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.View)]
        public IActionResult EditRegistration(int studentCourseRegistrationId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var registration = _studentCourseRegistrationService.GetStudentCourseRegistration(studentCourseRegistrationId);

            var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(selectedOrganizationId, registration.StudentUserId);

            ViewBag.Semester = $"{ EnumExtentions.GetDisplayName((enu_Season)registration.Semester.Season) } { registration.Semester.Year }";
            if (studentOrgInfo != null)
            {
                ViewBag.StudentId = studentOrgInfo.StudentId;
                ViewBag.StudentGroup = studentOrgInfo.DepartmentGroup?.Department?.Code + studentOrgInfo.DepartmentGroup?.Code;
            }
            ViewBag.StudentName = _userManager.GetUserFullName(registration.StudentUserId);

            ViewBag.RegistrationStates = new SelectList(Enum.GetValues(typeof(enu_RegistrationState)).Cast<enu_RegistrationState>()
                .Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList(), "Value", "Text", registration.State.ToString());

            ViewBag.AddDropStates = new SelectList(Enum.GetValues(typeof(enu_RegistrationState)).Cast<enu_RegistrationState>()
                .Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList(), "Value", "Text", registration.AddDropState.ToString());

            return View(registration);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.Edit)]
        [HttpPost]
        public IActionResult SetRegistrationState(int registrationId, enu_RegistrationState state)
        {
            _studentCourseRegistrationService.SetRegistrationState(registrationId, state);
            return Ok();
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.Edit)]
        [HttpPost]
        public IActionResult SetAddDropState(int registrationId, enu_RegistrationState state)
        {
            _studentCourseRegistrationService.SetAddDropState(registrationId, state);
            return Ok();
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.Edit)]
        public ViewResult GetCoursesForSelection(int semesterId, enu_CourseState state)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semester = _semesterService.GetSemester(selectedOrganization, semesterId);

            var registrationCourses = _registrationCourseService.GetRegistrationCourses(selectedOrganization,
                                        semester.Year, semester.Season).OrderBy(x => x.Course.NameEng).ToList();

            ViewBag.State = state;

            return View("_SelectRegistrationCoursesPartial", registrationCourses);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.Edit)]
        [HttpPost]
        public ViewResult AddCourseByAdmin(int studentCourseRegistrationId, int registrationCourseId, enu_CourseState state)
        {
            var id = _studentCourseTempService.AddCourseToRegistrationByAdmin(studentCourseRegistrationId,
                registrationCourseId, state);
            var studentCourse = _studentCourseTempService.GetStudentCourse(id);

            return View("_EditCourseRow", studentCourse);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.Edit)]
        [HttpPost]
        public IActionResult RemoveCourseByAdmin(int studentCourseId)
        {
            _studentCourseTempService.RemoveCourseFromRegistrationByAdmin(studentCourseId);
            return Ok();
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrationsManagement.Edit)]
        [HttpPost]
        public IActionResult Create(int semesterId, string studentUserId) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _studentCourseRegistrationService.Create(selectedOrganizationId, semesterId, studentUserId);
            return Ok();
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.View)]
        public IActionResult CourseRegistration()
        {
            ApplicationUser user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
                return NotFound();

            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var model = _studentCourseRegistrationService.GetStudentCourseRegistrationStep(selectedOrganizationId, user.Id);
            var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(selectedOrganizationId, user.Id);

            var departmentGroupId = studentOrgInfo.DepartmentGroupId;
            if (studentOrgInfo.IsPrep && studentOrgInfo.PrepDepartmentGroup != null)
                departmentGroupId = studentOrgInfo.PrepDepartmentGroup.Id;

            ViewBag.DepartmentGroupId = departmentGroupId;

            return View(model);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.View)]
        public IActionResult AddDropCourses()
        {
            ApplicationUser user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
                return NotFound();

            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var model = _studentCourseRegistrationService.GetStudentAddDropCourseStep(selectedOrganizationId, user.Id);
            var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(selectedOrganizationId, user.Id);

            var departmentGroupId = studentOrgInfo.DepartmentGroupId;
            if (studentOrgInfo.IsPrep && studentOrgInfo.PrepDepartmentGroup != null)
                departmentGroupId = studentOrgInfo.PrepDepartmentGroup.Id;

            ViewBag.DepartmentGroupId = departmentGroupId;

            return View(model);
        }


        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        public IActionResult SelectCourses(int semesterId, int departmentGroupId, int studentCourseRegistrationId, bool onlySelectedCourses) 
        {
            ApplicationUser user = null;
            Task.Run(() => user = _userManager.GetUserAsync(User).GetAwaiter().GetResult()).Wait();
            if (user == null)
                return NotFound();

            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            //var courses = _studentCourseRegistrationService.GetCoursesForSelection(semesterId, departmentGroupId, user.Id, onlySelectedCourses);

            var courses = _studentCourseRegistrationService.GetCoursesForSelectionTemp(selectedOrganizationId, semesterId, user.Id, onlySelectedCourses);

            ViewBag.DepartmentGroups = new SelectList(_studyCardService.GetDepartmentGroupsForSemester(selectedOrganizationId,
                                        semesterId), "Id", "DepartmentCode", departmentGroupId);

            ViewBag.DepartmentGroupId = departmentGroupId;
            ViewBag.SemesterId = semesterId;
            ViewBag.StudentCourseRegistrationId = studentCourseRegistrationId;
            ViewBag.OnlySelectedCourses = onlySelectedCourses;

            return View(courses);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        public IActionResult SelectCoursesFromStudyCard(int semesterId, int departmentGroupId, int studentCourseRegistrationId)
        {
            ApplicationUser user =  _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
                return NotFound();

            var studyCardCourses = _studentCourseRegistrationService.GetCoursesForSelectionFromStudyCard(semesterId, 
                departmentGroupId, user.Id, studentCourseRegistrationId);

            return View(studyCardCourses);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult SelectAllFromStudyCard(int semesterId, int departmentGroupId, int studentCourseRegistrationId,
            int studyCardId)
        {
            try
            {
                _studentCourseTempService.AddAllCoursesFromStudyCardToRegistration(studentCourseRegistrationId, studyCardId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка во время отправки заявления на проверку эдвайзеру";
                _logger.LogError(ex.Message);
            }

            return RedirectToAction("SelectCoursesFromStudyCard", new { semesterId = semesterId, 
                departmentGroupId = departmentGroupId, studentCourseRegistrationId = studentCourseRegistrationId });
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        public IActionResult AddDropSelectCourses(int semesterId, int departmentGroupId, int studentCourseRegistrationId, bool onlySelectedCourses)
        {
            ApplicationUser user = null;
            Task.Run(() => user = _userManager.GetUserAsync(User).GetAwaiter().GetResult()).Wait();
            if (user == null)
                return NotFound();

            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var courses = _studentCourseRegistrationService.GetCoursesForAddDropSelectionTemp(selectedOrganizationId, semesterId, user.Id);

            ViewBag.DepartmentGroups = new SelectList(_studyCardService.GetDepartmentGroupsForSemester(selectedOrganizationId,
                                        semesterId), "Id", "DepartmentCode", departmentGroupId);

            ViewBag.DepartmentGroupId = departmentGroupId;
            ViewBag.SemesterId = semesterId;
            ViewBag.StudentCourseRegistrationId = studentCourseRegistrationId;
            ViewBag.OnlySelectedCourses = onlySelectedCourses;

            return View(courses);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        public IActionResult AddDropSelectCoursesFromStudyCard(int semesterId, int departmentGroupId, int studentCourseRegistrationId)
        {
            ApplicationUser user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
                return NotFound();

            var studyCardCourses = _studentCourseRegistrationService.GetCoursesForSelectionFromStudyCard(semesterId,
                departmentGroupId, user.Id, studentCourseRegistrationId);

            return View(studyCardCourses);
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult SelectCourse(int studentCourseRegistrationId, int studyCardCourseId)
        {
            //_studentCourseService.AddCourseToRegistration(studentCourseRegistrationId, studyCardCourseId);
            var result = _studentCourseTempService.AddCourseToRegistration(studentCourseRegistrationId, studyCardCourseId);
            return Ok(new { Places = result.RestPlaces, Queue = result.Queue });
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult RemoveCourse(int studentCourseRegistrationId, int studyCardCourseId)
        {
            //_studentCourseService.RemoveCourseFromRegistration(studentCourseRegistrationId, studyCardCourseId);
            var result = _studentCourseTempService.RemoveCourseFromRegistration(studentCourseRegistrationId, studyCardCourseId);
            return Ok(new { Places = result.RestPlaces });
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult DropCourse(int studentCourseRegistrationId, int studyCardCourseId)
        {
            try
            {
                _studentCourseTempService.DropCourseFromRegistration(studentCourseRegistrationId, studyCardCourseId);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult ReturnCourse(int studentCourseRegistrationId, int studyCardCourseId)
        {
            _studentCourseTempService.ReturnDroppedCourse(studentCourseRegistrationId, studyCardCourseId);
            return Ok();
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult MarkAudit(int studentCourseId, bool isAudit)
        {
            _studentCourseTempService.MarkAudit(studentCourseId, isAudit);
            return Ok();
        }


        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult AddCourse(int studentCourseRegistrationId, int studyCardCourseId)
        {
            _studentCourseTempService.AddNewCourseToRegistration(studentCourseRegistrationId, studyCardCourseId);
            return Ok();
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult RemoveAddedCourse(int studentCourseRegistrationId, int studyCardCourseId)
        {
            _studentCourseTempService.RemoveAddedCourseFromRegistration(studentCourseRegistrationId, studyCardCourseId);
            return Ok();
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult SaveStudentComment(int studentCourseRegistrationId, string comment)
        {
            try
            {
                _studentCourseRegistrationService.SaveStudentComment(studentCourseRegistrationId, comment);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка во время сохранения комментария";
                _logger.LogError(ex.Message);
            }

            return Ok();
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult SaveStudentAddDropComment(int studentCourseRegistrationId, string comment)
        {
            try
            {
                _studentCourseRegistrationService.SaveStudentAddDropComment(studentCourseRegistrationId, comment);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка во время сохранения комментария";
                _logger.LogError(ex.Message);
            }

            return Ok();
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult SendOnApproval(int studentCourseRegistrationId)
        {
            try
            {
                _studentCourseRegistrationService.SendOnApproval(studentCourseRegistrationId);
            }
            catch (Exception ex) 
            {
                TempData["Error"] = "Ошибка во время отправки заявления на проверку эдвайзеру";
                _logger.LogError(ex.Message);
            }

            return RedirectToAction("CourseRegistration");
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult SendAddDropOnApproval(int studentCourseRegistrationId)
        {
            try
            {
                _studentCourseRegistrationService.SendAddDropOnApproval(studentCourseRegistrationId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка во время отправки заявления на проверку эдвайзеру";
                _logger.LogError(ex.Message);
            }

            return RedirectToAction("AddDropCourses");
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult SubmitRegistration(int studentCourseRegistrationId)
        {
            try
            {
                _studentCourseRegistrationService.SubmitRegistration(studentCourseRegistrationId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка во время завершения регистрации";
                _logger.LogError(ex.Message);
            }

            return RedirectToAction("CourseRegistration");
        }

        [Authorize(Policy = Permissions.StudentCourseRegistrations.Edit)]
        [HttpPost]
        public IActionResult SubmitAddDropForm(int studentCourseRegistrationId)
        {
            try
            {
                _studentCourseRegistrationService.SubmitAddDropForm(studentCourseRegistrationId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка во время завершения регистрации";
                _logger.LogError(ex.Message);
            }

            return RedirectToAction("AddDropCourses");
        }

        public ViewResult GetSyllabus(int registrationCourseId) 
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var syllabus = _syllabusService.GetSyllabusByRegistrationCourseId(registrationCourseId);
            if (syllabus != null)
                syllabus.SyllabusDetails = _syllabusService.GetSyllabusDetails(selectedOrganizationId, registrationCourseId);

            return View("_Syllabus", syllabus);
        }

        public IActionResult PrintToPDF(int semesterId)
        {
            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            return PrintToPDFById(semesterId, user.Id);
        }

        public IActionResult PrintToPDFById(int semesterId, string studentUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semester = _semesterService.GetSemester(selectedOrganizationId, semesterId);
            if (semester == null)
                throw new Exception("Semester not found");

            var model = _studentCourseTempService.GetStudentCoursesByStudentUserIdTemp(semesterId, studentUserId);

            foreach (var studentCourse in model)
            {
                studentCourse.Queue = _studentCourseRegistrationService.GetStudentQueue(studentCourse.AnnouncementSectionId,
                    studentCourse.AnnouncementSection.Places, studentUserId);
            }

            var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(selectedOrganizationId, studentUserId);

            ViewBag.Semester = $"{EnumExtentions.GetDisplayName((enu_Season)semester.Season)} {semester.Year}";
            ViewBag.StudentName = _userManager.GetUserFullName(studentUserId);
            if (studentOrgInfo != null)
            {
                ViewBag.StudentId = studentOrgInfo.StudentId;
                ViewBag.StudentGroup = studentOrgInfo.DepartmentGroup?.Department?.Code + studentOrgInfo.DepartmentGroup?.Code;
            }

            HtmlToPdf converter = new HtmlToPdf();
            var html = this.RenderViewAsync("PrintToPDF", model, true).GetAwaiter().GetResult();

            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            PdfDocument doc = converter.ConvertHtmlString(html, baseUrl);
            using var ms = new MemoryStream();
            doc.Save(ms);

            return File(ms.ToArray(), "application/pdf");
            //return View(model);
        }
        

        public IActionResult PrintAddDropToPDF(int semesterId)
        {
            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            return PrintAddDropToPDFById(semesterId, user.Id);
        }

        public IActionResult PrintAddDropToPDFById(int semesterId, string studentUserId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var semester = _semesterService.GetSemester(selectedOrganizationId, semesterId);
            if (semester == null)
                throw new Exception("Semester not found");

            var model = _studentCourseTempService.GetStudentCoursesByStudentUserIdTemp(semesterId, studentUserId);
            
            foreach (var studentCourse in model) 
            {
                studentCourse.Queue = _studentCourseRegistrationService.GetStudentQueue(studentCourse.AnnouncementSectionId,
                    studentCourse.AnnouncementSection.Places, studentUserId);
            }

            var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(selectedOrganizationId, studentUserId);

            ViewBag.Semester = $"{EnumExtentions.GetDisplayName((enu_Season)semester.Season)} {semester.Year}";
            ViewBag.StudentName = _userManager.GetUserFullName(studentUserId);
            if (studentOrgInfo != null)
            {
                ViewBag.StudentId = studentOrgInfo.StudentId;
                ViewBag.StudentGroup = studentOrgInfo.DepartmentGroup?.Department?.Code + studentOrgInfo.DepartmentGroup?.Code;
            }

            HtmlToPdf converter = new HtmlToPdf();
            var html = this.RenderViewAsync("PrintAddDropToPDF", model, true).GetAwaiter().GetResult();

            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            PdfDocument doc = converter.ConvertHtmlString(html, baseUrl);
            using var ms = new MemoryStream();
            doc.Save(ms);

            return File(ms.ToArray(), "application/pdf");
            //return View(model);
        }

        public IActionResult GetStudentCourses(int semesterId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            if (semesterId == 0)
                semesterId = _envarSettingService.GetCurrentSemester(selectedOrganizationId);

            var semester = _semesterService.GetSemester(selectedOrganizationId, semesterId);

            if (semester == null)
                throw new Exception($"Semester with id {semesterId} not found");

            var semesters = _semesterService.GetSemesters(selectedOrganizationId);

            ApplicationUser user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
                return NotFound();

            string userId = user.Id;

            var model = _studentCourseTempService.GetStudentCoursesByStudentUserIdTemp(semester.Id, userId)
                    .Where(x => x.State != (int)enu_CourseState.Dropped)
                    .OrderBy(x => x.AnnouncementSection.Course.CourseType != (int)enu_CourseType.TermPaper)
                    .ToList();
            
            ViewBag.Semester = $"{EnumExtentions.GetDisplayName((enu_Season)semester.Season)} {semester.Year}";
            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semester.Id);

            return View(model);
        }
    }
}
