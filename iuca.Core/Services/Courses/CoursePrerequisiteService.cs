using AutoMapper;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Courses
{
    public class CoursePrerequisiteService : ICoursePrerequisiteService
    {
        private readonly IApplicationDbContext _db;

        public CoursePrerequisiteService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Create course prerequisite record
        /// </summary>
        /// <param name="coursePrerequisiteDTO">Course prerequisite model</param>
        public void Create(CoursePrerequisiteDTO coursePrerequisiteDTO)
        {
            if (coursePrerequisiteDTO == null)
                throw new Exception("coursePrerequisiteDTO is null");

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<CourseDTO, Course>();
                cfg.CreateMap<CoursePrerequisiteDTO, CoursePrerequisite>();
                }
            ).CreateMapper();

            CoursePrerequisite newCoursePrerequisite = mapperFromDTO.Map<CoursePrerequisiteDTO, CoursePrerequisite>(coursePrerequisiteDTO);

            _db.CoursePrerequisites.Add(newCoursePrerequisite);
            _db.SaveChanges();

        }

        /// <summary>
        /// Edit course prerequisite by id
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <param name="prerequisiteId">Course prerequisite id</param>
        /// <param name="coursePrerequisiteDTO">Course prerequisite model</param>
        public void Edit(int courseId, int prerequisiteId, CoursePrerequisiteDTO coursePrerequisiteDTO)
        {
            if (coursePrerequisiteDTO == null)
                throw new Exception("CoursePrerequisiteDTO is null");

            CoursePrerequisite coursePrerequisite = _db.CoursePrerequisites.FirstOrDefault(x => x.CourseId == courseId && x.PrerequisiteId == prerequisiteId);
            if (coursePrerequisite == null)
                throw new Exception($"CoursePrerequisiteDTO not found");

            coursePrerequisite.PrerequisiteId = coursePrerequisiteDTO.PrerequisiteId;

            _db.CoursePrerequisites.Update(coursePrerequisite);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete course prerequisite by id
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <param name="prerequisiteId">Course prerequisite id</param>
        public void Delete(int courseId, int prerequisiteId)
        {
            CoursePrerequisite coursePrerequisite = 
                _db.CoursePrerequisites.FirstOrDefault(x => x.CourseId == courseId && x.PrerequisiteId == prerequisiteId);
            if (coursePrerequisite == null)
                throw new Exception($"CoursePrerequisite not found");

            _db.CoursePrerequisites.Remove(coursePrerequisite);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
