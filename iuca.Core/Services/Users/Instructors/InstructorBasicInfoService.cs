using AutoMapper;
using iuca.Infrastructure.Persistence;
using System;
using System.Linq;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Application.Exceptions;
using iuca.Application.Enums;
using Microsoft.EntityFrameworkCore;
using iuca.Domain.Entities.Common;
using iuca.Application.DTO.Users.Instructors;
using System.Collections.Generic;
using iuca.Domain.Entities.Users.Instructors;

namespace iuca.Application.Services.Users.Instructors
{
    public class InstructorBasicInfoService : IInstructorBasicInfoService
    {
        private readonly IApplicationDbContext _db;
        private readonly IOrganizationService _organizationService;
        private readonly IInstructorOtherJobInfoService _instructorOtherJobInfoService;
        private readonly IInstructorEducationInfoService _instructorEducationInfoService;
        private readonly IInstructorContactInfoService _instructorContactInfoService;

        public InstructorBasicInfoService(IApplicationDbContext db,
            IOrganizationService organizationService,
            IInstructorOtherJobInfoService instructorOtherJobInfoService,
            IInstructorEducationInfoService instructorEducationInfoService,
            IInstructorContactInfoService instructorContactInfoService)
        {
            _db = db;
            _organizationService = organizationService;
            _instructorOtherJobInfoService = instructorOtherJobInfoService;
            _instructorEducationInfoService = instructorEducationInfoService;
            _instructorContactInfoService = instructorContactInfoService;
        }

        /// <summary>
        /// Get full instructor info record by intructor id
        /// </summary>
        /// <param name="id">id of instructor</param>
        /// <param name="generateException">If true generates exception when instructor info not found</param>
        /// <returns>InstructorBasicInfoDTO</returns>
        public InstructorBasicInfoDTO GetInstructorFullInfo(string id, bool generateException = true)
        {
            InstructorBasicInfo instructorBasicInfo = _db.InstructorBasicInfo
                .Include(x => x.InstructorOrgInfo)
                .Include(x => x.InstructorOrgInfo).ThenInclude(x => x.Department)
                .Include(x => x.InstructorOtherJobInfo)
                .Include(x => x.InstructorEducationInfo)
                .Include(x => x.InstructorEducationInfo).ThenInclude(x => x.University)
                .Include(x => x.InstructorEducationInfo).ThenInclude(x => x.EducationType)
                .Include(x => x.InstructorContactInfo)
                .Include(x => x.InstructorContactInfo).ThenInclude(x => x.Country)
                .Include(x => x.InstructorContactInfo).ThenInclude(x => x.CitizenshipCountry)
                .FirstOrDefault(x => x.InstructorUserId == id);

            if (instructorBasicInfo == null && generateException)
                throw new Exception($"InstructorBasicInfo with user id {id} not found");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<Organization, OrganizationDTO>();
                cfg.CreateMap<InstructorOrgInfo, InstructorOrgInfoDTO>();
                cfg.CreateMap<InstructorOtherJobInfo, InstructorOtherJobInfoDTO>();
                cfg.CreateMap<University, UniversityDTO>();
                cfg.CreateMap<EducationType, EducationTypeDTO>();
                cfg.CreateMap<InstructorEducationInfo, InstructorEducationInfoDTO>();
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<InstructorContactInfo, InstructorContactInfoDTO>();
                cfg.CreateMap<InstructorBasicInfo, InstructorBasicInfoDTO>();
                }).CreateMapper();

            var model = mapper.Map<InstructorBasicInfo, InstructorBasicInfoDTO>(instructorBasicInfo);

