using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels.Users.UserInfo;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using System;
using System.Threading.Tasks;

namespace iuca.Web.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedDefaultUsersAsync(IUserInfoService userInfoService,
            IUserBasicInfoService userBasicInfoService,
            INationalityService nationalityService,
            ICountryService countryService,
            ApplicationUserManager<ApplicationUser> userManager,
            IUserRolesService userRolesService, 
            IOrganizationService organizationService,
            IUserTypeOrganizationService userTypeOrganizationService)
        {
            var mainOrganization = organizationService.GetMainOrganization();

            UserInfoViewModel userInfo = new UserInfoViewModel();
            ApplicationUser user = new ApplicationUser();
            user.FirstNameEng = "Administrator";
            user.LastNameEng = "";
            user.Email = "kasymbekov_e@iuca.kg";
            user.UserName = "kasymbekov_e@iuca.kg";
            user.IsActive = true;
            userInfo.ApplicationUser = user;
            userInfo.IsStaff = true;
            userInfoService.Create(mainOrganization.Id, userInfo);

            ApplicationUser dbUser = await userManager.FindByNameAsync(user.Email);

            UserBasicInfoDTO userBasicInfo = new UserBasicInfoDTO();
            userBasicInfo.ApplicationUserId = dbUser.Id;
            userBasicInfo.LastNameRus = " ";
            userBasicInfo.FirstNameRus = "Администратор";
            userBasicInfo.MiddleNameRus = "";
            userBasicInfo.Sex = (int)enu_Sex.Male;
            userBasicInfo.DateOfBirth = new DateTime(2021, 1, 1);
            userBasicInfo.NationalityId = nationalityService.GetDefaultNationality().Id;
            userBasicInfo.CitizenshipId = countryService.GetDefaultCountry().Id;
            userBasicInfoService.Create(mainOrganization.Id, userBasicInfo);

            foreach (var organization in organizationService.GetOrganizations())
            {
                userRolesService.AddToRole(user, organization.Id, enu_Role.Admin);
                userTypeOrganizationService.Create(organization.Id, dbUser.Id, (int)enu_UserType.Staff, false);
            }
        }
    }
}
