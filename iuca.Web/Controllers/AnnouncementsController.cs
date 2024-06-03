using iuca.Application.Constants;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Services.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class AnnouncementsController : Controller
    {
        private readonly IAnnouncementService _announcementService;
        private readonly ISemesterService _semesterService;
        private readonly IOrganizationService _organizationService;
        private readonly IDepartmentService _departmentService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IInstructorInfoService _instructorInfoService;
        private readonly IProposalCourseService _proposalCourseService;
        private readonly IEnvarSettingService _envarSettingService;

        public AnnouncementsController(IAnnouncementService announcementService,
            ISemesterService semesterService,
            IOrganizationService organizationService,
            IDepartmentService departmentService,
            IDepartmentGroupService departmentGroupService,
            IInstructorInfoService instructorInfoService,
            IProposalCourseService proposalCourseService,
            IEnvarSettingService envarSettingService)
        {
            _announcementService = announcementService;
            _semesterService = semesterService;
            _organizationService = organizationService;
            _departmentService = departmentService;
            _departmentGroupService = departmentGroupService;
            _instructorInfoService = instructorInfoService;
            _proposalCourseService = proposalCourseService;
            _envarSettingService = envarSettingService;
        }

        [Authorize(Policy = Permissions.Announcements.View)]
        public IActionResult Index(int searchSemesterId, int searchDepartmentId, int searchCourseType, int searchIsForAll, int searchIsActivated)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semesterId = SemesterSelectList(selectedOrganization, searchSemesterId);

            var announcements = _announcementService.GetAnnouncementsForAnnouncementInfo(semesterId, searchDepartmentId, searchCourseType, searchIsForAll, searchIsActivated);

            DepartmentSelectList(selectedOrganization, searchDepartmentId);
            CourseTypeSelectList(searchCourseType);
            IsForAllSelectList(searchIsForAll);
            IsActivatedSelectList(searchIsActivated);

            return View(announcements);
        }

        [Authorize(Policy = Permissions.Announcements.View)]
        public ViewResult GetControls(int announcementId)
        {
            var announcement = _announcementService.GetAnnouncementForAnnouncementControls(announcementId);

            return View("_ControlsPartial", announcement);
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        [HttpPost]
        public IActionResult SetForAllValue(int announcementId, bool isForAll)
        {
            try
            {
                _announcementService.SetAnnouncementForAllValue(announcementId, isForAll);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Announcements.View)]
        public ViewResult GetSections(int announcementId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var announcement = _announcementService.GetAnnouncement(announcementId);
            InstructorIdSelectList(selectedOrganization, announcement.ProposalCourse);
            DepartmentGroupSelectList(selectedOrganization);

            return View("_EditSectionsPartial", announcement);
        }

        [Authorize(Policy = Permissions.Announcements.View)]
        public ViewResult GetInstructors(int announcementId)
        {
            var announcement = _announcementService.GetAnnouncement(announcementId);

            return View("_EditInstructorsPartial", announcement.ProposalCourse);
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        public IActionResult EditSection(int announcementId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var announcement = _announcementService.GetAnnouncement(announcementId);
            InstructorIdSelectList(selectedOrganization, announcement.ProposalCourse);
            DepartmentGroupSelectList(selectedOrganization);

            var newAnnouncementSection = new AnnouncementSectionDTO()
            {
                AnnouncementId = announcement.Id
            };

            return PartialView("_EditAnnouncementSectionPartial", newAnnouncementSection);
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        public IActionResult EditInstructor()
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            InstructorIdSelectList(selectedOrganization, null);

            return PartialView("_EditInstructorPartial", new UserDTO());
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        [HttpPost]
        public PartialViewResult EditSections(AnnouncementDTO newAnnouncement)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _announcementService.EditAnnouncementSections(newAnnouncement.Id, newAnnouncement.AnnouncementSections);
                    TempData["AnnouncementSuccessMessage"] = "Announcement saving succeeded!";
                }
                catch (ModelValidationException ex)
                {
                    TempData["AnnouncementError"] = ex.Message;
                }
            }
            else
            {
                TempData["AnnouncementErrorMessage"] = "Announcement saving failed.";
            }

            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var announcement = _announcementService.GetAnnouncement(newAnnouncement.Id);
            InstructorIdSelectList(selectedOrganization, announcement.ProposalCourse);
            DepartmentGroupSelectList(selectedOrganization);

            return PartialView("_EditSectionsFormPartial", newAnnouncement);
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        [HttpPost]
        public PartialViewResult EditInstructors(ProposalCourseDTO newProposalCourse)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newInstructorIds = newProposalCourse.Instructors != null ? newProposalCourse.Instructors.Select(x => x.Id) : new List<string>();
                    _proposalCourseService.EditProposalCourseInstructors(newProposalCourse.Id, newInstructorIds);
                    TempData["AnnouncementSuccessMessage"] = "Instructors saving succeeded!";
                }
                catch (ModelValidationException ex)
                {
                    TempData["AnnouncementError"] = ex.Message;
                }
            }
            else
            {
                TempData["AnnouncementErrorMessage"] = "Instructors saving failed.";
            }

            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            InstructorIdSelectList(selectedOrganization, null);

            return PartialView("_EditInstructorsFormPartial", newProposalCourse);
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        public IActionResult ReplaceInstructor()
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            InstructorIdSelectList(selectedOrganization, null);

            return PartialView("_ReplaceInstructorPartial");
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        [HttpPost]
        public IActionResult ReplaceProposalInstructor(int proposalCourseId, string previousInstructorId, string futureInstructorId)
        {
            try
            {
                _proposalCourseService.ReplaceProposalCourseInstructor(proposalCourseId, previousInstructorId, futureInstructorId);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        [HttpPost]
        public IActionResult ActivateAll(int[] ids)
        {
            try
            {
                _announcementService.SetAnnouncementStatuses(ids, true);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        [HttpPost]
        public IActionResult Activate(int id)
        {
            try
            {
                _announcementService.SetAnnouncementStatuses(new int[] { id }, true);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        [HttpPost]
        public IActionResult Deactivate(int id)
        {
            try
            {
                _announcementService.SetAnnouncementStatuses(new int[] { id }, false);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private int SemesterSelectList(int selectedOrganization, int searchSemesterId)
        {
            var semesters = _semesterService.GetSemesters(selectedOrganization);

            if (searchSemesterId == -1)
                searchSemesterId = _envarSettingService.GetUpcomingSemester(selectedOrganization);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", searchSemesterId);

            return searchSemesterId;
        }

        private void DepartmentSelectList(int selectedOrganization, int searchDepartmentId)
        {
            var departments = _departmentService.GetDepartments(selectedOrganization, true);

            ViewBag.Departments = new SelectList(departments, "Id", "Code", searchDepartmentId);
        }

        private void CourseTypeSelectList(int searchCourseType = -1)
        {
            var courseTypes = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "-1",
                    Text = "All types"
                },
                new SelectListItem
                {
                    Value = ((int)enu_CourseType.Standart).ToString(),
                    Text = enu_CourseType.Standart.ToString()
                },
                new SelectListItem
                {
                    Value = ((int)enu_CourseType.General).ToString(),
                    Text = enu_CourseType.General.ToString()
                },
                new SelectListItem
                {
                    Value = ((int)enu_CourseType.TermPaper).ToString(),
                    Text = enu_CourseType.TermPaper.ToString()
                }
            };

            ViewBag.CourseTypes = new SelectList(courseTypes, "Value", "Text", searchCourseType);
        }

        private void IsForAllSelectList(int searchIsForAll)
        {
            var statuses = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)enu_Status.All).ToString(),
                    Text = "All (for all)"
                },
                new SelectListItem
                {
                    Value = ((int)enu_Status.True).ToString(),
                    Text = "True"
                },
                new SelectListItem
                {
                    Value = ((int)enu_Status.False).ToString(),
                    Text = "False"
                }
            };

            ViewBag.IsForAllStatuses = new SelectList(statuses, "Value", "Text", searchIsForAll);
        }

        private void IsActivatedSelectList(int searchIsActivated)
        {
            var statuses = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)enu_Status.All).ToString(),
                    Text = "All (active status)"
                },
                new SelectListItem
                {
                    Value = ((int)enu_Status.True).ToString(),
                    Text = "Activated"
                },
                new SelectListItem
                {
                    Value = ((int)enu_Status.False).ToString(),
                    Text = "Deactivated"
                }
            };

            ViewBag.IsActivatedStatuses = new SelectList(statuses, "Value", "Text", searchIsActivated);
        }

        private void DepartmentGroupSelectList(int selectedOrganization)
        {
            var departmentGroups = _departmentGroupService.GetDepartmentGroups(selectedOrganization)
                .OrderBy(x => x.Department.Code).ThenByDescending(x => x.Year)
                .Select(x => new { Value = x.Id.ToString(), Text = x.DepartmentCode });

            ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Value", "Text");
        }

        private void InstructorIdSelectList(int selectedOrganization, ProposalCourseDTO proposalCourse)
        {
            if (proposalCourse != null)
            {
                var instructors = proposalCourse.Instructors
                    .Select(x => new { Value = x.Id, Text = x.FullName }).ToList();

                var defaultInstructor = _envarSettingService.GetDefaultInstructor(selectedOrganization);
                instructors.Add(new { Value = defaultInstructor.Id, Text = defaultInstructor.FullName + " (default)" });

                ViewBag.InstructorIds = new SelectList(instructors, "Value", "Text");
            } 
            else
            {
                var instructors = _instructorInfoService.GetInstructorInfoList(selectedOrganization, enu_InstructorState.Active, null);

                ViewBag.InstructorIds = new SelectList(instructors, "InstructorUserId", "FullNameEng");
            }
        }
    }
}
