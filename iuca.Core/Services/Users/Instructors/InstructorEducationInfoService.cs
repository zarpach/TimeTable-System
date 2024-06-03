using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Instructors;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Users.Instructors
{
    public class InstructorEducationInfoService : IInstructorEducationInfoService
    {
        private readonly IApplicationDbContext _db;

        public InstructorEducationInfoService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Create, update, or delete instructor education info
        /// </summary>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <param name="newEducationInfoList">List of instructor education records from new model</param>
        public void EditEducationInfo(int instructorBasicInfoId, List<InstructorEducationInfoDTO> newEducationInfoList)
        {
            var existingEducationInfoList = _db.InstructorEducationInfo
                        .Where(x => x.InstructorBasicInfoId == instructorBasicInfoId).ToList();
            
            if (newEducationInfoList != null && newEducationInfoList.Any())
            {
                foreach (InstructorEducationInfoDTO educationInfo in newEducationInfoList)
                {
                    educationInfo.InstructorBasicInfoId = instructorBasicInfoId;

                    //If education info exists - update, else create education info
                    InstructorEducationInfo existingEducationInfo = _db.InstructorEducationInfo.FirstOrDefault(x => x.Id == educationInfo.Id);
                    if (existingEducationInfo != null)
                        Edit(educationInfo.Id, educationInfo);
                    else
                        Create(educationInfo);
                }
            }

            //Delete education info if it is removed from model
            if (existingEducationInfoList != null && existingEducationInfoList.Any())
            {
                foreach (InstructorEducationInfo educationInfo in existingEducationInfoList)
                {
                    if (newEducationInfoList == null || !newEducationInfoList.Any(x => x.Id == educationInfo.Id))
                        Delete(educationInfo.Id);
                }
            }
        }

        /// <summary>
        /// Create instructor education info record
        /// </summary>
        /// <param name="instructorEducationInfoDTO">Instructor education info model</param>
        public void Create(InstructorEducationInfoDTO instructorEducationInfoDTO)
        {
            if (instructorEducationInfoDTO == null)
                throw new Exception("instructorEducationInfoDTO is null");

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<University, UniversityDTO>();
                cfg.CreateMap<EducationType, EducationTypeDTO>();
                cfg.CreateMap<InstructorEducationInfoDTO, InstructorEducationInfo>(); 
            }).CreateMapper();

            InstructorEducationInfo newInstructorEducationInfo = mapperFromDTO.Map<InstructorEducationInfoDTO, InstructorEducationInfo>(instructorEducationInfoDTO);

            _db.InstructorEducationInfo.Add(newInstructorEducationInfo);
            _db.SaveChanges();

        }

        /// <summary>
        /// Edit instructor education info by id
        /// </summary>
        /// <param name="instructorEducationInfoId">Instructor education info id</param>
        /// <param name="instructorEducationInfoDTO">Instructor education info model</param>
        public void Edit(int instructorEducationInfoId, InstructorEducationInfoDTO instructorEducationInfoDTO)
        {
            if (instructorEducationInfoDTO == null)
                throw new Exception("instructorEducationInfoDTO is null");

            InstructorEducationInfo instructorEducationInfo = _db.InstructorEducationInfo.FirstOrDefault(x => x.Id == instructorEducationInfoId);
            if (instructorEducationInfo == null)
                throw new Exception($"InstructorEducationInfoDTO with id {instructorEducationInfoId} not found");

            instructorEducationInfo.MajorEng = instructorEducationInfoDTO.MajorEng;
            instructorEducationInfo.MajorRus = instructorEducationInfoDTO.MajorRus;
            instructorEducationInfo.MajorKir = instructorEducationInfoDTO.MajorKir;
            instructorEducationInfo.GraduateYear = instructorEducationInfoDTO.GraduateYear;
            instructorEducationInfo.UniversityId = instructorEducationInfoDTO.UniversityId;
            instructorEducationInfo.EducationTypeId = instructorEducationInfoDTO.EducationTypeId;

            _db.InstructorEducationInfo.Update(instructorEducationInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete instructor education info by id
        /// </summary>
        /// <param name="id">Instructor education info id</param>
        public void Delete(int id)
        {
            InstructorEducationInfo instructorEducationInfo = _db.InstructorEducationInfo.FirstOrDefault(x => x.Id == id);
            if (instructorEducationInfo == null)
                throw new Exception($"InstructorEducationInfo with id {id} not found");

            _db.InstructorEducationInfo.Remove(instructorEducationInfo);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
