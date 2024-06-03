
using iuca.Application.DTO.Courses;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Courses
{
    public interface IProposalCourseService
    {
        /// <summary>
        /// Set proposal course status by id
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        /// <param name="status">Proposal status</param>
        void SetProposalCourseStatus(int proposalCourseId, int status);

        /// <summary>
        /// Set proposal course statuses by proposal id
        /// </summary>
        /// <param name="proposalId">Proposal id</param>
        /// <param name="status">Proposal status</param>
        void SetProposalCourseStatuses(int proposalId, int status);

        /// <summary>
        /// Create proposal course
        /// </summary>
        /// <param name="proposalCourseDTO">Proposal course</param>
        void CreateProposalCourse(ProposalCourseDTO proposalCourseDTO);

        /// <summary>
        /// Edit proposal course by id
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        /// <param name="proposalCourseDTO">Proposal course</param>
        void EditProposalCourse(int proposalCourseId, ProposalCourseDTO proposalCourseDTO);

        /// <summary>
        /// Delete proposal course by id
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        void DeleteProposalCourse(int proposalCourseId);

        /// <summary>
        /// Submit proposal course for approval
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        void SubmitProposalCourseForApproval(int proposalCourseId);

        /// <summary>
        /// Return proposal course from approval
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        void ReturnProposalCourseFromApproval(int proposalCourseId);

        /// <summary>
        /// Edit proposal course instructors
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        /// <param name="newInstructorIds">Instructor ids</param>
        void EditProposalCourseInstructors(int proposalCourseId, IEnumerable<string> newInstructorIds);

        /// <summary>
        /// Replace proposal course instructor
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        /// <param name="previousInstructorId">Previous instructor id</param>
        /// <param name="futureInstructorId">Future instructor id</param>
        void ReplaceProposalCourseInstructor(int proposalCourseId, string previousInstructorId, string futureInstructorId);
    }
}
