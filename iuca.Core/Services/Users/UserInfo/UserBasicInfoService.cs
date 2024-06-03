using AutoMapper;
using iuca.Domain.Entities.Users;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.Interfaces.Common;
using iuca.Application.DTO.Common;
using iuca.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using iuca.Domain.Entities.Common;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;

namespace iuca.Application.Services.Users.UserInfo
{
    public class UserBasicInfoService : IUserBasicInfoService
    {
        private readonly IApplicationDbContext _db;
        private readonly IOrganizationService _organizationService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public UserBasicInfoService(IApplicationDbContext db,
            IOrganizationService organizationService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _organizationService = organizationService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get user basic info list
        /// </summary>
        /// <returns>User basic info list</returns>
        public IEnumerable<UserBasicInfoDTO> GetUserBasicInfoList()
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Nationality, NationalityDTO>();
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>(); }).CreateMapper();
            return mapper.Map<IEnumerable<UserBasicInfo>, IEnumerable<UserBasicInfoDTO>>(_db.UserBasicInfo
                        .Include(x => x.Citizenship).Include(x => x.Nationality));
        }

        /// <summary>
        /// Get user basic info list by id list
        /// </summary>
        /// <param name="ids">List of ids</param>
        /// <returns>User basic info list</returns>
        public IEnumerable<UserBasicInfoDTO> GetUserBasicInfoList(List<int> ids)
        {
            var basicUserInfoList = _db.UserBasicInfo.Include(x => x.Citizenship)
                .Include(x => x.Nationality).Where(x => ids.Contains(x.Id));

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Nationality, NationalityDTO>();
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>();
            }).CreateMapper();

            return mapper.Map<IEnumerable<UserBasicInfo>, IEnumerable<UserBasicInfoDTO>>(basicUserInfoList);
        }

        /// <summary>
        /// Get user basic info record by id
        /// </summary>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="generateException">If true generates exception when info not found</param>
        /// <returns>User basic info record</returns>
        public UserBasicInfoDTO GetUserFullInfo(string applicationUserId, bool generateException = true)
        {
            var user = _userManager.Users.Include(x => x.UserBasicInfo).FirstOrDefault(x => x.Id == applicationUserId);

            if (user == null && generateException)
                throw new Exception("User not found");

            if (user.UserBasicInfo == null && generateException)
                throw new Exception("User info not found");

            return GetUserBasicInfo(user.UserBasicInfo.Id);
        }

        /// <summary>
        /// Get user basic info record by id
        /// </summary>
        /// <param name="id">Record id</param>
        /// <returns>User basic info record</returns>
        public UserBasicInfoDTO GetUserBasicInfo(int id)
        {
            UserBasicInfo basicUserInfo = _db.UserBasicInfo.Include(x => x.Citizenship).Include(x => x.Nationality).FirstOrDefault(x => x.Id == id);
            if (basicUserInfo == null)
                throw new Exception($"UserBasicInfo with id {id} not found");
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Nationality, NationalityDTO>();
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>();
                }).CreateMapper();

            return mapper.Map<UserBasicInfo, UserBasicInfoDTO>(basicUserInfo);
        }

        /// <summary>
        /// Create user basic info record
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="userBasicInfoDTO">User basic info model</param>
        /// <returns>Model of new created record</returns>
        public UserBasicInfoDTO Create(int selectedOrganizationId, UserBasicInfoDTO userBasicInfoDTO) 
        {
            if (userBasicInfoDTO == null)
                throw new Exception("userBasicInfoDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<Nationality, NationalityDTO>();
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>();
                }).CreateMapper();
            
            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<NationalityDTO, Nationality>();
                cfg.CreateMap<CountryDTO, Country>();
                cfg.CreateMap<UserBasicInfoDTO, UserBasicInfo>();
                }).CreateMapper();

            UserBasicInfo newUserBasicInfo = mapperFromDTO.Map<UserBasicInfoDTO, UserBasicInfo>(userBasicInfoDTO);

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");
            newUserBasicInfo.IsMainOrganization = organization.IsMain;

            _db.UserBasicInfo.Add(newUserBasicInfo);
            _db.SaveChanges();
            
            return mapperToDTO.Map<UserBasicInfo, UserBasicInfoDTO>(newUserBasicInfo);
        }

        /// <summary>
        /// Edit user basic info record
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="userBasicInfoDTO">User basic info model</param>
        public void Edit(int selectedOrganizationId, UserBasicInfoDTO userBasicInfoDTO)
        {
            if (userBasicInfoDTO == null)
                throw new Exception("UserBasicInfoDTO is null");

            UserBasicInfo userBasicInfo = _db.UserBasicInfo.FirstOrDefault(x => x.Id == userBasicInfoDTO.Id);
            if (userBasicInfo == null)
                throw new Exception($"UserBasicInfo with id {userBasicInfoDTO.Id} not found");

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");

            /*if (!organization.IsMain && userBasicInfo.IsMainOrganization)
                throw new ModelValidationException("The record cannot be modified in this organization", "ErrorMsg");*/
            
            userBasicInfo.LastNameRus = userBasicInfoDTO.LastNameRus;
            userBasicInfo.FirstNameRus = userBasicInfoDTO.FirstNameRus;
            userBasicInfo.MiddleNameRus = userBasicInfoDTO.MiddleNameRus;
            userBasicInfo.Sex = userBasicInfoDTO.Sex;
            userBasicInfo.DateOfBirth = userBasicInfoDTO.DateOfBirth;
            userBasicInfo.IsMainOrganization = userBasicInfoDTO.IsMainOrganization;
            userBasicInfo.NationalityId = userBasicInfoDTO.NationalityId;
            userBasicInfo.CitizenshipId = userBasicInfoDTO.CitizenshipId;

            _db.UserBasicInfo.Update(userBasicInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete user basic info record
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of the record</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            UserBasicInfo userBasicInfo = _db.UserBasicInfo.FirstOrDefault(x => x.Id == id);
            if (userBasicInfo == null)
                throw new Exception($"UserBasicInfo with id {id} not found");

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");

            if (!organization.IsMain && userBasicInfo.IsMainOrganization)
                throw new ModelValidationException("The record cannot be deleted in this organization", "ErrorMsg");

            _db.UserBasicInfo.Remove(userBasicInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Set is main organization flag
        /// </summary>
        /// <param name="userBasicInfoId">User basic info id</param>
        /// <param name="isMainOrganization">Is main organization flag</param>
        public void SetOwnerFlag(int userBasicInfoId, bool isMainOrganization)
        {
            UserBasicInfo userBasicInfo = _db.UserBasicInfo
                .FirstOrDefault(x => x.Id == userBasicInfoId);

            if (userBasicInfo == null)
                throw new Exception($"UserBasicInfo with id {userBasicInfoId} not found");

            userBasicInfo.IsMainOrganization = isMainOrganization;
            _db.UserBasicInfo.Update(userBasicInfo);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
