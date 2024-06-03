using iuca.Application.DTO.Users.UserInfo;
using System.Collections.Generic;


namespace iuca.Application.Interfaces.Users.UserInfo
{
    public interface IUserBasicInfoService
    {
        /// <summary>
        /// Get user basic info list
        /// </summary>
        /// <returns>User basic info list</returns>
        IEnumerable<UserBasicInfoDTO> GetUserBasicInfoList();

        /// <summary>
        /// Get user basic info record by id
        /// </summary>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="generateException">If true generates exception when info not found</param>
        /// <returns>User basic info record</returns>
        UserBasicInfoDTO GetUserFullInfo(string applicationUserId, bool generateException = true);

        /// <summary>
        /// Get user basic info list by id list
        /// </summary>
        /// <param name="ids">List of ids</param>
        /// <returns>User basic info list</returns>
        IEnumerable<UserBasicInfoDTO> GetUserBasicInfoList(List<int> ids);

        /// <summary>
        /// Get user basic info record by id
        /// </summary>
        /// <param name="id">Record id</param>
        /// <returns>User basic info record</returns>
        UserBasicInfoDTO GetUserBasicInfo(int id);

        /// <summary>
        /// Create user basic info record
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="userBasicInfoDTO">User basic info model</param>
        /// <returns>Model of new created record</returns>
        UserBasicInfoDTO Create(int selectedOrganizationId, UserBasicInfoDTO userBasicInfoDTO);

        /// <summary>
        /// Edti user basic info record
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="userBasicInfoDTO">User basic info model</param>
        void Edit(int selectedOrganizationId, UserBasicInfoDTO userBasicInfoDTO);

        /// <summary>
        /// Delete user basic info record
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of the record</param>
        void Delete(int selectedOrganizationId, int id);

        /// <summary>
        /// Set is main organization flag
        /// </summary>
        /// <param name="userBasicInfoId">User basic info id</param>
        /// <param name="isMainOrganization">Is main organization flag</param>
        void SetOwnerFlag(int userBasicInfoId, bool isMainOrganization);

        void Dispose();
    }
}
