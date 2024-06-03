using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Instructors;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Users.Instructors
{
    public class InstructorOrgInfoService : IInstructorOrgInfoService
    {
        private readonly IApplicationDbContext _db;

        public InstructorOrgInfoService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Check if organization info exists for instructor
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="instructorBasicInfoId"></param>
        /// <returns></returns>
        public bool IfExists(int organizationId, int instructorBasicInfoId) 
        {
            var instructorOrgInfo = _db.InstructorOrgInfo.FirstOrDefault(x => x.InstructorBasicInfoId == instructorBasicInfoId
                                                                                    && x.OrganizationId == organizationId);
            return instructorOrgInfo != null;
        }

        /// <summary>
        /// Get instructor org info by instructor basic info id and organization id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <returns>Instructor organization info model</returns>
        public InstructorOrgInfoDTO GetInstructorOrgInfo(int organizationId, int instructorBasicInfoId) 
        {
            InstructorOrgInfoDTO instructorOrgInfoDTO = null;
            var instructorOrgInfo = _db.InstructorOrgInfo.Include(x => x.Department)
                .FirstOrDefault(x => x.InstructorBasicInfoId == instructorBasicInfoId && x.OrganizationId == organizationId);
            if (instructorOrgInfo != null) 
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Department, DepartmentDTO>();
                    cfg.CreateMap<InstructorOrgInfo, InstructorOrgInfoDTO>()
                    .ForMember(x => x.Organization, opt => opt.Ignore())
                    .ForMember(x => x.InstructorBasicInfo, opt => opt.Ignore());
                }).CreateMapper();

                instructorOrgInfoDTO = mapper.Map<InstructorOrgInfo, InstructorOrgInfoDTO>(instructorOrgInfo);
            }

            return instructorOrgInfoDTO;
        }

        /// <summary>
        /// Create instructor org info
        /// </summary>
        /// <param name="instructorOrgInfoDTO">Country model</param>
        public void Create(InstructorOrgInfoDTO instructorOrgInfoDTO)
        {
            if (instructorOrgInfoDTO == null)
                throw new Exception($"instructorOrgInfoDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<InstructorOrgInfo, InstructorOrgInfoDTO>()
                .ForMember(x => x.Organization, opt => opt.Ignore())
                .ForMember(x => x.InstructorBasicInfo, opt => opt.Ignore()); ;
            }).CreateMapper();

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<DepartmentDTO, Department>();
                cfg.CreateMap<InstructorOrgInfoDTO, InstructorOrgInfo>()
                .ForMember(x => x.Organization, opt => opt.Ignore())
                .ForMember(x => x.InstructorBasicInfo, opt => opt.Ignore()); ;
            }).CreateMapper();

            InstructorOrgInfo newInstructorOrgInfo = mapperFromDTO.Map<InstructorOrgInfoDTO, InstructorOrgInfo>(instructorOrgInfoDTO);

            _db.InstructorOrgInfo.Add(newInstructorOrgInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit instructor org info
        /// </summary>
        /// <param name="organizationId">Id of organization</param>
        /// <param name="instructorOrgInfoDTO">Instructor org info model</param>
        public void Edit(int organizationId, InstructorOrgInfoDTO instructorOrgInfoDTO)
        {
            if (instructorOrgInfoDTO == null)
                throw new Exception($"instructorOrgInfoDTO is null");

            InstructorOrgInfo instructorOrgInfo = _db.InstructorOrgInfo
                .FirstOrDefault(x => x.InstructorBasicInfoId == instructorOrgInfoDTO.InstructorBasicInfoId
                        && x.OrganizationId == organizationId);
            
            if (instructorOrgInfo == null)
                throw new Exception($"InstructorOrgInfo is not found");

            instructorOrgInfo.DepartmentId = instructorOrgInfoDTO.DepartmentId;
            instructorOrgInfo.State = instructorOrgInfoDTO.State;
            instructorOrgInfo.PartTime = instructorOrgInfoDTO.PartTime;
            instructorOrgInfo.ImportCode = instructorOrgInfoDTO.ImportCode;

            _db.InstructorOrgInfo.Update(instructorOrgInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete instructor org info 
        /// </summary>
        /// <param name="organizationId">Id of organization</param>
        /// <param name="instructorBasicInfoId">Id of instructor basic info </param>
        /// <param name="generateException">If true generates exception when record not found</param>
        public void Delete(int organizationId, int instructorBasicInfoId, bool generateException = true)
        {
            InstructorOrgInfo instructorOrgInfo = _db.InstructorOrgInfo.FirstOrDefault(x => x.InstructorBasicInfoId == instructorBasicInfoId
                        && x.OrganizationId == organizationId);

            if (instructorOrgInfo == null)
            {
                if (generateException)
                    throw new Exception($"InstructorOrgInfo is not found");
                else
                    return;
            }

            _db.InstructorOrgInfo.Remove(instructorOrgInfo);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
