using AutoMapper;
using iuca.Infrastructure.Persistence;
using System;
using System.Linq;
using iuca.Application.Interfaces.Users.Staff;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Application.Exceptions;
using iuca.Application.Enums;
using iuca.Application.DTO.Users.Staff;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Staff;

namespace iuca.Application.Services.Users.Staff
{
    public class StaffBasicInfoService : IStaffBasicInfoService
    {
        private readonly IApplicationDbContext _db;
        private readonly IOrganizationService _organizationService;

        public StaffBasicInfoService(IApplicationDbContext db,
            IOrganizationService organizationService)
        {
            _db = db;
            _organizationService = organizationService;
        }

        /// <summary>
        /// Get staff full info by staff id
        /// </summary>
        /// <param name="staffUserId">Staff user id</param>
        /// <param name="generateException">If true generates exception when staff info not found</param>
        /// <returns>Staff full info model</returns>
        public StaffBasicInfoDTO GetStaffFullInfo(string staffUserId, bool generateException = true)
        {
            StaffBasicInfo staffBasicInfo = _db.StaffBasicInfo.FirstOrDefault(x => x.ApplicationUserId == staffUserId);
            if (staffBasicInfo == null && generateException)
                throw new Exception($"StaffBasicInfo with user id {staffUserId} not found");
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<StaffBasicInfo, StaffBasicInfoDTO>()).CreateMapper();

            return mapper.Map<StaffBasicInfo, StaffBasicInfoDTO>(staffBasicInfo);
        }

        /// <summary>
        /// Get staff basic info by id
        /// </summary>
        /// <param name="id">Id of record</param>
        /// <returns>Staff basic info model</returns>
        public StaffBasicInfoDTO GetStaffBasicInfo(int id)
        {
            StaffBasicInfo staffBasicInfo = _db.StaffBasicInfo.FirstOrDefault(x => x.Id == id);
            if (staffBasicInfo == null)
                throw new Exception($"StaffBasicInfo with id {id} not found");
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<StaffBasicInfo, StaffBasicInfoDTO>()).CreateMapper();

            return mapper.Map<StaffBasicInfo, StaffBasicInfoDTO>(staffBasicInfo);
        }


        /// <summary>
        /// Create staff basic info in selected organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="staffBasicInfoDTO">Staff basic info model</param>
        /// <returns>Model of new created staff basic info record</returns>
        public StaffBasicInfoDTO Create(int selectedOrganizationId, StaffBasicInfoDTO staffBasicInfoDTO)
        {
            if (staffBasicInfoDTO == null)
                throw new Exception("staffBasicInfoDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<Organization, OrganizationDTO>();
                cfg.CreateMap<StaffBasicInfo, StaffBasicInfoDTO>();
            }).CreateMapper();

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<OrganizationDTO, Organization>();
                cfg.CreateMap<StaffBasicInfoDTO, StaffBasicInfo>();
            }).CreateMapper();

            StaffBasicInfo newStaffBasicInfo = mapperFromDTO.Map<StaffBasicInfoDTO, StaffBasicInfo>(staffBasicInfoDTO);

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");
            newStaffBasicInfo.IsMainOrganization = organization.IsMain;

            _db.StaffBasicInfo.Add(newStaffBasicInfo);
            _db.SaveChanges();

            return mapperToDTO.Map<StaffBasicInfo, StaffBasicInfoDTO>(newStaffBasicInfo);
        }

        /// <summary>
        /// Edit staff basic info
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="staffBasicInfoId">Staff basic info id</param>
        /// <param name="staffBasicInfoDTO">Staff basic info model</param>
        public void Edit(int selectedOrganizationId, int staffBasicInfoId, StaffBasicInfoDTO staffBasicInfoDTO)
        {
            if (staffBasicInfoDTO == null)
                throw new Exception("staffBasicInfoDTO is null");

            StaffBasicInfo staffBasicInfo = _db.StaffBasicInfo
                .FirstOrDefault(x => x.Id == staffBasicInfoId);

            if (staffBasicInfo == null)
                throw new Exception($"StaffBasicInfo with id {staffBasicInfoId} not found");

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");

            if (organization.IsMain || !staffBasicInfo.IsMainOrganization)
            {
                staffBasicInfo.IsMainOrganization = staffBasicInfoDTO.IsMainOrganization;
                staffBasicInfo.StaffInfo = staffBasicInfoDTO.StaffInfo;

                _db.StaffBasicInfo.Update(staffBasicInfo);
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Delete staff basic info by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of record</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            StaffBasicInfo staffBasicInfo = _db.StaffBasicInfo.FirstOrDefault(x => x.Id == id);

            if (staffBasicInfo == null)
                throw new Exception($"StaffBasicInfo with id {id} not found");

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");

            if (!organization.IsMain && staffBasicInfo.IsMainOrganization)
                throw new ModelValidationException("The record cannot be deleted in this organization", "ErrorMsg");

            if (_db.UserTypeOrganizations.Any(x => x.ApplicationUserId == staffBasicInfo.ApplicationUserId &&
                        x.UserType == (int)enu_UserType.Staff && x.OrganizationId != selectedOrganizationId))
                throw new ModelValidationException("Staff exists in another organization", "ErrorMsg");

            _db.StaffBasicInfo.Remove(staffBasicInfo);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

    }
}
