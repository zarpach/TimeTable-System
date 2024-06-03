using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Reports;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.UserInfo;
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


namespace iuca.Application.Services.Reports
{
    public class StudentRegistrationReportService : IStudentRegistrationReportService
    {
        private readonly IApplicationDbContext _db;
        private readonly ISemesterService _semesterService;
        private readonly IDepartmentService _departmentService;
        private readonly IStudentInfoService _studentInfoService;
        private readonly IDeanService _deanService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IAdviserStudentService _adviserStudentService; 
        private readonly IMapper _mapper;
        private readonly IStudyCardService _studyCardService;

        public StudentRegistrationReportService(IApplicationDbContext db,
            ISemesterService semesterService,
            IDepartmentService departmentService,
            IStudentInfoService studentInfoService,
            IDeanService deanService,
            ApplicationUserManager<ApplicationUser> userManager,
            IAdviserStudentService adviserStudentService,
            IMapper mapper,
            IStudyCardService studyCardService) 
        {
            _db = db;
            _semesterService = semesterService;
            _departmentService = departmentService;
            _studentInfoService = studentInfoService;
            _deanService = deanService;
            _userManager = userManager;
            _adviserStudentService = adviserStudentService;
            _mapper = mapper;
            _studyCardService = studyCardService;
        }

        /// <summary>
        /// Get student registrations report by departments
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns></returns>
        public StudentRegistrationReportViewModel GetStudentRegistrationsReport(int organizationId, int semesterId, string deanUserId) 
        {
            StudentRegistrationReportViewModel model = new StudentRegistrationReportViewModel();
            var semester = _semesterService.GetSemester(organizationId, semesterId);
            var departments = new List<DepartmentDTO>();

            if (!string.IsNullOrEmpty(deanUserId))
            {
                departments = _deanService.GetDeanDepartments(organizationId, deanUserId).Departments;
            }
            else
                departments = _departmentService.GetDepartments(organizationId).ToList();

            foreach (var department in departments) 
            {
                var studentIds = _studentInfoService.GetStudentIdsByDepartment(organizationId, department.Id);
                if (studentIds.Count > 0) 
                {
                    var registrations = _db.StudentCourseRegistrations.Where(x => x.SemesterId == semesterId &&
                        x.OrganizationId == organizationId && studentIds.Contains(x.StudentUserId)).ToList();

                    var submittedRegistrationsCount = registrations.Where(x => (x.State == (int)enu_RegistrationState.Submitted && 
                        x.AddDropState == (int)enu_RegistrationState.NotSent) || x.AddDropState == (int)enu_RegistrationState.Submitted).Count();

                    var notSentRegistrationsCount = registrations.Where(x => x.State == (int)enu_RegistrationState.NotSent).Count();

                    DepartmentRegistrationReportViewModel departmentModel = new DepartmentRegistrationReportViewModel();
                    departmentModel.Department = department;
                    departmentModel.TotalActiveStudents = studentIds.Count;
                    departmentModel.SubmittedRegistrations = submittedRegistrationsCount;
                    departmentModel.NotSubmittedRegistrtations = studentIds.Count - submittedRegistrationsCount;
                    departmentModel.NotSentRegistrtations = notSentRegistrationsCount + (studentIds.Count - registrations.Count);

                    model.DepartmentRegistrations.Add(departmentModel);
                }
            }

            return model;
        }

