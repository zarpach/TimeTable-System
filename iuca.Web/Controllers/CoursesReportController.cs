using iuca.Application.Interfaces.Common;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace iuca.Web.Controllers
{
    public class CoursesReportController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly IDepartmentService _departmentService;

        public CoursesReportController(IOrganizationService organizationService,
            IDepartmentService departmentService) 
        {
            _organizationService = organizationService;
            _departmentService = departmentService;
        }

        public IActionResult AcademicPlanReport()
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);

            var departments = _departmentService.GetDepartments(selectedOrganization).OrderBy(x => x.Code);
            ViewBag.Departments = new SelectList(departments, "Id", "Code");
            
            return View();
        }
    }
}
