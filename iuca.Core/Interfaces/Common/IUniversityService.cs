using iuca.Application.DTO.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface IUniversityService
    {
        /// <summary>
        /// Get university list
        /// </summary>
        /// <returns>University list</returns>
        IEnumerable<UniversityDTO> GetUniversities();

        /// <summary>
        /// Get university by id
        /// </summary>
        /// <param name="id">Id of University</param>
        /// <returns>University model</returns>
        UniversityDTO GetUniversity(int id);

        /// <summary>
        /// Create university
        /// </summary>
        /// <param name="universityDTO">University model</param>
        void Create(UniversityDTO universityDTO);

        /// <summary>
        /// Edit university
        /// </summary>
        /// <param name="id">Id of university</param>
        /// <param name="universityDTO">University model</param>
        void Edit(int id, UniversityDTO universityDTO);

        /// <summary>
        /// Delete university by id
        /// </summary>
        /// <param name="id">Id of university</param>
        void Delete(int id);

        /// <summary>
        /// Get university SelectList
        /// </summary>
        /// <param name="universityId">Selected university id</param>
        /// <returns>SelectList of universities</returns>
        List<SelectListItem> GetUniversitySelectList(int? universityId);

        void Dispose();
    }
}
