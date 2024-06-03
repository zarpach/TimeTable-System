using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Common
{
    public class NationalityService : INationalityService
    {
        private readonly IApplicationDbContext _db;

        public NationalityService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get nationality list
        /// </summary>
        /// <returns>Nationality list</returns>
        public IEnumerable<NationalityDTO> GetNationalities()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Nationality, NationalityDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Nationality>, IEnumerable<NationalityDTO>>(_db.Nationalities).OrderBy(x => x.NameEng);
        }

        /// <summary>
        /// Get nationality by id
        /// </summary>
        /// <param name="id">Id of nationality</param>
        /// <returns>Nationality model</returns>
        public NationalityDTO GetNationality(int id)
        {
            Nationality nationality = _db.Nationalities.FirstOrDefault(x => x.Id == id);
            if (nationality == null)
                throw new Exception($"Nationality with id {id} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Nationality, NationalityDTO>()).CreateMapper();
            return mapper.Map<Nationality, NationalityDTO>(nationality);
        }

        /// <summary>
        /// Create nationality
        /// </summary>
        /// <param name="nationalityDTO">Nationality model</param>
        public void Create(NationalityDTO nationalityDTO)
        {
            if (nationalityDTO == null)
                throw new Exception($"nationalityDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<Nationality, NationalityDTO>()).CreateMapper();
            var mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<NationalityDTO, Nationality>()).CreateMapper();

            Nationality newNationality = mapperFromDTO.Map<NationalityDTO, Nationality>(nationalityDTO);

            _db.Nationalities.Add(newNationality);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit nationality
        /// </summary>
        /// <param name="id">Id of nationality</param>
        /// <param name="nationalityDTO">Nationality model</param>
        public void Edit(int id, NationalityDTO nationalityDTO)
        {
            if (nationalityDTO == null)
                throw new Exception($"nationalityDTO is null");

            Nationality nationality = _db.Nationalities.FirstOrDefault(x => x.Id == id);
            if (nationality == null)
                throw new Exception($"Nationality with id {id} not found");

            nationality.NameEng = nationalityDTO.NameEng;
            nationality.NameRus = nationalityDTO.NameRus;
            nationality.NameKir = nationalityDTO.NameKir;
            nationality.SortNum = nationalityDTO.SortNum;

            _db.Nationalities.Update(nationality);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete nationality by id
        /// </summary>
        /// <param name="id">Id of nationality</param>
        public void Delete(int id)
        {
            Nationality nationality = _db.Nationalities.FirstOrDefault(x => x.Id == id);
            if (nationality == null)
                throw new Exception($"Nationality with id {id} not found");

            _db.Nationalities.Remove(nationality);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get default nationality
        /// </summary>
        /// <returns>Nationality DTO</returns>
        public NationalityDTO GetDefaultNationality() 
        {
            Nationality nationality = _db.Nationalities.FirstOrDefault(x => x.NameEng == "Not assigned");
            if (nationality == null)
                throw new Exception($"Default nationality not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Nationality, NationalityDTO>()).CreateMapper();
            return mapper.Map<Nationality, NationalityDTO>(nationality);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
