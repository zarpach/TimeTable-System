using iuca.Application.DTO.Common;
using iuca.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface IOrganizationService
    {
        /// <summary>
        /// Get organization list
        /// </summary>
        /// <returns>Organization list</returns>
        IEnumerable<OrganizationDTO> GetOrganizations();

        /// <summary>
        /// Get organization by id
        /// </summary>
        /// <param name="id">Id of organization</param>
        /// <returns>Organization model</returns>
        OrganizationDTO GetOrganization(int id);

        /// <summary>
        /// Create organization
        /// </summary>
        /// <param name="organizationDTO">Organization model</param>
        void Create(OrganizationDTO organizationDTO);

        /// <summary>
        /// Edit organization
        /// </summary>
        /// <param name="id">Id of organization</param>
        /// <param name="organizationDTO">Organization model</param>
        void Edit(int id, OrganizationDTO organizationDTO);

        /// <summary>
        /// Delete organization by id
        /// </summary>
        /// <param name="id">Id of organization</param>
        void Delete(int id);

        /// <summary>
        /// Get selected organization of user
        /// </summary>
        /// <param name="userId">Application user id</param>
        /// <returns>Organization</returns>
        OrganizationDTO GetUserSelectedOrganization(string userId);

        /// <summary>
        /// Get selected organization of user
        /// </summary>
        /// <param name="userId">Application user</param>
        /// <returns>Organization</returns>
        OrganizationDTO GetUserSelectedOrganization(ApplicationUser applicationUser);

        /// <summary>
        /// Change user organization
        /// </summary>
        /// <param name="userId">Application user id</param>
        /// <param name="newOrganizationId">New Organization Id</param>
        void ChangeUserOrganization(string userId, int newOrganizationId);

        /// <summary>
        /// Change user organization
        /// </summary>
        /// <param name="userId">Application user</param>
        /// <param name="newOrganizationId">New Organization Id</param>
        void ChangeUserOrganization(ApplicationUser applicationUser, int newOrganizationId);

        /// <summary>
        /// Get selected organization of user
        /// </summary>
        /// <param name="principal">User claims</param>
        /// <returns>Selected organization id</returns>
        int GetSelectedOrganization(ClaimsPrincipal principal);

        /// <summary>
        /// Get main organization
        /// </summary>
        /// <returns>Organization DTO</returns>
        OrganizationDTO GetMainOrganization();

        void Dispose();
    }
}
