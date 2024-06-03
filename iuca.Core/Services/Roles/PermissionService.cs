using iuca.Application.Constants;
using iuca.Application.Interfaces.Roles;
using iuca.Application.ViewModels.Users.Roles;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using static iuca.Application.Constants.Permissions;

namespace iuca.Application.Services.Roles
{
    public class PermissionService : IPermissionService
    {
        private readonly IRoleService _roleService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IClaimService _claimService;
        public PermissionService(IRoleService roleService,
            RoleManager<ApplicationRole> roleManager,
            IUserRolesService userRolesService,
            IClaimService claimService) 
        {
            _roleService = roleService;
            _roleManager = roleManager;
            _claimService = claimService;
        }

        /// <summary>
        /// Get permissions by role name prefix
        /// </summary>
        /// <param name="roleNamePrefix">Role name prefix</param>
        /// <returns>Permissions model</returns>
        public PermissionViewModel GetPermissionsByRole(string roleNamePrefix) 
        {
            var model = new PermissionViewModel();
            var allPermissions = new List<RoleClaimsViewModel>();

            //Menu
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.MenuSettings)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.MenuCommon)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.MenuCourses)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.MenuUsers)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.MenuStudentDebts)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.MenuTranscripts)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.MenuReports)));

            //Settings
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.EnvarSettings)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.ImportData)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.ExportData)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Attendance)));

            //Common
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Organizations)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Nationalities)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Countries)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Departments)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Universities)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.EducationTypes)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Semesters)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.SemesterPeriods)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Languages)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.AttendanceFolders)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.DepartmentGroups)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Grades)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.GradeManagement)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.FFXXReport)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.GradeReport)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Policies)));

            //Courses
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Courses)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Cycles)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.AcademicPlans)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.StudyCards)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Proposals)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Announcements)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.StudentsInSections)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.StudyCardPlaces)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.StudentCourseRegistrationsManagement)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.StudentCourseRegistrations)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.TransferCourses)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.InstructorCourses)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.StudentMidterms)));

            // Users
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Roles)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.UserRoles)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Users)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Instructors)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Students)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Staff)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Advisers)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.Deans)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.StudentOrders)));

            //Student Debts
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.DebtsAccounting)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.DebtsLibrary)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.DebtsDormitory)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.DebtsRegistarOffice)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.DebtsMedicineOffice)));

            //Transcripts
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.TranscriptsAdmin)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.TranscriptsStudent)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.TranscriptsDean)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.TranscriptsAdviser)));

            //Syllabi
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.SyllabiEditor)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.SyllabiApprover)));

            //Reports
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.RegistrationCoursesReport)));
            allPermissions.AddRange(_claimService.GetPermissions(typeof(Permissions.RegistrationCoursesDetailedReport)));

            var role = _roleService.GetRoleByNamePrefix(roleNamePrefix);

            //Get the first role. All the rest should be the same
            var applicationRole =  _roleManager.FindByIdAsync(role.RoleIds[0]).GetAwaiter().GetResult();

            model.RoleNamePrefix = role.RoleName;
            var claims = _roleManager.GetClaimsAsync(applicationRole).GetAwaiter().GetResult();
            var allClaimValues = allPermissions.Select(a => a.Value).ToList();
            var roleClaimValues = claims.Select(a => a.Value).ToList();
            var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
            foreach (var permission in allPermissions)
            {
                if (authorizedClaims.Any(a => a == permission.Value))
                {
                    permission.Selected = true;
                }
            }
            model.RoleClaims = allPermissions.OrderBy(x => x.ModuleName).ThenBy(x => x.Value).ToList();
            return model;
        }

        /// <summary>
        /// Update role premission claims
        /// </summary>
        /// <param name="model">Permissions model</param>
        public void UpdatePermissions(PermissionViewModel model) 
        {
            var roleModel = _roleService.GetRoleByNamePrefix(model.RoleNamePrefix);
            foreach (var roleId in roleModel.RoleIds) 
            {
                var role = _roleManager.FindByIdAsync(roleId).GetAwaiter().GetResult();
                var claims = _roleManager.GetClaimsAsync(role).GetAwaiter().GetResult();
                foreach (var claim in claims)
                {
                    _roleManager.RemoveClaimAsync(role, claim).GetAwaiter().GetResult();
                }
                var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
                foreach (var claim in selectedClaims)
                {
                    _claimService.AddPermissionClaim(role, claim.Value);
                }
            }
        }

    }
}
