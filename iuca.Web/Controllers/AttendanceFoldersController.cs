using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class AttendanceFoldersController : Controller
    {
        private readonly IAttendanceFolderService _attendanceFolderService;
        private readonly ISemesterService _semesterService;
        private readonly IOrganizationService _organizationService;

        public AttendanceFoldersController(IAttendanceFolderService attendanceFolderService, 
            ISemesterService semesterService,
            IOrganizationService organizationService)
        {
            _attendanceFolderService = attendanceFolderService;
            _semesterService = semesterService;
            _organizationService = organizationService;
        }

        [Authorize(Policy = Permissions.AttendanceFolders.View)]
        public IActionResult Index()
        {
            var attendanceFolders = _attendanceFolderService.GetAttendanceFolders();

            return View(attendanceFolders);
        }

        [Authorize(Policy = Permissions.AttendanceFolders.Edit)]
        public IActionResult Create()
        {
            SemesterSelectList();

            return View();
        }

        [Authorize(Policy = Permissions.AttendanceFolders.Edit)]
        [HttpPost]
        public IActionResult Create(AttendanceFolderDTO attendanceFolderDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _attendanceFolderService.CreateAttendanceFolder(attendanceFolderDTO);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            SemesterSelectList();

            return View(attendanceFolderDTO);
        }

        [Authorize(Policy = Permissions.AttendanceFolders.Edit)]
        public IActionResult Edit(int id)
        {
            SemesterSelectList();
            var attendanceFolder = _attendanceFolderService.GetAttendanceFolder(id);

            return View(attendanceFolder);
        }

        [Authorize(Policy = Permissions.AttendanceFolders.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, AttendanceFolderDTO attendanceFolderDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _attendanceFolderService.EditAttendanceFolder(id, attendanceFolderDTO);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            SemesterSelectList();

            return View(attendanceFolderDTO);
        }

        [Authorize(Policy = Permissions.AttendanceFolders.Edit)]
        public IActionResult Delete(int id)
        {
            _attendanceFolderService.DeleteAttendanceFolder(id);
            return RedirectToAction("Index");
        }

        private void SemesterSelectList()
        {
            var organizationId = _organizationService.GetSelectedOrganization(User);
            var semesters = _semesterService.GetSemesters(organizationId);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear");
        }
    }
}
