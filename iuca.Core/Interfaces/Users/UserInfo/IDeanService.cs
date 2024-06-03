using iuca.Application.ViewModels.Users.UserInfo;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.UserInfo
{
    public interface IDeanService
    {
        /// <summary>
        /// Get dean departments model list who has dean role
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Dean departments model view model list who has dean role</returns>
        IEnumerable<DeanDepartmentViewModel> GetDeans(int organizationId);

        /// <summary>
        /// Get dean departments
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns>Dean departments model</returns>
        DeanDepartmentViewModel GetDeanDepartments(int organizationId, string deanUserId);

        /// <summary>
        /// Edit dean departments
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <param name="departmentIds">Department id array</param>
        void EditDeanDepartments(int organizationId, string deanUserId, List<int> departmentIds);

        /// <summary>
        /// Get instructor brief view model list of advisers by dean user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns>Instructor brief view model of user list who has adviser role</returns>
        IEnumerable<DeanAdviserViewModel> GetDeanAdvisers(int organizationId, string deanUserId);

        /// <summary>
        /// Get list of instructors that not belong to dean
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <param name="onlyActive">Only active instructors</param>
        /// <returns>List of instructors</returns>
        List<DeanAdviserViewModel> GetFreeInstructorsForDean(int organizationId, string deanUserId, bool onlyActive = true);

        /// <summary>
        /// Add or remove adviser from dean
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <param name="isSelected">If true - add, else - remove</param>
        void SetDeanAdviser(int organizationId, string deanUserId, string instructorUserId, bool isSelected);

        /// <summary>
        /// Get dean SelectList
        /// </summary>
        /// <param name="selectedOrganizationId">Id of selected organization</param>
        /// <param name="deanUserId">Selected dean user id</param>
        /// <returns>SelectList of deans</returns>
        List<SelectListItem> GetDeanSelectList(int selectedOrganizationId, string deanUserId = null);

        /// <summary>
        /// Get advisers by dean user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns>List of advisers</returns>
        List<ApplicationUser> GetAdvisersByDeanUserId(int organizationId, string deanUserId);
    }
}