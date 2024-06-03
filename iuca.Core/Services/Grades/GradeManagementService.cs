using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Grades;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.ViewModels.Courses;
using iuca.Application.ViewModels.Reports;
using iuca.Application.ViewModels.Users.Students;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Grades
{
    public class GradeManagementService : IGradeManagementService
    {
        private readonly IApplicationDbContext _db;
        private readonly ISemesterService _semesterService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IGradeService _gradeService;
        private readonly IAdviserStudentService _adviserStudentService;
        private readonly IMapper _mapper;
        private readonly IStudentTranscriptService _studentTranscriptService;

        public GradeManagementService(IApplicationDbContext db,
            ISemesterService semesterService,
            ApplicationUserManager<ApplicationUser> userManager,
            IGradeService gradeService,
            IAdviserStudentService adviserStudentService,
            IMapper mapper,
            IStudentTranscriptService studentTranscriptService)
        {
            _db = db;
            _semesterService = semesterService;
            _userManager = userManager;
            _gradeService = gradeService;
            _adviserStudentService = adviserStudentService;
            _mapper = mapper;
            _studentTranscriptService = studentTranscriptService;
        }

        /// <summary>
        /// Get courses and their students with grades
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="semesterId">Semester Id</param>
        /// <param name="departmentId">Department Id</param>
        /// <param name="courseImportCode">Course import code Id</param>
        /// <param name="studentId">Student Id</param>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="status">Status (submitted/not submitted)</param>
        /// <returns>Courses and their students with grades for semester</returns>
        public List<GradeReportViewModel> GetGradeReport(int organizationId, int semesterId, int? departmentId,
            int? courseImportCode, int? studentId, int? gradeId, enu_GradeReportStatus status) 
        {
            List<GradeReportViewModel> courseList = new List<GradeReportViewModel>();

            var semester = _semesterService.GetSemester(organizationId, semesterId);

            var registrationCoursesQuery = _db.AnnouncementSections
                .Include(x => x.StudentCourses)
                .ThenInclude(x => x.StudentCourseRegistration)
                .Include(x => x.StudentCourses)
                .ThenInclude(x => x.Grade)
                .Include(x => x.Course)
                .Where(x => x.OrganizationId == organizationId && x.Season == semester.Season 
                    && x.Year == semester.Year);

            if (departmentId != null) 
                registrationCoursesQuery = registrationCoursesQuery.Where(x => x.Course.DepartmentId == departmentId.Value);

            if (courseImportCode != null)
                registrationCoursesQuery = registrationCoursesQuery.Where(x => x.Course.ImportCode == courseImportCode.Value);

            if (status == enu_GradeReportStatus.Submitted)
                registrationCoursesQuery = registrationCoursesQuery.Where(x => x.GradeSheetSubmitted);
            else if (status == enu_GradeReportStatus.NotSubmitted)
                registrationCoursesQuery = registrationCoursesQuery.Where(x => !x.GradeSheetSubmitted);

            StudentOrgInfo studentOrgInfo = null;
            if (studentId != null) 
            {
                studentOrgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                    .FirstOrDefault(x => x.StudentId == studentId.Value && x.OrganizationId == organizationId);
                
                if (studentOrgInfo != null)
                {
                    registrationCoursesQuery = registrationCoursesQuery.Where(x => x.StudentCourses
                        .Any(x => x.StudentCourseRegistration.StudentUserId == studentOrgInfo.StudentBasicInfo.ApplicationUserId));
                }
                else
                    return courseList;
            }

            GradeDTO grade = null;
            if (gradeId != null) 
            {
                grade = _gradeService.GetGrade(gradeId.Value);

                if (grade.GradeMark == "*") 
                    registrationCoursesQuery = registrationCoursesQuery.Where(x => x.StudentCourses
                        .Any(y => y.GradeId == null || y.GradeId == gradeId.Value));
                else
                    registrationCoursesQuery = registrationCoursesQuery.Where(x => x.StudentCourses
                        .Any(y => y.GradeId == gradeId.Value));
            }

            var registrationCourses = registrationCoursesQuery.ToList();

            if (registrationCourses.Count > 0) 
            {
                var activeStudents = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                    .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                    .Include(x => x.PrepDepartmentGroup).ThenInclude(x => x.Department)
                    .Where(x => x.OrganizationId == organizationId && x.StudentBasicInfo != null
                        && x.State != (int)enu_StudentState.Dismissed && x.State != (int)enu_StudentState.AcadLeave)
                    .ToList();

                var activeStudentIds = activeStudents.Select(x => x.StudentBasicInfo.ApplicationUserId).ToList();

                foreach (var course in registrationCourses)
                {
                    //Display only active students or students that had been active for this semester (they can be dismissed or acad leave now)
                    var studentsQuery = course.StudentCourses.Where(x => x.State != (int)enu_CourseState.Dropped 
                            && activeStudentIds.Contains(x.StudentCourseRegistration.StudentUserId));

                    if (studentOrgInfo != null) 
                    {
                        studentsQuery = studentsQuery.Where(x => x.StudentCourseRegistration.StudentUserId == 
                                            studentOrgInfo.StudentBasicInfo.ApplicationUserId);
                    }

                    if (grade != null) 
                    {
                        if (grade.GradeMark == "*")
                            studentsQuery = studentsQuery.Where(x => x.GradeId == null || x.GradeId == gradeId.Value);
                        else
                            studentsQuery = studentsQuery.Where(x =>  x.GradeId == gradeId.Value);
                    }
                    
                    var students = studentsQuery.ToList();

                    if (students.Count > 0)
                    {
                        GradeReportViewModel courseModel = new GradeReportViewModel();
                        courseModel.AnnouncementSectionId = course.Id;
                        courseModel.CourseImportCode = course.Course.ImportCode;
                        courseModel.CourseName = $"{course.Course.Abbreviation}{course.Course.Number}, {course.Course.NameEng}/{course.Course.NameRus}" +
                            $"/{course.Course.NameKir} ({course.Section})";
                        courseModel.Credits = course.Credits;
                        courseModel.InstructorName = _userManager.GetUserFullName(course.InstructorUserId);
                        courseModel.GradeSheetSubmitted = course.GradeSheetSubmitted;

                        foreach (var studentCourse in students) 
                        {
                            GradeReportStudentRow row = new GradeReportStudentRow();
                            row.StudentUserId = studentCourse.StudentCourseRegistration.StudentUserId;
                            row.StudentName = _userManager.GetUserFullName(studentCourse.StudentCourseRegistration.StudentUserId);
                            //row.Grade = studentCourse.Grade != null ? studentCourse.Grade.GradeMark : "*";
                            row.GradeId = studentCourse.GradeId;

                            var orgInfo = activeStudents.FirstOrDefault(x => x.StudentBasicInfo.ApplicationUserId == studentCourse.StudentCourseRegistration.StudentUserId);
                            if (orgInfo != null)
                            {
                                row.StudentId = orgInfo.StudentId;
                                row.StudentGroup = GetStudentGroup(studentCourse.StudentCourseRegistration.StudentUserId, orgInfo);
                                row.StudentState = (enu_StudentState)orgInfo.State;
                            }

                            courseModel.Students.Add(row);
                        }
                        courseList.Add(courseModel);
                    }
                }
            }

            return courseList;
        }

        private string GetStudentGroup(string studentUserId, StudentOrgInfo studentOrgInfo) 
        {
            string group = $"{studentOrgInfo.DepartmentGroup.Department.Code}{studentOrgInfo.DepartmentGroup.Code}";
            if (studentOrgInfo.PrepDepartmentGroup != null)
                group += $"/{studentOrgInfo.PrepDepartmentGroup.Department.Code}{studentOrgInfo.PrepDepartmentGroup.Code}";

            return group;
        }

        /// <summary>
        /// Set student grade for course
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="gradeId">Grade id</param>
        public void SetStudentGrade(int organizationId, int announcementSectionId, string studentUserId, int? gradeId)
        {
            var studentCourse = _db.StudentCoursesTemp.Include(x => x.AnnouncementSection)
                .Include(x => x.StudentCourseRegistration)
                .Include(x => x.Grade)
                .FirstOrDefault(x => x.AnnouncementSection.OrganizationId == organizationId &&
                    x.AnnouncementSectionId == announcementSectionId &&
                    x.StudentCourseRegistration.StudentUserId == studentUserId);

            if (studentCourse == null)
                throw new Exception("Student course not found!");

            studentCourse.GradeId = gradeId;
            _db.SaveChanges();
        }


        /// <summary>
        /// Get student grades adviser report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="onlyActiveStudents">Only active students</param>
        /// <returns>Student grades report model</returns>
        public GradeAdviserReportViewModel GradeAdviserReport(int organizationId, int semesterId,
            string adviserUserId, int? departmentGroupId, int? gradeId, bool onlyActiveStudents = true)
        {
            var model = new GradeAdviserReportViewModel();
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

            var allStudentCoursesQuery = _db.StudentCoursesTemp
                    .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course)
                    .Include(x => x.StudentCourseRegistration)
                    .Include(x => x.Grade)
                    .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId &&
                            x.StudentCourseRegistration.SemesterId == semesterId &&
                            studentIds.Contains(x.StudentCourseRegistration.StudentUserId) &&
                            x.State != (int)enu_CourseState.Dropped &&
                            ((x.StudentCourseRegistration.State == (int)enu_RegistrationState.Submitted &&
                            x.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.NotSent) ||
                            x.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.Submitted));

            if (gradeId != null) 
            {
                var grade = _gradeService.GetGrade(gradeId.Value);
                if (grade.GradeMark != "*")
                    allStudentCoursesQuery = allStudentCoursesQuery.Where(x => x.GradeId == gradeId.Value);
                else 
                    allStudentCoursesQuery = allStudentCoursesQuery.Where(x => x.GradeId == gradeId.Value || x.GradeId == null);
            }

            var allStudentCourses = allStudentCoursesQuery.ToList();

            if (gradeId != null) 
            {
                var fileredStudentIds = allStudentCourses.Select(x => x.StudentCourseRegistration.StudentUserId).Distinct().ToList();
                adviserStudents = adviserStudents.Where(x => fileredStudentIds.Contains(x.StudentUserId)).ToList();
            }

            FillStudentGPAs(semesterId, adviserStudents);

            model.AllStudents.AddRange(adviserStudents);
            model.AllCourses = _mapper.Map<List<CourseDTO>>(allStudentCourses
                .GroupBy(x => x.AnnouncementSection.Course).Select(x => x.Key).ToList());


            foreach (var studentCourse in allStudentCourses)
            {
                var index = $"{studentCourse.StudentCourseRegistration.StudentUserId}_{studentCourse.AnnouncementSection.CourseId}";
                model.AllStudentCourses.Add(index, _mapper.Map<StudentCourseTempDTO>(studentCourse));
            }

            return model;
        }

        private void FillStudentGPAs(int semesterId, List<AdviserStudentViewModel> students) 
        {
            foreach (var student in students) 
            {
                student.SemsterGPA = _studentTranscriptService.GetStudentSemesterGPA(student.StudentUserId, semesterId);
                student.TotalGPA = _studentTranscriptService.GetStudentTotalGPA(student.StudentUserId);
            }
        }

        /// <summary>
        /// Return students that have X or F grades for the same course more then one time
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="departmentIds">Department ids</param>
        /// <param name="onlyActiveStudents">Only active students</param>
        /// <returns>List of students and grades</returns>
        public List<FFXXReportViewModel> FFXXReport(int organizationId, List<int> departmentIds, bool onlyActiveStudents) 
        {
            var model = new List<FFXXReportViewModel>();

            var studentCoursesBaseQuery = _db.StudentCoursesTemp
                .Include(x => x.StudentCourseRegistration).ThenInclude(x => x.Semester)
                .Include(x => x.Grade)
                .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course)
                .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId && x.Grade != null &&
                    x.State != (int)enu_CourseState.Dropped);

            var studentCoursesQuery = studentCoursesBaseQuery
                .Where(x => (x.Grade.GradeMark == "F" || x.Grade.GradeMark == "X"));

            var studentCourses = studentCoursesQuery
                .Select(x =>
                new
                {
                    StudentUserId = x.StudentCourseRegistration.StudentUserId,
                    Course = x.AnnouncementSection.Course,
                    Grade = x.Grade,
                    Semester = x.StudentCourseRegistration.Semester
                }).ToList().GroupBy(x => new { x.StudentUserId, x.Course })
                .Where(x => x.Count() > 1).ToList();

            foreach (var studentCourse in studentCourses)
            {
                var orgInfo = _db.StudentOrgInfo
                    .Include(x => x.StudentBasicInfo)
                    .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                    .FirstOrDefault(x => x.StudentBasicInfo.ApplicationUserId == studentCourse.Key.StudentUserId);

                var studentName = _userManager.GetUserFullName(studentCourse.Key.StudentUserId);
                var studentId = 0;
                var studentGroup = "";

                if (orgInfo != null) 
                {
                    if (onlyActiveStudents && orgInfo.State != (int)enu_StudentState.Active)
                        continue;

                    if (departmentIds != null && departmentIds.Count > 0) 
                    {
                        if (orgInfo.PrepDepartmentGroup != null && 
                            !departmentIds.Contains(orgInfo.PrepDepartmentGroup.DepartmentId)) 
                            continue;
                        else if (!departmentIds.Contains(orgInfo.DepartmentGroup.DepartmentId)) 
                            continue;
                    } 

                    studentId = orgInfo.StudentId;
                    studentGroup = $"{orgInfo.DepartmentGroup.Department.Code}{orgInfo.DepartmentGroup.Code}"; 
                }
                

                foreach (var course in studentCourse) 
                {
                    var studentCourseModel = new FFXXReportViewModel();
                    studentCourseModel.StudentName = studentName;
                    studentCourseModel.StudentId = studentId;
                    studentCourseModel.StudentGroup = studentGroup;
                    studentCourseModel.Course = _mapper.Map<CourseDTO>(studentCourse.Key.Course);
                    studentCourseModel.Semester = _mapper.Map<SemesterDTO>(course.Semester);
                    studentCourseModel.GradeMark = course.Grade.GradeMark;
                    model.Add(studentCourseModel);
                }

                var closedGrade = studentCoursesBaseQuery.Where(x => 
                        x.StudentCourseRegistration.StudentUserId == studentCourse.Key.StudentUserId && 
                        x.AnnouncementSection.CourseId == studentCourse.Key.Course.Id &&
                        x.Grade.GradeMark != "F" && x.Grade.GradeMark != "X" && x.Grade.GradeMark != "*")
                        .FirstOrDefault();

                if (closedGrade != null) 
                {
                    var studentCourseModel = new FFXXReportViewModel();
                    studentCourseModel.StudentName = studentName;
                    studentCourseModel.StudentId = studentId;
                    studentCourseModel.StudentGroup = studentGroup;
                    studentCourseModel.Course = _mapper.Map<CourseDTO>(studentCourse.Key.Course);
                    studentCourseModel.Semester = _mapper.Map<SemesterDTO>(closedGrade.StudentCourseRegistration.Semester);
                    studentCourseModel.GradeMark = closedGrade.Grade.GradeMark;
                    studentCourseModel.IsClosed = true;
                    model.Add(studentCourseModel);
                }

            }

            return model;
        }

        /// <summary>
        /// Get department grade report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="onlyActiveStudents">Display only active students</param>
        /// <returns>Department grade report</returns>
        public List<DepartmentStudentGradeViewModel> DepartmentGradeReport(int organizationId, int semesterId, int? departmentId, int? departmentGroupId,
            int? gradeId, bool onlyActiveStudents)
        {
            var model = new List<DepartmentStudentGradeViewModel>();

            var semester = _semesterService.GetSemester(semesterId);

            var studentCoursesQuery = _db.StudentCoursesTemp.Include(x => x.StudentCourseRegistration)
                .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course)
                .Where(x => x.StudentCourseRegistration.SemesterId == semesterId && x.State != (int)enu_CourseState.Dropped);

            GradeDTO grade = null;
            if (gradeId != null)
            {
                grade = _gradeService.GetGrade(gradeId.Value);
                if (grade.GradeMark == "*")
                    studentCoursesQuery = studentCoursesQuery.Where(x => x.GradeId == null || x.GradeId == gradeId.Value);
                else
                    studentCoursesQuery = studentCoursesQuery.Where(x => x.GradeId == gradeId.Value);
            }

            var studentCourses = studentCoursesQuery.ToList();

            var studentOrgInfoes = GetStudentOrgInfoes(studentCourses.Select(x => x.StudentCourseRegistration.StudentUserId).ToList(), 
                    departmentId, departmentGroupId, onlyActiveStudents);

            var studentIds = studentOrgInfoes.Select(x => x.StudentBasicInfo.ApplicationUserId).ToList();

            studentCourses = studentCourses.Where(x => studentIds.Contains(x.StudentCourseRegistration.StudentUserId)).ToList();

            foreach (var studentCourse in studentCourses)
            {
                var student = new DepartmentStudentGradeViewModel();

                var studentOrgInfo = studentOrgInfoes.FirstOrDefault(x => x.StudentBasicInfo.ApplicationUserId == studentCourse.StudentCourseRegistration.StudentUserId);

                if (studentOrgInfo != null) 
                {
                    student.Department = _mapper.Map<DepartmentDTO>(studentOrgInfo.PrepDepartmentGroup?.Department ?? studentOrgInfo.DepartmentGroup.Department);
                    student.DepartmentGroup = _mapper.Map<DepartmentGroupDTO>(studentOrgInfo.PrepDepartmentGroup ?? studentOrgInfo.DepartmentGroup);
                    student.IsPrep = studentOrgInfo.PrepDepartmentGroup != null;
                    student.StudentName = _userManager.GetUserFullName(studentOrgInfo.StudentBasicInfo.ApplicationUserId);
                    student.StudentState = ((enu_StudentState)studentOrgInfo.State).ToString();
                }
                
                student.Grade = studentCourse.Grade?.GradeMark;
                student.CourseName = studentCourse.AnnouncementSection.Course.NameEng;
                student.CourseId = studentCourse.AnnouncementSection.Course.ImportCode;
                
                model.Add(student);
            }

            return model;
        }

        private List<StudentOrgInfo> GetStudentOrgInfoes(List<string> studentIds, int? departmentId, int? departmentGroupId,
            bool onlyActiveStudents)
        {
            var studentOrgInfoesQuery = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                    .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                    .Include(x => x.PrepDepartmentGroup).ThenInclude(x => x.Department)
                    .Where(x => x.StudentBasicInfo != null && studentIds.Contains(x.StudentBasicInfo.ApplicationUserId));

            if (departmentId != null)
            {
                studentOrgInfoesQuery = studentOrgInfoesQuery.Where(x => (x.DepartmentGroup != null &&
                    x.DepartmentGroup.DepartmentId == departmentId.Value) ||
                    (x.PrepDepartmentGroup != null && x.PrepDepartmentGroup.DepartmentId == departmentId.Value));
            }

            if (departmentGroupId != null)
            {
                studentOrgInfoesQuery = studentOrgInfoesQuery.Where(x => x.DepartmentGroupId == departmentGroupId.Value ||
                    x.PrepDepartmentGroupId == departmentGroupId.Value);
            }

            if (onlyActiveStudents)
                studentOrgInfoesQuery = studentOrgInfoesQuery.Where(x => x.State == (int)enu_StudentState.Active);

            return studentOrgInfoesQuery.ToList();
        }

    }
}