            return model;
        }

        /// <summary>
        /// Get instructor basic info by id
        /// </summary>
        /// <param name="id">id of instructor basic info record</param>
        /// <returns>InstructorBasicInfoDTO</returns>
        public InstructorBasicInfoDTO GetInstructorBasicInfo(int id)
        {
            InstructorBasicInfo instructorBasicInfo = _db.InstructorBasicInfo
                .Include(x => x.InstructorOrgInfo)
                .Include(x => x.InstructorOrgInfo).ThenInclude(x => x.Department)
                .Include(x => x.InstructorOtherJobInfo)
                .Include(x => x.InstructorEducationInfo)
                .Include(x => x.InstructorEducationInfo).ThenInclude(x => x.University)
                .Include(x => x.InstructorEducationInfo).ThenInclude(x => x.EducationType)
                .Include(x => x.InstructorContactInfo)
                .Include(x => x.InstructorContactInfo).ThenInclude(x => x.Country)
                .Include(x => x.InstructorContactInfo).ThenInclude(x => x.CitizenshipCountry)
                .FirstOrDefault(x => x.Id == id);

            if (instructorBasicInfo == null)
                throw new Exception($"InstructorBasicInfo with user id {id} not found");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<Organization, OrganizationDTO>();
                cfg.CreateMap<InstructorOrgInfo, InstructorOrgInfoDTO>();
                cfg.CreateMap<InstructorOtherJobInfo, InstructorOtherJobInfoDTO>();
                cfg.CreateMap<University, UniversityDTO>();
                cfg.CreateMap<EducationType, EducationTypeDTO>();
                cfg.CreateMap<InstructorEducationInfo, InstructorEducationInfoDTO>();
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<InstructorContactInfo, InstructorContactInfoDTO>();
                cfg.CreateMap<InstructorBasicInfo, InstructorBasicInfoDTO>();
            }).CreateMapper();

            var model = mapper.Map<InstructorBasicInfo, InstructorBasicInfoDTO>(instructorBasicInfo);

            return model;
        }

        /// <summary>
        /// Create instructor basic info in selected organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="instructorBasicInfoDTO">Instructor basic info model</param>
        /// <returns>Model of new created instructor basic info record</returns>
        public InstructorBasicInfoDTO Create(int selectedOrganizationId, InstructorBasicInfoDTO instructorBasicInfoDTO) 
        {
            if (instructorBasicInfoDTO == null)
                throw new Exception("instructorBasicInfoDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<University, UniversityDTO>();
                cfg.CreateMap<EducationType, EducationTypeDTO>();
                cfg.CreateMap<InstructorEducationInfo, InstructorEducationInfoDTO>();
                cfg.CreateMap<InstructorOtherJobInfo, InstructorOtherJobInfoDTO>();
                cfg.CreateMap<InstructorContactInfo, InstructorContactInfoDTO>();
                cfg.CreateMap<Organization, OrganizationDTO>();
                cfg.CreateMap<InstructorBasicInfo, InstructorBasicInfoDTO>();
                }).CreateMapper();

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<UniversityDTO, University>();
                cfg.CreateMap<EducationTypeDTO, EducationType>();
                cfg.CreateMap<InstructorEducationInfoDTO, InstructorEducationInfo>();
                cfg.CreateMap<InstructorOtherJobInfoDTO, InstructorOtherJobInfo>();
                cfg.CreateMap<InstructorContactInfoDTO, InstructorContactInfo>();
                cfg.CreateMap<OrganizationDTO, Organization>();
                cfg.CreateMap<InstructorBasicInfoDTO, InstructorBasicInfo>();
                }).CreateMapper();

            InstructorBasicInfo newInstructorBasicInfo = mapperFromDTO.Map<InstructorBasicInfoDTO, InstructorBasicInfo>(instructorBasicInfoDTO);

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");
            newInstructorBasicInfo.IsMainOrganization = organization.IsMain;

            _db.InstructorBasicInfo.Add(newInstructorBasicInfo);
            _db.SaveChanges();

            return mapperToDTO.Map<InstructorBasicInfo, InstructorBasicInfoDTO>(newInstructorBasicInfo);
        }

        /// <summary>
        /// Edit instructor basic info
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="instructorBasicInfoDTO">Instructor basic info model</param>
        public void Edit(int selectedOrganizationId, InstructorBasicInfoDTO instructorBasicInfoDTO)
        {
            if (instructorBasicInfoDTO == null)
                throw new Exception("instructorBasicInfoDTO is null");

            InstructorBasicInfo instructorBasicInfo = _db.InstructorBasicInfo
                .Include(x => x.InstructorOtherJobInfo)
                .Include(x => x.InstructorEducationInfo)
                .Include(x => x.InstructorContactInfo)
                .Include(x => x.InstructorOrgInfo)
                .FirstOrDefault(x => x.Id == instructorBasicInfoDTO.Id);

            if (instructorBasicInfo == null)
                throw new Exception($"InstructorBasicInfo with id {instructorBasicInfoDTO.Id} not found");

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");

            if (organization.IsMain || !instructorBasicInfo.IsMainOrganization)
            {
                instructorBasicInfo.IsMainOrganization = instructorBasicInfoDTO.IsMainOrganization;
                instructorBasicInfo.IsMarried = instructorBasicInfoDTO.IsMarried;
                instructorBasicInfo.ChildrenQty = instructorBasicInfoDTO.ChildrenQty;
                instructorBasicInfo.IsChanged = true;

                _db.InstructorBasicInfo.Update(instructorBasicInfo);
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Delete instructor basic info by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of record</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            InstructorBasicInfo instructorBasicInfo = _db.InstructorBasicInfo
                .Include(x => x.InstructorOtherJobInfo)
                .Include(x => x.InstructorEducationInfo)
                .Include(x => x.InstructorContactInfo)
                .FirstOrDefault(x => x.Id == id);

            if (instructorBasicInfo == null)
                throw new Exception($"InstructorBasicInfo with id {id} not found");

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");

            if (!organization.IsMain && instructorBasicInfo.IsMainOrganization)
                throw new ModelValidationException("The record cannot be deleted in this organization", "ErrorMsg");

            if (_db.UserTypeOrganizations.Any(x => x.ApplicationUserId == instructorBasicInfo.InstructorUserId &&
                        x.UserType == (int)enu_UserType.Instructor && x.OrganizationId != selectedOrganizationId))
                throw new ModelValidationException("Instructor exists in another organization", "ErrorMsg");

            //Delete instructor other job info
            if (instructorBasicInfo.InstructorOtherJobInfo.Any())
            {
                foreach (InstructorOtherJobInfo otherJobInfo in instructorBasicInfo.InstructorOtherJobInfo.ToList())
                    _instructorOtherJobInfoService.Delete(otherJobInfo.Id);
            }

            //Delete instructor education info
            if (instructorBasicInfo.InstructorEducationInfo.Any())
            {
                foreach (InstructorEducationInfo educationInfo in instructorBasicInfo.InstructorEducationInfo.ToList())
                    _instructorEducationInfoService.Delete(educationInfo.Id);
            }

            if (instructorBasicInfo.InstructorContactInfo != null)
                _instructorContactInfoService.Delete(instructorBasicInfo.InstructorContactInfo.Id);

            _db.InstructorBasicInfo.Remove(instructorBasicInfo);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        /// <summary>
        /// Set is main organization flag
        /// </summary>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <param name="isMainOrganization">Is main organization flag</param>
        public void SetOwnerFlag(int instructorBasicInfoId, bool isMainOrganization) 
        {
            InstructorBasicInfo instructorBasicInfo = _db.InstructorBasicInfo
                .FirstOrDefault(x => x.Id == instructorBasicInfoId);

            if (instructorBasicInfo == null)
                throw new Exception($"InstructorBasicInfo with id {instructorBasicInfoId} not found");

            instructorBasicInfo.IsMainOrganization = isMainOrganization;
            _db.InstructorBasicInfo.Update(instructorBasicInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Set IsChanged flag for export to old db
        /// </summary>
        /// <param name="applicationUserId">Application User Id</param>
        /// <param name="isChanged">Is changed</param>
        public void SetIsChangedFlag(string applicationUserId, bool isChanged)
        {
            var instructorBasicInfo = _db.InstructorBasicInfo.FirstOrDefault(x => x.InstructorUserId == applicationUserId);
            if (instructorBasicInfo == null)
                throw new Exception($"Instructor basic info not found");

            instructorBasicInfo.IsChanged = isChanged;
            _db.SaveChanges();
        }

        /// <summary>
        /// Set IsChanged flag for export to old db
        /// </summary>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <param name="isChanged">Is changed</param>
        public void SetIsChangedFlag(int instructorBasicInfoId, bool isChanged) 
        {
            var instructorBasicInfo = _db.InstructorBasicInfo.FirstOrDefault(x => x.Id == instructorBasicInfoId);
            if (instructorBasicInfo == null)
                throw new Exception($"Instructor basic info with id {instructorBasicInfoId} not found");

            instructorBasicInfo.IsChanged = isChanged;
            _db.SaveChanges();
        }

    }
}
