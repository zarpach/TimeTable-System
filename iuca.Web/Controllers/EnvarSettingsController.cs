using iuca.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Users.Instructors;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class EnvarSettingsController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly IEnvarSettingService _envarSettingService;
        private readonly IInstructorInfoService _instructorInfoService;
        private readonly ISemesterService _semesterService;

        public EnvarSettingsController(IOrganizationService organizationService,
            IEnvarSettingService envarSettingService,
            IInstructorInfoService instructorInfoService,
            ISemesterService semesterService)
        {
            _organizationService = organizationService;
            _envarSettingService = envarSettingService;
            _instructorInfoService = instructorInfoService;
            _semesterService = semesterService;
        }

        public IActionResult Index()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            InstructorSelectList(selectedOrganizationId);
            SemesterSelectList(selectedOrganizationId);

            return View(_envarSettingService.GetEnvarSettings(selectedOrganizationId));
        }

        [HttpPost]
        public void SetMaxRegistrationCredits(int id, int maxRegistrationCredits) 
        {
            _envarSettingService.SetMaxRegistrationCredits(id, maxRegistrationCredits);
        }

        [HttpPost]
        public void SetDefaultInstructor(int id, string defaultInstructorId)
        {
            _envarSettingService.SetDefaultInstructor(id, defaultInstructorId);
        }

        [HttpPost]
        public void SetCurrentSemester(int id, int semesterId)
        {
            _envarSettingService.SetCurrentSemester(id, semesterId);
        }

        [HttpPost]
        public void SetUpcomingSemester(int id, int semesterId)
        {
            _envarSettingService.SetUpcomingSemester(id, semesterId);
        }

        private void InstructorSelectList(int selectedOrganization)
        {
            var instructors = _instructorInfoService.GetInstructorInfoList(selectedOrganization, enu_InstructorState.Active, null);
            ViewBag.Instructors = new SelectList(instructors, "InstructorUserId", "FullNameEng");
        }

        private void SemesterSelectList(int selectedOrganization)
        {
            var semesters = _semesterService.GetSemesters(selectedOrganization);
            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear");
        }
    }
}
