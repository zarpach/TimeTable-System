using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.ViewModels.Users.Instructors;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


namespace iuca.Application.Services.Users.Instructors
{
    public class InstructorInfoService : IInstructorInfoService
    {
        private readonly IApplicationDbContext _db;
        private readonly IOrganizationService _organizationService;
        private readonly IInstructorBasicInfoService _instructorBasicInfoService;
        private readonly IInstructorOrgInfoService _instructorOrgInfoService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public InstructorInfoService(IApplicationDbContext db, 
            IInstructorBasicInfoService instructorBasicInfoService,
            IOrganizationService organizationService,
            IInstructorOrgInfoService instructorOrgInfoService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _organizationService = organizationService;
            _instructorBasicInfoService = instructorBasicInfoService;
            _instructorOrgInfoService = instructorOrgInfoService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get instructor info list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="state">Instructor state</param>
        /// <param name="departmentId">Department id</param>
        /// <returns>List of instructor info</returns>
        public IEnumerable<InstructorInfoBriefViewModel> GetInstructorInfoList(int organizationId, enu_InstructorState state,
            int? departmentId) 
        {
            List<InstructorInfoBriefViewModel> instructorInfoList = new List<InstructorInfoBriefViewModel>();

            List<string> userIds = _db.UserTypeOrganizations
                                .Where(x => x.OrganizationId == organizationId && x.UserType == (int)enu_UserType.Instructor)
                                .Select(x => x.ApplicationUserId).ToList();

            var usersQuery = _userManager.Users
                            .Include(x => x.UserBasicInfo)
                            .Include(x => x.InstructorBasicInfo)
                            .ThenInclude(x => x.InstructorOrgInfo)
                            .ThenInclude(x => x.Department)
                            .Where(x => userIds.Contains(x.Id) && (x.InstructorBasicInfo == null || 
                                x.InstructorBasicInfo.InstructorOrgInfo.Count(y => y.OrganizationId == organizationId) == 0 ||
                                x.InstructorBasicInfo.InstructorOrgInfo.Any(x => x.State == (int)state && x.OrganizationId == organizationId)));

            if (departmentId != null) 
            {
                usersQuery = usersQuery.Where(x => x.InstructorBasicInfo.InstructorOrgInfo
                                .Any(x => x.DepartmentId == departmentId));
            }

            var users = usersQuery.ToList();

            foreach (var user in users)
                instructorInfoList.Add(FillInstructorBriefInfo(organizationId, user));

            return instructorInfoList.OrderBy(x => x.FullNameEng);
        }

        private InstructorInfoBriefViewModel FillInstructorBriefInfo(int organizationId, ApplicationUser user)
        {
            InstructorInfoBriefViewModel instructorInfoVM = new InstructorInfoBriefViewModel();
            instructorInfoVM.FullNameEng = user.FullNameEng;
            instructorInfoVM.Email = user.Email;
            instructorInfoVM.InstructorUserId = user.Id;
            instructorInfoVM.BasicInfoExists = user.UserBasicInfo != null;
            instructorInfoVM.InstructorBasicInfoId = user.InstructorBasicInfo?.Id ?? 0;
            instructorInfoVM.IsChanged = user.InstructorBasicInfo != null && user.InstructorBasicInfo.IsChanged;

            if (user.InstructorBasicInfo != null)
            {
                OrganizationDTO organization = _organizationService.GetOrganization(organizationId);
                if (organization == null)
                    throw new Exception("organization is null");

                var instructorOrgInfo = user.InstructorBasicInfo.InstructorOrgInfo
                        .FirstOrDefault(x => x.OrganizationId == organizationId);

                if (instructorOrgInfo != null)
                {
                    instructorInfoVM.Department = instructorOrgInfo.Department.Code;
                    instructorInfoVM.State = (enu_InstructorState)instructorOrgInfo.State;
                    instructorInfoVM.ImportCode = instructorOrgInfo.ImportCode;
                }

                //Only main organization is allowed to modify data if IsMainOrganization = true
                instructorInfoVM.IsReadOnly = user.InstructorBasicInfo.IsMainOrganization && !organization.IsMain;
            }

            return instructorInfoVM;
        }

        /// <summary>
        /// Refresh instructor states. If email starts with "_" - inactive, else active
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        public void RefreshInstructorStates(int organizationId) 
        {
            var instructorOrgInfo = _db.InstructorOrgInfo.Include(x => x.InstructorBasicInfo)
                .Where(x => x.OrganizationId == organizationId)
                .ToList();

            var userIds = instructorOrgInfo.Select(x => x.InstructorBasicInfo.InstructorUserId).ToList();
            var instructorUsers = _userManager.Users.Where(x => userIds.Contains(x.Id)).ToList();

            foreach (var orgInfo in instructorOrgInfo) 
            {
                var user = instructorUsers.FirstOrDefault(x => x.Id == orgInfo.InstructorBasicInfo.InstructorUserId);
                if (user.Email.StartsWith("_"))
                    orgInfo.State = (int)enu_InstructorState.Inactive;
                else 
                    orgInfo.State = (int)enu_InstructorState.Active;
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// Get instructor info by instructor id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <returns>Instructor info model</returns>
        public InstructorInfoDetailsViewModel GetInstructorDetailsInfo(int organizationId, string instructorUserId) 
        {
            var user = _userManager.Users
                .Include(x => x.UserBasicInfo).ThenInclude(x => x.Citizenship)
                .Include(x => x.UserBasicInfo).ThenInclude(x => x.Nationality)
                .FirstOrDefault(x => x.Id == instructorUserId);

            if (user == null)
                throw new Exception("User not found");

            InstructorInfoDetailsViewModel instructorInfoVM = new InstructorInfoDetailsViewModel();
            instructorInfoVM.InstructorUserId = instructorUserId;
            
            InstructorUserInfoViewModel userInfo = new InstructorUserInfoViewModel();
            userInfo.ApplicationUserId = user.Id;
            userInfo.LastNameEng = user.LastNameEng;
            userInfo.FirstNameEng = user.FirstNameEng;
            userInfo.MiddleNameEng = user.MiddleNameEng;
            userInfo.Email = user.Email;
            instructorInfoVM.UserInfo = userInfo;

            //User basic info
            if (user.UserBasicInfo != null)
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Nationality, NationalityDTO>();
                    cfg.CreateMap<Country, CountryDTO>();
                    cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>();
                }).CreateMapper();

                instructorInfoVM.UserBasicInfo = mapper.Map<UserBasicInfo, UserBasicInfoDTO>(user.UserBasicInfo);
            }
            else 
            {
                instructorInfoVM.UserBasicInfo = new UserBasicInfoDTO { ApplicationUserId = instructorUserId };
            }

            //Instructor full info
            instructorInfoVM.InstructorBasicInfo = _instructorBasicInfoService.GetInstructorFullInfo(instructorUserId, false);
            if (instructorInfoVM.InstructorBasicInfo != null) 
            {
                OrganizationDTO organization = _organizationService.GetOrganization(organizationId);
                if (organization == null)
                    throw new Exception("organization is null");

                //Only main organization is allowed to modify data if IsMainOrganization = true
                instructorInfoVM.IsReadOnly = instructorInfoVM.InstructorBasicInfo.IsMainOrganization && !organization.IsMain;

                //Get instructor organization information
                instructorInfoVM.InstructorOrgInfo = _instructorOrgInfoService.GetInstructorOrgInfo(organizationId, instructorInfoVM.InstructorBasicInfo.Id);
            }

            return instructorInfoVM;
        }

