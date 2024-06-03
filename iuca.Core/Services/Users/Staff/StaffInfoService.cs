using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Staff;
using iuca.Application.ViewModels.Users.Staff;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Users.Staff
{
    public class StaffInfoService : IStaffInfoService
    {
        private readonly IApplicationDbContext _db;
        private readonly IOrganizationService _organizationService;
        private readonly IStaffBasicInfoService _staffBasicInfoService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public StaffInfoService(IApplicationDbContext db,
            IStaffBasicInfoService staffBasicInfoService,
            IOrganizationService organizationService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _organizationService = organizationService;
            _staffBasicInfoService = staffBasicInfoService;
            _userManager = userManager;
        }


        /// <summary>
        /// Get staff info list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>List of staff info</returns>
        public IEnumerable<StaffInfoBriefViewModel> GetStaffInfoList(int organizationId)
        {
            List<StaffInfoBriefViewModel> staffInfoList = new List<StaffInfoBriefViewModel>();

            List<string> userIds = _db.UserTypeOrganizations
                                .Where(x => x.OrganizationId == organizationId && x.UserType == (int)enu_UserType.Staff)
                                .Select(x => x.ApplicationUserId).Distinct().ToList();

            var users = _userManager.Users
                            .Include(x => x.UserBasicInfo)
                            .Include(x => x.StaffBasicInfo)
                            .Where(x => userIds.Contains(x.Id))
                            .ToList();

            foreach (var user in users)
                staffInfoList.Add(FillStaffBriefInfo(organizationId, user));

            return staffInfoList.OrderBy(x => x.FullNameEng);
        }

        private StaffInfoBriefViewModel FillStaffBriefInfo(int selectedOrganizationId, ApplicationUser user)
        {
            StaffInfoBriefViewModel staffInfoVM = new StaffInfoBriefViewModel();
            staffInfoVM.StaffUserId = user.Id;
            staffInfoVM.FullNameEng = user.FirstNameEng;
            staffInfoVM.BasicInfoExists = user.StaffBasicInfo != null;

            if (user.StaffBasicInfo != null)
            {
                staffInfoVM.StaffInfo = user.StaffBasicInfo.StaffInfo;

                OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
                if (organization == null)
                    throw new Exception("organization is null");

                //Only main organization is allowed to modify data if IsMainOrganization = true
                staffInfoVM.IsReadOnly = user.StaffBasicInfo.IsMainOrganization && !organization.IsMain;
            }

            return staffInfoVM;
        }

        /// <summary>
        /// Get staff full by user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="staffUserId">Staff user id</param>
        /// <returns>Staff info model</returns>
        public StaffInfoDetailsViewModel GetStaffFullInfo(int organizationId, string staffUserId)
        {
            var user = _userManager.Users
                .Include(x => x.UserBasicInfo).ThenInclude(x => x.Citizenship)
                .Include(x => x.UserBasicInfo).ThenInclude(x => x.Nationality)
                .FirstOrDefault(x => x.Id == staffUserId);

            if (user == null)
                throw new Exception("User not found");

            StaffInfoDetailsViewModel staffInfoVM = new StaffInfoDetailsViewModel();
            staffInfoVM.StaffUserId = staffUserId;
            staffInfoVM.FullNameEng = user.FullNameEng;

            //User basic info
            if (user.UserBasicInfo != null)
            {
                var mapper = new MapperConfiguration(cfg => {
                    cfg.CreateMap<Nationality, NationalityDTO>();
                    cfg.CreateMap<Country, CountryDTO>();
                    cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>();
                }).CreateMapper();

                staffInfoVM.UserBasicInfo = mapper.Map<UserBasicInfo, UserBasicInfoDTO>(user.UserBasicInfo);
            }

            //Staff basic info
            staffInfoVM.StaffBasicInfo = _staffBasicInfoService.GetStaffFullInfo(staffUserId);
            if (staffInfoVM.StaffBasicInfo != null) 
            {
                OrganizationDTO organization = _organizationService.GetOrganization(organizationId);
                if (organization == null)
                    throw new Exception("organization is null");

                //Only main organization is allowed to modify data if IsMainOrganization = true
                staffInfoVM.IsReadOnly = staffInfoVM.StaffBasicInfo.IsMainOrganization && !organization.IsMain;
            }
            
            return staffInfoVM;
        }

        /// <summary>
        /// Create staff info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="staffInfo">Staff info model</param>
        public void Create(int organizationId, StaffInfoDetailsViewModel staffInfo)
        {
            if (staffInfo == null)
                throw new Exception("staffInfo is null");

            var staff = _db.StaffBasicInfo.FirstOrDefault(x => x.ApplicationUserId == staffInfo.StaffUserId);
            if (staff != null)
                throw new ModelValidationException("Staff info already exists", "ErrorMsg");

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var newStaffBasicInfo = _staffBasicInfoService.Create(organizationId, staffInfo.StaffBasicInfo);

                    //Create staff organization information

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Edit staff info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="staffInfo">Staff info model</param>
        public void Edit(int organizationId, StaffInfoDetailsViewModel staffInfo)
        {
            if (staffInfo == null)
                throw new Exception("staffInfo is null");

            int staffBasicInfoId = staffInfo.StaffBasicInfo.Id;

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _staffBasicInfoService.Edit(organizationId, staffBasicInfoId, staffInfo.StaffBasicInfo);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete staff info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="id">Staff basic info id</param>
        public void Delete(int organizationId, int id)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _staffBasicInfoService.Delete(organizationId, id);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Get user list with empty staff
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>List of users</returns>
        public IEnumerable<ApplicationUser> GetEmptyStaff(int selectedOrganizationId)
        {
            List<string> staffUserIds = _db.UserTypeOrganizations
                            .Where(x => x.OrganizationId == selectedOrganizationId && x.UserType == (int)enu_UserType.Staff)
                            .Select(x => x.ApplicationUserId).ToList();

            return _userManager.Users.Include(x => x.StaffBasicInfo)
                .Where(x => staffUserIds.Contains(x.Id) && x.StaffBasicInfo == null);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

    }
}
