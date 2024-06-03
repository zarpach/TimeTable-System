using iuca.Application.DTO.Users.Staff;
using System.Collections.Generic;


namespace iuca.Application.Interfaces.Users.Staff
{
    public interface IStaffBasicInfoService
    {
        /// <summary>
        /// Get staff full info by staff id
        /// </summary>
        /// <param name="staffUserId">Staff user id</param>
        /// <param name="generateException">If true generates exception when staff info not found</param>
        /// <returns>Staff full info model</returns>
        public StaffBasicInfoDTO GetStaffFullInfo(string staffUserId, bool generateException = true);

        /// <summary>
        /// Get staff basic info by id
        /// </summary>
        /// <param name="id">Id of record</param>
        /// <returns>Staff basic info model</returns>
        StaffBasicInfoDTO GetStaffBasicInfo(int id);

        /// <summary>
        /// Create staff basic info in selected organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="staffBasicInfoDTO">Staff basic info model</param>
        /// <returns>Model of new created staff basic info record</returns>
        StaffBasicInfoDTO Create(int selectedOrganizationId, StaffBasicInfoDTO staffBasicInfoDTO);

        /// <summary>
        /// Edit staff basic info
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="staffBasicInfoId">Staff basic info id</param>
        /// <param name="staffBasicInfoDTO">Staff basic info model</param>
        void Edit(int selectedOrganizationId, int staffBasicInfoId, StaffBasicInfoDTO staffBasicInfoDTO);

        /// <summary>
        /// Delete staff basic info by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of record</param>
        void Delete(int selectedOrganizationId, int id);

        void Dispose();
    }
}
