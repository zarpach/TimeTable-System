using iuca.Application.DTO.Users.Instructors;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Users.Instructors
{
    public interface IInstructorBasicInfoService
    {
        /// <summary>
        /// Get full instructor info record by intructor id
        /// </summary>
        /// <param name="id">id of instructor</param>
        /// <param name="generateException">If true generates exception when instructor info not found</param>
        /// <returns>InstructorBasicInfoDTO</returns>
        public InstructorBasicInfoDTO GetInstructorFullInfo(string id, bool generateException = true);

        /// <summary>
        /// Get instructor basic info by id
        /// </summary>
        /// <param name="id">id of instructor basic info record</param>
        /// <returns>InstructorBasicInfoDTO</returns>
        InstructorBasicInfoDTO GetInstructorBasicInfo(int id);

        /// <summary>
        /// Create instructor basic info in selected organization
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="instructorBasicInfoDTO">Instructor basic info model</param>
        /// <returns>Model of new created instructor basic info record</returns>
        InstructorBasicInfoDTO Create(int selectedOrganizationId, InstructorBasicInfoDTO instructorBasicInfoDTO);

        /// <summary>
        /// Edit instructor basic info
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="instructorBasicInfoDTO">Instructor basic info model</param>
        void Edit(int selectedOrganizationId, InstructorBasicInfoDTO instructorBasicInfoDTO);

        /// <summary>
        /// Delete instructor basic info by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of record</param>
        void Delete(int selectedOrganizationId, int id);

        void Dispose();

        /// <summary>
        /// Set is main organization flag
        /// </summary>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <param name="isMainOrganization">Is main organization flag</param>
        void SetOwnerFlag(int instructorBasicInfoId, bool isMainOrganization);

        /// <summary>
        /// Set IsChanged flag for export to old db
        /// </summary>
        /// <param name="applicationUserId">Application User Id</param>
        /// <param name="isChanged">Is changed</param>
        void SetIsChangedFlag(string applicationUserId, bool isChanged);

        /// <summary>
        /// Set IsChanged flag for export to old db
        /// </summary>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <param name="isChanged">Is changed</param>
        void SetIsChangedFlag(int instructorBasicInfoId, bool isChanged);

    }
}
