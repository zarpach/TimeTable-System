using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Instructors;
using iuca.Application.DTO.Users.Staff;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Staff;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


namespace iuca.Application.Services.Users
{
    public class UserTypeOrganizationService : IUserTypeOrganizationService
    {
        private readonly IApplicationDbContext _db;
        private readonly IUserRolesService _userRolesService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IOrganizationService _organizationService;
        private readonly IUserBasicInfoService _userBasicInfoService;
        private readonly IInstructorBasicInfoService _instructorBasicInfoService;
        private readonly IStudentBasicInfoService _studentBasicInfoService;
        private readonly IStaffBasicInfoService _staffBasicInfoService;

        public UserTypeOrganizationService(IApplicationDbContext db,
            IUserRolesService userRolesService,
            ApplicationUserManager<ApplicationUser> userManager,
            IOrganizationService organizationService,
            IUserBasicInfoService userBasicInfoService,
            IInstructorBasicInfoService instructorBasicInfoService,
            IStudentBasicInfoService studentBasicInfoService,
            IStaffBasicInfoService staffBasicInfoService)
        {
            _db = db;
            _userRolesService = userRolesService;
            _userManager = userManager;
            _organizationService = organizationService;
            _userBasicInfoService = userBasicInfoService;
            _instructorBasicInfoService = instructorBasicInfoService;
            _studentBasicInfoService = studentBasicInfoService;
            _staffBasicInfoService = staffBasicInfoService;
        }

        /// <summary>
        /// Check if relation exists
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="userType">User type</param>
        /// <returns></returns>
        public bool UserHasType(int organizationId, string applicationUserId, int userType) 
        {
            return _db.UserTypeOrganizations.Any(x => x.OrganizationId == organizationId && 
                                x.ApplicationUserId == applicationUserId && x.UserType == userType);
        }

        /// <summary>
        /// Get user's organizations
        /// </summary>
        /// <param name="applicationUserId">ApplicationUserId</param>
        /// <returns>List of organizations</returns>
        public IEnumerable<OrganizationDTO> GetUserOrganizations(string applicationUserId) 
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Organization, OrganizationDTO>()).CreateMapper();
            var organizations = _db.UserTypeOrganizations.Include(x => x.Organization)
                .Where(x => x.ApplicationUserId == applicationUserId).GroupBy(x => x.OrganizationId)
                .Select(x => x.First().Organization);

