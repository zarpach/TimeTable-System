using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.ViewModels.Courses;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Courses
{
    public interface IStudentMidtermService
    {
        /// <summary>
        /// Get student midterms
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="registrationCourseId">Registration course Id</param>
        /// <returns>Student midterm view model list</returns>
        List<StudentMidtermViewModel> GetStudentMidterms(int organizationId, int registrationCourseId);

        /// <summary>
        /// Create student midterm
        /// </summary>
        /// <param name="studentMidtermDTO">Student midterm model</param>
        int Create(StudentMidtermDTO studentMidtermDTO);

        /// <summary>
        /// Edit student midterm
        /// </summary>
        /// <param name="id">Id of studentMidterm</param>
        /// <param name="studentMidtermDTO">StudentMidterm model</param>
        void Edit(int id, StudentMidtermDTO studentMidtermDTO);

        /// <summary>
        /// Set student midterm adviser comment
        /// </summary>
        /// <param name="id">Student midterm id</param>
        /// <param name="comment">Adviser comment</param>
        void SetStudentMidtermAdviserComment(int id, string comment);

        /// <summary>
        /// Get student midterm statistics report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Report model</returns>
        List<MidtermStatisticsReportViewModel> MidtermStatisticsReport(int organizationId, int semesterId);

        /// <summary>
        /// Get student midterm statistics detailed report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <param name="courseId">Course id</param>
        /// <param name="state">Row state</param>
        /// <returns>Report model</returns>
        List<MidtermStatisticsDetailedReportViewModel> MidtermStatisticsDetailedReport(int organizationId, int semesterId,
            int? departmentId, string instructorUserId, int? courseId, enu_MidtermReportState state);

        /// <summary>
        /// Get midterm adviser report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="onlyActiveStudents">Only active students</param>
        /// <returns>Midterm report model</returns>
        MidtermAdviserReportViewModel MidtermAdviserReport(int organizationId, int semesterId,
            string adviserUserId, int? departmentGroupId, bool onlyActiveStudents = true);

        /// <summary>
        /// Get student midterm report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="courseId">Course id</param>
        /// <returns></returns>
        List<StudentCourseTempDTO> MidtermStudentReport(int organizationId, int semesterId,
            string studentUserId, int? courseId);
    }
}
