using AutoMapper;
using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Courses;
using iuca.Application.ViewModels.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Courses
{
    public class StudentCourseTempService : IStudentCourseTempService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;

        public StudentCourseTempService(IApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Get student courses by registration id
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        /// <returns>Student registration courses</returns>
        public StudentCourseTempDTO GetStudentCourse(int id)
        {
            var studentCourse = _db.StudentCoursesTemp.Include(x => x.AnnouncementSection).ThenInclude(x => x.Course)
                .Include(x => x.StudentCourseRegistration)
                .FirstOrDefault(x => x.Id == id);

            if (studentCourse == null)
                throw new Exception($"Student course with id {id} not found");

            return _mapper.Map<StudentCourseTempDTO>(studentCourse);
        }

        /// <summary>
        /// Get student courses by registration id
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        /// <returns>Student registration courses</returns>
        public IEnumerable<StudentCourseTempDTO> GetStudentCoursesByRegistrationId(int id)
        {
            var studentCourses = _db.StudentCoursesTemp.Where(x => x.StudentCourseRegistrationId == id)
                .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course);
            
            return _mapper.Map<IEnumerable<StudentCourseTempDTO>>(studentCourses);
        }

        /// <summary>
        /// Get sudent registration courses by student user id
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student registration courses</returns>
        public List<StudentCourseTempDTO> GetStudentCoursesByStudentUserIdTemp(int semesterId, string studentUserId)
        {
            var studentCourses = _db.StudentCourseRegistrations
                .Where(x => x.StudentUserId == studentUserId && x.SemesterId == semesterId)
                .Include(x => x.StudentCoursesTemp)
                .ThenInclude(x => x.AnnouncementSection)
                .ThenInclude(x => x.Announcement)
                .Include(x => x.StudentCoursesTemp)
                .ThenInclude(x => x.AnnouncementSection)
                .ThenInclude(x => x.Course)
                .Include(x => x.StudentCoursesTemp)
                .ThenInclude(x => x.AnnouncementSection)
                .ThenInclude(x => x.Syllabus)
                .Include(x => x.StudentCoursesTemp)
                .ThenInclude(x => x.StudentMidterm)
                .Include(x => x.StudentCoursesTemp)
                .ThenInclude(x => x.Grade)
                .SelectMany(x => x.StudentCoursesTemp).ToList();

            return _mapper.Map<List<StudentCourseTempDTO>>(studentCourses);
        }

        /// <summary>
        /// Add course to student registration by admin
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <param name="state">Course state</param>
        /// <returns>
        /// Id of new created row
        /// </returns>
        public int AddCourseToRegistrationByAdmin(int studentCourseRegistrationId, int registrationCourseId, 
            enu_CourseState state)
        {
            if (studentCourseRegistrationId == 0)
                throw new Exception("Student course registrationId id is wrong");

            if (registrationCourseId == 0)
                throw new Exception("Registration course id is wrong");

            var selectedCourses = _db.StudentCoursesTemp
                        .Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId)
                        .ToList();

            if (selectedCourses.Any(x => x.AnnouncementSectionId == registrationCourseId))
                throw new Exception("The course already exists");

            StudentCourseTemp studentCourse = new StudentCourseTemp();
            studentCourse.AnnouncementSectionId = registrationCourseId;
            studentCourse.StudentCourseRegistrationId = studentCourseRegistrationId;
            studentCourse.DateCreated = DateTime.Now;
            studentCourse.State = (int)state;

            if (state == enu_CourseState.Regular) 
            {
                studentCourse.IsApproved = true;
                studentCourse.IsProcessed = true;
            } 
            else if (state == enu_CourseState.Added)
            {
                studentCourse.IsAddDropApproved = true;
                studentCourse.IsAddDropProcessed = true;
            }
            else if (state == enu_CourseState.Dropped)
            {
                studentCourse.IsApproved = true;
                studentCourse.IsProcessed = true;
                studentCourse.IsAddDropApproved = true;
                studentCourse.IsAddDropProcessed = true;
            }

            _db.StudentCoursesTemp.Add(studentCourse);
            _db.SaveChanges();

            return studentCourse.Id;
        }

        /// <summary>
        /// Add course to student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Registration course result</returns>
        public RegistrationCourseResultViewModel AddCourseToRegistration(int studentCourseRegistrationId, int registrationCourseId)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == studentCourseRegistrationId);
            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            if (registration.State != (int)enu_RegistrationState.NotSent &&
                registration.State != (int)enu_RegistrationState.NotApproved)
                throw new Exception("Courses cannot be changed");

            if (registrationCourseId == 0)
                throw new Exception("Course id is wrong");

            var selectedCourses = _db.StudentCoursesTemp
                        .Include(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                        .Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId)
                        .ToList();

            if (selectedCourses.Any(x => x.AnnouncementSectionId == registrationCourseId))
                throw new Exception("The course already exists");

            var section = _db.AnnouncementSections.Include(x => x.Announcement).FirstOrDefault(x => x.Id == registrationCourseId);
            if (section == null)
                throw new Exception("Announcement section not found");

            if (section.Announcement.IsForAll) 
            {
                if (selectedCourses.Any(x => x.AnnouncementSection.Announcement.IsForAll))
                    throw new Exception("Student has already taken elective course");
            }

            var registrationCourse = _db.AnnouncementSections.Include(x => x.Course).FirstOrDefault(x => x.Id == registrationCourseId);

            StudentCourseTemp studentCourse = new StudentCourseTemp();
            studentCourse.AnnouncementSection = registrationCourse;
            studentCourse.StudentCourseRegistrationId = studentCourseRegistrationId;
            studentCourse.AnnouncementSectionId = registrationCourseId;
            studentCourse.DateCreated = DateTime.Now;

            _db.StudentCoursesTemp.Add(studentCourse);
            _db.SaveChanges();

            var result = GetCourseQueueAfterAdding(registrationCourseId, registration.SemesterId, registrationCourse.Places);


            return result;
        }

        /// <summary>
        /// Add courses from study card to student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="studyCardId">Study card id</param>
        public void AddAllCoursesFromStudyCardToRegistration(int studentCourseRegistrationId, int studyCardId)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == studentCourseRegistrationId);
            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            if (registration.State != (int)enu_RegistrationState.NotSent &&
                registration.State != (int)enu_RegistrationState.NotApproved)
                throw new Exception("Courses cannot be changed");

            var selectedCourses = _db.StudentCoursesTemp
                        .Include(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                        .Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId)
                        .ToList();

            var studyCardCourses = _db.StudyCardCourses.Include(x => x.AnnouncementSection)
                .ThenInclude(x => x.Announcement)
                .Where(x => x.StudyCardId == studyCardId).ToList();

            foreach (var studyCardCourse in studyCardCourses) 
            {
                if (!selectedCourses.Any(x => x.AnnouncementSectionId == studyCardCourse.AnnouncementSectionId))
                {
                    if (studyCardCourse.AnnouncementSection.Announcement.IsForAll)
                    {
                        if (selectedCourses.Any(x => x.AnnouncementSection.Announcement.IsForAll))
                            continue;
                    }

                    StudentCourseTemp studentCourse = new StudentCourseTemp();
                    studentCourse.StudentCourseRegistrationId = studentCourseRegistrationId;
                    studentCourse.AnnouncementSectionId = studyCardCourse.AnnouncementSectionId;
                    studentCourse.DateCreated = DateTime.Now;

                    _db.StudentCoursesTemp.Add(studentCourse);
                }
            }
            _db.SaveChanges();
        }

        private RegistrationCourseResultViewModel GetCourseQueueAfterAdding(int registrationCourseId, int semesterId, int places) 
        {
            RegistrationCourseResultViewModel model = new RegistrationCourseResultViewModel();
            int queue = 0;
            var studentCourses = _db.StudentCoursesTemp.Include(x => x.StudentCourseRegistration)
                                    .Where(x => x.AnnouncementSectionId == registrationCourseId &&
                                        x.StudentCourseRegistration.SemesterId == semesterId 
                                        && !(x.State == (int)enu_CourseState.Dropped && x.IsAddDropApproved))
                                    .ToList();

            int restPlaces = places - studentCourses.Count;
            if (restPlaces < 0)
            {
                queue = restPlaces * (-1);
                restPlaces = 0;
            }

            model.Queue = queue;
            model.RestPlaces = restPlaces;

            return model;
        }

        /// <summary>
        /// Remove course from student registration
        /// </summary>
        /// <param name="studentCourseId">Student course  id</param>
        public void RemoveCourseFromRegistrationByAdmin(int studentCourseId)
        {
            if (studentCourseId == 0)
                throw new Exception("Course id is wrong");

            var studentCourse = _db.StudentCoursesTemp.FirstOrDefault(x => x.Id == studentCourseId);
            if (studentCourse == null)
                throw new Exception("Student course not found");

            _db.StudentCoursesTemp.Remove(studentCourse);
            _db.SaveChanges();
        }

        /// <summary>
        /// Remove course from student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        public RegistrationCourseResultViewModel RemoveCourseFromRegistration(int studentCourseRegistrationId, int registrationCourseId)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == studentCourseRegistrationId);
            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            if (registration.State != (int)enu_RegistrationState.NotSent &&
                registration.State != (int)enu_RegistrationState.NotApproved)
                throw new Exception("Courses cannot be changed");

            if (registrationCourseId == 0)
                throw new Exception("Course id is wrong");

            var registrationCourse = _db.AnnouncementSections.Include(x => x.Course).FirstOrDefault(x => x.Id == registrationCourseId);
            if (registrationCourse == null)
                throw new Exception("Registration course is null");

            var similarCourses = _db.AnnouncementSections.Include(x => x.Course)
                .Where(x => x.InstructorUserId == registrationCourse.InstructorUserId
                    && x.CourseId == registrationCourse.CourseId && x.Credits == registrationCourse.Credits).ToList();
            
            foreach (var similarCourse in similarCourses)
            {
                var course = _db.StudentCoursesTemp.FirstOrDefault(x => x.StudentCourseRegistrationId == studentCourseRegistrationId &&
                x.AnnouncementSectionId == similarCourse.Id);
                if (course != null)
                {
                    _db.StudentCoursesTemp.Remove(course);
                    break;
                }
            }

            _db.SaveChanges();

            //Get rest places
            var countStudents = _db.StudentCoursesTemp.Include(x => x.StudentCourseRegistration)
                                    .Count(x => x.AnnouncementSectionId == registrationCourseId &&
                                    x.StudentCourseRegistration.SemesterId == registration.SemesterId 
                                    && x.State != (int)enu_CourseState.Dropped);

            RegistrationCourseResultViewModel model = new RegistrationCourseResultViewModel();
            model.RestPlaces = registrationCourse.Places - countStudents;
            if (model.RestPlaces < 0)
                model.RestPlaces = 0;

            return model;
        }

        /// <summary>
        /// Drop course from student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        public void DropCourseFromRegistration(int studentCourseRegistrationId, int registrationCourseId)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == studentCourseRegistrationId);
            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            if (registration.AddDropState != (int)enu_RegistrationState.NotSent &&
                registration.AddDropState != (int)enu_RegistrationState.NotApproved)
                throw new Exception("Courses cannot be changed");

            if (registrationCourseId == 0)
                throw new Exception("Course id is wrong");

            var registrationCourse = _db.AnnouncementSections
                .Include(x => x.Course)
                .Include(x => x.StudentCourses)
                .ThenInclude(x => x.StudentCourseRegistration)
                .FirstOrDefault(x => x.Id == registrationCourseId);
            if (registrationCourse == null)
                throw new Exception("Registration course is null");

            if (registration.OrganizationId == 1) 
            {
                var students = registrationCourse.StudentCourses.Where(x => x.State != (int)enu_CourseState.Dropped &&
                    ((x.StudentCourseRegistration.State == (int)enu_RegistrationState.Submitted &&
                    x.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.NotSent) ||
                    x.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.Submitted)).ToList();

                if (students.Count <= 8)
                    throw new ModelValidationException("На этот курс записано мало студентов. " +
                        "Для отмены регистрации вам необходимо получить согласие руководителя вашей программы." +
                        "This course has low number of registered students. If you want to drop this " +
                        "course please contact your program head for approval.", "");
            }

            var similarCourses = _db.AnnouncementSections.Include(x => x.Course)
                .Where(x => x.InstructorUserId == registrationCourse.InstructorUserId
                    && x.CourseId == registrationCourse.CourseId).ToList();

            foreach (var similarCourse in similarCourses)
            {
                var course = _db.StudentCoursesTemp.FirstOrDefault(x => x.StudentCourseRegistrationId == studentCourseRegistrationId &&
                        x.AnnouncementSectionId == similarCourse.Id);
                if (course != null)
                {
                    course.State = (int)enu_CourseState.Dropped;
                    _db.StudentCoursesTemp.Update(course);
                    break;
                }
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// Return dropped course to student registration
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        public void ReturnDroppedCourse(int studentCourseRegistrationId, int registrationCourseId)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == studentCourseRegistrationId);
            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            if (registration.AddDropState != (int)enu_RegistrationState.NotSent &&
                registration.AddDropState != (int)enu_RegistrationState.NotApproved)
                throw new Exception("Courses cannot be changed");

            if (registrationCourseId == 0)
                throw new Exception("Course id is wrong");

            var course = _db.StudentCoursesTemp.Include(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                .FirstOrDefault(x => x.StudentCourseRegistrationId == studentCourseRegistrationId &&
                        x.AnnouncementSectionId == registrationCourseId);

            if (course == null)
                throw new Exception("Registration course is null");

            if (course.AnnouncementSection.Announcement.IsForAll) 
            {
                if (_db.StudentCoursesTemp.Include(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                    .Any(x => x.StudentCourseRegistrationId == studentCourseRegistrationId && 
                        x.AnnouncementSectionId != registrationCourseId && x.AnnouncementSection.Announcement.IsForAll)) 
                {
                    throw new Exception("Student has taken elective course");
                }
            }

            course.State = (int)enu_CourseState.Regular;
            course.IsAddDropApproved = true;

            _db.StudentCoursesTemp.Update(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// Add new course to student registration while add/drop period
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Registration course result</returns>
        public void AddNewCourseToRegistration(int studentCourseRegistrationId, int registrationCourseId)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == studentCourseRegistrationId);
            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            if (registration.AddDropState != (int)enu_RegistrationState.NotSent &&
                registration.AddDropState != (int)enu_RegistrationState.NotApproved)
                throw new Exception("Courses cannot be changed");

            if (registrationCourseId == 0)
                throw new Exception("Course id is wrong");

            var selectedCourses = _db.StudentCoursesTemp
                        .Include(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                        .Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId)
                        .ToList();

            var section = selectedCourses.FirstOrDefault(x => x.AnnouncementSectionId == registrationCourseId);
            if (section != null)
                throw new Exception("The course already exists");

            var registrationCourse = _db.AnnouncementSections.Include(x => x.Announcement).Include(x => x.Course)
                .FirstOrDefault(x => x.Id == registrationCourseId);

            if (registrationCourse.Announcement.IsForAll)
            {
                if (selectedCourses.Any(x => x.State != (int)enu_CourseState.Dropped && 
                    x.AnnouncementSection.Announcement.IsForAll))
                    throw new Exception("Student has already taken elective course");
            }

            StudentCourseTemp studentCourse = new StudentCourseTemp();
            studentCourse.AnnouncementSection = registrationCourse;
            studentCourse.StudentCourseRegistrationId = studentCourseRegistrationId;
            studentCourse.AnnouncementSectionId = registrationCourseId;
            studentCourse.State = (int)enu_CourseState.Added;
            studentCourse.DateCreated = DateTime.Now;

            _db.StudentCoursesTemp.Add(studentCourse);
            _db.SaveChanges();
        }

        /// <summary>
        /// Remove added course from student registration for add/drop perdiod
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        public void RemoveAddedCourseFromRegistration(int studentCourseRegistrationId, int registrationCourseId)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == studentCourseRegistrationId);
            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            if (registration.AddDropState != (int)enu_RegistrationState.NotSent &&
                registration.AddDropState != (int)enu_RegistrationState.NotApproved)
                throw new Exception("Courses cannot be changed");

            if (registrationCourseId == 0)
                throw new Exception("Course id is wrong");

            var registrationCourse = _db.AnnouncementSections.Include(x => x.Course).FirstOrDefault(x => x.Id == registrationCourseId);
            if (registrationCourse == null)
                throw new Exception("Registration course is null");

            var similarCourses = _db.AnnouncementSections.Include(x => x.Course)
                .Where(x => x.InstructorUserId == registrationCourse.InstructorUserId
                    && x.CourseId == registrationCourse.CourseId && x.Credits == registrationCourse.Credits).ToList();

            foreach (var similarCourse in similarCourses)
            {
                var course = _db.StudentCoursesTemp.FirstOrDefault(x => x.StudentCourseRegistrationId == studentCourseRegistrationId &&
                        x.AnnouncementSectionId == similarCourse.Id);
                if (course != null)
                {
                    _db.StudentCoursesTemp.Remove(course);
                    break;
                }
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// Remove new course from student registration while add/drop period
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Registration course result</returns>
        public void RemoveNewCourseFromRegistration(int studentCourseRegistrationId, int registrationCourseId)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == studentCourseRegistrationId);
            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            if (registration.State != (int)enu_RegistrationState.NotSent &&
                registration.State != (int)enu_RegistrationState.NotApproved)
                throw new Exception("Courses cannot be changed");

            if (registrationCourseId == 0)
                throw new Exception("Course id is wrong");

            var registrationCourse = _db.AnnouncementSections.Include(x => x.Course).FirstOrDefault(x => x.Id == registrationCourseId);
            if (registrationCourse == null)
                throw new Exception("Registration course is null");

            var course = _db.StudentCoursesTemp.FirstOrDefault(x => x.StudentCourseRegistrationId == studentCourseRegistrationId &&
                        x.AnnouncementSectionId == registrationCourseId);
            if (course == null)
                throw new Exception("Registration course is null");

            _db.StudentCoursesTemp.Remove(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// Make course aprroved or disapproved according to flag
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <param name="approve">True - approve, False - disapprove</param>
        public void ApproveCourse(int studentCourseRegistrationId, int registrationCourseId, bool approve)
        {
            if (studentCourseRegistrationId == 0)
                throw new Exception("Registration id is wrong");

            if (registrationCourseId == 0)
                throw new Exception("Course id is wrong");

            var course = _db.StudentCoursesTemp.FirstOrDefault(x => x.StudentCourseRegistrationId == studentCourseRegistrationId &&
                    x.AnnouncementSectionId == registrationCourseId);
            if (course == null)
                throw new Exception("Указанный курс не найден");

            course.IsApproved = approve;

            _db.StudentCoursesTemp.Update(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// Comment course
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <param name="comment">Comment</param>
        public void CommentCourse(int studentCourseRegistrationId, int registrationCourseId, string comment)
        {
            if (studentCourseRegistrationId == 0)
                throw new Exception("Registration id is wrong");

            if (registrationCourseId == 0)
                throw new Exception("Course id is wrong");

            var course = _db.StudentCoursesTemp.FirstOrDefault(x => x.StudentCourseRegistrationId == studentCourseRegistrationId &&
                    x.AnnouncementSectionId == registrationCourseId);
            if (course == null)
                throw new Exception("Указанный курс не найден");

            course.Comment = comment;

            _db.StudentCoursesTemp.Update(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// Mark student course deleted flag
        /// </summary>
        /// <param name="studentCourseId">Student course id</param>
        /// <param name="isDeleted">Is deleted</param>
        public void MarkDeleted(int studentCourseId, bool isDeleted)
        {
            if (studentCourseId == 0)
                throw new Exception("Student course id is wrong");

            var course = _db.StudentCoursesTemp.FirstOrDefault(x => x.Id == studentCourseId);
            if (course == null)
                throw new Exception("Course is not found");

            course.MarkedDeleted = isDeleted;

            _db.StudentCoursesTemp.Update(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// Mark student course audit flag
        /// </summary>
        /// <param name="studentCourseId">Student course id</param>
        /// <param name="isAudit">Is audit</param>
        public void MarkAudit(int studentCourseId, bool isAudit)
        {
            if (studentCourseId == 0)
                throw new Exception("Student course id is wrong");

            var course = _db.StudentCoursesTemp.FirstOrDefault(x => x.Id == studentCourseId);
            if (course == null)
                throw new Exception("Course is not found");

            course.IsAudit = isAudit;
            if (isAudit)
            {
                var auditGrade = _db.Grades.FirstOrDefault(x => x.GradeMark == "AU");
                if (auditGrade == null)
                    throw new Exception("Audit grade not found");

                course.GradeId = auditGrade.Id;
            }
            else
                course.GradeId = null;

            _db.StudentCoursesTemp.Update(course);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