        /// <summary>
        /// Edit instructor user info
        /// </summary>
        /// <param name="model">Instructor info view model</param>
        public void EditInstructorUserInfo(InstructorUserInfoViewModel model) 
        {
            if (model == null)
                throw new Exception("Model is null");

            var user = _userManager.Users.FirstOrDefault(x => x.Id == model.ApplicationUserId);
            if (user == null)
                throw new Exception("User not found");

            user.LastNameEng = model.LastNameEng;
            user.FirstNameEng = model.FirstNameEng;
            user.MiddleNameEng = model.MiddleNameEng;

            var result = _userManager.UpdateAsync(user).Result;

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors));
        }

        /// <summary>
        /// Delete instructor info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="id">Instructor basic info id</param>
        public void Delete(int organizationId, int id) 
        {
            using (var transaction = _db.Database.BeginTransaction()) 
            {
                try
                {
                    _instructorOrgInfoService.Delete(organizationId, id, false);
                    _instructorBasicInfoService.Delete(organizationId, id);
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
        /// Get user list with empty instructors
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>List of users</returns>
        public IEnumerable<ApplicationUser> GetEmptyInstructors(int selectedOrganizationId) 
        {
            List<string> instructorUserIds = _db.UserTypeOrganizations
                            .Where(x => x.OrganizationId == selectedOrganizationId && x.UserType == (int)enu_UserType.Instructor)
                            .Select(x => x.ApplicationUserId).Distinct().ToList();
            
            return _userManager.Users.Include(x => x.InstructorBasicInfo).Where(x => instructorUserIds.Contains(x.Id) && x.InstructorBasicInfo == null);
        }

        /// <summary>
        /// Get instructor SelectList
        /// </summary>
        /// <param name="instructorUserId">Selected instructor user id</param>
        /// <param name="selectedOrganizationId">Id of selected organization</param>
        /// <returns>SelectList of instructors</returns>
        public List<SelectListItem> GetInstructorSelectList(string instructorUserId, int selectedOrganizationId)
        {
            var instructors = GetInstructorInfoList(selectedOrganizationId, enu_InstructorState.Active, null);
            return new SelectList(instructors, "InstructorUserId", "FullNameEng", instructorUserId).ToList();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
