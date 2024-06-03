using iuca.Application.DTO.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Courses
{
    public interface ICycleService
    {
        /// <summary>
        /// Get cycle list
        /// </summary>
        /// <returns>Cycle list</returns>
        IEnumerable<CycleDTO> GetCycles();

        /// <summary>
        /// Get cycle by id
        /// </summary>
        /// <param name="id">Id of cycle</param>
        /// <returns>Cycle model</returns>
        CycleDTO GetCycle(int id);

        /// <summary>
        /// Create cycle
        /// </summary>
        /// <param name="cycleDTO">Cycle model</param>
        void Create(CycleDTO cycleDTO);

        /// <summary>
        /// Edit cycle
        /// </summary>
        /// <param name="id">Id of cycle</param>
        /// <param name="cycleDTO">Cycle model</param>
        void Edit(int id, CycleDTO cycleDTO);

        /// <summary>
        /// Delete cycle by id
        /// </summary>
        /// <param name="id">Id of cycle</param>
        void Delete(int id);

        void Dispose();
    }
}
