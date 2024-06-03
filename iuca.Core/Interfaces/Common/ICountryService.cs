using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface ICountryService
    {
        /// <summary>
        /// Get country list
        /// </summary>
        /// <returns>Country list</returns>
        IEnumerable<CountryDTO> GetCountries();

        /// <summary>
        /// Get country by id
        /// </summary>
        /// <param name="id">Id of country</param>
        /// <returns>Country model</returns>
        CountryDTO GetCountry(int id);

        /// <summary>
        /// Create country
        /// </summary>
        /// <param name="countryDTO">Country model</param>
        void Create(CountryDTO countryDTO);

        /// <summary>
        /// Edit country
        /// </summary>
        /// <param name="id">Id of country</param>
        /// <param name="countryDTO">Country model</param>
        void Edit(int id, CountryDTO countryDTO);

        /// <summary>
        /// Delete country by id
        /// </summary>
        /// <param name="id">Id of country</param>
        void Delete(int id);

        /// <summary>
        /// Get default country
        /// </summary>
        /// <returns>Country DTO</returns>
        CountryDTO GetDefaultCountry();

        void Dispose();
    }
}
