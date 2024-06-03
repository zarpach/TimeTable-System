using iuca.Application.DTO.Common;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Common
{
    public interface IPolicyService
    {
        /// <summary>
        /// Get policy list
        /// </summary>
        /// <returns>Policy list</returns>
        IEnumerable<PolicyDTO> GetPolicies();

        /// <summary>
        /// Get policy by id
        /// </summary>
        /// <param name="policyId">Policy id</param>
        /// <returns>Policy</returns>
        PolicyDTO GetPolicy(int policyId);

        /// <summary>
        /// Create policy
        /// </summary>
        /// <param name="policyDTO">Policy</param>
        void CreatePolicy(PolicyDTO policyDTO);

        /// <summary>
        /// Edit policy by id
        /// </summary>
        /// <param name="policyId">Policy id</param>
        /// <param name="policyDTO">Policy</param>
        void EditPolicy(int policyId, PolicyDTO policyDTO);

        /// <summary>
        /// Delete policy by id
        /// </summary>
        /// <param name="policyId">Policy id</param>
        void DeletePolicy(int policyId);
    }
}
