using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface INationalityService
    {
        /// <summary>
        /// Get nationality list
        /// </summary>
        /// <returns>Nationality list</returns>
        IEnumerable<NationalityDTO> GetNationalities();

        /// <summary>
        /// Get nationality by id
        /// </summary>
        /// <param name="id">Id of nationality</param>
        /// <returns>Nationality model</returns>
        NationalityDTO GetNationality(int id);

        /// <summary>
        /// Create nationality
        /// </summary>
        /// <param name="nationalityDTO">Nationality model</param>
        void Create(NationalityDTO nationalityDTO);

        /// <summary>
        /// Edit nationality
        /// </summary>
        /// <param name="id">Id of nationality</param>
        /// <param name="nationalityDTO">Nationality model</param>
        void Edit(int id, NationalityDTO nationalityDTO);

        /// <summary>
        /// Delete nationality by id
        /// </summary>
        /// <param name="id">Id of nationality</param>
        void Delete(int id);

        /// <summary>
        /// Get default nationality
        /// </summary>
        /// <returns>Nationality DTO</returns>
        NationalityDTO GetDefaultNationality();

        void Dispose();
    }
}
