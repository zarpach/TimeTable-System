using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.ViewModels.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Courses
{
    public interface IStudentCourseTempService
    {
        /// <summary>
        /// Get student courses by registration id
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        /// <returns>Student registration courses</returns>
        StudentCourseTempDTO GetStudentCourse(int id);

        /// <summary>
        /// Get student courses by registration id
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        /// <returns>Student registration courses</returns>
        IEnumerable<StudentCourseTempDTO> GetStudentCoursesByRegistrationId(int id);

        /// <summary>
        /// Get sudent registration courses by student user id
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student registration courses</returns>
        public List<StudentCourseTempDTO> GetStudentCoursesByStudentUserIdTemp(int semesterId, string studentUserId);

        /// <summary>
        /// Add course to student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Registration course result</returns>
        RegistrationCourseResultViewModel AddCourseToRegistration(int studentCourseRegistrationId, int registrationCourseId);

        /// <summary>
        /// Add courses from study card to student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardId">Study card id</param>
        void AddAllCoursesFromStudyCardToRegistration(int studentCourseRegistrationId, int studyCardId);

        /// <summary>
        /// Add course to student registration by admin
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <param name="state">Course state</param>
        /// <returns>
        /// Id of new created row
        /// </returns>
        int AddCourseToRegistrationByAdmin(int studentCourseRegistrationId, int registrationCourseId,
            enu_CourseState state);

        /// <summary>
        /// Remove course from student registration
        /// </summary>
        /// <param name="studentCourseId">Student course  id</param>
        void RemoveCourseFromRegistrationByAdmin(int studentCourseId);

        /// <summary>
        /// Remove course from student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardCourseId">Study card course id</param>
        RegistrationCourseResultViewModel RemoveCourseFromRegistration(int studentCourseRegistrationId, int registrationCourseId);

        /// <summary>
        /// Drop course from student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        void DropCourseFromRegistration(int studentCourseRegistrationId, int registrationCourseId);

        /// <summary>
        /// Return dropped course to student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        void ReturnDroppedCourse(int studentCourseRegistrationId, int registrationCourseId);

        /// <summary>
        /// Add new course to student registration while add/drop period
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Registration course result</returns>
        void AddNewCourseToRegistration(int studentCourseRegistrationId, int registrationCourseId);

        /// <summary>
        /// Remove added course from student registration for add/drop perdiod
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        void RemoveAddedCourseFromRegistration(int studentCourseRegistrationId, int registrationCourseId);

        /// <summary>
        /// Make course aprroved or disapproved according to flag
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <param name="approve">True - approve, False - disapprove</param>
        void ApproveCourse(int studentCourseRegistrationId, int registrationCourseId, bool approve);

        /// <summary>
        /// Comment course
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <param name="comment">Comment</param>
        void CommentCourse(int studentCourseRegistrationId, int registrationCourseId, string comment);

        /// <summary>
        /// Mark student course deleted flag
        /// </summary>
        /// <param name="studentCourseId">Student course id</param>
        /// <param name="isDeleted">Is deleted</param>
        void MarkDeleted(int studentCourseId, bool isDeleted);

        /// <summary>
        /// Mark student course audit flag
        /// </summary>
        /// <param name="studentCourseId">Student course id</param>
        /// <param name="isAudit">Is audit</param>
        void MarkAudit(int studentCourseId, bool isAudit);

        void Dispose();
    }
}
