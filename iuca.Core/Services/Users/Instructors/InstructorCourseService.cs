using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Users.Instructors;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace iuca.Application.Services.Users.Instructors
{
    public class InstructorCourseService : IInstructorCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly ISemesterPeriodService _semesterPeriodService;
        private readonly ISemesterService _semesterService;
        private readonly IStudentInfoService _studentInfoService;
        private readonly IMapper _mapper;

        public InstructorCourseService(IApplicationDbContext db,
            ApplicationUserManager<ApplicationUser> userManager,
            ISemesterPeriodService semesterPeriodService,
            ISemesterService semesterService,
            IStudentInfoService studentInfoService,
            IMapper mapper)
        {
            _db = db;
            _userManager = userManager;
            _semesterPeriodService = semesterPeriodService;
            _semesterService = semesterService;
            _studentInfoService = studentInfoService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get instructor courses for semester
        /// </summary>
        /// <param name="organizationId">organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <returns>List of instructor courses</returns>
        public List<InstructorCourseViewModel> GetInstructorCourses(int organizationId, int semesterId, 
            string instructorUserId) 
        {
            List<InstructorCourseViewModel> instructorCourses = new List<InstructorCourseViewModel>();

            var semester = _semesterService.GetSemester(organizationId, semesterId);

            string serializedInstructorUserId = JsonSerializer.Serialize(instructorUserId);

            var registrationCourses = _db.AnnouncementSections
                .Include(x => x.ExtraInstructors)
                .Include(x => x.Course)
                .Include(x => x.Announcement)
                .Include(x => x.Syllabus)
                .Where(x => x.OrganizationId == organizationId && x.Season == semester.Season && x.Year == semester.Year && x.Announcement.IsActivated &&
                    (x.InstructorUserId == instructorUserId || x.ExtraInstructors.Any(x => x.InstructorUserId == instructorUserId) ||
                    x.ExtraInstructorsJson != null && EF.Functions.JsonContains(x.ExtraInstructorsJson, serializedInstructorUserId)))
                .ToList();

            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.Grade)
                .Include(x => x.StudentCourseRegistration)
                .Where(x => registrationCourses.Select(x => x.Id).Contains(x.AnnouncementSectionId) &&
                        x.StudentCourseRegistration.SemesterId == semesterId && x.State != (int)enu_CourseState.Dropped)
                        .ToList();

            var activeStudentIds = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .Where(x => x.OrganizationId == organizationId &&
                    x.StudentBasicInfo != null && x.State == (int)enu_StudentState.Active)
                .Select(x => x.StudentBasicInfo.ApplicationUserId).ToList();

            foreach (var course in registrationCourses) 
            {
                InstructorCourseViewModel courseVM = new InstructorCourseViewModel();
                courseVM.AnnouncementSetcionId = course.Id;
                courseVM.Name = course.Course.NameEng + " \\ " +
                                course.Course.NameRus + " \\ " +
                                course.Course.NameKir;
                courseVM.Code = course.Course.Abbreviation + " " +
                                course.Course.Number;
                courseVM.ImportCode = course.Course.ImportCode;
                courseVM.Points = course.Credits;
                courseVM.Places = course.Places;
                courseVM.StudentCount = CountCourseStudents(course.Id, studentCourses, activeStudentIds);
                courseVM.Schedule = course.Schedule;
                courseVM.Section = course.Section;
                courseVM.AttendanceSpreadsheetId = course.Announcement.AttendanceSpreadsheetId;
                courseVM.InstructorUserId = instructorUserId;
                if (course.Syllabus == null)
                    courseVM.SyllabusStatus = 0;
                else
                    courseVM.SyllabusStatus = course.Syllabus.Status;

                instructorCourses.Add(courseVM);
            }

            return instructorCourses;
        }

        /// <summary>
        /// Get instructor registration course by id
        /// </summary>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Instructor course</returns>
        public InstructorCourseViewModel GetInstructorCourse(int registrationCourseId)
        {
            var announcementSection = _db.AnnouncementSections
                .Include(x => x.Course)
                .Include(x => x.Announcement)
                .Include(x => x.ExtraInstructors)
                .FirstOrDefault(x => x.Id == registrationCourseId);

            if (announcementSection == null)
                throw new Exception("Registration course not found");

            InstructorCourseViewModel courseVM = new InstructorCourseViewModel();
            courseVM.AnnouncementSetcionId = announcementSection.Id;
            courseVM.GradeSheetSubmitted = announcementSection.GradeSheetSubmitted;
            courseVM.Name = announcementSection.Course.NameEng + " \\ " +
                            announcementSection.Course.NameRus + " \\ " +
                            announcementSection.Course.NameKir;
            courseVM.Code = announcementSection.Course.Abbreviation + " " +
                            announcementSection.Course.Number;
            courseVM.ImportCode = announcementSection.Course.ImportCode;
            courseVM.Section = announcementSection.Section;
            courseVM.Points = announcementSection.Credits;
            courseVM.Places = announcementSection.Places;
            courseVM.Schedule = announcementSection.Schedule;
            courseVM.AttendanceSpreadsheetId = announcementSection.Announcement.AttendanceSpreadsheetId;
            courseVM.InstructorUserId = announcementSection.InstructorUserId;
            courseVM.InstructorName = _userManager.GetUserFullName(announcementSection.InstructorUserId);
            courseVM.ExtraInstructorIds = announcementSection.ExtraInstructors.Select(x => x.InstructorUserId).ToList();
            if (announcementSection.ExtraInstructorsJson != null)
                courseVM.ExtraInstructorIds.AddRange(announcementSection.ExtraInstructorsJson.ToList());

            return courseVM;
        }

        private int CountCourseStudents(int registrationCourseId,  
            List<StudentCourseTemp> studentCourses, List<string> activeStudentIds)
        {
            return studentCourses.Count(x => x.AnnouncementSectionId == registrationCourseId &&
                (activeStudentIds.Contains(x.StudentCourseRegistration.StudentUserId) || (x.Grade != null && x.Grade.GradeMark != "*")));
        }

        /// <summary>
        /// Get students for instructor course
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="announcementSectionId">Announcement sectionId id</param>
        /// <param name="onlyActiveStudents">Only active students</param>
        /// <returns>List of students</returns>
        public List<InstructorCourseStudentViewModel> GetInstructorCourseStudents(int organizationId, 
            int announcementSectionId, bool onlyActiveStudents) 
        {
            var studentList = new List<InstructorCourseStudentViewModel>();

            var studentCourses = GetStudentCourses(organizationId, announcementSectionId);

            var studentIds = studentCourses.Select(x => x.StudentCourseRegistration.StudentUserId).ToList();

            var studentOrgInfoes = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                .Include(x => x.PrepDepartmentGroup).ThenInclude(x => x.Department)
                .Where(x => x.OrganizationId == organizationId && studentIds.Contains(x.StudentBasicInfo.ApplicationUserId)).ToList();

            foreach (var studentOrgInfo in studentOrgInfoes)
            {
                var model = new InstructorCourseStudentViewModel();
                model.StudentUserId = studentOrgInfo.StudentBasicInfo.ApplicationUserId;

                var studentCourse = studentCourses.FirstOrDefault(x => x.StudentCourseRegistration.StudentUserId == model.StudentUserId);

                if (onlyActiveStudents)
                {
                    if (studentOrgInfo.State != (int)enu_StudentState.Active &&
                        (studentCourse.GradeId == null || studentCourse.Grade?.GradeMark == "*"))
                        continue;
                }

                model.StudentId = studentOrgInfo.StudentId;
                model.StudentStatus = ((enu_StudentState)studentOrgInfo.State).ToString();
                var major = studentOrgInfo.DepartmentGroup.Department.Code;
                if (studentOrgInfo.PrepDepartmentGroup != null)
                    major += $"/{studentOrgInfo.PrepDepartmentGroup.Department.Code}";
                model.StudentMajor = major;
                model.StudentGroup = studentOrgInfo.DepartmentGroup.Code;
                model.StudentName = _userManager.GetUserFullName(model.StudentUserId);
                model.RegistrationState = (enu_RegistrationState)studentCourse.StudentCourseRegistration.AddDropState != enu_RegistrationState.NotSent ?
                    (enu_RegistrationState)studentCourse.StudentCourseRegistration.AddDropState : (enu_RegistrationState)studentCourse.StudentCourseRegistration.State;

                studentList.Add(model);
            }

            return studentList;
        }

        /// <summary>
        /// Get student grades for instructor course
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="announcementSectionId">Announcement sectionId id</param>
        /// <returns>List of student grades</returns>
        public List<InstructorCourseStudentGradeViewModel> GetInstructorCourseStudentGrades(int organizationId, int announcementSectionId)
        {
            var studentList = new List<InstructorCourseStudentGradeViewModel>();

            var announcementSection = _db.AnnouncementSections.Include(x => x.Course)
                .ThenInclude(x => x.CoursePrerequisites)
                .FirstOrDefault(x => x.Id == announcementSectionId);

            if (announcementSection == null)
                throw new Exception("Announcement section not found");

            bool hasPrerequisites = announcementSection.Course.CoursePrerequisites.Any();

            var studentCourses = GetStudentCourses(organizationId, announcementSectionId);

            var studentIds = studentCourses.Select(x => x.StudentCourseRegistration).Select(x => x.StudentUserId).ToList();

            foreach (var studentId in studentIds)
            {
                var model = new InstructorCourseStudentGradeViewModel();
                model.StudentUserId = studentId;
                var studentCourse = studentCourses.FirstOrDefault(x => x.StudentCourseRegistration.StudentUserId == studentId
                                                && x.AnnouncementSectionId == announcementSectionId);

                var studentInfo = _db.StudentBasicInfo.Include(x => x.StudentOrgInfo).ThenInclude(x => x.DepartmentGroup)
                    .ThenInclude(x => x.Department)
                    .FirstOrDefault(x => x.ApplicationUserId == studentId);

                if (studentInfo != null)
                {
                    var studentOrgInfo = studentInfo.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId);
                    if (studentOrgInfo != null)
                    {
                        //Skip not active students for current semester
                        if (studentOrgInfo.State != (int)enu_StudentState.Active &&
                            (studentCourse.GradeId == null || studentCourse.Grade?.GradeMark == "*"))
                            continue;

                        model.StudentCourseId = studentCourse != null ? studentCourse.Id : 0;
                        model.StudentId = studentOrgInfo.StudentId;
                        model.StudentStatus = ((enu_StudentState)studentOrgInfo.State).ToString();
                        model.StudentMajor = studentOrgInfo.DepartmentGroup.Department.Code;
                        model.StudentGroup = studentOrgInfo.DepartmentGroup.Code;

                        model.GradeId = studentCourse?.GradeId;
                        model.Grade = _mapper.Map<GradeDTO>(studentCourse.Grade);
                        var user = _userManager.Users.FirstOrDefault(x => x.Id == studentId);
                        if (user != null)
                            model.StudentName = user.FullNameEng;
                        model.RegistrationState = (enu_RegistrationState)studentCourse.StudentCourseRegistration.AddDropState != enu_RegistrationState.NotSent ?
                            (enu_RegistrationState)studentCourse.StudentCourseRegistration.AddDropState : (enu_RegistrationState)studentCourse.StudentCourseRegistration.State;

                        studentList.Add(model);
                    }
                }
            }

            return studentList;
        }

        /// <summary>
        /// Set student grade for course
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentCourseId">Student course id</param>
        /// <param name="gradeId">Grade id</param>
        public void SetStudentGrade(int organizationId, int studentCourseId, int? gradeId) 
        {
            var gradingPeriod = _semesterPeriodService.GetSemesterPeriod(organizationId, (int)enu_Period.Grading, DateTime.Now);

            if (gradingPeriod == null)
                throw new Exception("Grading period is closed!");

            var studentCourse = _db.StudentCoursesTemp
                .Include(x => x.AnnouncementSection)
                .Include(x => x.Grade)
                .FirstOrDefault(x => x.Id == studentCourseId);

            if (studentCourse == null)
                throw new Exception("Student course not found!");

            if (studentCourse.AnnouncementSection.GradeSheetSubmitted)
                throw new Exception("Grade cannot be changed!");

            List<string> notAllowedToChange = new List<string> { "AU", "I", "W", "X" };

            if (studentCourse.Grade != null && notAllowedToChange.Contains(studentCourse.Grade.GradeMark))
                throw new Exception($"Grade {studentCourse.Grade.GradeMark} cannot be changed!");

            studentCourse.GradeId = gradeId;
            _db.SaveChanges();
        }

        private List<StudentCourseTemp> GetStudentCourses(int organizationId, int registrationCourseId) 
        {
            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.AnnouncementSection)
                .Include(x => x.StudentCourseRegistration)
                .Include(x => x.Grade)
                .Where(x => x.AnnouncementSection.OrganizationId == organizationId &&
                        x.AnnouncementSectionId == registrationCourseId && x.State != (int)enu_CourseState.Dropped)
                .ToList();

            return studentCourses;
        }

        /// <summary>
        /// Submit or unsubmit grade sheet
        /// </summary>
        /// <param name="announcementSectionId">Announcement setction id</param>
        /// <param name="submit">Submitted flag</param>
        public void SetGradeSheetSubmitted(int announcementSectionId, bool submit) 
        {
            var section = _db.AnnouncementSections.Include(x => x.StudentCourses)
                .ThenInclude(x => x.StudentCourseRegistration)
                .FirstOrDefault(x => x.Id == announcementSectionId);

            if (section == null)
                throw new Exception($"Student course with id {announcementSectionId} not found");

            if (submit) 
            {
                var activeStudentIds = _studentInfoService.GetStudents(section.OrganizationId, true)
                .Select(x => x.Id).ToList();

                var noGradeCourses = section.StudentCourses
                .Where(x => (x.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.Submitted ||
                    x.StudentCourseRegistration.State == (int)enu_RegistrationState.Submitted &&
                    x.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.NotSent) &&
                    activeStudentIds.Contains(x.StudentCourseRegistration.StudentUserId) &&
                    x.State != (int)enu_CourseState.Dropped && x.GradeId == null).ToList();

                if (noGradeCourses.Count > 0)
                    throw new ModelValidationException("Все оценки должны быть выставлены / All grades must be assigned", "");
            } 

            section.GradeSheetSubmitted = submit;
            _db.SaveChanges();
        }
    }

}
