using AutoMapper;
using iuca.Domain.Entities.Users;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Application.Exceptions;
using iuca.Application.Enums;
using iuca.Application.DTO.Users.Students;
using Microsoft.EntityFrameworkCore;
using iuca.Domain.Entities.Common;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Domain.Entities.Users.Students;
using iuca.Domain.Entities.Users.UserInfo;

namespace iuca.Application.Services.Users.Students
{
    public class StudentBasicInfoService : IStudentBasicInfoService
    {
        private readonly IApplicationDbContext _db;
        private readonly IOrganizationService _organizationService;
        private readonly IStudentContactInfoService _studentContactInfoService;
        private readonly IStudentParentsInfoService _studentParentsInfoService;
        public StudentBasicInfoService(IApplicationDbContext db,
            IOrganizationService organizationService,
            IStudentContactInfoService studentContactInfoService,
            IStudentParentsInfoService studentParentsInfoService)
        {
            _db = db;
            _organizationService = organizationService;
            _studentContactInfoService = studentContactInfoService;
            _studentParentsInfoService = studentParentsInfoService;
        }

        /// <summary>
        /// Get student basic info by id
        /// </summary>
        /// <param name="id">Id of record</param>
        /// <returns>Student basic info model</returns>
        public StudentBasicInfoDTO GetStudentBasicInfo(int id)
        {
            StudentBasicInfo studentBasicInfo = _db.StudentBasicInfo
                .Include(x => x.StudentOrgInfo).ThenInclude(x => x.DepartmentGroup)
                .FirstOrDefault(x => x.Id == id);

            if (studentBasicInfo == null)
                throw new Exception($"StudentBasicInfo with id {id} not found");

            var mapper = new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>()
                .ForMember(x => x.Department, opt => opt.Ignore())
                .ForMember(x => x.Organization, opt => opt.Ignore());
                cfg.CreateMap<StudentOrgInfo, StudentOrgInfoDTO>()
                .ForMember(x => x.Organization, opt => opt.Ignore());
                cfg.CreateMap<StudentBasicInfo, StudentBasicInfoDTO>()
                .ForMember(x => x.StudentContactInfo, opt => opt.Ignore())
                .ForMember(x => x.StudentParentsInfo, opt => opt.Ignore());
                }).CreateMapper();

