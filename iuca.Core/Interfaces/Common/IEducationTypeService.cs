using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface IEducationTypeService
    {
        /// <summary>
        /// Get education type list
        /// </summary>
        /// <returns>Education type list</returns>
        IEnumerable<EducationTypeDTO> GetEducationTypes();

        /// <summary>
        /// Get education type by id
        /// </summary>
        /// <param name="id">Id of education type</param>
        /// <returns>Education type model</returns>
        EducationTypeDTO GetEducationType(int id);

        /// <summary>
        /// Create education type
        /// </summary>
        /// <param name="educationTypeDTO">Education type model</param>
        void Create(EducationTypeDTO educationTypeDTO);

        /// <summary>
        /// Edit education type
        /// </summary>
        /// <param name="id">Id of education type</param>
        /// <param name="educationTypeDTO">Education type model</param>
        void Edit(int id, EducationTypeDTO educationTypeDTO);

        /// <summary>
        /// Delete education type by id
        /// </summary>
        /// <param name="id">Id of education type</param>
        void Delete(int id);
    }
}
