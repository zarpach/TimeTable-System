using iuca.Application.DTO.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface IDepartmentService
    {
        /// <summary>
        /// Get department list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="activeOnly">Active only</param>
        /// <returns>Department list</returns>
        IEnumerable<DepartmentDTO> GetDepartments(int selectedOrganizationId, bool activeOnly = false);

        /// <summary>
        /// Get department by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department/param>
        /// <returns>Department model</returns>
        DepartmentDTO GetDepartment(int selectedOrganizationId, int id);

        /// <summary>
        /// Create department
        /// </summary>
        /// <param name="departmentDTO">Department model</param>
        void Create(DepartmentDTO departmentDTO);

        /// <summary>
        /// Edit department
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department</param>
        /// <param name="departmentDTO">Department model</param>
        void Edit(int selectedOrganizationId, int id, DepartmentDTO departmentDTO);

        /// <summary>
        /// Delete department by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department</param>
        void Delete(int selectedOrganizationId, int id);

        /// <summary>
        /// Get department SelectList
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="selectedDepartment">Selected department id</param>
        /// <returns>SelectList of departments</returns>
        List<SelectListItem> GetDepartmentSelectList(int organizationId, int? selectedDepartment);

        void Dispose();
    }
}
