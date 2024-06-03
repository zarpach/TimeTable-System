using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Interfaces.Users.Students;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Users.Students
{
    public class StudentContactInfoService : IStudentContactInfoService
    {
        private readonly IApplicationDbContext _db;

        public StudentContactInfoService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Create student contact info record
        /// </summary>
        /// <param name="studentContactInfoDTO">Student contact info model</param>
        public void Create(StudentContactInfoDTO studentContactInfoDTO)
        {
            if (studentContactInfoDTO == null)
                throw new Exception("studentContactInfoDTO is null");

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<StudentContactInfoDTO, StudentContactInfo>();
            }).CreateMapper();

            StudentContactInfo newStudentContactInfo = mapperFromDTO.Map<StudentContactInfoDTO, StudentContactInfo>(studentContactInfoDTO);

            _db.StudentContactInfo.Add(newStudentContactInfo);
            _db.SaveChanges();

        }

        /// <summary>
        /// Edit student contact info by id
        /// </summary>
        /// <param name="studentContactInfoId">Student contact info id</param>
        /// <param name="studentContactInfoDTO">Student contact info model</param>
        public void Edit(int studentContactInfoId, StudentContactInfoDTO studentContactInfoDTO)
        {
            if (studentContactInfoDTO == null)
                throw new Exception("studentContactInfoDTO is null");

            StudentContactInfo studentContactInfo = _db.StudentContactInfo.FirstOrDefault(x => x.Id == studentContactInfoId);
            if (studentContactInfo == null)
                throw new Exception($"StudentContactInfoDTO with id {studentContactInfoId} not found");

            studentContactInfo.StreetEng = studentContactInfoDTO.StreetEng;
            studentContactInfo.CityEng = studentContactInfoDTO.CityEng;
            studentContactInfo.StreetRus = studentContactInfoDTO.StreetRus;
            studentContactInfo.CityRus = studentContactInfoDTO.CityRus;
            studentContactInfo.CountryId = studentContactInfoDTO.CountryId;
            studentContactInfo.Zip = studentContactInfoDTO.Zip;
            studentContactInfo.Phone = studentContactInfoDTO.Phone;
            studentContactInfo.CitizenshipStreetEng = studentContactInfoDTO.CitizenshipStreetEng;
            studentContactInfo.CitizenshipCityEng = studentContactInfoDTO.CitizenshipCityEng;
            studentContactInfo.CitizenshipStreetRus = studentContactInfoDTO.CitizenshipStreetRus;
            studentContactInfo.CitizenshipCityRus = studentContactInfoDTO.CitizenshipCityRus;
            studentContactInfo.CitizenshipCountryId = studentContactInfoDTO.CitizenshipCountryId;
            studentContactInfo.CitizenshipZip = studentContactInfoDTO.CitizenshipZip;
            studentContactInfo.CitizenshipPhone = studentContactInfoDTO.CitizenshipPhone;
            studentContactInfo.ContactNameEng = studentContactInfoDTO.ContactNameEng;
            studentContactInfo.ContactNameRus = studentContactInfoDTO.ContactNameRus;
            studentContactInfo.ContactPhone = studentContactInfoDTO.ContactPhone;
            studentContactInfo.RelationEng = studentContactInfoDTO.RelationEng;
            studentContactInfo.RelationRus = studentContactInfoDTO.RelationRus;
            studentContactInfo.RelationKir = studentContactInfoDTO.RelationKir;

            _db.StudentContactInfo.Update(studentContactInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete student contact info by id
        /// </summary>
        /// <param name="id">Student contact info id</param>
        public void Delete(int id)
        {
            StudentContactInfo studentContactInfo = _db.StudentContactInfo.FirstOrDefault(x => x.Id == id);
            if (studentContactInfo == null)
                throw new Exception($"StudentContactInfo with id {id} not found");

            _db.StudentContactInfo.Remove(studentContactInfo);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
