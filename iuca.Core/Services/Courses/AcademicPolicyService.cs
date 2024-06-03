using AutoMapper;
using System;
using System.Linq;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;

namespace iuca.Application.Services.Courses
{
    public class AcademicPolicyService : IAcademicPolicyService
    {
        private readonly IApplicationDbContext _db;

        public AcademicPolicyService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Create academic policy
        /// </summary>
        /// <param name="academicPolicyDTO">Academic policy</param>
        public void CreateAcademicPolicy(AcademicPolicyDTO academicPolicyDTO)
        {
            if (academicPolicyDTO == null)
                throw new Exception("The academic policy is null.");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<AcademicPolicyDTO, AcademicPolicy>();
            }).CreateMapper();

            var newAcademicPolicy = mapper.Map<AcademicPolicyDTO, AcademicPolicy>(academicPolicyDTO);

            _db.AcademicPolicies.Add(newAcademicPolicy);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit academic policy by id
        /// </summary>
        /// <param name="academicPolicyId">Academic policy id</param>
        /// <param name="academicPolicyDTO">Academic policy</param>
        public void EditAcademicPolicy(int academicPolicyId, AcademicPolicyDTO academicPolicyDTO)
        {
            if (academicPolicyDTO == null)
                throw new Exception("The academic policy is null.");
            if (academicPolicyId == 0)
                throw new Exception($"The academic policy id is 0.");

            var academicPolicy = _db.AcademicPolicies.FirstOrDefault(x => x.Id == academicPolicyId);
            if (academicPolicy == null)
                throw new Exception($"The academic policy with id {academicPolicyId} does not exist.");

            academicPolicy.SyllabusId = academicPolicyDTO.SyllabusId;
            academicPolicy.Name = academicPolicyDTO.Name;
            academicPolicy.Description = academicPolicyDTO.Description;

            _db.AcademicPolicies.Update(academicPolicy);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete academic policy by id
        /// </summary>
        /// <param name="academicPolicyId">Academic policy id</param>
        public void DeleteAcademicPolicy(int academicPolicyId)
        {
            if (academicPolicyId == 0)
                throw new Exception($"The academic policy id is 0.");

            var academicPolicy= _db.AcademicPolicies.FirstOrDefault(x => x.Id == academicPolicyId);
            if (academicPolicy == null)
                throw new Exception($"The academic policy with id {academicPolicyId} does not exist.");

            _db.AcademicPolicies.Remove(academicPolicy);
            _db.SaveChanges();
        }
    }
}
