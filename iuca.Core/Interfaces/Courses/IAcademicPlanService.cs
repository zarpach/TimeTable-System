using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Students;
using iuca.Application.ViewModels.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Courses
{
    public interface IAcademicPlanService
    {
        /// <summary>
        /// Get academic plan list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Academic plan list</returns>
        IEnumerable<AcademicPlanDTO> GetAcademicPlans(int selectedOrganizationId);

        /// <summary>
        /// Get academic plan by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of academic plan/param>
        /// <returns>Academic plan model</returns>
        AcademicPlanDTO GetAcademicPlan(int selectedOrganizationId, int id);

        /// <summary>
        /// Get academic plans by year and department
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="departmentGroup">Department group/param>
        /// <returns>Academic plans for department group</returns>
        public AcademicPlanDTO GetAcademicPlanForDepartmentGroup(int selectedOrganizationId, DepartmentGroupDTO departmentGroup);

        /// <summary>
        /// Create academic plan
        /// </summary>
        /// <param name="academicPlanDTO">Academic plan model</param>
        void Create(AcademicPlanDTO academicPlanDTO);

        /// <summary>
        /// Edit academic plan
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of academic plan</param>
        /// <param name="academicPlanDTO">Academic plan model</param>
        void Edit(int selectedOrganizationId, int id, AcademicPlanDTO academicPlanDTO);

        /// <summary>
        /// Delete academic plan by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of academic plan</param>
        void Delete(int selectedOrganizationId, int id);

        /// <summary>
        /// Get academic plan by id to edit courses
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="academicPlanId">Id of academic plan/param>
        /// <returns>Academic plan edit model</returns>
        AcademicPlanViewModel GetAcademicPlanEditModel(int selectedOrganizationId, int academicPlanId);

        /// <summary>
        /// Get academic plan model filled by courses of selected year
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="academicPlanId">Id of academic plan/param>
        /// <param name="year">Year of the academic plan to get courses from<param>
        /// <returns>Academic plan edit model</returns>
        AcademicPlanViewModel FillAcademicPlanByYear(int selectedOrganizationId, int academicPlanId, int year);

        /// <summary>
        /// Edit academic plan courses
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="academicPlanId">Id of academic plan</param>
        /// <param name="modelCycleParts">List of academic plan cycle parts</param>
        void EditAcademicPlanCourses(int selectedOrganizationId, int academicPlanId, List<CyclePartDTO> modelCycleParts);

        void Dispose();
    }
}
