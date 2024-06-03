using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface ISemesterPeriodService
    {
        /// <summary>
        /// Get semester period list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Semester period list</returns>
        IEnumerable<SemesterPeriodDTO> GetSemesterPeriods(int selectedOrganizationId);

        /// <summary>
        /// Get semester period by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester period/param>
        /// <returns>Semester period model</returns>
        SemesterPeriodDTO GetSemesterPeriod(int selectedOrganizationId, int id);

        /// <summary>
        /// Get semester period
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="period">Semester period value</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Semester period model</returns>
        SemesterPeriodDTO GetSemesterPeriod(int selectedOrganizationId, int period, int semesterId);

        /// <summary>
        /// Get semester period
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="period">Semester period value</param>
        /// <param name="date">Date included in the period</param>
        /// <returns>Semester period model</returns>
        SemesterPeriodDTO GetSemesterPeriod(int selectedOrganizationId, int period, DateTime date);

        /// <summary>
        /// Create semester period
        /// </summary>
        /// <param name="semesterPeriodDTO">Semester period model</param>
        void Create(SemesterPeriodDTO semesterPeriodDTO);

        /// <summary>
        /// Edit semester period
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester period</param>
        /// <param name="semesterPeriodDTO">Semester period model</param>
        void Edit(int selectedOrganizationId, int id, SemesterPeriodDTO semesterPeriodDTO);

        /// <summary>
        /// Delete semester period by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester period</param>
        void Delete(int selectedOrganizationId, int id);

        void Dispose();
    }
}
