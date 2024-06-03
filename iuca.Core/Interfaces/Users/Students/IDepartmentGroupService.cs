using iuca.Application.DTO.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.Students
{
    public interface IDepartmentGroupService
    {
        /// <summary>
        /// Get department group list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Department group list</returns>
        IEnumerable<DepartmentGroupDTO> GetDepartmentGroups(int selectedOrganizationId);

        /// <summary>
        /// Get department group by code
        /// </summary>
        /// <param name="code">Department code</param>
        /// <returns>Department groups of selected department</returns>
        public IEnumerable<DepartmentGroupDTO> GetDepartmentGroupsByParam(int selectedOrganizationId, int departmentId);

        /// <summary>
        /// Get department group by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department group</param>
        /// <returns>Department group model</returns>
        DepartmentGroupDTO GetDepartmentGroup(int selectedOrganizationId, int id);

        /// <summary>
        /// Create department group
        /// </summary>
        /// <param name="departmentGroupDTO">Department group model</param>
        void Create(DepartmentGroupDTO departmentGroupDTO);

        /// <summary>
        /// Edit department group
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department group</param>
        /// <param name="departmentGroupDTO">Department group model</param>
        void Edit(int selectedOrganizationId, int id, DepartmentGroupDTO departmentGroupDTO);

        /// <summary>
        /// Delete department group by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department group</param>
        void Delete(int selectedOrganizationId, int id);

        /// <summary>
        /// Generate department groups for selected year
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="year">Year</param>
        void GenerateDepartmentGroupsForYear(int selectedOrganizationId, int year);

        void Dispose();
    }
}
