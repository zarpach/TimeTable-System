using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Constants;
using iuca.Application.Enums;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using iuca.Application.Interfaces.Common;
using iuca.Infrastructure.Identity;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.UserInfo;
using System.Linq;
using iuca.Application.DTO.Courses;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.DTO.Common;
using System.Collections.Generic;
using System;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class ProposalsController : Controller
    {
        private readonly IProposalService _proposalService;
        private readonly ISemesterService _semesterService;
        private readonly IDepartmentService _departmentService;
        private readonly IOrganizationService _organizationService;
        private readonly IInstructorInfoService _instructorInfoService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserRolesService _userRolesService;
        private readonly IDeanService _deanService;
        private readonly IAnnouncementService _announcementService;
        private readonly IProposalCourseService _proposalCourseService;
        private readonly IEnvarSettingService _envarSettingService;

        public ProposalsController(
            IProposalService proposalService,
            ISemesterService semesterService,
            IDepartmentService departmentService,
            IOrganizationService organizationService,
            IInstructorInfoService instructorInfoService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService,
            IDeanService deanService,
            IProposalCourseService proposalCourseService,
            IAnnouncementService announcementService,
            IEnvarSettingService envarSettingService)
        {
            _proposalService = proposalService;
            _proposalCourseService = proposalCourseService;
            _semesterService = semesterService;
            _departmentService = departmentService;
            _organizationService = organizationService;
            _instructorInfoService = instructorInfoService;
            _userManager = userManager;
            _userRolesService = userRolesService;
            _deanService = deanService;
            _announcementService = announcementService;
            _envarSettingService = envarSettingService;
        }

        [Authorize(Policy = Permissions.Proposals.View)]
        public IActionResult Index(int searchSemesterId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var semesterId = SemesterSelectList(selectedOrganization, searchSemesterId);
            var departments = DepartmentsSelectList(selectedOrganization);

            var proposals = _proposalService.GetProposals(semesterId, departments);

            return View(proposals);
        }

        [Authorize(Policy = Permissions.Announcements.View)]
        public IActionResult ManageProposals(int searchSemesterId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var semesterId = SemesterSelectList(selectedOrganization, searchSemesterId);
            var proposals = _proposalService.GetProposals(semesterId, null);

            return View(proposals);
        }

        [Authorize(Policy = Permissions.Proposals.View)]
        public IActionResult Details(int id, int searchSemesterId)
        {
            var proposal = _proposalService.GetProposal(id);

            ViewBag.SemesterId = searchSemesterId;

            return View(proposal);
        }

        [Authorize(Policy = Permissions.Proposals.View)]
        public IActionResult ManageDetails(int id, int searchSemesterId)
        {
            var proposal = _proposalService.GetProposal(id);

            proposal.ProposalCourses = proposal.ProposalCourses.Where(x => !(x.Status == (int)enu_ProposalCourseStatus.New));

            ViewBag.SemesterId = searchSemesterId;

            return View(proposal);
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        public IActionResult Create(int searchSemesterId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            if (searchSemesterId == 0)
                searchSemesterId = _envarSettingService.GetUpcomingSemester(selectedOrganization);
            ViewBag.Semesters = new SelectList(_semesterService.GetSemesters(selectedOrganization), "Id", "SeasonYear", searchSemesterId);
            ViewBag.SemesterId = searchSemesterId;

            DepartmentsSelectList(selectedOrganization);

            var newProposal = new ProposalDTO();

            return View(newProposal);
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        [HttpPost]
        public IActionResult Create(ProposalDTO newProposal, int searchSemesterId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var semesterId = _envarSettingService.GetUpcomingSemester(selectedOrganization);
            var semester = _semesterService.GetSemester(semesterId);
            ViewBag.Semesters = new SelectList(new List<SemesterDTO> { semester }, "Id", "SeasonYear", searchSemesterId);
            ViewBag.SemesterId = searchSemesterId;
            DepartmentsSelectList(selectedOrganization);
            InstructorSelectList(selectedOrganization);

            if (ModelState.IsValid)
            {
                try
                {
                    _proposalService.CreateProposal(newProposal);
                    TempData["ProposalSuccessMessage"] = "Proposal creation succeeded!";
                    return RedirectToAction("Index", new
                    {
                        searchSemesterId = searchSemesterId
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["ProposalError"] = ex.Message;
                }
            }
            else
            {
                TempData["ProposalErrorMessage"] = "Proposal creation failed.";
            }

            if (newProposal.ProposalCourses != null) 
            {
                foreach (var newProposalCourse in newProposal.ProposalCourses)
                    newProposalCourse.Course = _proposalService.GetCourseFromSelection(selectedOrganization, newProposalCourse.CourseId);
            }

            return View(newProposal);
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        public IActionResult Edit(int id, int searchSemesterId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            SemesterSelectList(selectedOrganization, searchSemesterId);
            DepartmentsSelectList(selectedOrganization);
            InstructorSelectList(selectedOrganization);

            var proposal = _proposalService.GetProposal(id);

            return View(proposal);
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, ProposalDTO newProposal, int searchSemesterId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            SemesterSelectList(selectedOrganization, searchSemesterId);
            DepartmentsSelectList(selectedOrganization);
            InstructorSelectList(selectedOrganization);

            if (ModelState.IsValid)
            {
                try
                {
                    _proposalService.EditProposal(id, newProposal);
                    _proposalService.EditProposalCourses(id, newProposal.ProposalCourses);
                    TempData["ProposalSuccessMessage"] = "Proposal saving succeeded!";
                    return RedirectToAction("Edit", new
                    {
                        id = id,
                        searchSemesterId = searchSemesterId
                    });
                }
                catch (ModelValidationException ex)
                {
                    TempData["ProposalError"] = ex.Message;
                }
            }
            else
            {
                TempData["ProposalErrorMessage"] = "Proposal saving failed.";
            }

            foreach (var newProposalCourse in newProposal.ProposalCourses)
                newProposalCourse.Course = _proposalService.GetCourseFromSelection(selectedOrganization, newProposalCourse.CourseId);

            return View(newProposal);
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        [HttpPost]
        public void SubmitForApprovalAll(int id)
        {
            try
            {
                _proposalCourseService.SetProposalCourseStatuses(id, (int)enu_ProposalCourseStatus.Pending);
            }
            catch (ModelValidationException ex)
            {
                TempData["ProposalError"] = ex.Message;
            }
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        [HttpPost]
        public void ReturnFromApprovalAll(int id)
        {
            try
            {
                _proposalCourseService.SetProposalCourseStatuses(id, (int)enu_ProposalCourseStatus.New);
            }
            catch (ModelValidationException ex)
            {
                TempData["ProposalError"] = ex.Message;
            }
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        [HttpPost]
        public void ApproveAll(int id)
        {
            try
            {
                _proposalCourseService.SetProposalCourseStatuses(id, (int)enu_ProposalCourseStatus.Approved);
            }
            catch (ModelValidationException ex)
            {
                TempData["ProposalError"] = ex.Message;
            }
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        [HttpPost]
        public IActionResult SubmitForApproval(int proposalCourseId)
        {
            bool success = false;
            string msg = string.Empty;

            try
            {
                _proposalCourseService.SubmitProposalCourseForApproval(proposalCourseId);
                success = true;
                msg = "Course is submitted";
            }
            catch (ModelValidationException ex)
            {
                msg = ex.Message;
            }
            catch (Exception ex)
            {
                msg = "Saving failed";
            }

            return Json(new { Success = success, Message = msg });
           
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        [HttpPost]
        public IActionResult ReturnFromApproval(int proposalCourseId)
        {
            bool success = false;
            string msg = string.Empty;

            try
            {
                _proposalCourseService.ReturnProposalCourseFromApproval(proposalCourseId);
                success = true;
                msg = "Course is returned";
            }
            catch (ModelValidationException ex)
            {
                msg = ex.Message;
            }
            catch (Exception ex) 
            {
                msg = "Saving failed";
            }

            return Json(new { Success = success, Message = msg });

        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        [HttpPost]
        public IActionResult Approve(int id)
        {
            try
            {
                _proposalCourseService.SetProposalCourseStatus(id, (int)enu_ProposalCourseStatus.Approved);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Announcements.Edit)]
        [HttpPost]
        public IActionResult Reject(int id)
        {
            try
            {
                _proposalCourseService.SetProposalCourseStatus(id, (int)enu_ProposalCourseStatus.Rejected);
                return Json(new { success = true });
            }
            catch (ModelValidationException ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        [HttpPost]
        public void Delete(int id)
        {
            try
            {
                _proposalService.DeleteProposal(id);
            }
            catch (ModelValidationException ex)
            {
                TempData["ProposalError"] = ex.Message;
            }
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        public ViewResult GetCoursesForSelection(int semesterId, int departmentId, int[] excludedCourseIds)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var semester = _semesterService.GetSemester(selectedOrganization, semesterId);

            var courses = _proposalService.GetCoursesForSelection(selectedOrganization, excludedCourseIds);
  
            ViewBag.Departments = new SelectList(_departmentService.GetDepartments(selectedOrganization, true),
                "Id", "Code", departmentId);
            ViewBag.Semester = semester.SeasonYear;

            return View("_SelectCoursesPartial", courses);
        }

        [Authorize(Policy = Permissions.Proposals.Edit)]
        public IActionResult GetCourseFromSelection(int selectedCourseId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var course = _proposalService.GetCourseFromSelection(selectedOrganization, selectedCourseId);

            var newCourseProposal = new ProposalCourseDTO
            {
                CourseId = course.Id,
                Course = course
            };

            InstructorSelectList(selectedOrganization);

            return PartialView("_EditProposalsPartial", newCourseProposal);
        }

        private IEnumerable<DepartmentDTO> DepartmentsSelectList(int selectedOrganization)
        {
            var departments = _departmentService.GetDepartments(selectedOrganization, true);

            ApplicationUser user = _userManager.GetUserAsync(User).Result;

            if (_userRolesService.IsUserInRole(user.Id, selectedOrganization, enu_Role.Dean))
                departments = _deanService.GetDeanDepartments(selectedOrganization, user.Id).Departments;

            ViewBag.Departments = new SelectList(departments, "Id", "Code");

            return departments;
        }

        private int SemesterSelectList(int selectedOrganization, int searchSemesterId)
        {
            var semesters = _semesterService.GetSemesters(selectedOrganization);

            if (searchSemesterId == -1)
                searchSemesterId = _envarSettingService.GetUpcomingSemester(selectedOrganization);

            ViewBag.Semesters = new SelectList(semesters, "Id", "SeasonYear", searchSemesterId);
            ViewBag.SemesterId = searchSemesterId;

            return searchSemesterId;
        }

        private void InstructorSelectList(int selectedOrganization)
        {
            var instructors = _instructorInfoService.GetInstructorInfoList(selectedOrganization, enu_InstructorState.Active, null)
                .Select(x => new { Value = x.InstructorUserId, Text = x.FullNameEng });

            ViewBag.Instructors = new SelectList(instructors, "Value", "Text");
        }
    }
}
