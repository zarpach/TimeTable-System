using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface ISemesterService
    {
        /// <summary>
        /// Get semester list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Semester list</returns>
        IEnumerable<SemesterDTO> GetSemesters(int selectedOrganizationId);

        /// <summary>
        /// Get semester list starting from admission year
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="admissionYear">Year of admission</param>
        /// <returns>Semester list</returns>
        List<SemesterDTO> GetSemestersByAdmissionYear(int selectedOrganizationId, int admissionYear);

        /// <summary>
        /// Get semester by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester</param>
        /// <returns>Semester model</returns>
        SemesterDTO GetSemester(int selectedOrganizationId, int id);

        /// <summary>
        /// Get semester by id
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Semester</returns>
        SemesterDTO GetSemester(int semesterId);

        /// <summary>
        /// Get semester by year and season
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="year">Year of semester</param>
        /// <param name="season">Season of semester</param>
        /// <param name="generateException">If true and semester not found generate exception</param>
        /// <returns>Semester model</returns>
        SemesterDTO GetSemester(int selectedOrganizationId, int year, int season, bool generateException = true);

        /// <summary>
        /// Create semester
        /// </summary>
        /// <param name="semesterDTO">Semester model</param>
        void Create(SemesterDTO semesterDTO);

        /// <summary>
        /// Edit semester
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester</param>
        /// <param name="semesterDTO">Semester model</param>
        void Edit(int selectedOrganizationId, int id, SemesterDTO semesterDTO);

        /// <summary>
        /// Delete semester by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of semester</param>
        void Delete(int selectedOrganizationId, int id);

        void Dispose();

        /// <summary>
        /// Get current semester for current date
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="generateException">If true generates exception when record not found</param>
        /// <returns>Current semester</returns>
        SemesterDTO GetCurrentSemester(int selectedOrganizationId, bool generateException = true);

        /// <summary>
        /// Get current educational (fall or spring) semester for current date
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="generateException">If true generates exception when the record not found</param>
        /// <returns>Current educational semester</returns>
        SemesterDTO GetCurrentEducationalSemester(int selectedOrganizationId, bool generateException = true);

        /// <summary>
        /// Get next educational (fall or spring) semester for current date
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Next educational semester</returns>
        SemesterDTO GetNextEducationalSemester(int selectedOrganizationId);

        /// <summary>
        /// Get season select list
        /// </summary>
        /// <param name="season">Selected season</param>>
        /// <returns>Season select list</returns>
        List<SelectListItem> GetSeasonList(int season);
    }
}
