using iuca.Application.DTO.Users.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.Instructors
{
    public interface IInstructorOrgInfoService
    {
        /// <summary>
        /// Check if organization info exists for instructor
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="instructorBasicInfoId"></param>
        /// <returns></returns>
        bool IfExists(int organizationId, int instructorBasicInfoId);

        /// <summary>
        /// Get instructor org info by instructor basic info id and organization id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <returns>Instructor organization info model</returns>
        InstructorOrgInfoDTO GetInstructorOrgInfo(int organizationId, int instructorBasicInfoId);

        /// <summary>
        /// Create instructor org info
        /// </summary>
        /// <param name="instructorOrgInfoDTO">Country model</param>
        void Create(InstructorOrgInfoDTO instructorOrgInfoDTO);

        /// <summary>
        /// Edit instructor org info
        /// </summary>
        /// <param name="organizationId">Id of organization</param>
        /// <param name="instructorOrgInfoDTO">Instructor org info model</param>
        void Edit(int organizationId, InstructorOrgInfoDTO instructorOrgInfoDTO);

        /// <summary>
        /// Delete instructor org info 
        /// </summary>
        /// <param name="organizationId">Id of organization</param>
        /// <param name="instructorBasicInfoId">Id of instructor basic info </param>
        /// <param name="generateException">If true generates exception when record not found</param>
        void Delete(int organizationId, int instructorBasicInfoId, bool generateException = true);

        void Dispose();
    }
}