        /// <summary>
        /// Get student registrations detailed report by departments
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="registrationState">Registration state</param>
        /// <returns>Student registration detailed report model</returns>
        public StudentRegistrationDetailedReportViewModel GetStudentRegistrationsDetailedReport(int organizationId, 
            int semesterId, string deanUserId, int? departmentId, enu_RegistrationState registrationState)
        {
            StudentRegistrationDetailedReportViewModel model = new StudentRegistrationDetailedReportViewModel();
            var semester = _semesterService.GetSemester(organizationId, semesterId);

            var departmentIds = GetDepartmentIds(departmentId, organizationId, deanUserId);

            var studentOrgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                .Include(x => x.StudentBasicInfo)
                .Include(x => x.PrepDepartmentGroup).ThenInclude(x => x.Department)
                .Where(x => x.OrganizationId == organizationId && x.State == (int)enu_StudentState.Active &&
                    x.StudentBasicInfo != null && (departmentIds.Contains(x.DepartmentGroup.DepartmentId) ||
                        x.PrepDepartmentGroup != null && departmentIds.Contains(x.PrepDepartmentGroup.DepartmentId)))
                .ToList();

            var studentRegistrations = GetRegistrations(studentOrgInfo, semesterId, organizationId);

            foreach (var orgInfo in studentOrgInfo)
            {
                var registration = studentRegistrations.FirstOrDefault(x => x.StudentUserId == orgInfo.StudentBasicInfo.ApplicationUserId);

                if (!PassRegStateFilter(registration, registrationState))
                    continue;
                
                StudentReportRowViewModel studentRowModel = new StudentReportRowViewModel();
                studentRowModel.Department = orgInfo.PrepDepartmentGroup != null ? orgInfo.PrepDepartmentGroup.Department.Code 
                                                        : orgInfo.DepartmentGroup.Department.Code;
                studentRowModel.StudentName = _userManager.GetUserFullName(orgInfo.StudentBasicInfo.ApplicationUserId);
                studentRowModel.Group = GetDepartmentGroup(orgInfo);
                studentRowModel.RegistrationState = enu_RegistrationState.NotSent;
                studentRowModel.AddDropState = enu_RegistrationState.NotSent;

                if (registration != null) 
                {
                    studentRowModel.RegistrationState = (enu_RegistrationState)registration.State;
                    studentRowModel.AddDropState = (enu_RegistrationState)registration.AddDropState;
                }

                studentRowModel.AdviserName = GetAdviserNames(orgInfo.StudentBasicInfo.ApplicationUserId);

                model.StudentReportRows.Add(studentRowModel);  
            }

            return model;
        }
        private List<int> GetDepartmentIds(int? departmentId, int organizationId, string deanUserId) 
        {
            var departmentIds = new List<int>();

            if (departmentId != null)
                departmentIds = new List<int>() { _departmentService.GetDepartment(organizationId, departmentId.Value).Id };
            else if (!string.IsNullOrEmpty(deanUserId))
                departmentIds = _deanService.GetDeanDepartments(organizationId, deanUserId)
                    .Departments.Select(x => x.Id).ToList();
            else
                departmentIds = _departmentService.GetDepartments(organizationId).Select(x => x.Id).ToList();

            return departmentIds;
        }

        private List<StudentCourseRegistration> GetRegistrations(List<StudentOrgInfo> studentOrgInfo, 
            int semesterId, int organizationId) 
        {
            var studentIds = studentOrgInfo.Select(x => x.StudentBasicInfo.ApplicationUserId).ToList();

            var studentRegistrations = _db.StudentCourseRegistrations.Where(x => x.SemesterId == semesterId &&
                        x.OrganizationId == organizationId && studentIds.Contains(x.StudentUserId));

            return studentRegistrations.ToList();
        }

        private bool PassRegStateFilter(StudentCourseRegistration registration, enu_RegistrationState registrationState)
        {
            bool passed = true;
            if (registrationState != enu_RegistrationState.NotSpecified)
            {
                if (registration == null)
                {
                    if (registrationState != enu_RegistrationState.NotSent)
                        passed = false;
                }
                else
                {
                    if (registration.State != (int)enu_RegistrationState.Submitted)
                    {
                        if (registration.State != (int)registrationState)
                            passed = false;
                    }
                    else
                    {
                        if (registration.AddDropState == (int)enu_RegistrationState.NotSent)
                        {
                            if (registrationState == enu_RegistrationState.NotSent)
                                passed = false;
                            if (registrationState != enu_RegistrationState.Submitted &&
                                registration.AddDropState != (int)registrationState)
                                passed = false;
                        }
                        else
                        {
                            if (registration.AddDropState != (int)registrationState)
                                passed = false;
                        }
                    }
                }
            }
            return passed;
        }

        private string GetDepartmentGroup(StudentOrgInfo orgInfo)
        {
            string departmentGroup = $"{orgInfo.DepartmentGroup.Department.Code}{orgInfo.DepartmentGroup.Code}";
            if (orgInfo.DepartmentGroup.Department.Code == "PREP" && orgInfo.PrepDepartmentGroup != null)
            {
                departmentGroup += $" / {orgInfo.PrepDepartmentGroup.Department.Code}{orgInfo.PrepDepartmentGroup.Code}";
            }

            return departmentGroup;
        }

        private string GetAdviserNames(string studentUserId) 
        {
            string adviserName = "";
            
            var advisers = _db.AdviserStudents.Where(x => x.StudentUserId == studentUserId).ToList();
            if (advisers.Count > 0) 
            {
                List<string> adviserNameList = new List<string>();
                foreach (var adviser in advisers) 
                {
                    adviserNameList.Add(_userManager.GetUserFullName(adviser.InstructorUserId));
                }
                adviserName = String.Join(", ", adviserNameList);
            }

            return adviserName;
        }