            return mapper.Map<StudentBasicInfo, StudentBasicInfoDTO>(studentBasicInfo);
        }

        /// <summary>
        /// Get student basic info record by id
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="generateException">If true generates exeption if record is not found</param>
        /// <returns>StudentBasicInfoDTO</returns>
        public StudentBasicInfoDTO GetStudentFullInfo(string studentUserId, bool generateException = true)
        {
            StudentBasicInfo studentBasicInfo = _db.StudentBasicInfo
                .Include(x => x.StudentContactInfo)
                .Include(x => x.StudentOrgInfo).ThenInclude(x => x.DepartmentGroup)
                .Include(x => x.StudentLanguages)
                .Include(x => x.StudentParentsInfo)
                .FirstOrDefault(x => x.ApplicationUserId == studentUserId);

            if (studentBasicInfo == null) 
            {
                if (generateException)
                    throw new Exception($"StudentBasicInfo with user id {studentUserId} not found");
                else 
                {
                    studentBasicInfo = new StudentBasicInfo();
                    studentBasicInfo.ApplicationUserId = studentUserId;
                }
            }


            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<StudentOrgInfo, StudentOrgInfoDTO>();
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<Nationality, NationalityDTO>();
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<Organization, OrganizationDTO>();
                cfg.CreateMap<StudentContactInfo, StudentContactInfoDTO>();
                cfg.CreateMap<StudentLanguage, StudentLanguageDTO>();
                cfg.CreateMap<StudentParentsInfo, StudentParentsInfoDTO>();
                cfg.CreateMap<StudentBasicInfo, StudentBasicInfoDTO>();
            }).CreateMapper();

            var model = mapper.Map<StudentBasicInfo, StudentBasicInfoDTO>(studentBasicInfo);

            return model;
        }

        /// <summary>
        /// Create student basic info in selected organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studentBasicInfoDTO">Student basic info model</param>
        /// <returns>Model of new created student basic info record</returns>
        public StudentBasicInfoDTO Create(int selectedOrganizationId, StudentBasicInfoDTO studentBasicInfoDTO)
        {
            if (studentBasicInfoDTO == null)
                throw new Exception("studentBasicInfoDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<Organization, OrganizationDTO>();
                cfg.CreateMap<StudentContactInfo, StudentContactInfoDTO>();
                cfg.CreateMap<StudentBasicInfo, StudentBasicInfoDTO>();
            }).CreateMapper();

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<OrganizationDTO, Organization>();
                cfg.CreateMap<StudentContactInfoDTO, StudentContactInfo>();
                cfg.CreateMap<StudentBasicInfoDTO, StudentBasicInfo>();
            }).CreateMapper();

            StudentBasicInfo newStudentBasicInfo = mapperFromDTO.Map<StudentBasicInfoDTO, StudentBasicInfo>(studentBasicInfoDTO);

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");
            newStudentBasicInfo.IsMainOrganization = organization.IsMain;

            _db.StudentBasicInfo.Add(newStudentBasicInfo);
            _db.SaveChanges();

            return mapperToDTO.Map<StudentBasicInfo, StudentBasicInfoDTO>(newStudentBasicInfo);
        }

        /// <summary>
        /// Edit student basic info
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studentBasicInfoDTO">Student basic info model</param>
        public void Edit(int selectedOrganizationId, StudentBasicInfoDTO studentBasicInfoDTO)
        {
            if (studentBasicInfoDTO == null)
                throw new Exception("studentBasicInfoDTO is null");

            StudentBasicInfo studentBasicInfo = _db.StudentBasicInfo
                .Include(x => x.StudentOrgInfo)
                .FirstOrDefault(x => x.Id == studentBasicInfoDTO.Id);

            if (studentBasicInfo == null)
                throw new Exception($"StudentBasicInfo with id {studentBasicInfoDTO.Id} not found");

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");

            if (organization.IsMain || !studentBasicInfo.IsMainOrganization)
            {
                studentBasicInfo.IsMainOrganization = studentBasicInfoDTO.IsMainOrganization;
                studentBasicInfo.ArmyService = studentBasicInfoDTO.ArmyService;
                studentBasicInfo.Toefl = studentBasicInfoDTO.Toefl;

                //EditContactInfo(studentBasicInfoId, studentBasicInfoDTO.StudentContactInfo);

                /*EditLanguages(studentBasicInfoId, studentBasicInfoDTO.StudentLanguages);

                _studentParentsInfoService.EditStudentParentsInfo(studentBasicInfoId, 
                        studentBasicInfoDTO.StudentParentsInfo);*/

                _db.StudentBasicInfo.Update(studentBasicInfo);
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Delete student basic info by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of record</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            StudentBasicInfo studentBasicInfo = _db.StudentBasicInfo.FirstOrDefault(x => x.Id == id);

            if (studentBasicInfo == null)
                throw new Exception($"StudentBasicInfo with id {id} not found");

            OrganizationDTO organization = _organizationService.GetOrganization(selectedOrganizationId);
            if (organization == null)
                throw new Exception("organization is null");

            if (!organization.IsMain && studentBasicInfo.IsMainOrganization)
                throw new ModelValidationException("The record cannot be deleted in this organization", "ErrorMsg");

            if (_db.UserTypeOrganizations.Any(x => x.ApplicationUserId == studentBasicInfo.ApplicationUserId &&
                        x.UserType == (int)enu_UserType.Student && x.OrganizationId != selectedOrganizationId))
                throw new ModelValidationException("Student exists in another organization", "ErrorMsg");

            _db.StudentBasicInfo.Remove(studentBasicInfo);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        private void EditContactInfo(int studentBasicInfoId, StudentContactInfoDTO contactInfo)
        {
            if (contactInfo != null)
            {
                if (contactInfo.Id != 0)
                    _studentContactInfoService.Edit(contactInfo.Id, contactInfo);
                else
                {
                    contactInfo.StudentBasicInfoId = studentBasicInfoId;
                    _studentContactInfoService.Create(contactInfo);
                }
            }
        }

        private void EditLanguages(int studentBasicInfoId, List<StudentLanguageDTO> languages) 
        {
            var dbLanguages = _db.StudentLanguages.Where(x => x.StudentBasicInfoId == studentBasicInfoId).ToList();

            foreach (var language in languages) 
            {
                var dbLanguage = dbLanguages.FirstOrDefault(x => x.LanguageId == language.LanguageId);
                if (dbLanguage == null)
                {
                    _db.StudentLanguages.Add(new StudentLanguage
                    {
                        StudentBasicInfoId = studentBasicInfoId,
                        LanguageId = language.LanguageId
                    });
                }
                else 
                    dbLanguages.Remove(dbLanguage);
            }

            foreach (var restLanguage in dbLanguages) 
                _db.StudentLanguages.Remove(restLanguage);
        }

    }
}
