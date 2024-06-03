using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Common
{
    public class PolicyService : IPolicyService
    {
        private readonly IApplicationDbContext _db;

        public PolicyService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get policy list
        /// </summary>
        /// <returns>Policy list</returns>
        public IEnumerable<PolicyDTO> GetPolicies()
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Policy, PolicyDTO>();
            }).CreateMapper();

            var policies = _db.Policies.ToList();
            
            return mapper.Map<IEnumerable<Policy>, IEnumerable<PolicyDTO>>(policies);
        }

        /// <summary>
        /// Get policy by id
        /// </summary>
        /// <param name="policyId">Policy id</param>
        /// <returns>Policy</returns>
        public PolicyDTO GetPolicy(int policyId)
        {
            if (policyId == 0)
                throw new Exception($"The policy id is 0.");

            var policy = _db.Policies.Find(policyId);
            if (policy == null)
                throw new Exception($"The policy with id {policyId} does not exist.");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Policy, PolicyDTO>();
            }).CreateMapper();

            return mapper.Map<Policy, PolicyDTO>(policy);
        }

        /// <summary>
        /// Create policy
        /// </summary>
        /// <param name="policyDTO">Policy</param>
        public void CreatePolicy(PolicyDTO policyDTO)
        {
            if (policyDTO == null)
                throw new Exception("The policy is null.");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<PolicyDTO, Policy>();
            }).CreateMapper();

            var newPolicy = mapper.Map<PolicyDTO, Policy>(policyDTO);

            _db.Policies.Add(newPolicy);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit policy by id
        /// </summary>
        /// <param name="policyId">Policy id</param>
        /// <param name="policyDTO">Policy</param>
        public void EditPolicy(int policyId, PolicyDTO policyDTO)
        {
            if (policyDTO == null)
                throw new Exception("The policy is null.");
            if (policyId == 0)
                throw new Exception($"The policy id is 0.");

            var policy = _db.Policies.FirstOrDefault(x => x.Id == policyId);
            if (policy == null)
                throw new Exception($"The policy with id {policyId} does not exist.");

            policy.NameRus = policyDTO.NameRus;
            policy.NameEng = policyDTO.NameEng;
            policy.NameKir = policyDTO.NameKir;
            policy.DescriptionRus = policyDTO.DescriptionRus;
            policy.DescriptionEng = policyDTO.DescriptionEng;
            policy.DescriptionKir = policyDTO.DescriptionKir;

            _db.Policies.Update(policy);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete policy by id
        /// </summary>
        /// <param name="policyId">Policy id</param>
        public void DeletePolicy(int policyId)
        {
            if (policyId == 0)
                throw new Exception($"The policy id is 0.");

            var policy = _db.Policies.FirstOrDefault(x => x.Id == policyId);
            if (policy == null)
                throw new Exception($"The policy with id {policyId} does not exist.");

            _db.Policies.Remove(policy);
            _db.SaveChanges();
        }
    }
}
