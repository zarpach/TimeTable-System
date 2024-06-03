using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using iuca.Application.Interfaces.Courses;
using iuca.Application.DTO.Courses;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Enums;
using iuca.Web.Extensions;
using SelectPdf;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using iuca.Application.Constants;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.UserInfo;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class SyllabiController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly ISyllabusService _syllabusService;
        private readonly IRegistrationCourseService _registrationCourseService;
        private readonly ISemesterService _semesterService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserRolesService _userRolesService;
        private readonly IDeanService _deanService;
        private readonly IDepartmentService _departmentService;
        private readonly IEnvarSettingService _envarSettingService;

        public SyllabiController(IOrganizationService organizationService,
            ISyllabusService syllabusService,
            IRegistrationCourseService registrationCourseService,
            ISemesterService semesterService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService,
            IDeanService deanService,
            IDepartmentService departmentService,
            IEnvarSettingService envarSettingService)
        {
            _organizationService = organizationService;
            _syllabusService = syllabusService;
            _registrationCourseService = registrationCourseService;
            _semesterService = semesterService;
            _userManager = userManager;
            _userRolesService = userRolesService;
            _deanService = deanService;
            _departmentService = departmentService;
            _envarSettingService = envarSettingService;
        }

        [Authorize(Policy = Permissions.SyllabiEditor.View)]
        public IActionResult Index(int registrationCourseId, int semesterId, string instructorUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var syllabus = _syllabusService.GetSyllabusByRegistrationCourseId(registrationCourseId);
            if (syllabus != null)
                syllabus.SyllabusDetails = _syllabusService.GetSyllabusDetails(selectedOrganization, registrationCourseId);

            ViewBag.RegistrationCourseId = registrationCourseId;
            ViewBag.SemesterId = semesterId;
            ViewBag.InstructorUserId = instructorUserId;

            return View(syllabus);
        }

        [Authorize(Policy = Permissions.SyllabiApprover.View)]
        public IActionResult ManageSyllabi(int semesterId, int departmentId, int? status)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var semesters = _semesterService.GetSemesters(selectedOrganization);
            if (semesterId == 0)
                semesterId = _envarSettingService.GetCurrentSemester(selectedOrganization);

            var departments = _departmentService.GetDepartments(selectedOrganization, true);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return NotFound();

            if (_userRolesService.IsUserInRole(user.Id, selectedOrganization, enu_Role.Dean))
                departments = _deanService.GetDeanDepartments(selectedOrganization, user.Id).Departments;

            var registrationCourses = _registrationCourseService.GetRegistrationCoursesWithSyllabi(selectedOrganization, semesterId, departmentId, departments, status);

            ViewBag.SemesterId = semesterId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", semesterId);
            ViewBag.Departments = new SelectList(departments, "Id", "Code", departmentId);

            return View(registrationCourses);
        }

        public IActionResult Details(int id)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var syllabus = _syllabusService.GetSyllabusById(id);
            syllabus.SyllabusDetails = _syllabusService.GetSyllabusDetails(selectedOrganization, syllabus.RegistrationCourseId);

            return View(syllabus);
        }

        [Authorize(Policy = Permissions.SyllabiEditor.Edit)]
        public IActionResult Create(int registrationCourseId, int language, int semesterId, string instructorUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var newSyllabus = new SyllabusDTO();
            newSyllabus.SyllabusDetails = _syllabusService.GetSyllabusDetails(selectedOrganization, registrationCourseId);
            newSyllabus.RegistrationCourseId = registrationCourseId;

            ViewData["Language"] = language;
            ViewBag.SemesterId = semesterId;
            ViewBag.InstructorUserId = instructorUserId;

            return View(newSyllabus);
        }

        [Authorize(Policy = Permissions.SyllabiEditor.Edit)]
        [HttpPost]
        public IActionResult Create(SyllabusDTO newSyllabus, int language, int semesterId, string instructorUserId)
        {
            ViewData["Language"] = language;
            ViewBag.SemesterId = semesterId;
            ViewBag.InstructorUserId = instructorUserId;

            if (ModelState.IsValid)
            {
                try
                {
                    _syllabusService.CreateSyllabus(newSyllabus);
                    return RedirectToAction("Index", new
                    {
                        registrationCourseId = newSyllabus.RegistrationCourseId,
                        semesterId = semesterId,
                        instructorUserId = instructorUserId
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["SyllabiError"] = ex.Message;
                }
            }
            else
            {
                TempData["SyllabiErrorMessage"] = "Syllabus creation failed.";
            }

            return View(newSyllabus);
        }

        [Authorize(Policy = Permissions.SyllabiEditor.Edit)]
        public IActionResult Edit(int id, int semesterId, string instructorUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var syllabus = _syllabusService.GetSyllabusById(id);
            syllabus.SyllabusDetails = _syllabusService.GetSyllabusDetails(selectedOrganization, syllabus.RegistrationCourseId);

            ViewData["Language"] = syllabus.Language;
            ViewBag.SemesterId = semesterId;
            ViewBag.InstructorUserId = instructorUserId;

            return View(syllabus);
        }

        [Authorize(Policy = Permissions.SyllabiEditor.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, SyllabusDTO newSyllabus, int semesterId, string instructorUserId)
        {
            ViewData["Language"] = newSyllabus.Language;
            ViewBag.SemesterId = semesterId;
            ViewBag.InstructorUserId = instructorUserId;

            if (ModelState.IsValid)
            {
                try
                {
                    _syllabusService.EditSyllabus(id, newSyllabus);
                    TempData["SyllabiSuccessMessage"] = "Syllabus saving succeeded!";
                    return RedirectToAction("Edit", new { 
                        id = id,
                        semesterId = semesterId,
                        instructorUserId = instructorUserId
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["SyllabiError"] = ex.Message;
                }
            }
            else
            {
                TempData["SyllabiErrorMessage"] = "Syllabus saving failed.";
            }

            return View(newSyllabus);
        }

        [Authorize(Policy = Permissions.SyllabiEditor.Edit)]
        public IActionResult EditAcademicPolicy()
        {
            return PartialView("_EditAcademicPolicyPartial", new AcademicPolicyDTO());
        }

        [Authorize(Policy = Permissions.SyllabiEditor.Edit)]
        public IActionResult EditCourseRequirement(int language)
        {
            ViewData["language"] = language;
            return PartialView("_EditCourseRequirementPartial", new CourseRequirementDTO());
        }

        [Authorize(Policy = Permissions.SyllabiEditor.Edit)]
        public IActionResult EditCourseCalendarRow()
        {
            return PartialView("_EditCourseCalendarRowPartial", new CourseCalendarRowDTO());
        }

        [Authorize(Policy = Permissions.SyllabiEditor.Edit)]
        [HttpPost]
        public void Delete(int id)
        {
            _syllabusService.DeleteSyllabus(id);
        }

        [Authorize(Policy = Permissions.SyllabiEditor.Edit)]
        [HttpPost]
        public void SubmitForApproval(int id, string instructorComment)
        {
            _syllabusService.SetSyllabusStatus(id, (int)enu_SyllabusStatus.Pending);
            _syllabusService.SetSyllabusInstructorComment(id, instructorComment);
        }

        [Authorize(Policy = Permissions.SyllabiEditor.Edit)]
        [HttpPost]
        public void ReturnFromApproval(int id)
        {
            _syllabusService.SetSyllabusStatus(id, (int)enu_SyllabusStatus.Draft);
        }

        [Authorize(Policy = Permissions.SyllabiApprover.Edit)]
        [HttpPost]
        public void Approve(int id, string approverComment)
        {
            _syllabusService.SetSyllabusStatus(id, (int)enu_SyllabusStatus.Approved);
            _syllabusService.SetSyllabusApproverComment(id, approverComment);
        }

        [Authorize(Policy = Permissions.SyllabiApprover.Edit)]
        [HttpPost]
        public void Reject(int id, string approverComment)
        {
            _syllabusService.SetSyllabusStatus(id, (int)enu_SyllabusStatus.Rejected);
            _syllabusService.SetSyllabusApproverComment(id, approverComment);
        }

        public IActionResult Print(int id)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var syllabus = _syllabusService.GetSyllabusById(id);
            syllabus.SyllabusDetails = _syllabusService.GetSyllabusDetails(selectedOrganization, syllabus.RegistrationCourseId);

            HtmlToPdf converter = new HtmlToPdf();

            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginTop = 50;
            converter.Options.MarginRight = 50;
            converter.Options.MarginBottom = 50;
            converter.Options.MarginLeft = 50;

            var html = this.RenderViewAsync("Print", syllabus, true).GetAwaiter().GetResult();
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            PdfDocument doc = converter.ConvertHtmlString(html, baseUrl);

            MemoryStream stream = new MemoryStream();
            doc.Save(stream);

            stream.Position = 0;

            string fileName = "Syllabus - " + syllabus.SyllabusDetails.CourseNameEng;
            Response.Headers["Content-Disposition"] = "inline; filename=" + fileName;
            return File(stream.ToArray(), "application/pdf");
        }
    }
}
