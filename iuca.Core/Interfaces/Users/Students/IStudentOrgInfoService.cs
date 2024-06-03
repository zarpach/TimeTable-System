using iuca.Application.DTO.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.Students
{
    public interface IStudentOrgInfoService
    {
        /// <summary>
        /// Check if organization info exists for student
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="studentBasicInfoId"></param>
        /// <returns></returns>
        bool IfExists(int organizationId, int studentBasicInfoId);

        /// <summary>
        /// Get student org info list by student basic info ids and organization id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentBasicInfoIdList">List of student basic info id</param>
        /// <returns>List of student organization info model</returns>
        List<StudentOrgInfoDTO> GetStudentOrgInfoList(int organizationId, List<int> studentBasicInfoIdList);

        /// <summary>
        /// Get student org info by student basic info id and organization id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student organization info model</returns>
        public StudentOrgInfoDTO GetStudentOrgInfoByUserId(int organizationId, string studentUserId);

        /// <summary>
        /// Get student org info by student basic info id and organization id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentBasicInfoId">Student basic info id</param>
        /// <returns>Student organization info model</returns>
        StudentOrgInfoDTO GetStudentOrgInfo(int organizationId, int studentBasicInfoId);


        /// <summary>
        /// Create student org info
        /// </summary>
        /// <param name="studentOrgInfoDTO">Country model</param>
        void Create(StudentOrgInfoDTO studentOrgInfoDTO);

        /// <summary>
        /// Edit student org info
        /// </summary>
        /// <param name="organizationId">Id of organization</param>
        /// <param name="studentOrgInfoDTO">student org info model</param>
        void Edit(int organizationId, StudentOrgInfoDTO studentOrgInfoDTO);

        /// <summary>
        /// Delete student org info 
        /// </summary>
        /// <param name="organizationId">Id of organization</param>
        /// <param name="studentBasicInfoId">Id of student basic info/param>
        void Delete(int organizationId, int studentBasicInfoId);

        void Dispose();
    }
}
