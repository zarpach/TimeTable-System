using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Courses
{
    public class StudentMidtermService: IStudentMidtermService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IAdviserStudentService _adviserStudentService;
        private readonly ISemesterService _semesterService;
        private readonly IStudentInfoService _studentInfoService;

        public StudentMidtermService(IApplicationDbContext db,
            IMapper mapper,
            ApplicationUserManager<ApplicationUser> userManager,
            IAdviserStudentService adviserStudentService,
            ISemesterService semesterService,
            IStudentInfoService studentInfoService)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _adviserStudentService = adviserStudentService;
            _semesterService = semesterService;
            _studentInfoService = studentInfoService;
        }

        /// <summary>
        /// Get student midterms
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="registrationCourseId">Registration course Id</param>
        /// <returns>Student midterm view model list</returns>
        public List<StudentMidtermViewModel> GetStudentMidterms(int organizationId, int registrationCourseId)
        {
            List<StudentMidtermViewModel> studentList = new List<StudentMidtermViewModel>();

            var registrationCourse = _db.AnnouncementSections.Include(x => x.Course)
                .FirstOrDefault(x => x.Id == registrationCourseId);

            if (registrationCourse == null)
                throw new Exception("Registration course not found");

            var studentCourses = GetStudentCourses(registrationCourseId);
            var studentIds = studentCourses.Select(x => x.StudentCourseRegistration.StudentUserId).ToList();

            foreach (var studentId in studentIds)
            {
                StudentMidtermViewModel model = new StudentMidtermViewModel();
                var studentCourse = studentCourses.FirstOrDefault(x => x.StudentCourseRegistration.StudentUserId == studentId
                                                && x.AnnouncementSectionId == registrationCourseId);

                var studentInfo = _db.StudentBasicInfo.Include(x => x.StudentOrgInfo).ThenInclude(x => x.DepartmentGroup)
                    .ThenInclude(x => x.Department)
                    .FirstOrDefault(x => x.ApplicationUserId == studentId);

                if (studentInfo != null)
                {
                    var studentOrgInfo = studentInfo.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId);
                    if (studentOrgInfo != null)
                    {
                        //Skip not active students for current semester
                        if (studentOrgInfo.State != (int)enu_StudentState.Active && studentCourse.GradeId == null)
                            continue;

                        model.StudentId = studentOrgInfo.StudentId;
                        model.StudentStatus = ((enu_StudentState)studentOrgInfo.State).ToString();
                        model.StudentMajor = studentOrgInfo.DepartmentGroup.Department.Code;
                        model.StudentGroup = studentOrgInfo.DepartmentGroup.Code;
                        var user = _userManager.Users.FirstOrDefault(x => x.Id == studentId);
                        if (user != null)
                            model.StudentName = user.FullNameEng;

                        model.StudentMidterm = new StudentMidtermDTO();
                        model.StudentMidterm.StudentCourseId = studentCourse.Id;

                        if (studentCourse.StudentMidterm != null) 
                        {
                            model.StudentMidterm.Id = studentCourse.StudentMidterm.Id;
                            model.StudentMidterm.Score = studentCourse.StudentMidterm.Score;
                            model.StudentMidterm.MaxScore = studentCourse.StudentMidterm.MaxScore;
                            model.StudentMidterm.Attention = studentCourse.StudentMidterm.Attention;
                            model.StudentMidterm.Comment = studentCourse.StudentMidterm.Comment;
                            model.StudentMidterm.Recommendation = studentCourse.StudentMidterm.Recommendation;
                            model.StudentMidterm.AdviserComment = studentCourse.StudentMidterm.AdviserComment;
                        }

                        studentList.Add(model);
                    }
                }
            }

            return studentList;
        }

        private List<StudentCourseTemp> GetStudentCourses(int registrationCourseId)
        {
            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                .Include(x => x.StudentCourseRegistration)
                .Include(x => x.StudentMidterm)
                .Where(x => x.AnnouncementSection.Announcement.IsActivated && 
                    x.AnnouncementSectionId == registrationCourseId && x.State != (int)enu_CourseState.Dropped)
                .ToList();

            return studentCourses;
        }


        /// <summary>
        /// Create student midterm
        /// </summary>
        /// <param name="studentMidtermDTO">Student midterm model</param>
        public int Create(StudentMidtermDTO studentMidtermDTO)
        {
            if (studentMidtermDTO == null)
                throw new Exception($"studentMidtermDTO is null");
            var studentMidterm = _mapper.Map<StudentMidterm>(studentMidtermDTO);

            _db.StudentMidterms.Add(studentMidterm);
            _db.SaveChanges();

            return studentMidterm.Id;
        }

        /// <summary>
        /// Edit student midterm
        /// </summary>
        /// <param name="id">Id of studentMidterm</param>
        /// <param name="studentMidtermDTO">StudentMidterm model</param>
        public void Edit(int id, StudentMidtermDTO studentMidtermDTO)
        {
            if (studentMidtermDTO == null)
                throw new Exception($"studentMidtermDTO is null");

            var studentMidterm = _db.StudentMidterms.Find(id);
            if (studentMidterm == null)
                throw new Exception($"StudentMidterm with id {id} not found");

            studentMidterm.Score = studentMidtermDTO.Score;
            studentMidterm.MaxScore = studentMidtermDTO.MaxScore;
            studentMidterm.Attention = studentMidtermDTO.Attention;
            studentMidterm.Comment = studentMidtermDTO.Comment;
            studentMidterm.Recommendation = studentMidtermDTO.Recommendation;

            _db.SaveChanges();
        }

        /// <summary>
        /// Set student midterm adviser comment
        /// </summary>
        /// <param name="id">Student midterm id</param>
        /// <param name="comment">Adviser comment</param>
        public void SetStudentMidtermAdviserComment(int id, string comment)
        {
            if (id == 0)
                throw new ArgumentException($"The student midterm id is 0.", nameof(id));

            var studentMidterm = _db.StudentMidterms.Find(id);
            if (studentMidterm == null)
                throw new ArgumentException($"The student midterm with id {id} does not exist.", nameof(id));

            studentMidterm.AdviserComment = comment;
            _db.SaveChanges();
        }

        /// <summary>
        /// Get student midterm statistics report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Report model</returns>
        public List<MidtermStatisticsReportViewModel> MidtermStatisticsReport(int organizationId, int semesterId)
        {
            var model = new List<MidtermStatisticsReportViewModel>();

            var activeStudentIds = _studentInfoService.GetStudentSelectList(organizationId, 0, 0, null, null, 0, true)
                .Select(x => x.StudentUserId);

            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course).ThenInclude(x => x.Department)
                .Include(x => x.StudentCourseRegistration)
                .Include(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                .Include(x => x.StudentMidterm)
                .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId && 
                        x.StudentCourseRegistration.SemesterId == semesterId && x.AnnouncementSection.Announcement.IsActivated &&
                        activeStudentIds.Contains(x.StudentCourseRegistration.StudentUserId) &&
                        x.State != (int)enu_CourseState.Dropped)
                .ToList();

            foreach (var department in studentCourses.GroupBy(x => x.AnnouncementSection.Course.Department))
            {
                var row = new MidtermStatisticsReportViewModel();
                row.Department = _mapper.Map<DepartmentDTO>(department.Key);
                var courses = department.GroupBy(x => x.AnnouncementSection).ToList();
                row.CountCourses = courses.Count();
                foreach (var course in courses) 
                {
                    if (course.Count(x => x.StudentMidterm != null) == course.Count())
                        row.CountMidterms++;
                }
                row.State = enu_MidtermReportState.Submitted;
                if (row.CountMidterms == 0)
                    row.State = enu_MidtermReportState.NotStarted;
                else if (row.CountCourses != row.CountMidterms)
                    row.State = enu_MidtermReportState.InProgress;
                model.Add(row);
            }

            return model;
        }

        /// <summary>
        /// Get student midterm statistics detailed report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <param name="courseId">Course id</param>
        /// <param name="state">Row state</param>
        /// <returns>Report model</returns>
        public List<MidtermStatisticsDetailedReportViewModel> MidtermStatisticsDetailedReport(int organizationId, int semesterId,
            int? departmentId, string instructorUserId, int? courseId, enu_MidtermReportState state)
        {
            var model = new List<MidtermStatisticsDetailedReportViewModel>();

            var studentCourses = GetStudentCourses(organizationId, semesterId, departmentId, instructorUserId, courseId);

            foreach (var announcementSection in studentCourses.GroupBy(x => x.AnnouncementSection))
            {
                var row = new MidtermStatisticsDetailedReportViewModel();
                row.AnnouncementSection = _mapper.Map<AnnouncementSectionDTO>(announcementSection.Key);
                row.AnnouncementSection.InstructorUserName = _userManager.GetUserFullName(row.AnnouncementSection.InstructorUserId);
                if (row.AnnouncementSection.ExtraInstructorsJson != null) 
                {
                    foreach (var extraInstructor in row.AnnouncementSection.ExtraInstructorsJson) 
                        row.AnnouncementSection.InstructorUserName  += $", {_userManager.GetUserFullName(extraInstructor)}";
                }
                row.CountStudents = announcementSection.Count();
                row.CountMidterms = announcementSection.Count(x => x.StudentMidterm != null);
                
                row.State = enu_MidtermReportState.Submitted;
                if (row.CountMidterms == 0)
                    row.State = enu_MidtermReportState.NotStarted;
                else if (row.CountStudents != row.CountMidterms)
                    row.State = enu_MidtermReportState.InProgress;
                model.Add(row);
            }

            if (state != enu_MidtermReportState.NotSelected)
                model = model.Where(x => x.State == state).ToList();

            return model.OrderBy(x => x.AnnouncementSection.Announcement.Course.Name).ToList();
        }

        private List<StudentCourseTemp> GetStudentCourses(int organizationId, int semesterId,
            int? departmentId, string instructorUserId, int? courseId)
        {
            var activeStudentIds = _studentInfoService.GetStudentSelectList(organizationId, 0, 0, null, null, 0, true)
                .Select(x => x.StudentUserId);

            var studentCoursesQuery = _db.StudentCoursesTemp
                    .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course).ThenInclude(x => x.Department)
                    .Include(x => x.AnnouncementSection.ExtraInstructors)
                    .Include(x => x.AnnouncementSection.Announcement).ThenInclude(x => x.Course).ThenInclude(x => x.Department)
                    .Include(x => x.StudentCourseRegistration)
                    .Include(x => x.StudentMidterm)
                    .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId &&
                            x.StudentCourseRegistration.SemesterId == semesterId && x.AnnouncementSection.Announcement.IsActivated &&
                            activeStudentIds.Contains(x.StudentCourseRegistration.StudentUserId) &&
                            x.State != (int)enu_CourseState.Dropped);

            if (departmentId != null)
                studentCoursesQuery = studentCoursesQuery.Where(x => x.AnnouncementSection.Course.DepartmentId == departmentId.Value);

            if (instructorUserId != null)
                studentCoursesQuery = studentCoursesQuery.Where(x => x.AnnouncementSection.InstructorUserId == instructorUserId ||
                    x.AnnouncementSection.ExtraInstructors != null &&
                    x.AnnouncementSection.ExtraInstructors.Select(x => x.InstructorUserId).Contains(instructorUserId));

            if (courseId != null)
                studentCoursesQuery = studentCoursesQuery.Where(x => x.AnnouncementSection.Course.ImportCode == courseId.Value);

            return studentCoursesQuery.ToList();
        }

        /// <summary>
        /// Get midterm adviser report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="onlyActiveStudents">Only active students</param>
        /// <returns>Midterm report model</returns>
        public MidtermAdviserReportViewModel MidtermAdviserReport(int organizationId, int semesterId,
            string adviserUserId, int? departmentGroupId, bool onlyActiveStudents = true) 
        {
            var model = new MidtermAdviserReportViewModel();
            model.Semester = _semesterService.GetSemester(organizationId, semesterId);
            model.AdviserUserId = adviserUserId;
            model.AdviserName = _userManager.GetUserFullName(adviserUserId);

            var adviserStudents = _adviserStudentService.GetAdviserStudentsByInstuctorId(organizationId, adviserUserId);

            if (departmentGroupId != null)
            {
                adviserStudents = adviserStudents.Where(x => x.DepartmentGroupId == departmentGroupId.Value ||
                    x.PrepDepartmentGroupId == departmentGroupId).ToList();
            }

            if (onlyActiveStudents)
                adviserStudents = adviserStudents.Where(x => x.State == enu_StudentState.Active).ToList();

            var studentIds = adviserStudents.Select(x => x.StudentUserId).ToList();
            
            var allStudentCourses = _db.StudentCoursesTemp
                    .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course)
                    .Include(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                    .Include(x => x.StudentCourseRegistration)
                    .Include(x => x.StudentMidterm)
                    .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId &&
                            x.StudentCourseRegistration.SemesterId == semesterId && 
                            x.AnnouncementSection.Announcement.IsActivated &&
                            studentIds.Contains(x.StudentCourseRegistration.StudentUserId) &&
                            x.State != (int)enu_CourseState.Dropped)
                    .ToList();

            model.AllCourses = _mapper.Map<List<CourseDTO>>(allStudentCourses
                .GroupBy(x => x.AnnouncementSection.Course).Select(x => x.Key).ToList());


            foreach (var student in adviserStudents) 
            {
                var studentRow = new MidtermAdviserReportStudentRow();
                studentRow.StudentInfo = student;
                studentRow.AttentionCount = allStudentCourses
                    .Where(x => x.StudentCourseRegistration.StudentUserId == student.StudentUserId)
                    .Count(x => x.StudentMidterm != null && x.StudentMidterm.Attention);
                model.AllStudents.Add(studentRow);
            }

            foreach (var studentCourse in allStudentCourses) 
            {
                var index = $"{studentCourse.StudentCourseRegistration.StudentUserId}_{studentCourse.AnnouncementSection.CourseId}";
                model.AllStudentCourses.Add(index, _mapper.Map<StudentCourseTempDTO>(studentCourse));
            }

            return model;
        }

        /// <summary>
        /// Get student midterm report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="courseId">Course id</param>
        /// <returns></returns>
        public List<StudentCourseTempDTO> MidtermStudentReport(int organizationId, int semesterId,
            string studentUserId, int? courseId) 
        {
            if(string.IsNullOrEmpty(studentUserId))
                return new List<StudentCourseTempDTO>();

            var studentCoursesQuery = _db.StudentCoursesTemp
                    .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course).ThenInclude(x => x.Department)
                    .Include(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                    .Include(x => x.AnnouncementSection.ExtraInstructors)
                    .Include(x => x.StudentCourseRegistration)
                    .Include(x => x.StudentMidterm)
                    .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId &&
                            x.StudentCourseRegistration.SemesterId == semesterId && 
                            x.AnnouncementSection.Announcement.IsActivated &&
                            x.StudentCourseRegistration.StudentUserId == studentUserId &&
                            x.State != (int)enu_CourseState.Dropped);

            if (courseId != null)
                studentCoursesQuery = studentCoursesQuery.Where(x => x.AnnouncementSection.Course.ImportCode == courseId.Value);

            return _mapper.Map<List<StudentCourseTempDTO>>(studentCoursesQuery.ToList());
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
