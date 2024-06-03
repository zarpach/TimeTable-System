using iuca.Application.DTO.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface IGradeService
    {
        /// <summary>
        /// Get grade list
        /// </summary>
        /// <returns>Grade list</returns>
        IEnumerable<GradeDTO> GetGrades();

        /// <summary>
        /// Get grade by id
        /// </summary>
        /// <param name="id">Id of grade</param>
        /// <returns>Grade model</returns>
        GradeDTO GetGrade(int id);

        /// <summary>
        /// Create grade
        /// </summary>
        /// <param name="gradeDTO">Grade model</param>
        void Create(GradeDTO gradeDTO);

        /// <summary>
        /// Edit grade
        /// </summary>
        /// <param name="id">Id of grade</param>
        /// <param name="gradeDTO">Grade model</param>
        void Edit(int id, GradeDTO gradeDTO);

        /// <summary>
        /// Delete grade by id
        /// </summary>
        /// <param name="id">Id of grade</param>
        void Delete(int id);

        /// <summary>
        /// Get grade SelectList
        /// </summary>
        /// <param name="gradeId">Selected grade id</param>
        /// <returns>SelectList of grade</returns>
        List<SelectListItem> GetGradeSelectList(int? gradeId);

        /// <summary>
        /// Get grade SelectList for transfer courses
        /// </summary>
        /// <param name="gradeId">Selected grade id</param>
        /// <returns>SelectList of grade for transfer courses</returns>
        List<SelectListItem> GetGradeSelectListForTransferCourses(int? gradeId);

        void Dispose();
    }
}
