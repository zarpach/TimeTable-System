using iuca.Application.ViewModels.Users.Instructors;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Users.Instructors
{
    public interface IInstructorCourseService
    {
        /// <summary>
        /// Get instructor courses for semester
        /// </summary>
        /// <param name="organizationId">organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <returns>List of instructor courses</returns>
        List<InstructorCourseViewModel> GetInstructorCourses(int organizationId, int semesterId,
            string instructorUserId);

        /// <summary>
        /// Get instructor registration course by id
        /// </summary>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Instructor course</returns>
        InstructorCourseViewModel GetInstructorCourse(int registrationCourseId);

        /// <summary>
        /// Get students for instructor course
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="announcementSectionId">Announcement sectionId id</param>
        /// <param name="onlyActiveStudents">Only active students</param>
        /// <returns>List of students</returns>
        List<InstructorCourseStudentViewModel> GetInstructorCourseStudents(int organizationId,
            int announcementSectionId, bool onlyActiveStudents);

        /// <summary>
        /// Get student grades for instructor course
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="announcementSectionId">Announcement sectionId id</param>
        /// <returns>List of student grades</returns>
        List<InstructorCourseStudentGradeViewModel> GetInstructorCourseStudentGrades(int organizationId, int announcementSectionId);

        /// <summary>
        /// Set student grade for course
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentCourseId">Student course id</param>
        /// <param name="gradeId">Grade id</param>
        void SetStudentGrade(int organizationId, int studentCourseId, int? gradeId);

        /// <summary>
        /// Submit or unsubmit grade sheet
        /// </summary>
        /// <param name="announcementSectionId">Announcement setction id</param>
        /// <param name="submitted">Sublitted flag</param>
        void SetGradeSheetSubmitted(int announcementSectionId, bool submitted);
    }
}
