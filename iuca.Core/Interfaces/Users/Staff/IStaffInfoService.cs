using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.ViewModels;
using iuca.Application.ViewModels.Users.Staff;
using iuca.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.Staff
{
    public interface IStaffInfoService
    {
        /// <summary>
        /// Get staff info list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>List of staff info</returns>
        IEnumerable<StaffInfoBriefViewModel> GetStaffInfoList(int organizationId);

        /// <summary>
        /// Get staff full by user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="staffUserId">Staff user id</param>
        /// <returns>Staff info model</returns>
        StaffInfoDetailsViewModel GetStaffFullInfo(int organizationId, string staffUserId);

        /// <summary>
        /// Create staff info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="staffInfo">Staff info model</param>
        void Create(int organizationId, StaffInfoDetailsViewModel staffInfo);

        /// <summary>
        /// Edit staff info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="staffInfo">Staff info model</param>
        void Edit(int organizationId, StaffInfoDetailsViewModel staffInfo);

        /// <summary>
        /// Delete staff info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="id">Staff basic info id</param>
        void Delete(int organizationId, int id);

        /// <summary>
        /// Get user list with empty staff
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>List of users</returns>
        IEnumerable<ApplicationUser> GetEmptyStaff(int selectedOrganizationId);

        void Dispose();
    }
}
