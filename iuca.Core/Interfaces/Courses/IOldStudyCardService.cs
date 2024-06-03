using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Students;
using iuca.Application.ViewModels.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Courses
{
    public interface IOldStudyCardService
    {
        /// <summary>
        /// Get study card list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Study card list</returns>
        IEnumerable<OldStudyCardDTO> GetStudyCards(int selectedOrganizationId);

        /// <summary>
        /// Get study card list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <returns>Study card list</returns>
        IEnumerable<OldStudyCardDTO> GetStudyCards(int selectedOrganizationId, int departmentGroupId);

        /// <summary>
        /// Get study card by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of study card</param>
        /// <returns>Study card model</returns>
        OldStudyCardDTO GetStudyCard(int selectedOrganizationId, int id);

        /// <summary>
        /// Get study card courses for selection window
        /// </summary>
        /// <param name="semester">Semester to select courses</param>
        /// <param name="group">Group for courses selection</param>
        /// <param name="academicPlan">Academic plan with courses for group</param>
        /// <returns>List of courses with recommendations</returns>
        List<StudyCardSelectionCourseViewModel> GetCoursesForSelection(SemesterDTO semester, DepartmentGroupDTO group,
            AcademicPlanDTO academicPlan, int[] excludedIds);

        /// <summary>
        /// Get courses from selection window
        /// </summary>
        /// <param name="cyclePartCourseIds">Array of cycle part courses ids</param>
        /// <returns>Listcourses from selection window</returns>
        List<OldStudyCardCourseDTO> GetCoursesFromSelection(int studyCardId, int[] cyclePartCourseIds);

        /// <summary>
        /// Create study card
        /// </summary>
        /// <param name="studyCardDTO">Study card model</param>
        void Create(OldStudyCardDTO studyCardDTO);

        /// <summary>
        /// Edit study card
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of study card</param>
        /// <param name="studyCardDTO">Study card model</param>
        void Edit(int selectedOrganizationId, int id, OldStudyCardDTO studyCardDTO);

        /// <summary>
        /// Delete study card by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of study card</param>
        void Delete(int selectedOrganizationId, int id);

        /// <summary>
        /// Edit study card courses
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studyCardId">Id of study card</param>
        /// <param name="modelCourses">List of study card courses</param>
        void EditStudyCardCourses(int selectedOrganizationId, int studyCardId, List<OldStudyCardCourseDTO> modelCourses);

        /// <summary>
        /// Get study card places list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Study card places list</returns>
        IEnumerable<StudyCardPlacesViewModel> GetStudyCardPlaces(int selectedOrganizationId, int semesterId);

        /// <summary>
        /// Set study card places for given list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="courses">List of grouped courses</param>
        void SetStudyCardPlaces(int selectedOrganizationId, int semesterId, List<StudyCardPlacesViewModel> courses);

        /// <summary>
        /// Get department groups of existing study cards on semester
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>List of department groups</returns>
        List<DepartmentGroupDTO> GetDepartmentGroupsForSemester(int organizationId, int semesterId);

        void Dispose();
    }
}
