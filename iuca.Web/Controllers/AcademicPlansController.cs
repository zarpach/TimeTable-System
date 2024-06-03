using iuca.Application.Constants;
using iuca.Application.DTO.Courses;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.ViewModels.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iuca.Web.Controllers
{
    [Authorize]
    public class AcademicPlansController : Controller
    {
        private readonly IAcademicPlanService _academicPlanService;
        private readonly IOrganizationService _organizationService;
        private readonly IDepartmentService _departmentService;
        private readonly ICourseService _courseService;
        public AcademicPlansController(IAcademicPlanService academicPlanService,
            IOrganizationService organizationService,
            IDepartmentService departmentService,
            ICourseService courseService)
        {
            _academicPlanService = academicPlanService;
            _organizationService = organizationService;
            _departmentService = departmentService;
            _courseService = courseService;
        }

        [Authorize(Policy = Permissions.AcademicPlans.View)]
        public IActionResult Index()
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_academicPlanService.GetAcademicPlans(selectedOrganizationId));
        }

        [Authorize(Policy = Permissions.AcademicPlans.View)]
        public IActionResult Details(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_academicPlanService.GetAcademicPlan(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public IActionResult Create()
        {
            FillSelectLists();
            return View();
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        [HttpPost]
        public IActionResult Create(AcademicPlanDTO academicPlan)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            academicPlan.OrganizationId = selectedOrganizationId;
            if (ModelState.IsValid)
            {
                try
                {
                    _academicPlanService.Create(academicPlan);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            FillSelectLists(academicPlan);

            return View(academicPlan);
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public IActionResult Edit(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            var academicPlan = _academicPlanService.GetAcademicPlan(selectedOrganizationId, id);
            FillSelectLists(academicPlan);

            return View(academicPlan);
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        [HttpPost]
        public IActionResult Edit(int id, AcademicPlanDTO academicPlan)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _academicPlanService.Edit(selectedOrganizationId, id, academicPlan);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            FillSelectLists(academicPlan);

            return View(academicPlan);
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public IActionResult Delete(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            return View(_academicPlanService.GetAcademicPlan(selectedOrganizationId, id));
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _academicPlanService.Delete(selectedOrganizationId, id);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return RedirectToAction("Delete", new { id = id });
        }

        private void FillSelectLists(AcademicPlanDTO academicPlan = null)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var departments = _departmentService.GetDepartments(selectedOrganization).OrderBy(x => x.Code);

            if (academicPlan != null)
            {
                ViewBag.Departments = new SelectList(departments, "Id", "Code", academicPlan.DepartmentId);
            }
            else
            {
                ViewBag.Departments = new SelectList(departments, "Id", "Code");
            }
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public IActionResult EditAcademicPlanCourses(int id) 
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            return View(_academicPlanService.GetAcademicPlanEditModel(selectedOrganization, id));
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public IActionResult FillAcademicPlanByYear(int academicPlanId, int year)
        {
            AcademicPlanViewModel model = new AcademicPlanViewModel();
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            try
            {
                model = _academicPlanService.FillAcademicPlanByYear(selectedOrganization, academicPlanId, year);
            }
            catch (ModelValidationException ex)
            {
                model = _academicPlanService.GetAcademicPlanEditModel(selectedOrganization, academicPlanId);
                TempData["Error"] = ex.Message;
            }
            catch 
            {
                throw;
            }

            return View("EditAcademicPlanCourses", model);
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        [HttpPost]
        public IActionResult EditAcademicPlanCourses(int academicPlanId, List<CyclePartDTO> modelCycleParts)
        {
            int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _academicPlanService.EditAcademicPlanCourses(selectedOrganizationId, academicPlanId, modelCycleParts);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View("EditAcademicPlanCourses", new { academicPlanId = academicPlanId, modelCourses = modelCycleParts });
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public ViewResult GetCoursesForSelection(int cyclePartIndex, int cycleId, string cycleName, int part, string partName, int[] excludedIds)
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var courses = _courseService.GetCoursesWithExclusion(selectedOrganization, excludedIds).ToList();

            ViewBag.CyclePartIndex = cyclePartIndex;
            ViewBag.CycleId = cycleId;
            ViewBag.CycleName = cycleName;
            ViewBag.Part = part;
            ViewBag.PartName = partName;

            return View("_SelectCoursesPartial", courses);
        }

        [Authorize(Policy = Permissions.AcademicPlans.Edit)]
        public ViewResult GetCoursesFromSelection(int cyclePartIndex, int cycleId, int part, int[] ids) 
        {
            List<CyclePartCourseDTO> cyclePartCourses = new List<CyclePartCourseDTO>();
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            var courses = _courseService.GetCourses(selectedOrganization, ids).ToList();

            ViewData["CyclePartIndex"] = cyclePartIndex;
            ViewData["NumId"] = $"num_{cycleId}_{part}";
            ViewData["CycleId"] = cycleId;
            ViewData["Part"] = part;

            foreach (var course in courses) 
            {
                cyclePartCourses.Add(new CyclePartCourseDTO 
                { 
                    Course = course 
                });
            }
            
            return View("_CourseRow", cyclePartCourses);
        }
    }
}
