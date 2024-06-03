using AutoMapper;
using System;
using System.Linq;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;

namespace iuca.Application.Services.Courses
{
    public class CourseCalendarRowService : ICourseCalendarRowService
    {
        private readonly IApplicationDbContext _db;

        public CourseCalendarRowService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Create course calendar row
        /// </summary>
        /// <param name="courseCalendarRowDTO">Course calendar row</param>
        public void CreateCourseCalendarRow(CourseCalendarRowDTO courseCalendarRowDTO)
        {
            if (courseCalendarRowDTO == null)
                throw new Exception("The course calendar row is null.");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<CourseCalendarRowDTO, CourseCalendarRow>();
            }).CreateMapper();

            var newCourseCalendarRow = mapper.Map<CourseCalendarRowDTO, CourseCalendarRow>(courseCalendarRowDTO);

            _db.CourseCalendar.Add(newCourseCalendarRow);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit course calendar row by id
        /// </summary>
        /// <param name="courseCalendarRowId">Course calendar row id</param>
        /// <param name="courseCalendarRowDTO">Course calendar row</param>
        public void EditCourseCalendarRow(int courseCalendarRowId, CourseCalendarRowDTO courseCalendarRowDTO)
        {
            if (courseCalendarRowDTO == null)
                throw new Exception("The course calendar row is null.");
            if (courseCalendarRowId == 0)
                throw new Exception($"The course calendar row id is 0.");

            var courseCalendarRow = _db.CourseCalendar
                .FirstOrDefault(x => x.Id == courseCalendarRowId);
            if (courseCalendarRow == null)
                throw new Exception($"The course calendar row with id {courseCalendarRowId} does not exist.");

            courseCalendarRow.SyllabusId = courseCalendarRowDTO.SyllabusId;
            courseCalendarRow.Week = courseCalendarRowDTO.Week;
            courseCalendarRow.Date = courseCalendarRowDTO.Date;
            courseCalendarRow.Topics = courseCalendarRowDTO.Topics;
            courseCalendarRow.Assignments = courseCalendarRowDTO.Assignments;
            
            _db.CourseCalendar.Update(courseCalendarRow);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete course calendar row by id
        /// </summary>
        /// <param name="courseCalendarRowId">Course calendar row id</param>
        public void DeleteCourseCalendarRow(int courseCalendarRowId)
        {
            if (courseCalendarRowId == 0)
                throw new Exception($"The course calendar row id is 0.");

            var courseCalendarRow = _db.CourseCalendar
                .FirstOrDefault(x => x.Id == courseCalendarRowId);
            if (courseCalendarRow == null)
                throw new Exception($"The course calendar row with id {courseCalendarRowId} does not exist.");

            _db.CourseCalendar.Remove(courseCalendarRow);
            _db.SaveChanges();
        }
    }
}
