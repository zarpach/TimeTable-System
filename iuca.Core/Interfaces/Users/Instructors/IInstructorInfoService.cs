using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.ViewModels.Users.Instructors;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;


namespace iuca.Application.Interfaces.Users.Instructors
{
    public interface IInstructorInfoService
    {
        /// <summary>
        /// Get instructor info list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="state">Instructor state</param>
        /// <param name="departmentId">Department id</param>
        /// <returns>List of instructor info</returns>
        public IEnumerable<InstructorInfoBriefViewModel> GetInstructorInfoList(int organizationId, enu_InstructorState state,
            int? departmentId);

        /// <summary>
        /// Refresh instructor states. If email starts with "_" - inactive, else active
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        void RefreshInstructorStates(int organizationId);

        /// <summary>
        /// Get instructor info by instructor id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <returns>Instructor info model</returns>
        InstructorInfoDetailsViewModel GetInstructorDetailsInfo(int organizationId, string instructorUserId);

        /// <summary>
        /// Edit instructor user info
        /// </summary>
        /// <param name="model">Instructor info view model</param>
        void EditInstructorUserInfo(InstructorUserInfoViewModel model);

        /// <summary>
        /// Delete instructor info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="id">Instructor basic info id</param>
        void Delete(int organizationId, int id);

        /// <summary>
        /// Get user list with empty instructors
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>List of users</returns>
        IEnumerable<ApplicationUser> GetEmptyInstructors(int selectedOrganizationId);

        /// <summary>
        /// Get instructor SelectList
        /// </summary>
        /// <param name="instructorUserId">Selected instructor user id</param>
        /// <param name="selectedOrganizationId">Id of selected organization</param>
        /// <returns>SelectList of instructors</returns>
        public List<SelectListItem> GetInstructorSelectList(string instructorUserId, int selectedOrganizationId);

        void Dispose();
    }
}