        /// <summary>
        /// Get advisers and their students 
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <returns>Model with dean, advisers and their students</returns>
        public DeanAdviserStudentReportViewModel DeanAdviserStudentsReport(int organizationId, string deanUserId, string adviserUserId)
        {
            DeanAdviserStudentReportViewModel model = new DeanAdviserStudentReportViewModel();

            List<string> adviserIds = new List<string>();
            if (!string.IsNullOrEmpty(adviserUserId))
                adviserIds.Add(adviserUserId);
            else
                adviserIds = _db.DeanAdvisers.Where(x => x.OrganizationId == organizationId &&
                    x.DeanUserId == deanUserId).Select(x => x.AdviserUserId).ToList();

            if (adviserIds.Count == 0)
                return model;

            var advisers = _userManager.Users.Where(x => adviserIds.Contains(x.Id)).ToList();

            var adviserStudents = _db.AdviserStudents.Where(x => x.OrganizationId == organizationId &&
                adviserIds.Contains(x.InstructorUserId)).ToList();

            if (adviserStudents.Count == 0)
                return model;

            var students = _userManager.Users
                .Where(x => adviserStudents.Select(x => x.StudentUserId).Contains(x.Id))
                .ToList();

            var studentOrgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .Include(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .Include(x => x.PrepDepartmentGroup)
                .ThenInclude(x => x.Department)
                .Where(x => adviserStudents.Select(x => x.StudentUserId)
                .Contains(x.StudentBasicInfo.ApplicationUserId) && x.OrganizationId == organizationId)
                .ToList();

            foreach (var adviser in advisers)
            {
                AdviserStudentRowViewModel adviserStudentRow = new AdviserStudentRowViewModel();
                adviserStudentRow.Adviser = adviser;

                var _adviserStudents = adviserStudents.Where(x => x.InstructorUserId == adviser.Id).ToList();
                foreach (var adviserStudent in _adviserStudents)
                {
                    StudentRowViewModel studentRow = new StudentRowViewModel();
                    studentRow.Student = students.FirstOrDefault(x => x.Id == adviserStudent.StudentUserId);
                    var orgInfo = studentOrgInfo.FirstOrDefault(x => x.StudentBasicInfo.ApplicationUserId == adviserStudent.StudentUserId);
                    if (orgInfo != null)
                    {
                        studentRow.DepartmentGroup = $"{orgInfo.DepartmentGroup.Department.Code}{orgInfo.DepartmentGroup.Code}";
                        if (orgInfo.DepartmentGroup.Department.Code.StartsWith("PREP") &&
                            orgInfo.PrepDepartmentGroup != null)
                        {
                            studentRow.DepartmentGroup += $"/{orgInfo.PrepDepartmentGroup.Department.Code}{orgInfo.PrepDepartmentGroup.Code}";
                        }
                        studentRow.State = (enu_StudentState)orgInfo.State;
                    }

                    adviserStudentRow.Students.Add(studentRow);
                }
                model.AdviserStudents.Add(adviserStudentRow);
            }

            return model;
        }

        /// <summary>
        /// Get students without adviser by dean user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns>List of students without adviser</returns>
        public List<StudentInfoBriefViewModel> StudentsWithoutAdviser(int organizationId, string deanUserId)
        {
            List<StudentInfoBriefViewModel> students = new List<StudentInfoBriefViewModel>();

            var deanDepartmentIds = _db.DeanDepartments.Where(x => x.OrganizationId == organizationId &&
                    x.DeanUserId == deanUserId).Select(x => x.DepartmentId).ToList();

            if (deanDepartmentIds.Count == 0)
                return students;

            var studentOrgInfo = _db.StudentOrgInfo
                .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                .Include(x => x.PrepDepartmentGroup).ThenInclude(x => x.Department)
                .Include(x => x.StudentBasicInfo)
                .Where(x => x.OrganizationId == organizationId && x.State == (int)enu_StudentState.Active &&
                    x.DepartmentGroup != null && deanDepartmentIds.Contains(x.DepartmentGroup.DepartmentId))
                .ToList();

            if (studentOrgInfo.Count == 0)
                return students;

            var studentUserIds = studentOrgInfo.Select(x => x.StudentBasicInfo.ApplicationUserId).ToList();

            var studentsWithAdsviserIds = _db.AdviserStudents.Where(x => x.OrganizationId == organizationId && 
                studentUserIds.Contains(x.StudentUserId)).Select(x => x.StudentUserId).ToList();

            var studentsWithoutAdviserIds = studentUserIds.Except(studentsWithAdsviserIds).ToList();

            var studentsWithoutAdviser = _userManager.Users.Where(x => studentsWithoutAdviserIds.Contains(x.Id)).ToList();

            foreach (var studentUser in studentsWithoutAdviser)
            {
                StudentInfoBriefViewModel model = new StudentInfoBriefViewModel();
                model.StudentUserId = studentUser.Id;
                model.FullNameEng = studentUser.FullNameEng;
                model.Email = studentUser.Email;
                var orgInfo = studentOrgInfo.FirstOrDefault(x => x.StudentBasicInfo.ApplicationUserId == studentUser.Id);
                if (orgInfo != null) 
                {
                    model.DepartmentGroup = orgInfo.DepartmentGroup.Department.Code + orgInfo.DepartmentGroup.Code;
                    if (orgInfo.DepartmentGroup.Department.Code.StartsWith("PREP") && orgInfo.PrepDepartmentGroup != null) 
                        model.DepartmentGroup += $"/{orgInfo.PrepDepartmentGroup.Department.Code + orgInfo.PrepDepartmentGroup.Code}";
                    model.State = (enu_StudentState)orgInfo.State;
                }

                students.Add(model);
            }

            return students;
        }

        /// <summary>
        /// Get course registration adviser report
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="onlyActiveStudents">Only active students</param>
        /// <returns>Course registration report model</returns>
        public CourseRegistrationAdviserReportViewModel CourseRegistrationAdviserReport(int organizationId, int semesterId,
            string adviserUserId, int departmentGroupId, bool onlyActiveStudents = true)
        {
            var model = new CourseRegistrationAdviserReportViewModel();
            model.Semester = _semesterService.GetSemester(organizationId, semesterId);
            model.AdviserUserId = adviserUserId;
            model.AdviserName = _userManager.GetUserFullName(adviserUserId);

            var adviserStudents = GetAdviserStudents(organizationId, adviserUserId, departmentGroupId, onlyActiveStudents);

            var studentIds = adviserStudents.Select(x => x.StudentUserId).ToList();

            var allStudentCourses = _db.StudentCoursesTemp
                    .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course)
                    .Include(x => x.StudentCourseRegistration)
                    .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId &&
                            x.StudentCourseRegistration.SemesterId == semesterId &&
                            studentIds.Contains(x.StudentCourseRegistration.StudentUserId) &&
                            x.State != (int)enu_CourseState.Dropped)
                    .ToList();

