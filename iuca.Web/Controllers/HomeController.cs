using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Users;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Courses;
using iuca.Application.ViewModels.Users.Students;
using iuca.Infrastructure.Identity.Entities;
using iuca.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iuca.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOrganizationService _organizationService;
        private readonly IStudentCourseRegistrationService _studentCourseRegistrationService;

        public HomeController(ILogger<HomeController> logger,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IOrganizationService organizationService,
            IStudentCourseRegistrationService studentCourseRegistrationService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _organizationService = organizationService;
            _studentCourseRegistrationService = studentCourseRegistrationService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            RegistrationInfoViewModel model = new RegistrationInfoViewModel();
            if (_signInManager.IsSignedIn(User))
            {
                string userId = _userManager.GetUserId(User);
                int selectedOrganizationId = _organizationService.GetSelectedOrganization(User);
                model = _studentCourseRegistrationService.GetStudentRegistrationInfo(selectedOrganizationId, userId);
            }

            return View(model);
        }

        /*public IActionResult Privacy()
        {
            return View();
        }*/

        [Authorize(Policy = "InstructorAccessClaim")]
        public IActionResult InstructorPage() 
        {
            return View();
        }

        [Authorize(Policy = "StudentAccessClaim")]
        public IActionResult StudentPage()
        {
            return View();
        }

        [Authorize(Policy = "StaffAccessClaim")]
        public IActionResult StaffPage()
        {
            return View();
        }
    }
}
