using iuca.Application.DTO.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Courses
{
    public interface IStudentCourseService
    {
        /// <summary>
        /// Get student courses by registration id
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        /// <returns>Student registration courses</returns>
        IEnumerable<StudentCourseDTO> GetStudentCoursesByRegistrationId(int id);

        /// <summary>
        /// Add course to student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardCourseId">Study card course id</param>
        void AddCourseToRegistration(int studentCourseRegistrationId, int studyCardCourseId);

        /// <summary>
        /// Remove course from student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardCourseId">Study card course id</param>
        void RemoveCourseFromRegistration(int studentCourseRegistrationId, int studyCardCourseId);

        /// <summary>
        /// Make course aprroved or disapproved according to flag
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardCourseId">Study card course id</param>
        /// <param name="approve">True - approve, False - disapprove</param>
        void ApproveCourse(int studentCourseRegistrationId, int studyCardCourseId, bool approve);

        /// <summary>
        /// Comment course
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardCourseId">Study card course id</param>
        /// <param name="approve">True - approve, False - disapprove</param>
        void CommentCourse(int studentCourseRegistrationId, int studyCardCourseId, string comment);
    }
}
