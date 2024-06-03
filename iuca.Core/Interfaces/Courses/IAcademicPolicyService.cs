using iuca.Application.DTO.Courses;

namespace iuca.Application.Interfaces.Courses
{
    public interface IAcademicPolicyService
    {
        /// <summary>
        /// Create academic policy
        /// </summary>
        /// <param name="academicPolicyDTO">Academic policy</param>
        void CreateAcademicPolicy(AcademicPolicyDTO academicPolicyDTO);

        /// <summary>
        /// Edit academic policy by id
        /// </summary>
        /// <param name="academicPolicyId">Academic policy id</param>
        /// <param name="academicPolicyDTO">Academic policy</param>
        void EditAcademicPolicy(int academicPolicyId, AcademicPolicyDTO academicPolicyDTO);

        /// <summary>
        /// Delete academic policy by id
        /// </summary>
        /// <param name="academicPolicyId">Academic policy id</param>
        void DeleteAcademicPolicy(int academicPolicyId);
    }
}
