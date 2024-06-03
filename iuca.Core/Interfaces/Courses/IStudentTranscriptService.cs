using iuca.Application.DTO.Common;
using iuca.Application.ViewModels.Reports;
using iuca.Application.ViewModels.Users.Students;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Courses
{
    public interface IStudentTranscriptService
    {
        /// <summary>
        /// Get student transcript
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Transcript model</returns>
        public TranscriptViewModel GetTranscript(int organizationId, string studentUserId);

        /// <summary>
        /// Recalc student GPAs
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        void RecalcStudentsGPA(int organizationId);

        /// <summary>
        /// Get student total GPA
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <returns></returns>
        float GetStudentTotalGPA(string studentUserId);

        /// <summary>
        /// Get student semester GPA
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="semesterId">Semster id</param>
        /// <returns>Semester GPA</returns>
        float GetStudentSemesterGPA(string studentUserId, int semesterId);

        /// <summary>
        /// Get student semester GPA
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="season">Season</param>
        /// <param name="year">Year</param>
        /// <returns>Semester GPA</returns>
        float GetStudentSemesterGPA(string studentUserId, int season, int year);

        /// <summary>
        /// Get GPA student report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="minGPA">Minimum GPA</param>
        /// <param name="maxGPA">Maximum GPA</param>
        /// <param name="onlyActiveStudents">Display only active students</param>
        /// <returns>GPA report</returns>
        List<StudentGPAViewModel> GPAReport(int organizationId, int semesterId, int? departmentId, int? departmentGroupId,
            float minGPA, float maxGPA, bool onlyActiveStudents);
    }
}
