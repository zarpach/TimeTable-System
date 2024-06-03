using iuca.Application.DTO.Courses;
using iuca.Application.ViewModels.Courses;

namespace iuca.Application.Interfaces.Courses
{
    public interface ISyllabusService
    {
        /// <summary>
        /// Get syllabus by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        /// <returns>Syllabus</returns>
        SyllabusDTO GetSyllabusById(int syllabusId);

        /// <summary>
        /// Get syllabus by registration course id
        /// </summary>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Syllabus</returns>
        SyllabusDTO GetSyllabusByRegistrationCourseId(int registrationCourseId);

        /// <summary>
        /// Get syllabus details by registration course id
        /// </summary>
        /// <param name="registrationCourseId">Кegistration course id</param>
        /// <returns>Syllabus details</returns>
        SyllabusDetailsViewModel GetSyllabusDetails(int selectedOrganizationId, int registrationCourseId);

        /// <summary>
        /// Set syllabus status by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        /// <param name="status">Syllabus status</param>
        void SetSyllabusStatus(int syllabusId, int status);

        /// <summary>
        /// Set syllabus instructor comment by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        /// <param name="comment">Syllabus instructor comment</param>
        void SetSyllabusInstructorComment(int syllabusId, string comment);

        /// <summary>
        /// Set syllabus approver comment by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        /// <param name="comment">Syllabus approver comment</param>
        void SetSyllabusApproverComment(int syllabusId, string comment);

        /// <summary>
        /// Create syllabus
        /// </summary>
        /// <param name="syllabusDTO">Syllabus</param>
        void CreateSyllabus(SyllabusDTO syllabusDTO);

        /// <summary>
        /// Edit syllabus by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        /// <param name="syllabusDTO">Syllabus</param>
        void EditSyllabus(int syllabusId, SyllabusDTO syllabusDTO);

        /// <summary>
        /// Delete syllabus by id
        /// </summary>
        /// <param name="syllabusId">Syllabus id</param>
        void DeleteSyllabus(int syllabusId);
    }
}