            model.AllCourses.AddRange(GetAllCourses(semesterId, departmentGroupId, allStudentCourses));
            model.AllStudents.AddRange(GetAllStudents(organizationId, adviserStudents, allStudentCourses));

            foreach (var studentCourse in allStudentCourses)
            {
                var index = $"{studentCourse.StudentCourseRegistration.StudentUserId}_{studentCourse.AnnouncementSection.CourseId}";
                model.AllStudentCourses.Add(index, _mapper.Map<StudentCourseTempDTO>(studentCourse));
            }

            return model;
        }

        private List<AdviserStudentViewModel> GetAdviserStudents(int organizationId, string adviserUserId,
            int departmentGroupId, bool onlyActiveStudents) 
        {
            var adviserStudents = _adviserStudentService.GetAdviserStudentsByInstuctorId(organizationId, adviserUserId);

            adviserStudents = adviserStudents.Where(x => x.DepartmentGroupId == departmentGroupId ||
                x.PrepDepartmentGroupId == departmentGroupId).ToList();

            if (onlyActiveStudents)
                adviserStudents = adviserStudents.Where(x => x.State == enu_StudentState.Active).ToList();

            return adviserStudents;
        } 

        private List<CourseRegistrationAdviserReportCourse> GetAllCourses(int semesterId, int departmentGroupId,
            List<StudentCourseTemp> allStudentCourses) 
        {
            var courses = new List<CourseRegistrationAdviserReportCourse>();

            var studyCardCoursesIds = new List<int>();
            var studyCard = _studyCardService.GetStudyCard(semesterId, departmentGroupId);
            if (studyCard != null)
                studyCardCoursesIds = studyCard.StudyCardCourses.Select(x => x.AnnouncementSection.CourseId).ToList();

            foreach (var course in allStudentCourses.GroupBy(x => x.AnnouncementSection.Course)
                    .Select(x => new 
                    { 
                        Course = x.Key, 
                        Credits = x.Select(y => y.AnnouncementSection.Credits).Distinct().ToList() 
                    }).ToList())
            {
                var courseModel = new CourseRegistrationAdviserReportCourse();
                courseModel.Course = _mapper.Map<CourseDTO>(course.Course);
                courseModel.Credits = String.Join(", ", course.Credits);
                courseModel.IsFromStudyCard = studyCardCoursesIds.Contains(course.Course.Id);
                courses.Add(courseModel);
            }

            return courses.OrderByDescending(x => x.IsFromStudyCard).ThenBy(x => x.Course.Id).ToList();
        }

