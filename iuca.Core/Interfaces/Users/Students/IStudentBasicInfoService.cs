using iuca.Application.DTO.Users.Students;
using System.Collections.Generic;


namespace iuca.Application.Interfaces.Users.Students
{
    public interface IStudentBasicInfoService
    {
        /// <summary>
        /// Get student basic info by id
        /// </summary>
        /// <param name="id">Id of record</param>
        /// <returns>Student basic info model</returns>
        StudentBasicInfoDTO GetStudentBasicInfo(int id);

        /// <summary>
        /// Get student basic info record by id
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="generateException">If true generates exeption if record is not found</param>
        /// <returns>StudentBasicInfoDTO</returns>
        StudentBasicInfoDTO GetStudentFullInfo(string studentUserId, bool generateException = true);

        /// <summary>
        /// Create student basic info in selected organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studentBasicInfoDTO">Student basic info model</param>
        /// <returns>Model of new created student basic info record</returns>
        StudentBasicInfoDTO Create(int selectedOrganizationId, StudentBasicInfoDTO studentBasicInfoDTO);


        /// <summary>
        /// Edit student basic info
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studentBasicInfoDTO">Student basic info model</param>
        void Edit(int selectedOrganizationId, StudentBasicInfoDTO studentBasicInfoDTO);

        /// <summary>
        /// Delete student basic info by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of record</param>
        public void Delete(int selectedOrganizationId, int id);

        void Dispose();
    }
}
