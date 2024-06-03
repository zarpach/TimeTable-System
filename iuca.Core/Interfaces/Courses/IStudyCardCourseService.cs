using iuca.Application.DTO.Courses;

namespace iuca.Application.Interfaces.Courses
{
    public interface IStudyCardCourseService
    {
        /// <summary>
        /// Create study card course
        /// </summary>
        /// <param name="studyCardCourseDTO">Study card course</param>
        void CreateStudyCardCourse(StudyCardCourseDTO studyCardCourseDTO);

        /// <summary>
        /// Edit study card course by id
        /// </summary>
        /// <param name="studyCardCourseId">Study card course id</param>
        /// <param name="studyCardCourseDTO">Study card course</param>
        void EditStudyCardCourse(int studyCardCourseId, StudyCardCourseDTO studyCardCourseDTO);

        /// <summary>
        /// Delete study card course by id
        /// </summary>
        /// <param name="studyCardCourseId">Study card course id</param>
        void DeleteStudyCardCourse(int studyCardCourseId);
    }
}
