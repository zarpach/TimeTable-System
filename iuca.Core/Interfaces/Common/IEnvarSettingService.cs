using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.UserInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface IEnvarSettingService
    {
        /// <summary>
        /// Get EnvarSettings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>EnvarSettings model. Creates new if does not exist</returns>
        EnvarSettingDTO GetEnvarSettings(int organizationId);

        /// <summary>
        /// Get maximum registration settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Maximum registration credits </returns>
        int GetMaxRegistrationCredits(int organizationId);

        /// <summary>
        /// Set maximum registration settings
        /// </summary>
        /// <param name="id">Settings id</param>
        /// <param name="maxRegistrationCredits">Max registrstion credits value</param>
        void SetMaxRegistrationCredits(int id, int maxRegistrationCredits);

        /// <summary>
        /// Get default instructor settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Default instructor</returns>
        UserDTO GetDefaultInstructor(int organizationId);

        /// <summary>
        /// Set default instructor settings
        /// </summary>
        /// <param name="id">Settings id</param>
        /// <param name="defaultInstructorId">Default instructor id value</param>
        void SetDefaultInstructor(int id, string defaultInstructorId);

        /// <summary>
        /// Get current semester settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Current semester</returns>
        int GetCurrentSemester(int organizationId);

        /// <summary>
        /// Set current semester settings
        /// </summary>
        /// <param name="id">Settings id</param>
        /// <param name="semesterId">Current semester id value</param>
        void SetCurrentSemester(int id, int semesterId);

        /// <summary>
        /// Get upcoming semester settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Upcoming semester</returns>
        int GetUpcomingSemester(int organizationId);

        /// <summary>
        /// Set upcoming semester settings
        /// </summary>
        /// <param name="id">Settings id</param>
        /// <param name="semesterId">Upcoming semester id value</param>
        void SetUpcomingSemester(int id, int semesterId);

        /// <summary>
        /// Get last attendance update settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Upcoming semester</returns>
        DateTime GetLastAttendanceUpdate(int organizationId);

        /// <summary>
        /// Set last attendance update settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="lastAttendanceUpdate">Last attendance update date and time</param>
        void SetLastAttendanceUpdate(int organizationId, DateTime lastAttendanceUpdate);
    }
}
