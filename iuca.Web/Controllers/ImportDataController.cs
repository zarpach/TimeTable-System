using iuca.Application.Constants;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.ImportData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class ImportDataController : Controller
    {
        private readonly string _iucaOldDbConnection;
        private readonly string _htcOldDbConnection;

        private readonly ISemesterService _semesterService;
        private readonly IImportCountryService _importCountryService;
        private readonly IImportCourseService _importCourseService;
        private readonly IImportDepartmentGroupService _importDepartmentGroupService;
        private readonly IImportDepartmentService _importDepartmentService;
        private readonly IImportGradeService _importGradeService;
        private readonly IImportLanguageService _importLanguageService;
        private readonly IImportNationalityService _importNationalityService;
        private readonly IImportStudentGradeService _importStudentGradeService;
        private readonly IImportStudentService _importStudentService;
        private readonly IImportStudentEmailService _importStudentEmailService;
        private readonly IImportUniversityService _importUniversityService;
        private readonly IImportTransferCourseService _importTransferCourseService;
        private readonly IOrganizationService _organizationService;
        private readonly IImportEducationTypeService _importEducationTypeService;
        private readonly IImportInstructorService _importInstructorService;
        private readonly IImportInstructorEmailService _importInstructorEmailService;
        private readonly IImportRegistrationCourseService _importRegistrationCourseService;
        private readonly IImportStudentCourseService _importStudentCourseService;

        public ImportDataController(IConfiguration configuration,
            ISemesterService semesterService,
            IImportCountryService importCountryService,
            IImportCourseService importCourseService,
            IImportDepartmentGroupService importDepartmentGroupService,
            IImportDepartmentService importDepartmentService,
            IImportGradeService importGradeService,
            IImportLanguageService importLanguageService,
            IImportNationalityService importNationalityService,
            IImportStudentGradeService importStudentGradeService,
            IImportStudentService importStudentService,
            IImportStudentEmailService importStudentEmailService,
            IImportUniversityService importUniversityService,
            IImportTransferCourseService importTransferCourseService,
            IOrganizationService organizationService,
            IImportEducationTypeService importEducationTypeService,
            IImportInstructorService importInstructorService,
            IImportInstructorEmailService importInstructorEmailService,
            IImportRegistrationCourseService importRegistrationCourseService,
            IImportStudentCourseService importStudentCourseService) 
        {
            _semesterService = semesterService;
            _importCountryService = importCountryService;
            _importCourseService = importCourseService;
            _importDepartmentGroupService = importDepartmentGroupService;
            _importDepartmentService = importDepartmentService;
            _importGradeService = importGradeService;
            _importLanguageService = importLanguageService;
            _importNationalityService = importNationalityService;
            _importStudentGradeService = importStudentGradeService;
            _importStudentService = importStudentService;
            _importStudentEmailService = importStudentEmailService;
            _importUniversityService = importUniversityService;
            _importTransferCourseService = importTransferCourseService;
            _organizationService = organizationService;
            _importEducationTypeService = importEducationTypeService;
            _importInstructorService = importInstructorService;
            _importInstructorEmailService = importInstructorEmailService;
            _importRegistrationCourseService = importRegistrationCourseService;
            _importStudentCourseService = importStudentCourseService;

            _iucaOldDbConnection = configuration.GetConnectionString("iucaOldDbConnection");
            _htcOldDbConnection = configuration.GetConnectionString("htcOldDbConnection");

            
        }

        [Authorize(Policy = Permissions.ImportData.View)]
        public IActionResult Index()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganizationId), "Id", "SeasonYear");
            return View();
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportGrades(bool overwrite) 
        {
            string connection = GetConnectionString();
            _importGradeService.ImportGrades(connection, overwrite);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportLanguages(bool overwrite)
        {
            string connection = GetConnectionString();
            _importLanguageService.ImportLanguages(connection, overwrite);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportDepartments(bool overwrite)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _importDepartmentService.ImportDepartments(connection, overwrite, selectedOrganizationId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportCourses(bool overwrite)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _importCourseService.ImportCourses(connection, overwrite, selectedOrganizationId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportNationalities(bool overwrite)
        {
            string connection = GetConnectionString();
            _importNationalityService.ImportNationalities(connection, overwrite);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportCountries(bool overwrite)
        {
            string connection = GetConnectionString();
            _importCountryService.ImportCountries(connection, overwrite);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportDepartmentGroups(bool overwrite)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _importDepartmentGroupService.ImportDepartmentGroups(connection, overwrite, selectedOrganizationId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportStudents(bool overwrite)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _importStudentService.ImportStudents(connection, overwrite, selectedOrganizationId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void SyncStudentsStates()
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _importStudentService.SyncStudentsStates(connection, selectedOrganizationId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void SyncStudentsGroups()
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _importStudentService.SyncStudentsGroups(connection, selectedOrganizationId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportStudentsGrades(bool overwrite)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _importStudentGradeService.ImportStudentsGrades(connection, overwrite, selectedOrganizationId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void UpdateStudentEmails(IFormFile file)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            using (var stream = file.OpenReadStream())
            {
                _importStudentEmailService.UpdateStudentEmails(selectedOrganizationId, stream);
            }

        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportUniversities(bool overwrite)
        {
            string connection = GetConnectionString();
            _importUniversityService.ImportUniversities(connection, overwrite);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportTransferCourses(bool overwrite)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _importTransferCourseService.ImportTransferCourses(connection, overwrite, selectedOrganizationId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportEducationTypes(bool overwrite)
        {
            string connection = GetConnectionString();
            _importEducationTypeService.ImportEducationTypes(connection, overwrite);
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public void ImportInstructors(bool overwrite)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            _importInstructorService.ImportInstructors(connection, overwrite, selectedOrganizationId);
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public void UpdateInstructorEmails(IFormFile file)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            using (var stream = file.OpenReadStream())
            {
                _importInstructorEmailService.UpdateInstructorEmails(selectedOrganizationId, stream);
            }
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportRegistrationCourses(bool overwrite, int semesterId)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            _importRegistrationCourseService.ImportAnnouncementSections(connection, overwrite, selectedOrganizationId, semesterId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportAnnouncementSectionData(int courseDetId)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            _importRegistrationCourseService.ImportAnnouncementSectionData(connection, selectedOrganizationId, courseDetId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportAnnouncementSectionsBySemester(bool overwrite, int semesterId)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            _importRegistrationCourseService.ImportAnnouncementSections(connection, overwrite, selectedOrganizationId, semesterId);
        }

        [Authorize(Policy = Permissions.ImportData.Edit)]
        [HttpPost]
        public void ImportStudentCourses(bool overwrite, int semesterId)
        {
            string connection = GetConnectionString();
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            _importStudentCourseService.ImportStudentCourses(connection, overwrite, selectedOrganizationId, semesterId);
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
