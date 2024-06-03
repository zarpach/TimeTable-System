using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Common
{
    public class UniversityService : IUniversityService
    {
        private readonly IApplicationDbContext _db;

        public UniversityService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get university list
        /// </summary>
        /// <returns>University list</returns>
        public IEnumerable<UniversityDTO> GetUniversities()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<University, UniversityDTO>();
            }).CreateMapper();
            return mapper.Map<IEnumerable<University>, IEnumerable<UniversityDTO>>(_db.Universities.Include(x => x.Country))
                .OrderBy(x => x.NameEng);
        }

        /// <summary>
        /// Get university by id
        /// </summary>
        /// <param name="id">Id of University</param>
        /// <returns>University model</returns>
        public UniversityDTO GetUniversity(int id)
        {
            University university = _db.Universities.Include(x => x.Country).FirstOrDefault(x => x.Id == id);
            if (university == null)
                throw new Exception($"University with id {id} not found");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<University, UniversityDTO>();
            }).CreateMapper();

            return mapper.Map<University, UniversityDTO>(university);
        }

        /// <summary>
        /// Create university
        /// </summary>
        /// <param name="universityDTO">University model</param>
        public void Create(UniversityDTO universityDTO)
        {
            if (universityDTO == null)
                throw new Exception($"universityDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<Country, CountryDTO>();
                cfg.CreateMap<University, UniversityDTO>();
            }).CreateMapper();

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<CountryDTO, Country>();
                cfg.CreateMap<UniversityDTO, University>();
            }).CreateMapper();

            University newUniversity = mapperFromDTO.Map<UniversityDTO, University>(universityDTO);

            _db.Universities.Add(newUniversity);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit university
        /// </summary>
        /// <param name="id">Id of university</param>
        /// <param name="universityDTO">University model</param>
        public void Edit(int id, UniversityDTO universityDTO)
        {
            if (universityDTO == null)
                throw new Exception($"universityDTO is null");

            University university = _db.Universities.FirstOrDefault(x => x.Id == id);
            if (university == null)
                throw new Exception($"University with id {id} not found");

            university.NameEng = universityDTO.NameEng;
            university.NameRus = universityDTO.NameRus;
            university.NameKir = universityDTO.NameKir;
            university.Code = universityDTO.Code;
            university.CountryId = universityDTO.CountryId;

            _db.Universities.Update(university);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete university by id
        /// </summary>
        /// <param name="id">Id of university</param>
        public void Delete(int id)
        {
            University university = _db.Universities.FirstOrDefault(x => x.Id == id);
            if (university == null)
                throw new Exception($"University with id {id} not found");

            _db.Universities.Remove(university);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get university SelectList
        /// </summary>
        /// <param name="universityId">Selected university id</param>
        /// <returns>SelectList of universities</returns>
        public List<SelectListItem> GetUniversitySelectList(int? universityId)
        {
            List<SelectListItem> list = new SelectList(_db.Universities, "Id", "NameEng", universityId)
                .OrderBy(x => x.Text).ToList();

            return list;
        }

        public void Dispose() 
        {
            _db.Dispose();
        }
    }
}
