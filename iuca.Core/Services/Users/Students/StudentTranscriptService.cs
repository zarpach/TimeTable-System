using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.ViewModels.Reports;
using iuca.Application.ViewModels.Users.Students;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static iuca.Application.Constants.Permissions;

namespace iuca.Application.Services.Users.Students
{
    public class StudentTranscriptService : IStudentTranscriptService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly ISemesterService _semesterService;
        private readonly IMapper _mapper;

        public StudentTranscriptService(IApplicationDbContext db,
            ApplicationUserManager<ApplicationUser> userManager,
            ISemesterService semesterService,
            IMapper mapper) 
        {
            _userManager = userManager;
            _db = db;
            _semesterService = semesterService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get student transcript
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Transcript model</returns>
        public TranscriptViewModel GetTranscript(int organizationId, string studentUserId) 
        {
            var user = _userManager.Users
                            .Include(x => x.UserBasicInfo)
                            .Include(x => x.StudentBasicInfo)
                            .ThenInclude(x => x.StudentOrgInfo)
                            .ThenInclude(x => x.DepartmentGroup)
                            .ThenInclude(x => x.Department)
                            .Include(x => x.StudentBasicInfo)
                            .ThenInclude(x => x.StudentMinorInfo)
                            .ThenInclude(x => x.Department)
                            .FirstOrDefault(x => x.Id == studentUserId);

            if (user == null)
                throw new Exception("User not found");

            TranscriptViewModel model = new TranscriptViewModel();
            model.StudentName = user.FullNameEng;

            if (user.UserBasicInfo != null) 
            {
                model.DateOfBirth = user.UserBasicInfo.DateOfBirth;
            }

            if (user.StudentBasicInfo != null && user.StudentBasicInfo.StudentOrgInfo != null) 
            {
                var studentOrgInfo = user.StudentBasicInfo.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId);
                if (studentOrgInfo != null) 
                {
                    model.StudentId = studentOrgInfo.StudentId;
                    model.Department = studentOrgInfo.DepartmentGroup?.Department?.NameEng;
                }
            }

            if (user.StudentBasicInfo != null && user.StudentBasicInfo.StudentMinorInfo != null)
            {
                var studentMinorInfo = user.StudentBasicInfo.StudentMinorInfo.Select(x => x.Department.NameEng);
                if (studentMinorInfo != null && studentMinorInfo.Any())
                {
                    model.StudentMinors = studentMinorInfo.ToList();
                }
            }

            var studentCourseGrades = _db.StudentCourseGrades
                        .Include(x => x.Course)
                        .Include(x => x.Grade)
                        .Where(x => x.StudentUserId == studentUserId)
                        .ToList();

            //Repeated courses with F are not accounted in GPA credits
            var repeatedFailedCourses = studentCourseGrades
                                            .GroupBy(x => x.Course.ImportCode)
                                            .Where(x => x.Count() > 1)
                                            .SelectMany(x => x)
                                            .Where(x => x.Grade.Gpa == 0)
                                            .Select(x => x.Course.ImportCode)
                                            .Distinct()
                                            .ToList();

            var semesterGrades = studentCourseGrades
                        .GroupBy(x => new { x.Year, x.Season })
                        .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Season)
                        .ToList();

            foreach (var semester in semesterGrades) 
            {
                TranscriptSemester transcriptSemester = new TranscriptSemester();
                transcriptSemester.Year = semester.Key.Year;
                transcriptSemester.Season = (enu_Season)semester.Key.Season;
                transcriptSemester.Order = GetSemesterOrder(transcriptSemester.Season);

                foreach (var course in semester) 
                {
                    TranscriptCourse transcriptCourse = new TranscriptCourse();
                    transcriptCourse.CourseAbbreviation = course.Course.Abbreviation;
                    transcriptCourse.CourseNumber = course.Course.Number;
                    transcriptCourse.CourseId = course.Course.ImportCode;
                    transcriptCourse.CourseName = course.Course.NameEng;
                    transcriptCourse.Grade = course.Grade != null ? course.Grade.GradeMark : "*";
                    transcriptCourse.Credits = course.Points;

                    transcriptSemester.TranscriptCourses.Add(transcriptCourse);
                }

                // * нигде не считается - полностью игнорировать
                // -1 счиатается только в Attempted Credits
                // -2 не считается в GPA Credits
                // -3 счиатается только в Attempted Credits
                var grades = semester.Where(x => x.Grade != null && x.Grade.GradeMark != "*").ToList();

                if (repeatedFailedCourses.Count > 0)
                    transcriptSemester.GPACredits = grades.Where(x => x.Grade.Gpa > 0 || (x.Grade.Gpa == 0 && !repeatedFailedCourses.Contains(x.Course.ImportCode))).Sum(x => x.Points);
                else
                    transcriptSemester.GPACredits = grades.Where(x => x.Grade.Gpa >= 0).Sum(x => x.Points);
                
                transcriptSemester.EarnedCredits = grades.Where(x => x.Grade.Gpa != -1 && x.Grade.Gpa != -3 && x.Grade.Gpa != 0).Sum(x => x.Points);
                transcriptSemester.AttemptedCredits = grades.Sum(x => x.Points);
                transcriptSemester.QualityPoints = (float)grades.Where(x => x.Grade.Gpa >= 0).Sum(x => Math.Round((decimal)(x.Points * x.Grade.Gpa), 2, MidpointRounding.AwayFromZero));
                transcriptSemester.SemesterGPA = transcriptSemester.GPACredits > 0 ? transcriptSemester.QualityPoints / transcriptSemester.GPACredits : 0;
                
                model.TranscriptSemesters.Add(transcriptSemester);
            }

