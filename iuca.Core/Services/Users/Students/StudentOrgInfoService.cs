using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Interfaces.Users.Students;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Users.Students
{
    public class StudentOrgInfoService : IStudentOrgInfoService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public StudentOrgInfoService(IApplicationDbContext db,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Check if organization info exists for student
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="studentBasicInfoId"></param>
        /// <returns></returns>
        public bool IfExists(int organizationId, int studentBasicInfoId)
        {
            var studentOrgInfo = _db.StudentOrgInfo.FirstOrDefault(x => x.StudentBasicInfoId == studentBasicInfoId
                                                                                    && x.OrganizationId == organizationId);
            return studentOrgInfo != null;
        }

        /// <summary>
        /// Get student org info list by student basic info ids and organization id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentBasicInfoIdList">List of student basic info id</param>
        /// <returns>List of student organization info model</returns>
        public List<StudentOrgInfoDTO> GetStudentOrgInfoList(int organizationId, List<int> studentBasicInfoIdList)
        {
            var studentOrgInfoList = _db.StudentOrgInfo.Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                .Where(x => studentBasicInfoIdList.Any(y => y == x.StudentBasicInfoId) && 
                x.OrganizationId == organizationId).ToList();

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Organization, OrganizationDTO>();
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<StudentOrgInfo, StudentOrgInfoDTO>()
                .ForMember(x => x.Organization, opt => opt.Ignore())
                .ForMember(x => x.StudentBasicInfo, opt => opt.Ignore());
            }).CreateMapper();

            return mapper.Map<List<StudentOrgInfo>, List<StudentOrgInfoDTO>>(studentOrgInfoList);
        }

        /// <summary>
        /// Get student org info by student basic info id and organization id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student organization info model</returns>
        public StudentOrgInfoDTO GetStudentOrgInfoByUserId(int organizationId, string studentUserId)
        {
            var student = _userManager.Users.Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.Id == studentUserId);
            
            if (student == null)
                throw new Exception("Student not found");

            if (student.StudentBasicInfo == null)
                throw new Exception("Student basic info not found");

            return GetStudentOrgInfo(organizationId, student.StudentBasicInfo.Id);
        }

        /// <summary>
        /// Get student org info by student basic info id and organization id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentBasicInfoId">Student basic info id</param>
        /// <returns>Student organization info model</returns>
        public StudentOrgInfoDTO GetStudentOrgInfo(int organizationId, int studentBasicInfoId)
        {
            StudentOrgInfoDTO studentOrgInfoDTO = null;
            var studentOrgInfo = _db.StudentOrgInfo.Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                .Include(x => x.PrepDepartmentGroup).ThenInclude(x => x.Department)
                .FirstOrDefault(x => x.StudentBasicInfoId == studentBasicInfoId && x.OrganizationId == organizationId);

            if (studentOrgInfo != null)
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Organization, OrganizationDTO>();
                    cfg.CreateMap<Department, DepartmentDTO>();
                    cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                    cfg.CreateMap<StudentOrgInfo, StudentOrgInfoDTO>()
                    .ForMember(x => x.Organization, opt => opt.Ignore())
                    .ForMember(x => x.StudentBasicInfo, opt => opt.Ignore());
                }).CreateMapper();

                studentOrgInfoDTO = mapper.Map<StudentOrgInfo, StudentOrgInfoDTO>(studentOrgInfo);
            }

            return studentOrgInfoDTO;
        }

        /// <summary>
        /// Create student org info
        /// </summary>
        /// <param name="studentOrgInfoDTO">Country model</param>
        public void Create(StudentOrgInfoDTO studentOrgInfoDTO)
        {
            if (studentOrgInfoDTO == null)
                throw new Exception($"studentOrgInfoDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<StudentOrgInfo, StudentOrgInfoDTO>()
                .ForMember(x => x.Organization, opt => opt.Ignore())
                .ForMember(x => x.StudentBasicInfo, opt => opt.Ignore()); ;
            }).CreateMapper();

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<DepartmentGroupDTO, DepartmentGroup>();
                cfg.CreateMap<StudentOrgInfoDTO, StudentOrgInfo>()
                .ForMember(x => x.Organization, opt => opt.Ignore())
                .ForMember(x => x.StudentBasicInfo, opt => opt.Ignore()); ;
            }).CreateMapper();

            StudentOrgInfo newStudentOrgInfo = mapperFromDTO.Map<StudentOrgInfoDTO, StudentOrgInfo>(studentOrgInfoDTO);

            _db.StudentOrgInfo.Add(newStudentOrgInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit student org info
        /// </summary>
        /// <param name="organizationId">Id of organization</param>
        /// <param name="studentOrgInfoDTO">student org info model</param>
        public void Edit(int organizationId, StudentOrgInfoDTO studentOrgInfoDTO)
        {
            if (studentOrgInfoDTO == null)
                throw new Exception($"studentOrgInfoDTO is null");

            StudentOrgInfo studentOrgInfo = _db.StudentOrgInfo.FirstOrDefault(x => x.StudentBasicInfoId == studentOrgInfoDTO.StudentBasicInfoId
                        && x.OrganizationId == organizationId);

            if (studentOrgInfo == null)
                throw new Exception($"StudentOrgInfo is not found");

            studentOrgInfo.DepartmentGroupId = studentOrgInfoDTO.DepartmentGroupId;
            studentOrgInfo.PrepDepartmentGroupId = studentOrgInfoDTO.PrepDepartmentGroupId;
            studentOrgInfo.StudentId = studentOrgInfoDTO.StudentId;
            if (studentOrgInfo.IsPrep && !studentOrgInfoDTO.IsPrep) 
                studentOrgInfo.PrepDepartmentGroupId = null;
            studentOrgInfo.IsPrep = studentOrgInfoDTO.IsPrep;
            studentOrgInfo.State = studentOrgInfoDTO.State;

            _db.StudentOrgInfo.Update(studentOrgInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete student org info 
        /// </summary>
        /// <param name="organizationId">Id of organization</param>
        /// <param name="studentBasicInfoId">Id of student basic info/param>
        public void Delete(int organizationId, int studentBasicInfoId)
        {
            StudentOrgInfo studentOrgInfo = _db.StudentOrgInfo.FirstOrDefault(x => x.StudentBasicInfoId == studentBasicInfoId
                        && x.OrganizationId == organizationId);

            if (studentOrgInfo == null)
                throw new Exception($"StudentOrgInfo is not found");

            _db.StudentOrgInfo.Remove(studentOrgInfo);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
