using iuca.Application.Constants;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Roles;

namespace iuca.Web.Seeds
{
    public static class DefaultPermissions
    {
        public static void SeedDefaultRolePermissions(
            IOrganizationService organizationService,
            IClaimService claimService)
        {
            foreach (var organization in organizationService.GetOrganizations())
            {
                SetAdminRoleClaims(claimService, organization.Id);
                SetDeanRoleClaims(claimService, organization.Id);
                SetAdviserRoleClaims(claimService, organization.Id);
                SetStudentRoleClaims(claimService, organization.Id);
                SetRegistarOfficeRoleClaims(claimService, organization.Id);
            }
        }

        private static void SetAdminRoleClaims(IClaimService claimService, int organizationId)
        {
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.MenuSettings).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.MenuCommon).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.MenuCourses).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.MenuUsers).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.MenuStudentDebts).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.MenuTranscripts).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.MenuReports).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.ImportData).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Organizations).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Nationalities).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Countries).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Departments).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Universities).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.EducationTypes).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Semesters).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.SemesterPeriods).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Languages).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.DepartmentGroups).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Grades).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Courses).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Cycles).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.AcademicPlans).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.StudyCardPlaces).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.StudyCards).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.StudentCourseRegistrations).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.TransferCourses).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Roles).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.UserRoles).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Users).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Instructors).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Students).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Staff).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Advisers).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.Deans).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.DebtsAccounting).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.DebtsLibrary).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.DebtsDormitory).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.DebtsRegistarOffice).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.TranscriptsAdmin).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.TranscriptsStudent).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.TranscriptsDean).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Admin, typeof(Permissions.TranscriptsAdviser).Name);
        }

        private static void SetDeanRoleClaims(IClaimService claimService, int organizationId)
        {
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Dean, typeof(Permissions.AcademicPlans).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Dean, typeof(Permissions.Advisers).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Dean, typeof(Permissions.MenuCourses).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Dean, typeof(Permissions.MenuUsers).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Dean, typeof(Permissions.MenuTranscripts).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Dean, typeof(Permissions.MenuReports).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Dean, typeof(Permissions.StudyCards).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Dean, typeof(Permissions.TranscriptsDean).Name);
        }

        private static void SetAdviserRoleClaims(IClaimService claimService, int organizationId)
        {
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Adviser, typeof(Permissions.MenuTranscripts).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Adviser, typeof(Permissions.TranscriptsAdviser).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Adviser, typeof(Permissions.StudentCourseRegistrations).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Adviser, typeof(Permissions.MenuCourses).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Adviser, typeof(Permissions.MenuReports).Name);

        }

        private static void SetStudentRoleClaims(IClaimService claimService, int organizationId)
        {
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Student, typeof(Permissions.MenuTranscripts).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Student, typeof(Permissions.TranscriptsStudent).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.Student, typeof(Permissions.StudentCourseRegistrations).Name);
        }

        private static void SetRegistarOfficeRoleClaims(IClaimService claimService, int organizationId)
        {
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.Countries).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.Courses).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.Cycles).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.Nationalities).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.DepartmentGroups).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.Departments).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.EducationTypes).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.Grades).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.EducationTypes).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.Languages).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.MenuCommon).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.MenuCourses).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.MenuTranscripts).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.Nationalities).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.MenuTranscripts).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.SemesterPeriods).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.Semesters).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.StudyCards).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.TranscriptsAdmin).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.TransferCourses).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.Universities).Name);
            claimService.AddPermissionClaimForModule(organizationId, enu_Role.RegistarOffice, typeof(Permissions.MenuReports).Name);

        }
    }
}
