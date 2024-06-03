
using iuca.Application.DTO.Common;
using System.Collections.Generic;
using iuca.Application.DTO.Courses;

namespace iuca.Application.Interfaces.Courses
{
    public interface IProposalService
    {
        /// <summary>
        /// Get proposals
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="deanDepartments">Dean departments</param>
        /// <returns>Proposals</returns>
        IEnumerable<ProposalDTO> GetProposals(int semesterId, IEnumerable<DepartmentDTO> deanDepartments);

        /// <summary>
        /// Get proposal by id
        /// </summary>
        /// <param name="proposalId">Proposal id</param>
        /// <returns>Proposal</returns>
        ProposalDTO GetProposal(int proposalId);

        /// <summary>
        /// Create proposal
        /// </summary>
        /// <param name="proposalDTO">Proposal</param>
        void CreateProposal(ProposalDTO proposalDTO);

        /// <summary>
        /// Edit proposal by id
        /// </summary>
        /// <param name="proposalId">Proposal id</param>
        /// <param name="proposalDTO">Proposal</param>
        void EditProposal(int proposalId, ProposalDTO proposalDTO);

        /// <summary>
        /// Edit proposal courses by proposal id
        /// </summary>
        /// <param name="proposalId">Proposal id</param>
        /// <param name="proposalCourseDTOs">Proposal courses</param>
        void EditProposalCourses(int proposalId, IEnumerable<ProposalCourseDTO> proposalCourseDTOs);

        /// <summary>
        /// Delete proposal by id
        /// </summary>
        /// <param name="proposalId">Proposal id</param>
        void DeleteProposal(int proposalId);

        /// <summary>
        /// Get courses for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="excludedCourseIds">Course ids to exclude</param>
        /// <returns>Course list without excluded course ids</returns>
        IEnumerable<CourseDTO> GetCoursesForSelection(int organizationId, int[] excludedCourseIds);

        /// <summary>
        /// Get course from selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="selectedCourseId">Selected course id</param>
        /// <returns>Course</returns>
        CourseDTO GetCourseFromSelection(int organizationId, int selectedCourseId);
    }
}
