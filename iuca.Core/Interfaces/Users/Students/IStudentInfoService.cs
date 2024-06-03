using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.ViewModels.Users.Students;
using iuca.Infrastructure.Identity.Entities;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Users.Students
{
    public interface IStudentInfoService
    {
        /// <summary>
        /// Get student info list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>List of student info</returns>
        IEnumerable<StudentInfoBriefViewModel> GetStudentInfoList(int organizationId);

        /// <summary>
        /// Get student info list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="onlyActive">Only active students</param>
        /// <returns>List of student info</returns>
        IEnumerable<StudentInfoBriefViewModel> GetStudentInfoList(int organizationId, bool onlyActive);

        /// <summary>
        /// Get minimum student info list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentStates">Student state list</param>
        /// <returns>List of minimum student info</returns>
        IEnumerable<StudentMinimumInfoViewModel> GetStudentInfoList(int organizationId, int[] studentStates);

        /// <summary>
        /// Get student info by user basic info id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student info model</returns>
        public StudentInfoDetailsViewModel GetStudentDetailsInfo(int organizationId, string studentUserId);

        /// <summary>
        /// Get student info by student user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student info model</returns>
        StudentMinimumInfoViewModel GetStudentMinimumInfo(int organizationId, string studentUserId);

        /// <summary>
        /// Get students list for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="lastName">Student last name</param>
        /// <param name="firstName">Student first name</param>
        /// <param name="studentId">StudentId</param>
        /// <param name="onlyActive">Only active students</param>
        /// <returns>Student list for selection</returns>
        public List<SelectStudentViewModel> GetStudentSelectList(int organizationId, int departmentId, int departmentGroupId,
            string lastName, string firstName, int studentId, bool onlyActive);

        /// <summary>
        /// Get students list for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="departmentIds">List of department id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="lastName">Student last name</param>
        /// <param name="firstName">Student first name</param>
        /// <param name="studentId">StudentId</param>
        /// <param name="onlyActive">Only active students</param>
        /// <returns>Student list for selection</returns>
        public List<SelectStudentViewModel> GetStudentSelectList(int organizationId, List<int> departmentIds, int departmentGroupId,
            string lastName, string firstName, int studentId, bool onlyActive);

        /// <summary>
        /// Set student state
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="state">New student state</param>
        void SetStudentState(int organizationId, string studentUserId, int state);

        /// <summary>
        /// Get student department group
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student department group</returns>
        GroupDTO GetStudentDepartmentGroup(int organizationId, string studentUserId);

        /// <summary>
        /// Set student department group
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="newDepartmentGroupId">New student department group</param>
        /// <param name="oldDepartmentGroupId">Old student department group</param>
        void SetStudentDepartmentGroup(int organizationId, string studentUserId, int newDepartmentGroupId, int oldDepartmentGroupId = 0);

        /// <summary>
        /// Create student info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentInfo">Student info model</param>
        void Create(int organizationId, StudentInfoDetailsViewModel studentInfo);

        /// <summary>
        /// Edit student info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentInfo">Student info model</param>
        void Edit(int organizationId, StudentInfoDetailsViewModel studentInfo);

        /// <summary>
        /// Delete student info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="id">Student basic info id</param>
        void Delete(int organizationId, int id);

        /// <summary>
        /// Get user list with empty student
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>List of users</returns>
        IEnumerable<ApplicationUser> GetEmptyStudents(int selectedOrganizationId);

        /// <summary>
        /// Get student user ids by department
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="activeOnly">If true - returns only active students</param>
        /// <returns></returns>
        List<string> GetStudentIdsByDepartment(int organizationId, int departmentId, bool activeOnly = true);

        /// <summary>
        /// Get student user id by student id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentId">Student id</param>
        /// <returns>Student user id</returns>
        string GetUserIdByStudentId(int organizationId, int studentId);

        /// <summary>
        /// Get prep student list
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="activeOnly">Only active students</param>
        /// <returns>Prep student list</returns>
        List<PrepStudentViewModel> GetPrepStudents(int organizationId, bool activeOnly = true);

        /// <summary>
        /// Save PREP student department group
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="studentBasicInfoId">Student Basic Info Id</param>
        /// <param name="prepDepartmentGroupId">PREP Department Group Id</param>
        void SavePrepStudentDepartmentGroup(int organizationId, int studentBasicInfoId, int? prepDepartmentGroupId);

        /// <summary>
        /// Get students list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="onlyActive">Only active students</param>
        /// <returns>Student list</returns>
        List<UserDTO> GetStudents(int organizationId, bool onlyActive);

        void Dispose();
    }
}