            var totalGPACredits = model.TranscriptSemesters.Sum(x => x.GPACredits);
            model.TotalGPA = totalGPACredits > 0 ? (float)Math.Round(model.TranscriptSemesters.Sum(x => x.QualityPoints) 
                / totalGPACredits, 2) : 0;

            model.TransferCourses = GetTransferCourses(organizationId, studentUserId);

            return model;
        }

        private List<TranscriptTransferCourse> GetTransferCourses(int organizationId, string studentUserId) 
        {
            List<TranscriptTransferCourse> model = new List<TranscriptTransferCourse>();
            var transferCourses = _db.TransferCourses.Where(x => x.OrganizationId == organizationId &&
                                    x.StudentUserId == studentUserId)
                .Include(x => x.University).ToList();

            if (transferCourses.Count > 0)
            {
                model = transferCourses.Select(x => new TranscriptTransferCourse
                {
                    UniversityName = x.University.NameEng,
                    CourseName = x.NameEng,
                    Season = EnumExtentions.GetDisplayName((enu_Season)x.Season),
                    Year = x.Year,
                    Credits = x.Points
                }).ToList();
            }

            return model;
        }

        private int GetSemesterOrder(enu_Season season) 
        {
            int order;
            switch (season) 
            {
                case enu_Season.Spring: 
                    order = 1; 
                    break;
                case enu_Season.Fall:
                    order = 3;
                    break;
                case enu_Season.Winter:
                    order = 4;
                    break;
                case enu_Season.Summer:
                    order = 2;
                    break;
                default:
                    order = 0;
                    break;
            }
            return order;
        }

        /// <summary>
        /// Recalc student GPAs
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        public void RecalcStudentsGPA(int organizationId) 
        {
            var studentUserIds = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .Where(x => x.OrganizationId == organizationId && x.StudentBasicInfo != null)
                .Select(x => x.StudentBasicInfo.ApplicationUserId).ToList();

            foreach (var studentUserId in studentUserIds) 
            {
                var transcript = GetTranscript(organizationId, studentUserId);

                foreach (var semester in transcript.TranscriptSemesters) 
                    ProcessStudentSemesterGPA(organizationId, studentUserId, semester);

                ProcessStudentTotalGPA(organizationId, studentUserId, transcript.TotalGPA);
            }
            _db.SaveChanges();
        }

        private void ProcessStudentSemesterGPA(int organizationId, string studentUserId, TranscriptSemester transcriptSemester) 
        {
            var studentSemester = _db.StudentSemesterGPAs.FirstOrDefault(x => x.StudentUserId == studentUserId &&
                x.Season == (int)transcriptSemester.Season && x.Year == transcriptSemester.Year);

            if (studentSemester != null)
            {
                studentSemester.GPA = transcriptSemester.SemesterGPA;
            }
            else
            {
                var newStudentSemester = new StudentSemesterGPA
                {
                    StudentUserId = studentUserId,
                    Season = (int)transcriptSemester.Season,
                    Year = transcriptSemester.Year,
                    GPA = (float)Math.Round(transcriptSemester.SemesterGPA, 2),
                    OrganizationId = organizationId
                };

                _db.StudentSemesterGPAs.Add(newStudentSemester);
            }
        }

        private void ProcessStudentTotalGPA(int organizationId, string studentUserId, float totalGPA)
        {
            var studentTotalGPA = _db.StudentTotalGPAs.FirstOrDefault(x => x.StudentUserId == studentUserId);

            if (studentTotalGPA != null)
            {
                studentTotalGPA.TotalGPA = totalGPA;
            }
            else
            {
                var newStudentTotalGPA = new StudentTotalGPA
                {
                    StudentUserId = studentUserId,
                    TotalGPA = totalGPA,
                    OrganizationId = organizationId
                };

                _db.StudentTotalGPAs.Add(newStudentTotalGPA);
            }
        }

        /// <summary>
        /// Get student total GPA
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <returns></returns>
        public float GetStudentTotalGPA(string studentUserId) 
        {
            var studentTotalGPA = _db.StudentTotalGPAs.FirstOrDefault(x => x.StudentUserId == studentUserId);

            if (studentTotalGPA != null)
                return studentTotalGPA.TotalGPA;
            else return 0;
        }

        /// <summary>
        /// Get student semester GPA
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="semesterId">Semster id</param>
        /// <returns>Semester GPA</returns>
        public float GetStudentSemesterGPA(string studentUserId, int semesterId)
        {
            var semester = _semesterService.GetSemester(semesterId);
            if (semester != null)
                return GetStudentSemesterGPA(studentUserId, semester.Season, semester.Year);
            else return 0;
        }

        /// <summary>
        /// Get student semester GPA
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="season">Season</param>
        /// <param name="year">Year</param>
        /// <returns>Semester GPA</returns>
        public float GetStudentSemesterGPA(string studentUserId, int season, int year) 
        {
            var studentSemesterGPA = _db.StudentSemesterGPAs.FirstOrDefault(x => x.StudentUserId == studentUserId
                && x.Season == season && x.Year == year);

            if (studentSemesterGPA != null)
                return studentSemesterGPA.GPA;
            else return 0;
        }

        /// <summary>
        /// Get GPA student report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="minGPA">Minimum GPA</param>
        /// <param name="maxGPA">Maximum GPA</param>
        /// <param name="onlyActiveStudents">Display only active students</param>
        /// <returns>GPA report</returns>
        public List<StudentGPAViewModel> GPAReport(int organizationId, int semesterId, int? departmentId, int? departmentGroupId, 
            float minGPA, float maxGPA, bool onlyActiveStudents) 
        {
            var model = new List<StudentGPAViewModel>();

            var semester = _semesterService.GetSemester(semesterId);

            var semesterGPAs = _db.StudentSemesterGPAs.Where(x => x.OrganizationId == organizationId &&
                x.Season == semester.Season && x.Year == semester.Year && x.GPA >= minGPA && x.GPA <= maxGPA)
                .ToList();

            var studentOrgInfoes = GetStudentOrgInfoes(semesterGPAs.Select(x => x.StudentUserId).ToList(), departmentId, 
                departmentGroupId, onlyActiveStudents);
            
            var studentIds = studentOrgInfoes.Select(x => x.StudentBasicInfo.ApplicationUserId).ToList();

            semesterGPAs = semesterGPAs.Where(x => studentIds.Contains(x.StudentUserId)).ToList();
            var totalGPAs = _db.StudentTotalGPAs.Where(x => studentIds.Contains(x.StudentUserId)).ToList();

            foreach (var studentOrgInfo in studentOrgInfoes) 
            {
                var student = new StudentGPAViewModel();
                student.Department = _mapper.Map<DepartmentDTO>(studentOrgInfo.PrepDepartmentGroup?.Department ?? studentOrgInfo.DepartmentGroup.Department);
                student.DepartmentGroup = _mapper.Map<DepartmentGroupDTO>(studentOrgInfo.PrepDepartmentGroup ?? studentOrgInfo.DepartmentGroup);
                student.IsPrep = studentOrgInfo.PrepDepartmentGroup != null;
                student.StudentName = _userManager.GetUserFullName(studentOrgInfo.StudentBasicInfo.ApplicationUserId);
                student.StudentState = ((enu_StudentState)studentOrgInfo.State).ToString();
                
                var semesterGPA = semesterGPAs.FirstOrDefault(x => x.StudentUserId == studentOrgInfo.StudentBasicInfo.ApplicationUserId);
                if (semesterGPA != null)
                    student.SemesterGPA = semesterGPA.GPA;

                var totalGPA = totalGPAs.FirstOrDefault(x => x.StudentUserId == studentOrgInfo.StudentBasicInfo.ApplicationUserId);
                if (totalGPA != null)
                    student.TotalGPA = totalGPA.TotalGPA;

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