        private List<CourseRegistrationAdviserReportStudent> GetAllStudents(int organizationId, 
            List<AdviserStudentViewModel> adviserStudents, List<StudentCourseTemp> allStudentCourses) 
        {
            var students = new List<CourseRegistrationAdviserReportStudent>();
            
            var studentRegistrations = allStudentCourses.Select(x => x.StudentCourseRegistration).ToList();
            foreach (var student in adviserStudents)
            {
                var studentRow = new CourseRegistrationAdviserReportStudent();
                studentRow.StudentInfo = student;
                studentRow.RegistrationState = enu_RegistrationState.NotSent;
                var registration = studentRegistrations.FirstOrDefault(x => x.StudentUserId == student.StudentUserId);
                if (registration != null)
                    studentRow.RegistrationState = (enu_RegistrationState)registration.AddDropState != enu_RegistrationState.NotSent ?
                        (enu_RegistrationState)registration.AddDropState : (enu_RegistrationState)registration.State;

                studentRow.SemesterCredits = allStudentCourses.Where(x => x.StudentCourseRegistration.StudentUserId == student.StudentUserId
                    && x.State != (int)enu_CourseState.Dropped && !x.IsAudit).Sum(x => x.AnnouncementSection.Credits);

                studentRow.EarnedCredits = GetEarnedCredits(organizationId, student.StudentUserId);

                students.Add(studentRow);
            }
            return students;
        }

        private float GetEarnedCredits(int organizationId, string studentUserId) 
        {
            var courseCredits = _db.StudentCourseGrades
                        .Include(x => x.Course)
                        .Include(x => x.Grade)
                        .Where(x => x.StudentUserId == studentUserId && x.Grade != null && x.Grade.GradeMark != "*" &&
                            x.Grade.Gpa != -1 && x.Grade.Gpa != -3 && x.Grade.Gpa != 0).Sum(x => x.Points);

            var transferCourseCredits = _db.TransferCourses.Where(x => x.OrganizationId == organizationId &&
                                    x.StudentUserId == studentUserId).Sum(x => x.Points);

            return courseCredits + transferCourseCredits;
        }

        /// <summary>
        /// Get registration course report
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="courseTypes">Course type list</param>
        /// <param name="maxQty">Max quantity</param>
        /// <returns>Report model</returns>
        public List<RegistrationCourseReportViewModel> GetRegistrationCoursesReport(int semesterId, int? departmentId,
            List<enu_CourseType> courseTypes, int maxQty = -1)
        {
            var model = new List<RegistrationCourseReportViewModel>();

            var sections = _db.AnnouncementSections
                .Include(x => x.Announcement).ThenInclude(x => x.Course).ThenInclude(x => x.Language)
                .Include(x => x.Announcement).ThenInclude(x => x.Course).ThenInclude(x => x.Department)
                .Include(x => x.StudentCourses).ThenInclude(x => x.StudentCourseRegistration)
                .Where(x => x.Announcement.SemesterId == semesterId && x.Announcement.IsActivated);

            if (departmentId != null)
                sections = sections.Where(x => x.Course.DepartmentId == departmentId.Value);

            sections = sections.Where(x => courseTypes.Contains((enu_CourseType)x.Announcement.Course.CourseType));

            if (maxQty >= 0)
                sections = sections.Where(x => x.StudentCourses.Count() <= maxQty);

            foreach (var section in sections.ToList())
            {
                var row = new RegistrationCourseReportViewModel();
                row.AnnouncementSection = _mapper.Map<AnnouncementSectionDTO>(section);
                row.InstructorName = _userManager.GetUserFullName(section.InstructorUserId);
                row.TotalStudents = section.StudentCourses.Count(x => x.State != (int)enu_CourseState.Dropped);
                row.TotalAudits = section.StudentCourses.Count(x => x.State != (int)enu_CourseState.Dropped && x.IsAudit);
                row.NotSubmittedRegistration = section.StudentCourses.Count(x => x.State != (int)enu_CourseState.Dropped && 
                    (x.StudentCourseRegistration.State != (int)enu_RegistrationState.Submitted ||
                    x.StudentCourseRegistration.State == (int)enu_RegistrationState.Submitted &&
                    x.StudentCourseRegistration.AddDropState != (int)enu_RegistrationState.NotSent &&
                    x.StudentCourseRegistration.AddDropState != (int)enu_RegistrationState.Submitted));

                model.Add(row);
            }

            return model;
        }
    }
}
