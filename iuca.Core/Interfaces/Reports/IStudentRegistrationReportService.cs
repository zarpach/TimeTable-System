using iuca.Application.Enums;
using iuca.Application.ViewModels.Reports;
using iuca.Application.ViewModels.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Reports
{
    public interface IStudentRegistrationReportService
    {
        /// <summary>
        /// Get student registrations report by departments
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns></returns>
        StudentRegistrationReportViewModel GetStudentRegistrationsReport(int organizationId, int semesterId, string deanUserId);

        /// <summary>
        /// Get student registrations detailed report by departments
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="registrationState">Registration state</param>
        /// <returns>Student registration detailed report model</returns>
        StudentRegistrationDetailedReportViewModel GetStudentRegistrationsDetailedReport(int organizationId,
            int semesterId, string deanUserId, int? departmentId, enu_RegistrationState registrationState);

        /// <summary>
        /// Get advisers and their students 
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <returns>Model with dean, advisers and their students</returns>
        DeanAdviserStudentReportViewModel DeanAdviserStudentsReport(int organizationId, string deanUserId, string adviserUserId);

        /// <summary>
        /// Get students without adviser by dean user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns>List of students without adviser</returns>
        List<StudentInfoBriefViewModel> StudentsWithoutAdviser(int organizationId, string deanUserId);

        /// <summary>
        /// Get course registration adviser report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="onlyActiveStudents">Only active students</param>
        /// <returns>Course registration report model</returns>
        CourseRegistrationAdviserReportViewModel CourseRegistrationAdviserReport(int organizationId, int semesterId,
            string adviserUserId, int departmentGroupId, bool onlyActiveStudents = true);

        /// <summary>
        /// Get registration course report
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="courseTypes">Course type list</param>
        /// <param name="maxQty">Max quantity</param>
        /// <returns>Report model</returns>
        List<RegistrationCourseReportViewModel> GetRegistrationCoursesReport(int semesterId, int? departmentId,
            List<enu_CourseType> courseTypes, int maxQty = -1);
    }
}
