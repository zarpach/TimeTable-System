using AutoMapper;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Interfaces.Users.Students;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Users.Students
{
    public class StudentParentsInfoService : IStudentParentsInfoService
    {
        private readonly IApplicationDbContext _db;
        public StudentParentsInfoService(IApplicationDbContext db) 
        {
            _db = db;
        }

        /// <summary>
        /// Edit student parents info
        /// </summary>
        /// <param name="studentBasicInfoId">Student basic info id</param>
        /// <param name="studentParentsInfoList">Student parents info list from model</param>
        public void EditStudentParentsInfo(int studentBasicInfoId, List<StudentParentsInfoDTO> studentParentsInfoList) 
        {
            var dbPatentsInfoList = _db.StudentParentsInfo.Where(x => x.StudentBasicInfoId == studentBasicInfoId).ToList();

            foreach (var parentInfo in studentParentsInfoList)
            {
                var dbParentsInfo = dbPatentsInfoList.FirstOrDefault(x => x.Id == parentInfo.Id);
                if (dbParentsInfo == null)
                {
                    Create(parentInfo);
                }
                else 
                {
                    Edit(studentBasicInfoId, parentInfo);
                    dbPatentsInfoList.Remove(dbParentsInfo);
                }
            }

            foreach (var restParentInfo in dbPatentsInfoList)
                Delete(restParentInfo.Id);

            _db.SaveChanges();
        }


        /// <summary>
        /// Create student parents info record
        /// </summary>
        /// <param name="studentParentsInfoDTO">Student parents info model</param>
        private void Create(StudentParentsInfoDTO studentParentsInfoDTO) 
        {
            if (studentParentsInfoDTO == null)
                throw new Exception("studentParentsInfoDTO is null");

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<StudentParentsInfoDTO, StudentParentsInfo>();
            }).CreateMapper();

            StudentParentsInfo newStudentParentsInfo = mapperFromDTO.Map<StudentParentsInfoDTO, StudentParentsInfo>(studentParentsInfoDTO);

            _db.StudentParentsInfo.Add(newStudentParentsInfo);
        }

        /// <summary>
        /// Edit student parents info by id
        /// </summary>
        /// <param name="studentParentsInfoId">Student parents info id</param>
        /// <param name="studentParentsInfoDTO">Student parents info model</param>
        private void Edit(int studentParentsInfoId, StudentParentsInfoDTO studentParentsInfoDTO)
        {
            if (studentParentsInfoDTO == null)
                throw new Exception("studentParentsInfoDTO is null");

            StudentParentsInfo studentParentsInfo = _db.StudentParentsInfo.FirstOrDefault(x => x.Id == studentParentsInfoId);
            if (studentParentsInfo == null)
                throw new Exception($"StudentParentsInfoDTO with id {studentParentsInfoId} not found");

            studentParentsInfo.LastName = studentParentsInfoDTO.LastName;
            studentParentsInfo.FirstName = studentParentsInfoDTO.FirstName;
            studentParentsInfo.MiddleName = studentParentsInfoDTO.MiddleName;
            studentParentsInfo.Phone = studentParentsInfoDTO.Phone;
            studentParentsInfo.WorkPlace = studentParentsInfoDTO.WorkPlace;
            studentParentsInfo.Relation = studentParentsInfoDTO.Relation;
            studentParentsInfo.DeadYear = studentParentsInfoDTO.DeadYear;

            _db.StudentParentsInfo.Update(studentParentsInfo);
        }

        /// <summary>
        /// Delete student parents info by id
        /// </summary>
        /// <param name="id">Student parents info id</param>
        private void Delete(int id)
        {
            StudentParentsInfo studentParentsInfo = _db.StudentParentsInfo.FirstOrDefault(x => x.Id == id);
            if (studentParentsInfo == null)
                throw new Exception($"StudentParentsInfo with id {id} not found");

            _db.StudentParentsInfo.Remove(studentParentsInfo);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
