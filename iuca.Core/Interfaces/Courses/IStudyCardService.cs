using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Courses
{
    public interface IStudyCardService
    {
        /// <summary>
        /// Get study card list
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="deanDepartments">Dean departments</param>
        /// <returns>Study card list</returns>
        IEnumerable<StudyCardDTO> GetStudyCards(int semesterId, int departmentId, IEnumerable<DepartmentDTO> deanDepartments);

        /// <summary>
        /// Get study card by id
        /// </summary>
        /// <param name="studyCardId">Study card id</param>
        /// <returns>Study card</returns>
        StudyCardDTO GetStudyCard(int studyCardId);

        /// <summary>
        /// Get study card by semester id and department group id
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <returns>Study card</returns>
        StudyCardDTO GetStudyCard(int semesterId, int departmentGroupId);

        /// <summary>
        /// Create study card
        /// </summary>
        /// <param name="studyCardDTO">Study card</param>
        void CreateStudyCard(StudyCardDTO studyCardDTO);

        /// <summary>
        /// Edit study card by id
        /// </summary>
        /// <param name="studyCardId">Study card id</param>
        /// <param name="studyCardDTO">Study card</param>
        void EditStudyCard(int studyCardId, StudyCardDTO studyCardDTO);

        /// <summary>
        /// Edit study card courses by id
        /// </summary>
        /// <param name="studyCardId">Study card id</param>
        /// <param name="studyCardCourseDTOList">Study card course list</param>
        void EditStudyCardCourses(int studyCardId, IEnumerable<StudyCardCourseDTO> studyCardCourseDTOList);

        /// <summary>
        /// Delete study card by id
        /// </summary>
        /// <param name="studyCardId">Study card id</param>
        void DeleteStudyCard(int studyCardId);

        /// <summary>
        /// Get courses for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="season">Season</param>
        /// <param name="year">Year</param>
        /// <param name="excludedAnnouncementSectionIds">Announcement section ids to exclude</param>
        /// <returns>Announcement section list without excluded announcement section ids</returns>
        IEnumerable<AnnouncementSectionDTO> GetCoursesForSelection(int organizationId, int year, int season, int[] excludedAnnouncementSectionIds);

        /// <summary>
        /// Get course from selection
        /// </summary>
        /// <param name="selectedRegistrationCourseId">Selected registration course id</param>
        /// <returns>Registartion course</returns>
        AnnouncementSectionDTO GetCourseFromSelection(int selectedRegistrationCourseId);

        /// <summary>
        /// Get announcement section with ForAll flag
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Announcement section collection</returns>
        List<AnnouncementSectionDTO> GetForAllAnnouncementSections(int semesterId);
    }
}
