using AutoMapper;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Courses
{
    public class StudentCourseService : IStudentCourseService
    {
        private readonly IApplicationDbContext _db;

        public StudentCourseService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get student courses by registration id
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        /// <returns>Student registration courses</returns>
        public IEnumerable<StudentCourseDTO> GetStudentCoursesByRegistrationId(int id)
        {
            var studentCourses = _db.StudentCourses.Where(x => x.StudentCourseRegistrationId == id)
                .Include(x => x.OldStudyCardCourse).ThenInclude(x => x.CyclePartCourse).ThenInclude(x => x.Course)
                .Include(x => x.OldStudyCardCourse);
            
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Course, CourseDTO>();
                cfg.CreateMap<CyclePartCourse, CyclePartCourseDTO>();
                cfg.CreateMap<OldStudyCardCourse, OldStudyCardCourseDTO>();
                cfg.CreateMap<StudentCourse, StudentCourseDTO>();
            }).CreateMapper();
            return mapper.Map<IEnumerable<StudentCourse>, IEnumerable<StudentCourseDTO>>(studentCourses);
        }

        /// <summary>
        /// Add course to student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardCourseId">Study card course id</param>
        public void AddCourseToRegistration(int studentCourseRegistrationId, int studyCardCourseId)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == studentCourseRegistrationId);
            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            if (studyCardCourseId == 0)
                throw new Exception("Course id is wrong");

            var selectedCourses = _db.StudentCourses
                        .Include(x => x.OldStudyCardCourse)
                        .ThenInclude(x => x.CyclePartCourse)
                        .Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId)
                        .ToList();

            var studyCardCourse = _db.OldStudyCardCourses.Include(x => x.CyclePartCourse).FirstOrDefault(x => x.Id == studyCardCourseId);
            
            if (selectedCourses.Any(x => x.OldStudyCardCourse.CyclePartCourse.CourseId == studyCardCourse.CyclePartCourse.CourseId && 
                    x.OldStudyCardCourse.InstructorUserId == studyCardCourse.InstructorUserId))
                throw new Exception("Текущий курс уже был добавлен");

            StudentCourse studentCourse = new StudentCourse();
            studentCourse.StudentCourseRegistrationId = studentCourseRegistrationId;
            studentCourse.StudyCardCourseId = studyCardCourseId;
            studentCourse.Queue = GetCourseQueue(studyCardCourseId, registration.SemesterId, studyCardCourse.Places);

            _db.StudentCourses.Add(studentCourse);
            _db.SaveChanges();

        }

        private int GetCourseQueue(int studyCardCourseId, int semesterId, int places) 
        {
            int queue = 0;
            var addedByOtherStudents = _db.StudentCourses.Include(x => x.StudentCourseRegistration)
                                    .Where(x => x.StudyCardCourseId == studyCardCourseId &&
                                    x.StudentCourseRegistration.SemesterId == semesterId).ToList();

            int restPlaces = addedByOtherStudents.Count;

            if (places - restPlaces == 0)
                queue = 1;
            else if (places - restPlaces < 0)
            {
                var lastCourseInQueue = addedByOtherStudents.OrderByDescending(x => x.Queue).FirstOrDefault();
                if (lastCourseInQueue != null)
                    queue = lastCourseInQueue.Queue + 1;
            }

            return queue;
        }

        /// <summary>
        /// Remove course from student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardCourseId">Study card course id</param>
        public void RemoveCourseFromRegistration(int studentCourseRegistrationId, int studyCardCourseId)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == studentCourseRegistrationId);
            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            if (studyCardCourseId == 0)
                throw new Exception("Course id is wrong");

            var studyCardCourse = _db.OldStudyCardCourses.Include(x => x.CyclePartCourse).FirstOrDefault(x => x.Id == studyCardCourseId);
            if (studyCardCourse == null)
                throw new Exception("studyCardCourse is null");

            var similarCourses = _db.OldStudyCardCourses.Include(x => x.CyclePartCourse)
                .Where(x => x.InstructorUserId == studyCardCourse.InstructorUserId
                    && x.CyclePartCourse.CourseId == studyCardCourse.CyclePartCourse.CourseId).ToList();

            int removingCourseQueue = 0;
            foreach (var similarCourse in similarCourses) 
            {
                var course = _db.StudentCourses.FirstOrDefault(x => x.StudentCourseRegistrationId == studentCourseRegistrationId &&
                        x.StudyCardCourseId == similarCourse.Id);
                if (course != null) 
                {
                    removingCourseQueue = course.Queue;
                    _db.StudentCourses.Remove(course);
                    break;
                }
            }
            _db.SaveChanges();
            UpdateCoursesQueue(registration.SemesterId, studyCardCourseId, removingCourseQueue);
        }

        private void UpdateCoursesQueue(int semesterId, int studyCardCourseId, int removingCourseQueue) 
        {
            var coursesWithQueue = _db.StudentCourses.Include(x => x.StudentCourseRegistration)
                    .Where(x => x.StudentCourseRegistration.SemesterId == semesterId &&
                    x.StudyCardCourseId == studyCardCourseId && x.Queue > 0).OrderBy(x => x.Queue).ToList();

            //If removing course had no queue all courses with queue have to be updated from 0 else from 1
            int queue = 0;
            if (removingCourseQueue > 0)
                queue = 1;

            for (int i = 0; i < coursesWithQueue.Count; i++)
            {
                coursesWithQueue[i].Queue = i + queue;
                _db.StudentCourses.Update(coursesWithQueue[i]);
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Make course aprroved or disapproved according to flag
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardCourseId">Study card course id</param>
        /// <param name="approve">True - approve, False - disapprove</param>
        public void ApproveCourse(int studentCourseRegistrationId, int studyCardCourseId, bool approve)
        {
            if (studentCourseRegistrationId == 0)
                throw new Exception("Registration id is wrong");

            if (studyCardCourseId == 0)
                throw new Exception("Course id is wrong");

            var course = _db.StudentCourses.FirstOrDefault(x => x.StudentCourseRegistrationId == studentCourseRegistrationId &&
                    x.StudyCardCourseId == studyCardCourseId);
            if (course == null)
                throw new Exception("Указанный курс не найден");

            course.IsApproved = approve;

            _db.StudentCourses.Update(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// Comment course
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardCourseId">Study card course id</param>
        /// <param name="approve">True - approve, False - disapprove</param>
        public void CommentCourse(int studentCourseRegistrationId, int studyCardCourseId, string comment)
        {
            if (studentCourseRegistrationId == 0)
                throw new Exception("Registration id is wrong");

            if (studyCardCourseId == 0)
                throw new Exception("Course id is wrong");

            var course = _db.StudentCourses.FirstOrDefault(x => x.StudentCourseRegistrationId == studentCourseRegistrationId &&
                    x.StudyCardCourseId == studyCardCourseId);
            if (course == null)
                throw new Exception("Указанный курс не найден");

            course.Comment = comment;

            _db.StudentCourses.Update(course);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
