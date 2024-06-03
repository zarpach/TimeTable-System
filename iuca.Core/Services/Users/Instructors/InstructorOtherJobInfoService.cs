using AutoMapper;
using iuca.Application.DTO.Users.Instructors;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Domain.Entities.Users;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Users.Instructors
{
    public class InstructorOtherJobInfoService : IInstructorOtherJobInfoService
    {
        private readonly IApplicationDbContext _db;

        public InstructorOtherJobInfoService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Create, update, or delete instructor other job info
        /// </summary>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <param name="newOtherJobInfoList">List of other job records from new model</param>
        public void EditOtherJobInfo(int instructorBasicInfoId, List<InstructorOtherJobInfoDTO> newOtherJobInfoList)
        {
            var existingOtherJobInfoList = _db.InstructorOtherJobInfo
                .Where(x => x.InstructorBasicInfoId == instructorBasicInfoId).ToList();

            if (newOtherJobInfoList != null && newOtherJobInfoList.Any())
            {
                foreach (InstructorOtherJobInfoDTO otherJobInfo in newOtherJobInfoList)
                {
                    otherJobInfo.InstructorBasicInfoId = instructorBasicInfoId;

                    //If other job info exists - update, else create other job info
                    InstructorOtherJobInfo existingOtherJobInfo = _db.InstructorOtherJobInfo.FirstOrDefault(x => x.Id == otherJobInfo.Id);
                    if (existingOtherJobInfo != null)
                        Edit(otherJobInfo.Id, otherJobInfo);
                    else
                        Create(otherJobInfo);
                }
            }

            //Delete other job info if it is removed from model
            if (existingOtherJobInfoList.Any())
            {
                foreach (InstructorOtherJobInfo otherJobInfo in existingOtherJobInfoList)
                {
                    if (newOtherJobInfoList == null || !newOtherJobInfoList.Any(x => x.Id == otherJobInfo.Id))
                        Delete(otherJobInfo.Id);
                }
            }
        }

        /// <summary>
        /// Create other job info record
        /// </summary>
        /// <param name="otherJobInfoDTO">Other job info model</param>
        public void Create(InstructorOtherJobInfoDTO otherJobInfoDTO)
        {
            if (otherJobInfoDTO == null)
                throw new Exception("otherJobInfoDTO is null");

            var mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<InstructorOtherJobInfoDTO, InstructorOtherJobInfo>()).CreateMapper();

            InstructorOtherJobInfo newOtherJobInfo = mapperFromDTO.Map<InstructorOtherJobInfoDTO, InstructorOtherJobInfo>(otherJobInfoDTO);

            _db.InstructorOtherJobInfo.Add(newOtherJobInfo);
            _db.SaveChanges();

        }

        /// <summary>
        /// Edit other job info by id
        /// </summary>
        /// <param name="otherJobInfoId">Other job info id</param>
        /// <param name="otherJobInfoDTO">Other job info model</param>
        public void Edit(int otherJobInfoId, InstructorOtherJobInfoDTO otherJobInfoDTO)
        {
            if (otherJobInfoDTO == null)
                throw new Exception("otherJobInfoDTO is null");

            InstructorOtherJobInfo otherJobInfo = _db.InstructorOtherJobInfo.FirstOrDefault(x => x.Id == otherJobInfoId);
            if (otherJobInfo == null)
                throw new Exception($"OtherJobInfoDTO with id {otherJobInfoId} not found");

            otherJobInfo.PlaceNameEng = otherJobInfoDTO.PlaceNameEng;
            otherJobInfo.PlaceNameRus = otherJobInfoDTO.PlaceNameRus;
            otherJobInfo.PlaceNameKir = otherJobInfoDTO.PlaceNameKir;
            otherJobInfo.PositionEng = otherJobInfoDTO.PositionEng;
            otherJobInfo.PositionRus = otherJobInfoDTO.PositionRus;
            otherJobInfo.PositionKir = otherJobInfoDTO.PositionKir;
            otherJobInfo.Phone = otherJobInfoDTO.Phone;

            _db.InstructorOtherJobInfo.Update(otherJobInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete other job info by id
        /// </summary>
        /// <param name="id">Other job info id</param>
        public void Delete(int id)
        {
            InstructorOtherJobInfo otherJobInfo = _db.InstructorOtherJobInfo.FirstOrDefault(x => x.Id == id);
            if (otherJobInfo == null)
                throw new Exception($"OtherJobInfoDTO with id {id} not found");

            _db.InstructorOtherJobInfo.Remove(otherJobInfo);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
