using iuca.Application.Constants;
using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels.Users.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Web.Controllers
{

    [Authorize]
    public class StudentInfoController : Controller
    {
        private readonly IStudentInfoService _studentInfoService;
        private readonly IOrganizationService _organizationService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IDepartmentService _departmentService;
        private readonly ICountryService _countryService;
        private readonly IUserBasicInfoService _userBasicInfoService;
        private readonly INationalityService _nationalityService;
        private readonly IStudentBasicInfoService _studentBasicInfoService;
        private readonly IStudentOrgInfoService _studentOrgInfoService;
        private readonly IStudentMinorInfoService _studentMinorInfoService;

        public StudentInfoController(IStudentInfoService studentInfoService,
            IOrganizationService organizationService,
            IDepartmentGroupService departmentGroupService,
            IDepartmentService departmentService,
            ICountryService countryService,
            IUserBasicInfoService userBasicInfoService,
            INationalityService nationalityService,
            IStudentBasicInfoService studentBasicInfoService,
            IStudentOrgInfoService studentOrgInfoService,
            IStudentMinorInfoService studentMinorInfoService)
        {
            _studentInfoService = studentInfoService;
            _organizationService = organizationService;
            _departmentGroupService = departmentGroupService;
            _departmentService = departmentService;
            _countryService = countryService;
            _userBasicInfoService = userBasicInfoService;
            _nationalityService = nationalityService;
            _studentBasicInfoService = studentBasicInfoService;
            _studentOrgInfoService = studentOrgInfoService;
            _studentMinorInfoService = studentMinorInfoService;
        }

        [Authorize(Policy = Permissions.Students.View)]
        public IActionResult Index()
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View(_studentInfoService.GetStudentInfoList(selectedOrganization).ToList());
        }

        [Authorize(Policy = Permissions.Students.View)]
        public IActionResult Details(string studentUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View(_studentInfoService.GetStudentDetailsInfo(selectedOrganization, studentUserId));
        }

        [Authorize(Policy = Permissions.Students.Edit)]
        public IActionResult Edit(string studentUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var model = _studentInfoService.GetStudentDetailsInfo(selectedOrganization, studentUserId);
            
            FillSelectLists(model, selectedOrganization);

            return View(model);
        }

        [Authorize(Policy = Permissions.Students.Edit)]
        [HttpPost]
        public PartialViewResult EditUserBasicInfo(UserBasicInfoDTO userBasicInfo)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                if (ModelState.IsValid)
                {
                    if (userBasicInfo.Id != 0)
                        _userBasicInfoService.Edit(selectedOrganization, userBasicInfo);
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

        [Authorize(Policy = Permissions.Students.Edit)]
        [HttpPost]
        public PartialViewResult EditStudentBasicInfo(StudentBasicInfoDTO studentBasicInfo)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                if (ModelState.IsValid)
                {
                    if (studentBasicInfo.Id != 0)
                        _studentBasicInfoService.Edit(selectedOrganization, studentBasicInfo);
                    else
                        studentBasicInfo = _studentBasicInfoService.Create(selectedOrganization, studentBasicInfo);
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

            return PartialView("_EditStudentBasicInfoPartial", studentBasicInfo);
        }

        [Authorize(Policy = Permissions.Students.Edit)]
        [HttpPost]
        public PartialViewResult EditStudentOrgInfo(StudentOrgInfoDTO studentOrgInfo)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            try
            {
                if (ModelState.IsValid)
                {
                    if (studentOrgInfo.StudentBasicInfoId != 0 && studentOrgInfo.OrganizationId != 0)
                    {
                        _studentOrgInfoService.Edit(selectedOrganization, studentOrgInfo);
                        TempData["SuccessMessage"] = "(saved successfully)";
                    }
                    else
                    {
                        if (studentOrgInfo.StudentBasicInfoId == 0)
                        {
                            TempData["ErrorMessage"] = "(Instructor basic info is not filled)";
                        }
                        else
                        {
                            studentOrgInfo.OrganizationId = selectedOrganization;
                            _studentOrgInfoService.Create(studentOrgInfo);
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

            FillOrgInfoLists(selectedOrganization, studentOrgInfo);

            return PartialView("_EditOrgInfoPartial", studentOrgInfo);
        }

        [Authorize(Policy = Permissions.Students.Edit)]
        [HttpPost]
        public PartialViewResult EditStudentMinorInfo(StudentMinorInfoViewModel studentMinorInfo)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var departments = _departmentService.GetDepartments(selectedOrganization, true);
            ViewBag.Departments = new SelectList(departments, "Id", "Code");

            if (ModelState.IsValid)
            {
                try
                {
                    _studentMinorInfoService.EditStudentMinorInfo(studentMinorInfo);
                    TempData["SuccessMessage"] = "(saved successfully)";
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                } 
            }
            else
            {
                TempData["ErrorMessage"] = "(saving failed)";
            }

            return PartialView("_EditMinorInfoPartial", studentMinorInfo);
        }

        [Authorize(Policy = Permissions.Students.Edit)]
        public IActionResult Delete(string studentUserId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View(_studentInfoService.GetStudentDetailsInfo(selectedOrganization, studentUserId));
        }

        [Authorize(Policy = Permissions.Students.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int studentBasicInfoId, int userBasicInfoId)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                _studentInfoService.Delete(selectedOrganization, studentBasicInfoId);
            }
            catch (ModelValidationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Delete", new { userBasicInfoId = userBasicInfoId });
            }

            return RedirectToAction("Index");
        }

        

        [Authorize(Policy = Permissions.Students.Edit)]
        public ViewResult GetBlankLanguageEditorRow(int studentBasicInfoId)
        {
            return View("_EditLanguagesPartial", new StudentLanguageDTO() { StudentBasicInfoId = studentBasicInfoId });
        }

        [Authorize(Policy = Permissions.Students.Edit)]
        public ViewResult GetBlankParentsInfoEditorRow(int studentBasicInfoId)
        {
            return View("_EditParentsInfoPartial", new StudentParentsInfoDTO() { StudentBasicInfoId = studentBasicInfoId });
        }

        private void FillSelectLists(StudentInfoDetailsViewModel model, int selectedOrganization)
        {
            FillUserBasicInfoLists(model?.UserBasicInfo);
            FillOrgInfoLists(selectedOrganization, model?.StudentOrgInfo);

            var departments = _departmentService.GetDepartments(selectedOrganization, true);
            ViewBag.Departments = new SelectList(departments, "Id", "Code");

            var countries = _countryService.GetCountries().ToList();
            ViewBag.Countries = new SelectList(countries, "Id", "NameEng");
            ViewBag.CitizenshipCountries = new SelectList(countries, "Id", "NameEng");

            if (model != null)
            {
                if (model.StudentBasicInfo != null)
                {
                    if (model.StudentOrgInfo != null)

                    if (model.StudentBasicInfo.StudentContactInfo != null)
                    {
                        ViewBag.Countries = new SelectList(countries, "Id", "NameEng",
                            model.StudentBasicInfo.StudentContactInfo.CountryId);
                        ViewBag.CitizenshipCountries = new SelectList(countries, "Id", "NameEng",
                            model.StudentBasicInfo.StudentContactInfo.CitizenshipCountryId);
                    }
                }
            }
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

        private void FillOrgInfoLists(int organizationId, StudentOrgInfoDTO model)
        {
            var departmentGroups = _departmentGroupService.GetDepartmentGroups(organizationId).OrderBy(x => x.DepartmentCode).ToList();

            if (model != null)
            {
                ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode", model.DepartmentGroupId);
                ViewBag.PrepDepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode", model.PrepDepartmentGroupId);
            }
            else 
            {
                ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode");
                ViewBag.PrepDepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode");
            }
        }

        [Authorize(Policy = Permissions.Students.Edit)]
        public IActionResult PrepStudents(bool activeOnly)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var departmentGroups = _departmentGroupService.GetDepartmentGroups(selectedOrganization).OrderBy(x => x.DepartmentCode).ToList();
            ViewBag.DepartmentGroups = new SelectList(departmentGroups, "Id", "DepartmentCode");
            ViewBag.ActiveOnly = activeOnly;

            return View(_studentInfoService.GetPrepStudents(selectedOrganization, activeOnly));
        }

        [Authorize(Policy = Permissions.Students.Edit)]
        [HttpPost]
        public void SavePrepStudentDepartmentGroup(int organizationId, int studentBasicInfoId, int? prepDepartmentGroupId)
        {
            _studentInfoService.SavePrepStudentDepartmentGroup(organizationId, studentBasicInfoId, prepDepartmentGroupId);
        }
    }
}
