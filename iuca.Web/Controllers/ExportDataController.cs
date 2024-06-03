using iuca.Application.Constants;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.ExportData;
using iuca.Application.ViewModels.Common;
using iuca.Application.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class ExportDataController : Controller
    {
        private readonly string _iucaOldDbConnection;
        private readonly string _htcOldDbConnection;

        private readonly IOrganizationService _organizationService;
        private readonly ISemesterService _semesterService;
        private readonly IExportStudentCourseService _exportStudentCourseService;
        private readonly IExportStudentGradeService _exportStudentGradesService;
        private readonly IExportRegistrationCourseService _exportRegistrationCourseService;
        private readonly IExportInstructorService _exportInstructorService;
        private readonly IExportCourseService _exportCourseService;

        public ExportDataController(IConfiguration configuration,
            IOrganizationService organizationService,
            ISemesterService semesterService,
            IExportStudentCourseService exportDataService,
            IExportStudentGradeService exportStudentGradesService,
            IExportRegistrationCourseService exportRegistrationCourseService,
            IExportInstructorService exportInstructorService,
            IExportCourseService exportCourseService)
        {
            _organizationService = organizationService;
            _semesterService = semesterService;
            _exportStudentCourseService = exportDataService;
            _exportStudentGradesService = exportStudentGradesService;
            _exportRegistrationCourseService = exportRegistrationCourseService;
            _exportInstructorService = exportInstructorService;
            _exportCourseService = exportCourseService;

            _iucaOldDbConnection = configuration.GetConnectionString("iucaOldDbConnection");
            _htcOldDbConnection = configuration.GetConnectionString("htcOldDbConnection");
        }

        [Authorize(Policy = Permissions.ExportData.View)]
        public IActionResult ExportStudentCourses()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ViewBag.Semesters = _semesterService.GetSemesters(selectedOrganizationId);

            return View();
        }

        [Authorize(Policy = Permissions.ExportData.Edit)]
        [HttpPost]
        public ExportCourseResultViewModel SynchronizeStudentCoursesBySemester(int semesterId, bool force) 
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            return _exportStudentCourseService.ExportStudentCoursesBySemester(selectedOrganizationId, 
                semesterId, connection, force);
        }

        [Authorize(Policy = Permissions.ExportData.Edit)]
        [HttpPost]
        public ExportCourseResultViewModel SynchronizeStudentCoursesByRegistrationCourse(int registrationCourseId)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            return _exportStudentCourseService.ExportStudentCoursesByRegistrationCourse(selectedOrganizationId,
                registrationCourseId, connection);
        }

        [Authorize(Policy = Permissions.ExportData.Edit)]
        [HttpPost]
        public void ExportStudentGrades(int semesterId)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            _exportStudentGradesService.ExportStudentGrades(selectedOrganizationId, semesterId, connection);
        }

        [Authorize(Policy = Permissions.ExportData.Edit)]
        [HttpPost]
        public void ExportRegistrationCourseData(int announcementSectionId)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            _exportRegistrationCourseService.ExportRegistrationCourseData(announcementSectionId, connection);
        }

        [Authorize(Policy = Permissions.ExportData.Edit)]
        [HttpPost]
        public void ExportAnnouncementSections(int semesterId)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _exportRegistrationCourseService.ExportAnnouncementSections(selectedOrganizationId, semesterId, true, connection);
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public JsonResult ExportInstructorInfo(int instructorBasicInfoId)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            var result = _exportInstructorService.ExportInstructor(selectedOrganizationId, instructorBasicInfoId, connection);

            return Json(new { importCode = result.importCode, result = result.result });
        }

        [Authorize(Policy = Permissions.Courses.Edit)]
        [HttpPost]
        public void ExportCourse(int courseId)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            _exportCourseService.ExportCourse(selectedOrganizationId, courseId, connection);
        }

        private string GetConnectionString()
        {
            string connection;
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            bool isMainOrganization = _organizationService.GetOrganization(selectedOrganizationId).IsMain;
            if (isMainOrganization)
                connection = _iucaOldDbConnection;
            else
                connection = _htcOldDbConnection;

            return connection;
        }
    }
}