            return mapper.Map<IEnumerable<Organization>, IEnumerable<OrganizationDTO>>(organizations);
        }

        /// <summary>
        /// Create user type for organization, add claim, check and update IsMainOrganization flag
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="userType">User type</param>
        /// <param name="userType">If true genretates exception if record already exists</param>
        public void Create(int organizationId, string applicationUserId, int userType, bool generateException = true)
        {
            UserTypeOrganization userTypeOrganization = _db.UserTypeOrganizations.FirstOrDefault(x => x.OrganizationId == organizationId &&
                                x.ApplicationUserId == applicationUserId && x.UserType == userType);

            if (userTypeOrganization != null)
            {
                if (generateException)
                    throw new Exception("UserTypeOrganization already exists");
                else
                    return;
            }

            UserTypeOrganization newUserTypeOrganization = new UserTypeOrganization();
            newUserTypeOrganization.OrganizationId = organizationId;
            newUserTypeOrganization.ApplicationUserId = applicationUserId;
            newUserTypeOrganization.UserType = userType;
            _db.UserTypeOrganizations.Add(newUserTypeOrganization);
            _db.SaveChanges();

            _userRolesService.AddToRole(applicationUserId, organizationId, GetRoleByUserType(userType));

            //Check and update IsMainOrganizationFlag
            ChangeIsMainOrganizationFlagForCreate(organizationId, applicationUserId, userType);


        }

        private void ChangeIsMainOrganizationFlagForCreate(int organizationId, string applicationUserId, int userType) 
        {
            OrganizationDTO organization = _organizationService.GetOrganization(organizationId);
            if (organization == null)
                throw new Exception("Organization is null");

            if (organization.IsMain)
            {
                var user = _userManager.Users
                    .Include(x => x.UserBasicInfo)
                    .Include(x => x.StudentBasicInfo)
                    .Include(x => x.StaffBasicInfo)
                    .Include(x => x.InstructorBasicInfo)
                    .FirstOrDefault(x => x.Id == applicationUserId);
                if (user == null)
                    throw new Exception("User not found");

                //Edit user flag
                if (!user.IsMainOrganization)
                {
                    user.IsMainOrganization = true;
                    _userManager.UpdateAsync(user).GetAwaiter().GetResult();
                }

                //Edit user basic info owner
                if (user.UserBasicInfo != null && user.UserBasicInfo.IsMainOrganization != true)
                    _userBasicInfoService.SetOwnerFlag(user.UserBasicInfo.Id, true);

                //Edit instructor basic info false flag
                if (userType == (int)enu_UserType.Instructor && user.InstructorBasicInfo != null) 
                    EditInstructorOwner(organizationId, user.InstructorBasicInfo.Id, true);

                //Edit student basic info false flag
                if (userType == (int)enu_UserType.Student && user.StudentBasicInfo != null)
                    EditStudentOwner(organizationId, user.StudentBasicInfo.Id, true);

                //Edit staff basic info false flag
                if (userType == (int)enu_UserType.Staff && user.StaffBasicInfo != null)
                    EditStaffOwner(organizationId, user.StaffBasicInfo.Id, true);
            }
        }

        /// <summary>
        /// Delete user type for organization, remove claim, check and update IsMainOrganization flag
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="userType">User type</param>
        public void Delete(int organizationId, string applicationUserId, int userType)
        {
            UserTypeOrganization userTypeOrganization = _db.UserTypeOrganizations.FirstOrDefault(x => x.OrganizationId == organizationId &&
                                x.ApplicationUserId == applicationUserId && x.UserType == userType);
            if (userTypeOrganization == null)
                throw new Exception($"UserTypeOrganization not found");

            _db.UserTypeOrganizations.Remove(userTypeOrganization);
            _db.SaveChanges();

            //Removing role by user type
            ApplicationUser user = _userManager.Users.FirstOrDefault(x => x.Id == applicationUserId);
            if (user == null)
                throw new Exception("User not found");
            _userRolesService.RemoveFromRole(user.Id, organizationId, GetRoleByUserType(userType));

            //Check and update IsMainOrganizationFlag
            ChangeIsMainOrganizationFlagForDelete(organizationId, applicationUserId, userType);
        }

        private void ChangeIsMainOrganizationFlagForDelete(int organizationId, string applicationUserId, int userType)
        {
            OrganizationDTO organization = _organizationService.GetOrganization(organizationId);
            if (organization == null)
                throw new Exception("Organization is null");

            if (organization.IsMain)
            {
                var user = _userManager.Users
                    .Include(x => x.UserBasicInfo)
                    .Include(x => x.StudentBasicInfo)
                    .Include(x => x.StaffBasicInfo)
                    .Include(x => x.InstructorBasicInfo)
                    .FirstOrDefault(x => x.Id == applicationUserId);

                if (user == null)
                    throw new Exception("User not found");


                //Edit user flag
                if (!user.IsMainOrganization)
                {
                    user.IsMainOrganization = true;
                    _userManager.UpdateAsync(user).GetAwaiter().GetResult();
                }

                //Edit user basic info owner
                if (user.UserBasicInfo != null && user.UserBasicInfo.IsMainOrganization != true)
                    _userBasicInfoService.SetOwnerFlag(user.UserBasicInfo.Id, true);


                //Edit instructor basic info true flag
                if (userType == (int)enu_UserType.Instructor && user.InstructorBasicInfo != null)
                    EditInstructorOwner(organizationId, user.InstructorBasicInfo.Id, false);

                //Edit student basic info true flag
                if (userType == (int)enu_UserType.Student && user.StudentBasicInfo != null)
                    EditStudentOwner(organizationId, user.StudentBasicInfo.Id, false);

                //Edit staff basic info true flag
                if (userType == (int)enu_UserType.Staff && user.StaffBasicInfo != null)
                    EditStaffOwner(organizationId, user.StaffBasicInfo.Id, false);
            }
        }

        private void EditInstructorOwner(int organizationId, int instructorBasicInfoId, bool isMainOrganization)
        {
            InstructorBasicInfoDTO instructorBasicInfo = _instructorBasicInfoService.GetInstructorBasicInfo(instructorBasicInfoId);
            if (instructorBasicInfo.IsMainOrganization != isMainOrganization)
            {
                instructorBasicInfo.IsMainOrganization = isMainOrganization;
                _instructorBasicInfoService.Edit(organizationId, instructorBasicInfo);
            }
        }

        private void EditStudentOwner(int organizationId, int studentBasicInfoId, bool isMainOrganization)
        {
            StudentBasicInfoDTO studentBasicInfo = _studentBasicInfoService.GetStudentBasicInfo(studentBasicInfoId);
            if (studentBasicInfo.IsMainOrganization != isMainOrganization)
            {
                studentBasicInfo.IsMainOrganization = isMainOrganization;
                _studentBasicInfoService.Edit(organizationId, studentBasicInfo);
            }
        }

        private void EditStaffOwner(int organizationId, int staffBasicInfoId, bool isMainOrganization)
        {
            StaffBasicInfoDTO staffBasicInfo = _staffBasicInfoService.GetStaffBasicInfo(staffBasicInfoId);
            if (staffBasicInfo.IsMainOrganization != isMainOrganization)
            {
                staffBasicInfo.IsMainOrganization = isMainOrganization;
                _staffBasicInfoService.Edit(organizationId, staffBasicInfoId, staffBasicInfo);
            }
        }

        private enu_Role GetRoleByUserType(int userType)
        {
            enu_Role role;
            if (userType == (int)enu_UserType.Instructor)
                role = enu_Role.Instructor;
            else if (userType == (int)enu_UserType.Staff)
                role = enu_Role.Staff;
            else if (userType == (int)enu_UserType.Student)
                role = enu_Role.Student;
            else
                throw new Exception("User type not defined");

            return role;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

    }
}
