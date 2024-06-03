using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Courses
{
    public interface ICourseService
    {
        /// <summary>
        /// Get course list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="parameters">Paging parameters</param>
        /// <param name="departmentIds">Department ids</param>
        /// <param name="courseName">Course name</param>
        /// <param name="courseAbbr">Course abbreviation</param>
        /// <param name="courseNum">Course number</param>
        /// <param name="parameters">Course id</param>
        /// <param name="isDeleted">Return deleted courses</param>
        /// <returns>Course list</returns>
        public PagedList<CourseDTO> GetCourses(int selectedOrganizationId, QueryStringParameters parameters,
            List<int> departmentIds, string courseName, string courseAbbr, string? courseNum, int? courseId, bool isDeleted = false);

        /// <summary>
        /// Get course list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="isDeleted">Return deleted courses</param>
        /// <returns>Course list</returns>
        IEnumerable<CourseDTO> GetCourses(int selectedOrganizationId, bool isDeleted = false);

        /// <summary>
        /// Get course list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="ids">Courses ids</param>
        /// <param name="isDeleted">Return deleted courses</param>
        /// <returns>Course list</returns>
        IEnumerable<CourseDTO> GetCourses(int selectedOrganizationId, int[] ids, bool isDeleted = false);

        /// <summary>
        /// Get course list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="excludedIds">Courses ids to exclude</param>
        /// <param name="isDeleted">Return deleted courses</param>
        /// <returns>Course list</returns>
        IEnumerable<CourseDTO> GetCoursesWithExclusion(int selectedOrganizationId, int[] excludedIds, bool isDeleted = false);

        /// <summary>
        /// Get courses for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="excludedIds">Course ids to exclude</param>
        /// <param name="isDeleted">Return deleted courses</param>
        /// <returns>Course list without excluded ids</returns>
        IEnumerable<CourseDTO> GetCoursesForSelection(int organizationId, int[] excludedIds, bool isDeleted = false);

        /// <summary>
        /// Get cycle part course list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="departmentId">Department id of academic plan</param>
        /// <param name="year">Year of academic plan</param>
        /// <param name="courseId">Course import Id</param>
        /// <param name="excludedIds">Cycle part courses ids to exclude</param>
        /// <returns>Cycle part courses list</returns>
        List<CyclePartCourseDTO> GetCyclePartCoursesWithExclusion(int selectedOrganizationId, int departmentId,
            int year, int courseId, int[] excludedIds);

        /// <summary>
        /// Get course by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of course</param>
        /// <param name="isDeleted">Include deleted records</param>
        /// <returns>Course model</returns>
        CourseDTO GetCourse(int selectedOrganizationId, int id, bool isDeleted = false);

        /// <summary>
        /// Create course
        /// </summary>
        /// <param name="courseDTO">Course model</param>
        void Create(CourseDTO courseDTO);

        /// <summary>
        /// Edit course
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of course</param>
        /// <param name="courseDTO">Course model</param>
        void Edit(int selectedOrganizationId, int id, CourseDTO courseDTO);

        /// <summary>
        /// Delete course by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of course</param>
        void Delete(int selectedOrganizationId, int id);

        /// <summary>
        /// UnDelete course by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of course</param>
        void UnDelete(int selectedOrganizationId, int id);

        void Dispose();

        /// <summary>
        /// Get course SelectList
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="excludedCourseId">Course id that must be excluded</param>
        /// <param name="selectedCourseId">Selected course id</param>
        /// <returns>SelectList of courses</returns>
        List<SelectListItem> GetCourseSelectList(int selectedOrganizationId, int? excludedCourseId, int? selectedCourseId);
    }
}
