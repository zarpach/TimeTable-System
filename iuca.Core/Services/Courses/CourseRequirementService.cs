using AutoMapper;
using System;
using System.Linq;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;

namespace iuca.Application.Services.Courses
{
    public class CourseRequirementService : ICourseRequirementService
    {
        private readonly IApplicationDbContext _db;

        public CourseRequirementService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Create course requirement
        /// </summary>
        /// <param name="courseRequirementDTO">Course requirement</param>
        public void CreateCourseRequirement(CourseRequirementDTO courseRequirementDTO)
        {
            if (courseRequirementDTO == null)
                throw new Exception("The course requirement is null.");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<CourseRequirementDTO, CourseRequirement>();
            }).CreateMapper();

            var newCourseRequirement = mapper.Map<CourseRequirementDTO, CourseRequirement>(courseRequirementDTO);

            _db.CourseRequirements.Add(newCourseRequirement);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit course requirement by id
        /// </summary>
        /// <param name="courseRequirementId">Course requirement id</param>
        /// <param name="courseRequirementDTO">Course requirement</param>
        public void EditCourseRequirement(int courseRequirementId, CourseRequirementDTO courseRequirementDTO)
        {
            if (courseRequirementDTO == null)
                throw new Exception("The course requirement is null.");
            if (courseRequirementId == 0)
                throw new Exception($"The course requirement id is 0.");

            var courseRequirement = _db.CourseRequirements
                .FirstOrDefault(x => x.Id == courseRequirementId);
            if (courseRequirement == null)
                throw new Exception($"The course requirement with id {courseRequirementId} does not exist.");

            courseRequirement.SyllabusId = courseRequirementDTO.SyllabusId;
            courseRequirement.Name = courseRequirementDTO.Name;
            courseRequirement.Description = courseRequirementDTO.Description;
            courseRequirement.Points = courseRequirementDTO.Points;

            _db.CourseRequirements.Update(courseRequirement);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete course requirement by id
        /// </summary>
        /// <param name="courseRequirementId">Course requirement id</param>
        public void DeleteCourseRequirement(int courseRequirementId)
        {
            if (courseRequirementId == 0)
                throw new Exception($"The course requirement id is 0.");

            var courseRequirement = _db.CourseRequirements
                .FirstOrDefault(x => x.Id == courseRequirementId);
            if (courseRequirement == null)
                throw new Exception($"The course requirement with id {courseRequirementId} does not exist.");

            _db.CourseRequirements.Remove(courseRequirement);
            _db.SaveChanges();
        }
    }
}
