using AutoMapper;
using System;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Infrastructure.Persistence;
using iuca.Domain.Entities.Courses;

namespace iuca.Application.Services.Courses
{
    public class StudyCardCourseService : IStudyCardCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;

        public StudyCardCourseService(IApplicationDbContext db, 
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Create study card course
        /// </summary>
        /// <param name="studyCardCourseDTO">Study card course</param>
        public void CreateStudyCardCourse(StudyCardCourseDTO studyCardCourseDTO)
        {
            if (studyCardCourseDTO == null)
                throw new Exception("The study card course is null.");

            StudyCardCourse newStudyCardCourse = _mapper.Map<StudyCardCourse>(studyCardCourseDTO);

            _db.StudyCardCourses.Add(newStudyCardCourse);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit study card course by id
        /// </summary>
        /// <param name="studyCardCourseId">Study card course id</param>
        /// <param name="studyCardCourseDTO">Study card course</param>
        public void EditStudyCardCourse(int studyCardCourseId, StudyCardCourseDTO studyCardCourseDTO)
        {
            if (studyCardCourseDTO == null)
                throw new Exception("The study card course is null.");
            if (studyCardCourseId == 0)
                throw new Exception($"The study card course id is 0.");

            var studyCardCourse = _db.StudyCardCourses.Find(studyCardCourseId);
            if (studyCardCourse == null)
                throw new Exception($"The study card course with id {studyCardCourseId} does not exist.");

            studyCardCourse.Comment = studyCardCourseDTO.Comment;
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete study card course by id
        /// </summary>
        /// <param name="studyCardCourseId">Study card course id</param>
        public void DeleteStudyCardCourse(int studyCardCourseId)
        {
            if (studyCardCourseId == 0)
                throw new Exception($"The study card course id is 0.");

            var studyCardCourse = _db.StudyCardCourses.Find(studyCardCourseId);
            if (studyCardCourse == null)
                throw new Exception($"The study card course with id {studyCardCourseId} does not exist.");

            _db.StudyCardCourses.Remove(studyCardCourse);
            _db.SaveChanges();
        }
    }
}
