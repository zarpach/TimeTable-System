using iuca.Application.Constants;
using iuca.Application.DTO.Users.Instructors;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels.Users.Instructors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static iuca.Application.Constants.Permissions;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class InstructorInfoController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly IInstructorInfoService _instructorInfoService;
        private readonly IUserBasicInfoService _userBasicInfoService;
        private readonly IInstructorBasicInfoService _instructorBasicInfoService;
        private readonly IInstructorOrgInfoService _instructorOrgInfoService;
        private readonly IInstructorOtherJobInfoService _instructorOtherJobInfoService;
        private readonly IInstructorEducationInfoService _instructorEducationInfoService;
        private readonly IInstructorContactInfoService _instructorContactInfoService;
        private readonly INationalityService _nationalityService;
        private readonly IDepartmentService _departmentService;
        private readonly ICountryService _countryService;

        public InstructorInfoController(IOrganizationService organizationService,
                IInstructorInfoService instructorInfoService,
                IUserBasicInfoService userBasicInfoService,
                IInstructorBasicInfoService instructorBasicInfoService,
                IInstructorOrgInfoService instructorOrgInfoService,
                IInstructorOtherJobInfoService instructorOtherJobInfoService,
                IInstructorEducationInfoService instructorEducationInfoService,
                IInstructorContactInfoService instructorContactInfoService,
                INationalityService nationalityService,
                IDepartmentService departmentService,
                ICountryService countryService)
        {
            _organizationService = organizationService;
            _instructorInfoService = instructorInfoService;
            _userBasicInfoService = userBasicInfoService;
            _instructorBasicInfoService = instructorBasicInfoService;
            _instructorOrgInfoService = instructorOrgInfoService;
            _instructorOtherJobInfoService = instructorOtherJobInfoService;
            _instructorEducationInfoService = instructorEducationInfoService;
            _instructorContactInfoService = instructorContactInfoService;
            _nationalityService = nationalityService;
            _departmentService = departmentService;
            _countryService = countryService;
        }

        [Authorize(Policy = Permissions.Instructors.View)]
        public IActionResult Index(enu_InstructorState state, int? departmentId)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);

            ViewBag.Departments = new SelectList(_departmentService.GetDepartments(selectedOrganizationId, true)
                .OrderBy(x => x.Code), "Id", "Code", departmentId);

            ViewBag.InstructorStates = new SelectList(Enum.GetValues(typeof(enu_InstructorState)).Cast<enu_InstructorState>()
                .Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList(), "Value", "Text", ((int)state).ToString());

            ViewBag.State = state;
            ViewBag.DepartmentId = departmentId;

            return View(_instructorInfoService.GetInstructorInfoList(selectedOrganizationId, state, departmentId).ToList());
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public void RefreshInstructorStates()
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            _instructorInfoService.RefreshInstructorStates(selectedOrganization);
        }

        [Authorize(Policy = Permissions.Instructors.View)]
        public IActionResult Details(string instructorUserId, enu_InstructorState state, int? departmentId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            ViewBag.State = state;
            ViewBag.DepartmentId = departmentId;

            return View(_instructorInfoService.GetInstructorDetailsInfo(selectedOrganization, instructorUserId));
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        public IActionResult Edit(string instructorUserId, enu_InstructorState state, int? departmentId) 
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            InstructorInfoDetailsViewModel model = _instructorInfoService.GetInstructorDetailsInfo(selectedOrganization, instructorUserId);

            FillSelectLists(model, selectedOrganization);

            ViewBag.State = state;
            ViewBag.DepartmentId = departmentId;

            return View(model);
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public PartialViewResult EditUserInfo(InstructorUserInfoViewModel userInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _instructorInfoService.EditInstructorUserInfo(userInfo);
                    TempData["SuccessMessage"] = "(saved successfully)";
                }
                else
                {
                    TempData["ErrorMessage"] = "(saving failed)";
                }
            }
            catch (ModelValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                TempData["ErrorMessage"] = "(saving failed)";
            }

            return PartialView("_EditUserInfoPartial", userInfo);
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public PartialViewResult EditUserBasicInfo(UserBasicInfoDTO userBasicInfo)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                if (ModelState.IsValid)
                {
                    if (userBasicInfo.Id != 0) 
                    {
                        _userBasicInfoService.Edit(selectedOrganization, userBasicInfo);
                        _instructorBasicInfoService.SetIsChangedFlag(userBasicInfo.ApplicationUserId, true);
                    }
                    else
                        _userBasicInfoService.Create(selectedOrganization, userBasicInfo);
                    TempData["SuccessMessage"] = "(saved successfully)";
                }
                else 
                {
                    TempData["ErrorMessage"] = "(saving failed)";
                }
            }
            catch (ModelValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                TempData["ErrorMessage"] = "(saving failed)";
            }

            FillUserBasicInfoLists(userBasicInfo);

            return PartialView("_EditUserBasicInfoPartial", userBasicInfo);
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public PartialViewResult EditInstructorBasicInfo(InstructorBasicInfoDTO instructorBasicInfo)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                if (ModelState.IsValid)
                {
                    if (instructorBasicInfo.Id != 0)
                        _instructorBasicInfoService.Edit(selectedOrganization, instructorBasicInfo);
                    else
                        instructorBasicInfo = _instructorBasicInfoService.Create(selectedOrganization, instructorBasicInfo);
                    TempData["SuccessMessage"] = "(saved successfully)";
                }
                else
                {
                    TempData["ErrorMessage"] = "(saving failed)";
                }
            }
            catch (ModelValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                TempData["ErrorMessage"] = "(saving failed)";
            }

            return PartialView("_EditInstructorBasicInfoPartial", instructorBasicInfo);
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public PartialViewResult EditInstructorOrgInfo(InstructorOrgInfoDTO instructorOrgInfo)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            try
            {
                if (ModelState.IsValid)
                {
                    if (instructorOrgInfo.InstructorBasicInfoId != 0 && instructorOrgInfo.OrganizationId != 0)
                    {
                        _instructorOrgInfoService.Edit(selectedOrganization, instructorOrgInfo);
                        _instructorBasicInfoService.SetIsChangedFlag(instructorOrgInfo.InstructorBasicInfoId, true);
                        TempData["SuccessMessage"] = "(saved successfully)";
                    }
                    else
                    {
                        if (instructorOrgInfo.InstructorBasicInfoId == 0)
                        {
                            TempData["ErrorMessage"] = "(Instructor basic info is not filled)";
                        }
                        else 
                        {
                            instructorOrgInfo.OrganizationId = selectedOrganization;
                            _instructorOrgInfoService.Create(instructorOrgInfo);
                            _instructorBasicInfoService.SetIsChangedFlag(instructorOrgInfo.InstructorBasicInfoId, true);
                            TempData["SuccessMessage"] = "(saved successfully)";
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "(saving failed)";
                }
            }
            catch (ModelValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                TempData["ErrorMessage"] = "(saving failed)";
            }
            
            
            FillOrgInfoLists(selectedOrganization, instructorOrgInfo);

            return PartialView("_EditOrgInfoPartial", instructorOrgInfo);
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public PartialViewResult EditOtherJobInfo(int InstructorBasicInfoId, List<InstructorOtherJobInfoDTO> otherJobInfoList)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (InstructorBasicInfoId == 0)
                    {
                        TempData["ErrorMessage"] = "(Instructor basic info is not filled)";
                    }
                    else 
                    {
                        _instructorOtherJobInfoService.EditOtherJobInfo(InstructorBasicInfoId, otherJobInfoList);
                        _instructorBasicInfoService.SetIsChangedFlag(InstructorBasicInfoId, true);
                        TempData["SuccessMessage"] = "(saved successfully)";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "(saving failed)";
                }
            }
            catch (ModelValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                TempData["ErrorMessage"] = "(saving failed)";
            }

            ViewBag.InstructorBasicInfoId = InstructorBasicInfoId;

            return PartialView("_EditOtherJobInfoContainerPartial", otherJobInfoList);
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public PartialViewResult EditEducationInfo(int InstructorBasicInfoId, List<InstructorEducationInfoDTO> educationInfoList)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (InstructorBasicInfoId == 0)
                    {
                        TempData["ErrorMessage"] = "(Instructor basic info is not filled)";
                    }
                    else 
                    {
                        _instructorEducationInfoService.EditEducationInfo(InstructorBasicInfoId, educationInfoList);
                        _instructorBasicInfoService.SetIsChangedFlag(InstructorBasicInfoId, true);
                        TempData["SuccessMessage"] = "(saved successfully)";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "(saving failed)";
                }
            }
            catch (ModelValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                TempData["ErrorMessage"] = "(saving failed)";
            }

            ViewBag.InstructorBasicInfoId = InstructorBasicInfoId;

            return PartialView("_EditEducationInfoContainerPartial", educationInfoList);
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public PartialViewResult EditContactInfo(InstructorContactInfoDTO instructorContactInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (instructorContactInfo.InstructorBasicInfoId == 0)
                    {
                        TempData["ErrorMessage"] = "(Instructor basic info is not filled)";
                    }
                    else 
                    {
                        if (instructorContactInfo.Id != 0)
                        {
                            _instructorContactInfoService.Edit(instructorContactInfo);
                        }
                        else
                            instructorContactInfo = _instructorContactInfoService.Create(instructorContactInfo);
                        _instructorBasicInfoService.SetIsChangedFlag(instructorContactInfo.InstructorBasicInfoId, true);
                        TempData["SuccessMessage"] = "(saved successfully)";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "(saving failed)";
                }
            }
            catch (ModelValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                TempData["ErrorMessage"] = "(saving failed)";
            }

            FillContactInfoLists(instructorContactInfo);

            return PartialView("_EditContactInfoPartial", instructorContactInfo);
        }


        [Authorize(Policy = Permissions.Instructors.Edit)]
        public IActionResult Delete(string instructorUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View(_instructorInfoService.GetInstructorDetailsInfo(selectedOrganization, instructorUserId));
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int userBasicInfoId, int instructorBasicInfoId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                _instructorInfoService.Delete(selectedOrganization, instructorBasicInfoId);
            }
            catch (ModelValidationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Delete", new { userBasicInfoId = userBasicInfoId });
            }

            return RedirectToAction("Index");
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        public ViewResult GetBlankOtherJobInfoEditorRow() 
        {
            ViewData["Index"] = 0;
            return View("_EditOtherJobInfoPartial", new InstructorOtherJobInfoDTO());
        }

        [Authorize(Policy = Permissions.Instructors.Edit)]
        public ViewResult GetBlankEducationInfoEditorRow()
        {
            ViewData["Index"] = 0;
            return View("_EditEducationInfoPartial", new InstructorEducationInfoDTO());
        }

        private void FillSelectLists(InstructorInfoDetailsViewModel model, int selectedOrganization) 
        {

            FillUserBasicInfoLists(model?.UserBasicInfo);
            FillOrgInfoLists(selectedOrganization, model?.InstructorOrgInfo);
            FillContactInfoLists(model?.InstructorBasicInfo?.InstructorContactInfo);
        }

        private void FillUserBasicInfoLists(UserBasicInfoDTO model) 
        {
            var nationalities = _nationalityService.GetNationalities().ToList();
            var countries = _countryService.GetCountries().ToList();

            ViewBag.Nationalities = new SelectList(nationalities, "Id", "NameEng");
            ViewBag.Cintizehships = new SelectList(countries, "Id", "NameEng");

            if (model != null)
            {
                ViewBag.Nationalities = new SelectList(nationalities, "Id", "NameEng", model.NationalityId);
                ViewBag.Cintizehships = new SelectList(countries, "Id", "NameEng", model.CitizenshipId);
            }
        }

        private void FillOrgInfoLists(int organizationId, InstructorOrgInfoDTO model)
        {
            var departments = _departmentService.GetDepartments(organizationId, true).ToList();
            ViewBag.Departments = new SelectList(departments, "Id", "Code", model?.DepartmentId);
            ViewBag.InstructorStates = new SelectList(Enum.GetValues(typeof(enu_InstructorState)).Cast<enu_InstructorState>()
                .Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList(), "Value", "Text", model?.State.ToString());
        }

        private void FillContactInfoLists(InstructorContactInfoDTO model)
        {
            var countries = _countryService.GetCountries().ToList();
            
            ViewBag.Countries = new SelectList(countries, "Id", "NameEng");
            ViewBag.CitizenshipCountries = new SelectList(countries, "Id", "NameEng");

            if (model != null)
            {
                ViewBag.Countries = new SelectList(countries, "Id", "NameEng", model.CountryId);
                ViewBag.CitizenshipCountries = new SelectList(countries, "Id", "NameEng", model.CitizenshipCountryId);
            }
        }
    }
}
