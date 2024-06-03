using iuca.Application.Enums;
using iuca.Application.ViewModels.Courses;
using iuca.Application.ViewModels.Reports;
using System.Collections.Generic;


namespace iuca.Application.Interfaces.Grades
{
    public interface IGradeManagementService
    {
        /// <summary>
        /// Get courses and their students with grades
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="semesterId">Semester Id</param>
        /// <param name="departmentId">Department Id</param>
        /// <param name="courseImportCode">Course import code Id</param>
        /// <param name="studentId">Student Id</param>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="status">Status (submitted/not submitted)</param>
        /// <returns>Courses and their students with grades for semester</returns>
        List<GradeReportViewModel> GetGradeReport(int organizationId, int semesterId, int? departmentId,
            int? courseImportCode, int? studentId, int? gradeId, enu_GradeReportStatus status);

        /// <summary>
        /// Set student grade for course
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="gradeId">Grade id</param>
        void SetStudentGrade(int organizationId, int announcementSectionId, string studentUserId, int? gradeId);

        /// <summary>
        /// Get student grades adviser report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="onlyActiveStudents">Only active students</param>
        /// <returns>Student grades report model</returns>
        GradeAdviserReportViewModel GradeAdviserReport(int organizationId, int semesterId,
            string adviserUserId, int? departmentGroupId, int? gradeId, bool onlyActiveStudents = true);

        /// <summary>
        /// Return students that have X or F grades for the same course more then one time
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="departmentIds">Department ids</param>
        /// <param name="onlyActiveStudents">Only active students</param>
        /// <returns>List of students and grades</returns>
        List<FFXXReportViewModel> FFXXReport(int organizationId, List<int> departmentIds, bool onlyActiveStudents);

        /// <summary>
        /// Get department grade report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="onlyActiveStudents">Display only active students</param>
        /// <returns>Department grade report</returns>
        List<DepartmentStudentGradeViewModel> DepartmentGradeReport(int organizationId, int semesterId, int? departmentId, int? departmentGroupId,
            int? gradeId, bool onlyActiveStudents);
    }
}
