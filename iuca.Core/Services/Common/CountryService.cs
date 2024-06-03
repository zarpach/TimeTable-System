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
    public class CountryService : ICountryService
    {
        private readonly IApplicationDbContext _db;

        public CountryService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get country list
        /// </summary>
        /// <returns>Country list</returns>
        public IEnumerable<CountryDTO> GetCountries()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Country, CountryDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Country>, IEnumerable<CountryDTO>>(_db.Countries)
                .OrderByDescending(x => x.SortNum).ThenBy(x => x.NameEng);
        }

        /// <summary>
        /// Get country by id
        /// </summary>
        /// <param name="id">Id of country</param>
        /// <returns>Country model</returns>
        public CountryDTO GetCountry(int id)
        {
            Country country = _db.Countries.FirstOrDefault(x => x.Id == id);
            if (country == null)
                throw new Exception($"Country with id {id} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Country, CountryDTO>()).CreateMapper();
            return mapper.Map<Country, CountryDTO>(country);
        }

        /// <summary>
        /// Create country
        /// </summary>
        /// <param name="countryDTO">Country model</param>
        public void Create(CountryDTO countryDTO)
        {
            if (countryDTO == null)
                throw new Exception($"countryDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<Country, CountryDTO>()).CreateMapper();
            var mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<CountryDTO, Country>()).CreateMapper();

            Country newCountry = mapperFromDTO.Map<CountryDTO, Country>(countryDTO);

            _db.Countries.Add(newCountry);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit country
        /// </summary>
        /// <param name="id">Id of country</param>
        /// <param name="countryDTO">Country model</param>
        public void Edit(int id, CountryDTO countryDTO)
        {
            if (countryDTO == null)
                throw new Exception($"countryDTO is null");

            Country country = _db.Countries.FirstOrDefault(x => x.Id == id);
            if (country == null)
                throw new Exception($"Country with id {id} not found");

            country.NameEng = countryDTO.NameEng;
            country.NameRus = countryDTO.NameRus;
            country.NameKir = countryDTO.NameKir;
            country.Code = countryDTO.Code;
            country.SortNum = countryDTO.SortNum;

            _db.Countries.Update(country);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete country by id
        /// </summary>
        /// <param name="id">Id of country</param>
        public void Delete(int id)
        {
            Country country = _db.Countries.FirstOrDefault(x => x.Id == id);
            if (country == null)
                throw new Exception($"Country with id {id} not found");

            _db.Countries.Remove(country);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get default country
        /// </summary>
        /// <returns>Country DTO</returns>
        public CountryDTO GetDefaultCountry()
        {
            Country country = _db.Countries.FirstOrDefault(x => x.Code == "NA");
            if (country == null)
                throw new Exception($"Default country not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Country, CountryDTO>()).CreateMapper();
            return mapper.Map<Country, CountryDTO>(country);
        }

        public void Dispose() 
        {
            _db.Dispose();
        }
    }
}
