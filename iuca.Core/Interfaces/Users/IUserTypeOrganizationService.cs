

using iuca.Application.DTO.Common;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Users
{
    public interface IUserTypeOrganizationService
    {
        /// <summary>
        /// Check if relation exists
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="userType">User type</param>
        /// <returns></returns>
        bool UserHasType(int organizationId, string applicationUserId, int userType);

        /// <summary>
        /// Get user's organizations
        /// </summary>
        /// <param name="applicationUserId">ApplicationUserId</param>
        /// <returns>List of organizations</returns>
        IEnumerable<OrganizationDTO> GetUserOrganizations(string applicationUserId);

        /// <summary>
        /// Create user type for organization, add claim, check and update IsMainOrganization flag
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="userType">User type</param>
        /// <param name="userType">If true genretates exception if record already exists</param>
        public void Create(int organizationId, string applicationUserId, int userType, bool generateException = true);

        /// <summary>
        /// Delete user type for organization, remove claim, check and update IsMainOrganization flag
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="applicationUserId">Application user id</param>
        /// <param name="userType">User type</param>
        void Delete(int organizationId, string applicationUserId, int userType);

        void Dispose();
    }
}
