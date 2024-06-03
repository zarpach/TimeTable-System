using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.ViewModels.Courses;
using iuca.Application.ViewModels.Users.Students;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Courses
{
    public interface IRegistrationCourseService
    {
        /// <summary>
        /// Get registration course
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Registration course</returns>
        RegistrationCourseDTO GetRegistrationCourse(int selectedOrganizationId, int registrationCourseId);

        /// <summary>
        /// Get registration courses list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="season">Season</param>
        /// <param name="year">Year</param>
        /// <returns>Registration courses list</returns>
        IEnumerable<RegistrationCourseDTO> GetRegistrationCourses(int selectedOrganizationId, int year, int season);

        /// <summary>
        /// Get registration courses information
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Registration courses info list</returns>
        IEnumerable<RegistrationCourseViewModel> GetRegistrationCoursesInfo(int semesterId);

        /// <summary>
        /// Get registration courses with syllabi and syllabus status counts
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="deanDepartments">Dean departments</param>
        /// <param name="status">Syllabus status</param>
        /// <returns>Registration courses list with syllabi and syllabus status counts</returns>
        RegistrationCoursesSyllabusStatusesViewModel GetRegistrationCoursesWithSyllabi(int selectedOrganizationId, 
            int semesterId, int departmentId, IEnumerable<DepartmentDTO> deanDepartments, int? status);

        /// <summary>
        /// Get students for selection window
        /// </summary>
        /// <param name="organizationId">Organiaztion id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="excludedIds">Studnet ids to exclude</param>
        /// <param name="onlyActive">Select only active and academic leave students</param>
        /// <returns>List of students</returns>
        List<SelectStudentViewModel> GetStudentsForSelection(int organizationId, int semesterId,
                string[] excludedIds, bool onlyActive = true);

        /// <summary>
        /// Add students to course and get students from selection window
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserIds">Array of student user ids</param>
        /// <returns>List of students from selection window</returns>
        List<RegistrationCourseStudentViewModel> AddStudentsFromSelection(int organizationId, int semesterId,
            int registrationCourseId, string[] studentUserIds);

        /// <summary>
        /// Edit registration course
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="registrationCourse">Registration course</param>
        void EditRegistrationCourse(int organizationId, RegistrationCourseDTO registrationCourse);

        /// <summary>
        /// Mark registration course as deleted/undeleted
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="courseDetId">Course details id</param>
        /// <param name="isDeleted">Is deleted flag</param>
        void MarkRegistrationCourseDeleted(int organizationId, int courseDetId, bool isDeleted);

        /// <summary>
        /// Get registration course details with registered students
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Registration course details view model</returns>
        RegistrationCourseDetailsViewModel GetRegistrationCourseDetails(int organizationId, int registrationCourseId);
    }
}
