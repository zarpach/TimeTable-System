using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace iuca.Web.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager, 
            IOrganizationService organizationService)
        {
            foreach (var organiztion in organizationService.GetOrganizations()) 
            {
                await roleManager.CreateAsync(new ApplicationRole(enu_Role.Admin.ToString() + "_" + organiztion.Id));
                await roleManager.CreateAsync(new ApplicationRole(enu_Role.Staff.ToString() + "_" + organiztion.Id));
                await roleManager.CreateAsync(new ApplicationRole(enu_Role.Instructor.ToString() + "_" + organiztion.Id));
                await roleManager.CreateAsync(new ApplicationRole(enu_Role.Student.ToString() + "_" + organiztion.Id));
                await roleManager.CreateAsync(new ApplicationRole(enu_Role.Adviser.ToString() + "_" + organiztion.Id));
                await roleManager.CreateAsync(new ApplicationRole(enu_Role.Dean.ToString() + "_" + organiztion.Id));
                await roleManager.CreateAsync(new ApplicationRole(enu_Role.Accountant.ToString() + "_" + organiztion.Id));
                await roleManager.CreateAsync(new ApplicationRole(enu_Role.Librarian.ToString() + "_" + organiztion.Id));
                await roleManager.CreateAsync(new ApplicationRole(enu_Role.DormitoryManager.ToString() + "_" + organiztion.Id));
                await roleManager.CreateAsync(new ApplicationRole(enu_Role.RegistarOffice.ToString() + "_" + organiztion.Id));
            }

        }
    }
}
