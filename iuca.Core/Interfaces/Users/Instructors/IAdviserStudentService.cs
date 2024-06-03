using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.ViewModels.Courses;
using iuca.Application.ViewModels.Users.Instructors;
using iuca.Application.ViewModels.Users.Students;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Users.Instructors
{
    public interface IAdviserStudentService
    {
        /// <summary>
        /// Get adviser list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Adviser list</returns>
        IEnumerable<UserDTO> GetAdvisers(int organizationId);

        /// <summary>
        /// Get instructor brief view model list of advisers by dean user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns>Instructor brief view model of user list who has adviser role</returns>
        IEnumerable<InstructorInfoBriefViewModel> GetAdvisers(int organizationId, string deanUserId);

        /// <summary>
        /// Get students for intructor-adviser
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <returns>List of students</returns>
        List<AdviserStudentViewModel> GetAdviserStudentsByInstuctorId(int organizationId, string instructorUserId);

        /// <summary>
        /// Get students for intructor-adviser
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserIds">Instructor user ids</param>
        /// <returns>List of students</returns>
        List<AdviserStudentViewModel> GetAdviserStudentsByInstuctorId(int organizationId, List<string> instructorUserIds);

        /// <summary>
        /// Get departments for intructor-adviser
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <returns>List of departments</returns>
        List<DepartmentGroupDTO> GetAdviserDepartmentGroupsByInstuctorId(int organizationId, string instructorUserId);

        /// <summary>
        /// Get students for selection window
        /// </summary>
        /// <param name="departmentId">Department ids</param>
        /// <param name="departmentGroupId">Group id for courses selection</param>
        /// <param name="organizationId">Organiaztion id</param>
        /// <param name="excludedIds">Studnet ids to exclude</param>
        /// <param name="onlyActive">Select only active and academic leave students</param>
        /// <returns>List of students</returns>
        public List<SelectStudentViewModel> GetStudentsForSelection(int[] departmentId, int departmentGroupId,
            int organizationId, string[] excludedIds, bool onlyActive = true);

        /// <summary>
        /// Get students from selection window
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserIds">Array of student user ids</param>
        /// <returns>List of adviser students from selection window</returns>
        List<AdviserStudentViewModel> GetStudentsFromSelection(int organizationId, string[] studentUserIds);

        /// <summary>
        /// Get adviser list for student
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student advisers</returns>
        IEnumerable<UserDTO> GetStudentAdvisers(int organizationId, string studentUserId);

        /// <summary>
        /// Set new advisers for a student
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="newAdviserIds">List of new advisers</param>
        /// <returns>Non adviser user message</returns>
        string SetStudentAdvisers(int organizationId, string studentUserId, IEnumerable<string> newAdviserIds);

        /// <summary>
        /// Edit adviser students
        /// </summary>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserIds">Student user id list</param>
        void EditAdviserStudents(string instructorUserId, int organizationId, List<string> studentUserIds);

        /// <summary>
        /// Get students list for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="lastName">Student last name</param>
        /// <param name="firstName">Student first name</param>
        /// <param name="studentId">StudentId</param>
        /// <param name="onlyActive">Only active students</param>
        /// <returns>Student list for selection</returns>
        List<SelectStudentViewModel> GetStudentSelectList(int organizationId, string instructorUserId, int departmentGroupId,
            string lastName, string firstName, int studentId, bool onlyActive);

        /// <summary>
        /// Get list of students with their course registrations
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="activeOnly">If true - only active students</param>
        /// <param name="withDebtsOnly">If true - students with debts only</param>
        /// <param name="incompleteRegistration">If true - students with incomplete registartion only</param>
        /// <returns>List of students with their course registrations</returns>
        List<AdviserStudentRegistrationViewModel> GetAdviserStudentRegistrations(int organizationId, string instructorUserId,
                        int? departmentGroupId, int semesterId, bool activeOnly = true, bool withDebtsOnly = false, bool incompleteRegistration = false);

        /// <summary>
        /// Get student registration check model by registration id
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <returns>Check registration model</returns>
        CheckRegistrationViewModel GetStudentRegistrationCheckModel(int studentCourseRegistrationId);

        /// <summary>
        /// Get student add/drop check model by registration id
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <returns>Check registration model</returns>
        CheckRegistrationViewModel GetStudentAddDropCheckModel(int studentCourseRegistrationId);

        /// <summary>
        /// Save changes of student registration checking
        /// </summary>
        /// <param name="model">Check registration post view model</param>
        void CheckStudentRegistration(CheckRegistrationPostViewModel model);

        /// <summary>
        /// Save changes of student add/drop form
        /// </summary>
        /// <param name="model">Check registration post view model</param>
        void CheckStudentAddDropForm(CheckRegistrationPostViewModel model);

        /// <summary>
        /// Check if adviser has student
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns></returns>
        bool HasStudent(int organizationId, string adviserUserId, string studentUserId);
    }
}
