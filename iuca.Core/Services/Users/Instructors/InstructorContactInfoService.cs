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
    public class InstructorContactInfoService : IInstructorContactInfoService
    {
        private readonly IApplicationDbContext _db;

        public InstructorContactInfoService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Create instructor contact info record
        /// </summary>
        /// <param name="instructorContactInfoDTO">Instructor contact info model</param>
        public InstructorContactInfoDTO Create(InstructorContactInfoDTO instructorContactInfoDTO)
        {
            if (instructorContactInfoDTO == null)
                throw new Exception("instructorContactInfoDTO is null");

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<CountryDTO, Country>();
                cfg.CreateMap<UniversityDTO, University>();
                cfg.CreateMap<InstructorContactInfoDTO, InstructorContactInfo>();
            }).CreateMapper();

            var mapperToDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<University, UniversityDTO>();
                cfg.CreateMap<InstructorContactInfo, InstructorContactInfoDTO>();
            }).CreateMapper();

            InstructorContactInfo newInstructorContactInfo = mapperFromDTO.Map<InstructorContactInfoDTO, InstructorContactInfo>(instructorContactInfoDTO);

            _db.InstructorContactInfo.Add(newInstructorContactInfo);
            _db.SaveChanges();

            return mapperToDTO.Map<InstructorContactInfo, InstructorContactInfoDTO>(newInstructorContactInfo);

        }

        /// <summary>
        /// Edit instructor contact info by id
        /// </summary>
        /// <param name="instructorContactInfoDTO">Instructor contact info model</param>
        public void Edit(InstructorContactInfoDTO instructorContactInfoDTO)
        {
            if (instructorContactInfoDTO == null)
                throw new Exception("instructorContactInfoDTO is null");

            InstructorContactInfo instructorContactInfo = _db.InstructorContactInfo.FirstOrDefault(x => x.Id == instructorContactInfoDTO.Id);
            if (instructorContactInfo == null)
                throw new Exception($"InstructorContactInfoDTO with id {instructorContactInfoDTO.Id} not found");

            instructorContactInfo.CountryId = instructorContactInfoDTO.CountryId;
            instructorContactInfo.CityEng = instructorContactInfoDTO.CityEng;
            instructorContactInfo.StreetEng = instructorContactInfoDTO.StreetEng;
            instructorContactInfo.AddressEng = instructorContactInfoDTO.AddressEng;
            instructorContactInfo.CityRus = instructorContactInfoDTO.CityRus;
            instructorContactInfo.StreetRus = instructorContactInfoDTO.StreetRus;
            instructorContactInfo.AddressRus = instructorContactInfoDTO.AddressRus;
            instructorContactInfo.ZipCode = instructorContactInfoDTO.ZipCode;
            instructorContactInfo.Phone = instructorContactInfoDTO.Phone;
            instructorContactInfo.CitizenshipCountryId = instructorContactInfoDTO.CitizenshipCountryId;
            instructorContactInfo.CitizenshipCityEng = instructorContactInfoDTO.CitizenshipCityEng;
            instructorContactInfo.CitizenshipStreetEng = instructorContactInfoDTO.CitizenshipStreetEng;
            instructorContactInfo.CitizenshipAddressEng = instructorContactInfoDTO.CitizenshipAddressEng;
            instructorContactInfo.CitizenshipCityRus = instructorContactInfoDTO.CitizenshipCityRus;
            instructorContactInfo.CitizenshipStreetRus = instructorContactInfoDTO.CitizenshipStreetRus;
            instructorContactInfo.CitizenshipAddressRus = instructorContactInfoDTO.CitizenshipAddressRus;
            instructorContactInfo.CitizenshipZipCode = instructorContactInfoDTO.CitizenshipZipCode;
            instructorContactInfo.CitizenshipPhone = instructorContactInfoDTO.CitizenshipPhone;
            instructorContactInfo.ContactNameEng = instructorContactInfoDTO.ContactNameEng;
            instructorContactInfo.ContactNameRus = instructorContactInfoDTO.ContactNameRus;
            instructorContactInfo.ContactPhone = instructorContactInfoDTO.ContactPhone;
            instructorContactInfo.RelationEng = instructorContactInfoDTO.RelationEng;
            instructorContactInfo.RelationRus = instructorContactInfoDTO.RelationRus;
            instructorContactInfo.RelationKir = instructorContactInfoDTO.RelationKir;

            _db.InstructorContactInfo.Update(instructorContactInfo);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete instructor contact info by id
        /// </summary>
        /// <param name="id">Instructor contact info id</param>
        public void Delete(int id)
        {
            InstructorContactInfo instructorContactInfo = _db.InstructorContactInfo.FirstOrDefault(x => x.Id == id);
            if (instructorContactInfo == null)
                throw new Exception($"InstructorContactInfo with id {id} not found");

            _db.InstructorContactInfo.Remove(instructorContactInfo);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
