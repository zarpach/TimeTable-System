using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Application.ViewModels.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using iuca.Application.Enums;
using Microsoft.EntityFrameworkCore;
using iuca.Domain.Entities.Common;
using iuca.Application.DTO.Users.Students;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users;

namespace iuca.Application.Services.Courses
{
    public class StudentCourseRegistrationService : IStudentCourseRegistrationService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IStudentOrgInfoService _studentOrgInfoService;
        private readonly ISemesterService _semesterService;
        private readonly ISemesterPeriodService _semesterPeriodService;
        private readonly IStudentDebtService _studentDebtService;
        private readonly IUserTypeOrganizationService _userTypeOrganizationService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IEnvarSettingService _envarSettingService;
        private readonly IStudyCardService _studyCardService;

        public StudentCourseRegistrationService(IApplicationDbContext db,
            IMapper mapper,
            IStudentOrgInfoService studentOrgInfoService,
            ISemesterService semesterService,
            ISemesterPeriodService semesterPeriodService,
            IStudentDebtService studentDebtService,
            IUserTypeOrganizationService userTypeOrganizationService,
            ApplicationUserManager<ApplicationUser> userManager,
            IEnvarSettingService envarSettingService,
            IStudyCardService studyCardService)
        {
            _db = db;
            _mapper = mapper;
            _studentOrgInfoService = studentOrgInfoService;
            _semesterService = semesterService;
            _semesterPeriodService = semesterPeriodService;
            _studentDebtService = studentDebtService;
            _userTypeOrganizationService = userTypeOrganizationService;
            _userManager = userManager;
            _envarSettingService = envarSettingService;
            _studyCardService = studyCardService;
        }

        #region Student course registration list

        /// <summary>
        /// Get student course registration list
        /// </summary>
        /// <param name="organizationId">Selected organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="minCredits">Minimum registration credits</param>
        /// <param name="registrationState">Registration state</param>
        /// <param name="studentState">Student state</param>
        /// <returns>Student course registration list</returns>
        public IEnumerable<StudentCourseRegistrationBriefViewModel> GetStudentCourseRegistrations(int organizationId, 
            int semesterId, int? departmentId, int? minCredits, enu_RegistrationState registrationState, 
            enu_StudentState studentState)
        {
            List<StudentCourseRegistrationBriefViewModel> model = new List<StudentCourseRegistrationBriefViewModel>();
            
            var studentRegistrations = GetRegistrations(semesterId, organizationId, minCredits);
            var studentOrgInfo = GetStudentOrgInfo(studentRegistrations, organizationId, departmentId, studentState);

            foreach (var orgInfo in studentOrgInfo)
            {
                var registration = studentRegistrations.FirstOrDefault(x => x.StudentUserId == orgInfo.StudentBasicInfo.ApplicationUserId);

                if (!PassRegStateFilter(registration, registrationState))
                    continue;

                StudentCourseRegistrationBriefViewModel studentRowModel = new StudentCourseRegistrationBriefViewModel();
                studentRowModel.Department = orgInfo.PrepDepartmentGroup != null ? orgInfo.PrepDepartmentGroup.Department.Code
                                                        : orgInfo.DepartmentGroup.Department.Code;
                studentRowModel.StudentName = _userManager.GetUserFullName(orgInfo.StudentBasicInfo.ApplicationUserId);
                studentRowModel.Group = GetDepartmentGroup(orgInfo);
                studentRowModel.StudentState = (enu_StudentState)orgInfo.State;
                studentRowModel.RegistrationState = enu_RegistrationState.NotSent;
                studentRowModel.AddDropState = enu_RegistrationState.NotSent;

                if (registration != null)
                {
                    studentRowModel.StudentRegistrationId = registration.Id;
                    studentRowModel.RegistrationState = (enu_RegistrationState)registration.State;
                    studentRowModel.AddDropState = (enu_RegistrationState)registration.AddDropState;
                    studentRowModel.Credits = registration.StudentCoursesTemp
                        .Where(x => x.State != (int)enu_CourseState.Dropped && !x.IsAudit)
                        .Sum(x => x.AnnouncementSection.Credits);
                    studentRowModel.TotalCourses = registration.StudentCoursesTemp
                        .Count(x => x.State != (int)enu_CourseState.Dropped);
                    studentRowModel.DateCreate = registration.DateCreate;
                    studentRowModel.DateChange = registration.DateChange;
                    studentRowModel.NoCreditsLimitation = registration.NoCreditsLimitation;
                }

                model.Add(studentRowModel);
            }

            return model;
        }

        private List<StudentCourseRegistration> GetRegistrations(int semesterId, int organizationId, int? minCredits)
        {
            var studentRegistrations = _db.StudentCourseRegistrations.Include(x => x.StudentCoursesTemp)
                .ThenInclude(x => x.AnnouncementSection)
                .Where(x => x.SemesterId == semesterId && x.OrganizationId == organizationId);

            if (minCredits != null && minCredits.Value > 0) 
            {
                studentRegistrations = studentRegistrations.Where(x => x.StudentCoursesTemp
                    .Where(x => x.State != (int)enu_CourseState.Dropped && !x.IsAudit)
                    .Sum(x => x.AnnouncementSection.Credits) >= minCredits.Value);
            }

            return studentRegistrations.ToList();
        }

        private List<StudentOrgInfo> GetStudentOrgInfo(List<StudentCourseRegistration> studentRegistrations, int organizationId, 
            int? departmentId, enu_StudentState studentState)
        {
            var studentUserIds = studentRegistrations.Select(x => x.StudentUserId).ToList();
            var studentOrgInfoQuery = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                .Include(x => x.StudentBasicInfo)
                .Include(x => x.PrepDepartmentGroup).ThenInclude(x => x.Department)
                .Where(x => x.OrganizationId == organizationId && x.StudentBasicInfo != null 
                    && studentUserIds.Contains(x.StudentBasicInfo.ApplicationUserId));

            if (studentState != enu_StudentState.NotAssigned)
                studentOrgInfoQuery = studentOrgInfoQuery.Where(x => x.State == (int)studentState);

            if (departmentId != null)
                studentOrgInfoQuery = studentOrgInfoQuery.Where(x => x.DepartmentGroup.DepartmentId == departmentId.Value ||
                        x.PrepDepartmentGroup != null && x.PrepDepartmentGroup.DepartmentId == departmentId.Value);

            return studentOrgInfoQuery.ToList();
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

        #endregion

        /// <summary>
        /// Set no credit limitation flag to registration
        /// </summary>
        /// <param name="registrationId">Registration id</param>
        /// <param name="noCreditLimitation">No credit limitation flag</param>
        public void SetNoCreditsLimitation(int registrationId, bool noCreditLimitation)
        {
            var studentCourseRegistration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == registrationId);
            studentCourseRegistration.NoCreditsLimitation = noCreditLimitation;
            _db.SaveChanges();
        }

        /// <summary>
        /// Get student course registration by id
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        /// <returns>Student course registration model</returns>
        public StudentCourseRegistrationDTO GetStudentCourseRegistration(int id)
        {
            StudentCourseRegistration studentCourseRegistration = _db.StudentCourseRegistrations
                .Include(x => x.Semester)
                .Include(x => x.StudentCoursesTemp).ThenInclude(x => x.AnnouncementSection).ThenInclude(x => x.Course)
                .FirstOrDefault(x => x.Id == id);
            if (studentCourseRegistration == null)
                throw new Exception($"Student course registration with id {id} not found");

            return _mapper.Map<StudentCourseRegistrationDTO>(studentCourseRegistration);
        }

        /// <summary>
        /// Get student course registration by semester id and student id
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student course registration model</returns>
        public StudentCourseRegistrationDTO GetStudentCourseRegistration(int semesterId, string studentUserId)
        {
            StudentCourseRegistration studentCourseRegistration = _db.StudentCourseRegistrations
                .Include(x => x.Semester)
                .FirstOrDefault(x => x.SemesterId == semesterId && x.StudentUserId == studentUserId);

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Semester, SemesterDTO>();
                cfg.CreateMap<StudentCourseRegistration, StudentCourseRegistrationDTO>();
            }).CreateMapper();
            
            return mapper.Map<StudentCourseRegistration, StudentCourseRegistrationDTO>(studentCourseRegistration);
        }

        /// <summary>
        /// Set registration state
        /// </summary>
        /// <param name="registrationId">Registration id</param>
        /// <param name="state">State</param>
        public void SetRegistrationState(int registrationId, enu_RegistrationState state) 
        {
            var studentCourseRegistration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == registrationId);
            studentCourseRegistration.State = (int)state;
            studentCourseRegistration.IsApproved = (state == enu_RegistrationState.Approved || 
                    state == enu_RegistrationState.Submitted);
            _db.SaveChanges();
        }

        /// <summary>
        /// Set add/drop state
        /// </summary>
        /// <param name="registrationId">Registration id</param>
        /// <param name="state">State</param>
        public void SetAddDropState(int registrationId, enu_RegistrationState state)
        {
            var studentCourseRegistration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == registrationId);
            studentCourseRegistration.AddDropState = (int)state;
            studentCourseRegistration.IsAddDropApproved = (state == enu_RegistrationState.Approved ||
                    state == enu_RegistrationState.Submitted);
            _db.SaveChanges();
        }

        /// <summary>
        /// Create student course registration
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        public void Create(int organizationId, int semesterId, string studentUserId)
        {
            if (semesterId == 0)
                throw new ArgumentException("Wrong semester id");

            if (string.IsNullOrEmpty(studentUserId))
                throw new ArgumentException("Wrong student user id");

            var existingRegistration = _db.StudentCourseRegistrations
                            .FirstOrDefault(x => x.StudentUserId == studentUserId &&
                                x.SemesterId == semesterId);

            if (existingRegistration != null)
                throw new Exception("Registation already exists");

            StudentCourseRegistration newRegistration = new StudentCourseRegistration();
            newRegistration.OrganizationId = organizationId;
            newRegistration.DateCreate = DateTime.Now;
            newRegistration.SemesterId = semesterId;
            newRegistration.StudentUserId = studentUserId;
            newRegistration.State = (int)enu_RegistrationState.NotSent;
            _db.StudentCourseRegistrations.Add(newRegistration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Create student course registration
        /// </summary>
        /// <param name="studentCourseRegistrationDTO">Student course registration model</param>
        public void Create(StudentCourseRegistrationDTO studentCourseRegistrationDTO)
        {
            if (studentCourseRegistrationDTO == null)
                throw new Exception($"studentCourseRegistrationDTO is null");

            var existingRegistration = _db.StudentCourseRegistrations
                            .FirstOrDefault(x => x.StudentUserId == studentCourseRegistrationDTO.StudentUserId &&
                                x.SemesterId == studentCourseRegistrationDTO.SemesterId);

            if(existingRegistration != null)
                throw new Exception("Registation already exists");

            var mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<StudentCourseRegistration, StudentCourseRegistrationDTO>()).CreateMapper();
            var mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<StudentCourseRegistrationDTO, StudentCourseRegistration>()).CreateMapper();

            StudentCourseRegistration newStudentCourseRegistration = mapperFromDTO.Map<StudentCourseRegistrationDTO, StudentCourseRegistration>(studentCourseRegistrationDTO);

            _db.StudentCourseRegistrations.Add(newStudentCourseRegistration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit student course registration
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        /// <param name="studentCourseRegistrationDTO">Student course registration model</param>
        public void Edit(int id, StudentCourseRegistrationDTO studentCourseRegistrationDTO)
        {
            if (studentCourseRegistrationDTO == null)
                throw new Exception($"studentCourseRegistrationDTO is null");

            StudentCourseRegistration studentCourseRegistration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == id);
            if (studentCourseRegistration == null)
                throw new Exception($"Student course registration with id {id} not found");

            studentCourseRegistration.State = studentCourseRegistrationDTO.State;

            _db.StudentCourseRegistrations.Update(studentCourseRegistration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete student course registration by id
        /// </summary>
        /// <param name="id">Id of student course registration</param>
        public void Delete(int id)
        {
            StudentCourseRegistration studentCourseRegistration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == id);
            if (studentCourseRegistration == null)
                throw new Exception($"Student course registration with id {id} not found");

            _db.StudentCourseRegistrations.Remove(studentCourseRegistration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get registration courses model for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="onlySelectedCourses">If true gets courses selected by student only</param>
        /// <returns>Registration courses model</returns>
        public RegistrationSelectionCourseViewModel GetCoursesForSelectionTemp(int organizationId, int semesterId, string studentUserId, bool onlySelectedCourses)
        {
            RegistrationSelectionCourseViewModel model = new RegistrationSelectionCourseViewModel();

            var semester = _semesterService.GetSemester(organizationId, semesterId);

            var studentCourseList = GetStudentCourses(semester, onlySelectedCourses, studentUserId);
            
            var registrationCourseList = GetRegistrationCourses(studentCourseList, 
                semester, organizationId, onlySelectedCourses);

            var courseList = GetCourseRows(registrationCourseList, studentCourseList);

            var selectedCourses = studentCourseList
                .Where(x => x.StudentCourseRegistration.StudentUserId == studentUserId).ToList();

            foreach (var selectedCourse in selectedCourses) 
            {
                var course = courseList.FirstOrDefault(x => x.RegistrationCourseId == selectedCourse.AnnouncementSectionId);
                if (course != null)
                {
                    course.IsSelected = true;
                    course.Comment = selectedCourse.Comment;
                    course.IsProcessed = selectedCourse.IsProcessed;
                    course.IsApproved = selectedCourse.IsApproved;
                    course.Queue = GetStudentQueue(studentCourseList, course.RegistrationCourseId, 
                        course.TotalPlaces, studentUserId);
                    course.State = (enu_CourseState)selectedCourse.State;
                    course.IsAudit = selectedCourse.IsAudit;
                    course.IsForAll = course.IsForAll;
                    course.NoCreditsCount = selectedCourse.AnnouncementSection.Course.CourseType == (int)enu_CourseType.PhysEd;
                }
            }

            model.SelectedCoursesNumber = selectedCourses.Where(x => x.State != (int)enu_CourseState.Dropped).Count();
            model.TotalPoints = (int)selectedCourses.Where(x => x.State != (int)enu_CourseState.Dropped && 
                !x.IsAudit && x.AnnouncementSection.Course.CourseType != (int)enu_CourseType.PhysEd)
                .Sum(x => x.AnnouncementSection.Credits);
            model.TotalNoCreditsCount = (int)selectedCourses.Where(x => x.State != (int)enu_CourseState.Dropped &&
                !x.IsAudit && x.AnnouncementSection.Course.CourseType == (int)enu_CourseType.PhysEd)
                .Sum(x => x.AnnouncementSection.Credits);
            model.Courses = courseList.OrderBy(x => x.Course.Abbreviation).ThenBy(x => x.Course.Number).ToList();

            return model;
        }

        /// <summary>
        /// Get courses list from study card
        /// </summary>
                /// <param name="semesterId">Semester id</param>
        /// <param name="departmentGroupId">Student user id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="studentCourseRegistrationId">StudentCourseRegistration Id</param>
        /// <returns>Courses list from study card</returns>
        public StudyCardCoursesViewModel GetCoursesForSelectionFromStudyCard(int semesterId,
            int departmentGroupId, string studentUserId, int studentCourseRegistrationId)
        {
            var studyCard = _studyCardService.GetStudyCard(semesterId, departmentGroupId);
            
            if (studyCard == null)
                return null;

            studyCard.StudyCardCourses = studyCard.StudyCardCourses
                .Where(x => x.AnnouncementSection.Announcement.IsActivated).ToList();

            StudyCardCoursesViewModel model = new StudyCardCoursesViewModel();
            model.Semester = studyCard.Semester;
            model.DepartmentGroup = studyCard.DepartmentGroup;
            model.StudentCourseRegistrationId = studentCourseRegistrationId;
            model.StudyCardId = studyCard.Id;
            model.DisplayIUCAElectives = studyCard.DisplayIUCAElectives;

            var selectedCourses = GetStudentCourses(studyCard.Semester, true, studentUserId);

            model.StudyCardCourses.AddRange(GetStudyCardCourses(studentUserId, studyCard, selectedCourses));
            model.ElectiveCourses.AddRange(GetElectiveCourses(semesterId, studentUserId, 
                model.StudyCardCourses, selectedCourses));

            return model;
        }

        private List<StudentCourseTemp> GetStudentCourses(SemesterDTO semester, bool onlySelectedCourses, string studentUserId) 
        {
            List<StudentCourseTemp> studentCourseList = new List<StudentCourseTemp>();

            var studentCourseQuery = _db.StudentCoursesTemp
                    .Include(x => x.AnnouncementSection)
                    .ThenInclude(x => x.Course)
                    .Include(x => x.AnnouncementSection)
                    .ThenInclude(x => x.ExtraInstructors)
                    .Include(x => x.AnnouncementSection)
                    .ThenInclude(x => x.Announcement)
                    .Include(x => x.StudentCourseRegistration)
                    .Where(x => x.AnnouncementSection.Season == semester.Season &&
                        x.AnnouncementSection.Year == semester.Year);

            if (onlySelectedCourses)
                studentCourseQuery = studentCourseQuery.Where(x => x.StudentCourseRegistration.StudentUserId == studentUserId);

            return studentCourseQuery.ToList();
        }

        private List<StudyCardCourseRowViewModel> GetStudyCardCourses(string studentUserId, 
            StudyCardDTO studyCard, List<StudentCourseTemp> selectedCourses) 
        {
            var studyCardCourses = new List<StudyCardCourseRowViewModel>();
            foreach (var studyCardCourse in studyCard.StudyCardCourses)
            {
                var row = new StudyCardCourseRowViewModel();
                row.AnnouncementSection = studyCardCourse.AnnouncementSection;
                row.InstructorName = _userManager.GetUserFullName(studyCardCourse.AnnouncementSection.InstructorUserId);
                row.Comment = studyCardCourse.Comment;
                row.TotalPlaces = studyCardCourse.AnnouncementSection.Places;
                row.RestPlaces = GetCourseRestPlaces(studyCardCourse.AnnouncementSection.Id, studyCardCourse.AnnouncementSection.Places);
                row.Queue = GetStudentQueue(studyCardCourse.AnnouncementSection.Id,
                    studyCardCourse.AnnouncementSection.Places, studentUserId);
                row.IsForAll = studyCardCourse.AnnouncementSection.Announcement.IsForAll;

                var selectedCourse = selectedCourses
                    .FirstOrDefault(x => x.AnnouncementSectionId == studyCardCourse.AnnouncementSection.Id);

                if (selectedCourse != null)
                {
                    row.IsSelected = true;
                    row.State = (enu_CourseState)selectedCourse.State;
                }

                studyCardCourses.Add(row);
            }

            return studyCardCourses;
        }

        private List<StudyCardCourseRowViewModel> GetElectiveCourses(int semesterId, string studentUserId,
            List<StudyCardCourseRowViewModel> studyCardCourses, List<StudentCourseTemp> selectedCourses)
        {
            var forAllSections = _studyCardService.GetForAllAnnouncementSections(semesterId);
            var forAllFromStudyCard = studyCardCourses.Select(x => x.AnnouncementSection)
                .Where(x => x.Announcement.IsForAll).Select(x => x.Id).ToList();

            if (forAllFromStudyCard.Count > 0)
                forAllSections = forAllSections.Where(x => !forAllFromStudyCard.Contains(x.Id)).ToList();

            var electiveCourses = new List<StudyCardCourseRowViewModel>();
            foreach (var section in forAllSections)
            {
                var row = new StudyCardCourseRowViewModel();
                row.AnnouncementSection = section;
                row.InstructorName = _userManager.GetUserFullName(section.InstructorUserId);
                row.TotalPlaces = section.Places;
                row.RestPlaces = GetCourseRestPlaces(section.Id, section.Places);
                row.Queue = GetStudentQueue(section.Id, section.Places, studentUserId);
                row.IsForAll = true;

                var selectedCourse = selectedCourses
                    .FirstOrDefault(x => x.AnnouncementSectionId == section.Id);

                if (selectedCourse != null)
                {
                    row.IsSelected = true;
                    row.State = (enu_CourseState)selectedCourse.State;
                }

                electiveCourses.Add(row);
            }

            return electiveCourses;
        }

        private List<AnnouncementSection> GetRegistrationCourses(List<StudentCourseTemp> studentCourseList, 
                SemesterDTO semester, int organizationId, bool onlySelectedCourses) 
        {
            List<AnnouncementSection> registrationCourseList = new List<AnnouncementSection>();

            if (onlySelectedCourses)
                registrationCourseList = studentCourseList.Select(x => x.AnnouncementSection).Distinct().ToList();
            else
                registrationCourseList = _db.AnnouncementSections.Include(x => x.Announcement)
                    .Where(x => x.OrganizationId == organizationId &&
                                    x.Season == semester.Season && x.Year == semester.Year && x.Announcement.IsActivated)
                                .Include(x => x.Course).Include(x => x.ExtraInstructors).ToList();

            return registrationCourseList;
        }

        private List<RegistrationCourseRowViewModel> GetCourseRows(List<AnnouncementSection> registrationCourseList, 
            List<StudentCourseTemp> studentCourseList) 
        {
            var courseMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<Course, CourseDTO>();
            }).CreateMapper();


            var courseList = new List<RegistrationCourseRowViewModel>();

            foreach (var course in registrationCourseList) 
            {
                var row = new RegistrationCourseRowViewModel();
                row.Course = courseMapper.Map<Course, CourseDTO>(course.Course);
                row.Points = (int)course.Credits;
                row.Schedule = course.Schedule;
                row.InstructorUserId = course.InstructorUserId;
                row.RegistrationCourseId = course.Id;
                row.TotalPlaces = course.Places;
                row.RestPlaces = GetCourseRestPlaces(studentCourseList, course.Id, course.Places);
                row.Section = course.Section;
                row.InstructorName = _userManager.GetUserFullName(course.InstructorUserId);
                row.IsForAll = course.Announcement.IsForAll;
                row.NoCreditsCount = course.Course.CourseType == (int)enu_CourseType.PhysEd;

                if (course.ExtraInstructors.Any()) 
                {
                    foreach (var extraCourse in course.ExtraInstructors) 
                    {
                        row.InstructorName += $", {_userManager.GetUserFullName(extraCourse.InstructorUserId)}";
                    }
                }

                courseList.Add(row);
            }

            return courseList;
        }

        /// <summary>
        /// Get registration courses model for selection for add/drop period
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Registration courses model</returns>
        public RegistrationSelectionCourseViewModel GetCoursesForAddDropSelectionTemp(int organizationId, int semesterId, string studentUserId)
        {
            RegistrationSelectionCourseViewModel model = new RegistrationSelectionCourseViewModel();
            List<RegistrationCourseRowViewModel> courseList = new List<RegistrationCourseRowViewModel>();

            var courseMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<Course, CourseDTO>();
            }).CreateMapper();

            var semester = _semesterService.GetSemester(organizationId, semesterId);

            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.AnnouncementSection)
                    .ThenInclude(x => x.Announcement)
                    .Include(x => x.AnnouncementSection)
                    .ThenInclude(x => x.Course)
                    .Include(x => x.StudentCourseRegistration)
                    .Where(x => x.AnnouncementSection.Season == semester.Season &&
                        x.AnnouncementSection.Year == semester.Year).ToList();

            model.ElectiveSelected = studentCourses
                .Any(x => x.StudentCourseRegistration.StudentUserId == studentUserId && 
                    x.State != (int)enu_CourseState.Dropped && x.AnnouncementSection.Announcement.IsForAll);

            var selectedCoursesIds = studentCourses
                .Where(x => x.StudentCourseRegistration.StudentUserId == studentUserId)
                .Select(x => x.AnnouncementSectionId).Distinct().ToList();

            var notSelectedStudentCourses = studentCourses
                .Where(x => x.StudentCourseRegistration.StudentUserId != studentUserId)
                    .ToList();

            courseList = _db.AnnouncementSections.Include(x => x.Announcement).Where(x => x.OrganizationId == organizationId &&
                    x.Season == semester.Season && x.Year == semester.Year && !selectedCoursesIds.Contains(x.Id) && 
                    x.Announcement.IsActivated)
                    .Include(x => x.Course).ToList()
                    .Select(x => new RegistrationCourseRowViewModel
                    {
                        Course = courseMapper.Map<Course, CourseDTO>(x.Course),
                        Points = (int)x.Credits,
                        Schedule = x.Schedule,
                        InstructorUserId = x.InstructorUserId,
                        InstructorName = _userManager.GetUserFullName(x.InstructorUserId),
                        RegistrationCourseId = x.Id,
                        TotalPlaces = x.Places,
                        RestPlaces = GetCourseRestPlaces(notSelectedStudentCourses, x.Id, x.Places),
                        Section = x.Section,
                        IsForAll = x.Announcement.IsForAll
                    }).ToList();

            model.Courses = courseList.OrderBy(x => x.Course.Abbreviation).ThenBy(x => x.Course.Number).ToList();

            return model;
        }

        /// <summary>
        /// Get registration courses model for selection
        /// </summary>
        /// <param name="semesterId">Semester id to select study card</param>
        /// <param name="departmentGroupId">Group id for courses selection</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="onlySelectedCourses">If true gets courses selected by student only</param>
        /// <returns>Registration courses model</returns>
        public RegistrationSelectionCourseViewModel GetCoursesForSelection(int semesterId, int departmentGroupId, string studentUserId, bool onlySelectedCourses)
        {
            RegistrationSelectionCourseViewModel model = new RegistrationSelectionCourseViewModel();
            List<RegistrationCourseRowViewModel> courseList = new List<RegistrationCourseRowViewModel>();

            if (onlySelectedCourses) 
                courseList = GetOnlySelectedStudentCourses(semesterId, departmentGroupId, studentUserId);
            else if (departmentGroupId != 0)
                courseList = GetStudentCoursesWithDepartment(semesterId, departmentGroupId);
            else 
                courseList = GetStudentCoursesWithoutDepartment(semesterId);

            SetCourseQueue(courseList, semesterId);

            if (onlySelectedCourses)
            {
                model.SelectedCoursesNumber = courseList.Count;
                model.TotalPoints = courseList.Sum(x => x.Points);
            }
            else 
            {
                var selectedCourses = _db.StudentCourseRegistrations
                            .Include(x => x.StudentCourses)
                            .Where(x => x.StudentUserId == studentUserId && x.SemesterId == semesterId)
                            .SelectMany(x => x.StudentCourses)
                            .Include(x => x.OldStudyCardCourse)
                            .ThenInclude(x => x.CyclePartCourse)
                            .Select(x => new
                            {
                                InstructorUserId = x.OldStudyCardCourse.InstructorUserId,
                                CourseId = x.OldStudyCardCourse.CyclePartCourse.CourseId,
                                Points = x.OldStudyCardCourse.CyclePartCourse.Points,
                                Comment = x.Comment,
                                IsApproved = x.IsApproved,
                                Queue = x.Queue
                            }).ToList();

                if (selectedCourses.Count > 0 && courseList.Count > 0)
                {
                    foreach (var selectedCourse in selectedCourses)
                    {
                        var course = courseList.FirstOrDefault(x => x.Course.Id == selectedCourse.CourseId &&
                                        x.InstructorUserId == selectedCourse.InstructorUserId);
                        if (course != null) 
                        {
                            course.IsSelected = true;
                            course.Comment = selectedCourse.Comment;
                            course.IsApproved = selectedCourse.IsApproved;
                            course.Queue = selectedCourse.Queue;
                        }
                    }
                }

                model.SelectedCoursesNumber = selectedCourses.Count;
                model.TotalPoints = selectedCourses.Sum(x => x.Points);
            }
            
            model.Courses = courseList;
            
            return model;
        }

        private List<RegistrationCourseRowViewModel> GetOnlySelectedStudentCourses(int semesterId, int departmentGroupId, string studentUserId) 
        {
            List<RegistrationCourseRowViewModel> courseList = new List<RegistrationCourseRowViewModel>();

            var courseMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<Course, CourseDTO>();
            }).CreateMapper();

            var studentCourses = _db.StudentCourses
                    .Include(x => x.OldStudyCardCourse).ThenInclude(x => x.OldStudyCard).ThenInclude(x => x.Semester)
                    .Include(x => x.OldStudyCardCourse.CyclePartCourse.Course)
                    .Include(x => x.StudentCourseRegistration)
                    .Where(x => x.OldStudyCardCourse.OldStudyCard.Semester.Id == semesterId &&
                        x.StudentCourseRegistration.StudentUserId == studentUserId);

            if (departmentGroupId != 0)
                studentCourses = studentCourses.Where(x => x.OldStudyCardCourse.OldStudyCard.DepartmentGroupId == departmentGroupId);

            courseList = studentCourses.Select(x => new RegistrationCourseRowViewModel
            {
                Course = courseMapper.Map<Course, CourseDTO>(x.OldStudyCardCourse.CyclePartCourse.Course),
                Points = x.OldStudyCardCourse.CyclePartCourse.Points,
                IsSelected = true,
                InstructorUserId = x.OldStudyCardCourse.InstructorUserId,
                InstructorName = _userManager.GetUserFullName(x.OldStudyCardCourse.InstructorUserId),
                StudentCourseId = x.StudyCardCourseId,
                DepartmentGroups = x.OldStudyCardCourse.OldStudyCard.DepartmentGroup.Department.Code + x.OldStudyCardCourse.OldStudyCard.DepartmentGroup.Code,
                Comment = x.Comment,
                IsApproved = x.IsApproved,
                TotalPlaces = x.OldStudyCardCourse.Places,
                Queue = x.Queue

            }).ToList();

            return courseList;
        }

        private List<RegistrationCourseRowViewModel> GetStudentCoursesWithDepartment(int semesterId, int departmentGroupId)
        {
            var courseMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<Course, CourseDTO>();
            }).CreateMapper();

            List<RegistrationCourseRowViewModel>  courseList = _db.OldStudyCards.Where(x => x.SemesterId == semesterId && x.DepartmentGroupId == departmentGroupId)
                    .Include(x => x.OldStudyCardCourses)
                    .SelectMany(x => x.OldStudyCardCourses)
                    .Include(x => x.CyclePartCourse).ThenInclude(x => x.Course)
                    .Include(x => x.OldStudyCard).ThenInclude(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                    .Select(x => new RegistrationCourseRowViewModel
                    {
                        Course = courseMapper.Map<Course, CourseDTO>(x.CyclePartCourse.Course),
                        Points = x.CyclePartCourse.Points,
                        InstructorUserId = x.InstructorUserId,
                        InstructorName = _userManager.GetUserFullName(x.InstructorUserId),
                        StudentCourseId = x.Id,
                        DepartmentGroups = x.OldStudyCard.DepartmentGroup.Department.Code + x.OldStudyCard.DepartmentGroup.Code,
                        TotalPlaces = x.Places
                    }).ToList();

            return courseList;
        }


        private List<RegistrationCourseRowViewModel> GetStudentCoursesWithoutDepartment(int semesterId)
        {
            var courseMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<Course, CourseDTO>();
            }).CreateMapper();

            List<RegistrationCourseRowViewModel> courseList = _db.OldStudyCards.Where(x => x.SemesterId == semesterId)
                    .Include(x => x.OldStudyCardCourses)
                    .SelectMany(x => x.OldStudyCardCourses)
                    .Include(x => x.CyclePartCourse).ThenInclude(x => x.Course)
                    .Include(x => x.OldStudyCard).ThenInclude(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                    .GroupBy(x => new { x.InstructorUserId, x.CyclePartCourse.CourseId })
                    .Select(x => new RegistrationCourseRowViewModel
                    {
                        Course = courseMapper.Map<Course, CourseDTO>(x.FirstOrDefault().CyclePartCourse.Course),
                        Points = x.FirstOrDefault().CyclePartCourse.Points,
                        InstructorUserId = x.Key.InstructorUserId,
                        InstructorName = _userManager.GetUserFullName(x.Key.InstructorUserId),
                        StudentCourseId = x.FirstOrDefault().Id,
                        DepartmentGroups = string.Join(", ", x.Select(y => y.OldStudyCard.DepartmentGroup.Department.Code + y.OldStudyCard.DepartmentGroup.Code)),
                        TotalPlaces = x.FirstOrDefault().Places
                    }).ToList();

            return courseList;
        }

        private int GetCourseRestPlaces(List<StudentCourseTemp> studentCourses, int registrationCourseId, int places)
        {
            int addedByOtherStudentsCount = studentCourses.Count(x => x.AnnouncementSectionId == registrationCourseId &&
                !(x.State == (int)enu_CourseState.Dropped && x.IsAddDropApproved));

            int restPalces = places - addedByOtherStudentsCount;
            if (restPalces < 0) restPalces = 0;

            return restPalces;
        }

        private int GetCourseRestPlaces(int registrationCourseId, int places)
        {
            int addedByOtherStudentsCount = _db.StudentCoursesTemp.Count(x => x.AnnouncementSectionId == registrationCourseId &&
                !(x.State == (int)enu_CourseState.Dropped && x.IsAddDropApproved));

            int restPalces = places - addedByOtherStudentsCount;
            if (restPalces < 0) restPalces = 0;

            return restPalces;
        }

        /// <summary>
        /// Get student queue for registration course
        /// </summary>
        /// <param name="registrationCourseId"></param>
        /// <param name="places"></param>
        /// <param name="studentUserId"></param>
        /// <returns>Student queue</returns>
        public int GetStudentQueue(int registrationCourseId, int places, string studentUserId)
        {
            int queue = 0;
            var students = _db.StudentCoursesTemp
                .Include(x => x.StudentCourseRegistration)
                    .Where(x => x.AnnouncementSectionId == registrationCourseId &&
                        !(x.State == (int)enu_CourseState.Dropped && x.IsAddDropApproved))
                    .OrderBy(x => x.DateCreated)
                    .ToList();

            if (students.Count > places)
            {
                var acceptedStudents = students.Take(places).ToList();
                if (!acceptedStudents.Any(x => x.StudentCourseRegistration.StudentUserId == studentUserId))
                {
                    var queuedStudents = students.Skip(places).ToList();
                    for (int i = 0; i < queuedStudents.Count; i++)
                    {
                        queue = i + 1;
                        if (queuedStudents[i].StudentCourseRegistration.StudentUserId == studentUserId)
                            break;
                    }
                }
            }

            return queue;
        }

        private int GetStudentQueue(List<StudentCourseTemp> studentCourses, int registrationCourseId, 
            int places, string studentUserId)
        {
            int queue = 0;
            var students = studentCourses
                    .Where(x => x.AnnouncementSectionId == registrationCourseId &&
                        !(x.State == (int)enu_CourseState.Dropped && x.IsAddDropApproved))
                    .OrderBy(x => x.DateCreated)
                    .ToList();

            if (students.Count > places) 
            {
                var acceptedStudents = students.Take(places).ToList();
                if (!acceptedStudents.Any(x => x.StudentCourseRegistration.StudentUserId == studentUserId)) 
                {
                    var queuedStudents = students.Skip(places).ToList();
                    for (int i = 0; i < queuedStudents.Count; i++) 
                    {
                        queue = i + 1;
                        if (queuedStudents[i].StudentCourseRegistration.StudentUserId == studentUserId) 
                            break;
                    }
                }
            }

            return queue;
        }
        private void SetCourseQueue(List<RegistrationCourseRowViewModel> courseList, int semesterId) 
        {
            int addedByOtherStudentsCount = 0;
            foreach (var course in courseList)
            {
                addedByOtherStudentsCount = _db.StudentCourses.Include(x => x.StudentCourseRegistration)
                        .Count(x => x.StudyCardCourseId == course.StudentCourseId && x.StudentCourseRegistration.SemesterId == semesterId);

                course.RestPlaces = (course.TotalPlaces - addedByOtherStudentsCount) <= 0 ? 0 : (course.TotalPlaces - addedByOtherStudentsCount);
            }
        }


        /// <summary>
        /// Get current student course registration step
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Model with current step, registration id and state </returns>
        public CourseRegistrationStepsViewModel GetStudentCourseRegistrationStep(int selectedOrganizationId, string studentUserId)
        {
            CourseRegistrationStepsViewModel model = new CourseRegistrationStepsViewModel();

            var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(selectedOrganizationId, studentUserId);

            var semesterPeriod = _semesterPeriodService.GetSemesterPeriod(selectedOrganizationId, (int)enu_Period.CourseRegistration, DateTime.Now);
            if (semesterPeriod == null) 
            {
                model.RegistrationUnavailable = true;
                model.WarningMessage = "Регистрация на курсы закрыта";
                return model;
            }

            if (CheckStudentDebts(model, studentUserId, semesterPeriod.SemesterId))
                return model;


            model.SemesterId = semesterPeriod.SemesterId;
            model.SeasonYear = semesterPeriod.Semester.SeasonYear;
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.SemesterId == semesterPeriod.SemesterId && 
                                        x.StudentUserId == studentUserId);

            if (registration == null)
            {
                StudentCourseRegistration newRegistration = new StudentCourseRegistration();
                newRegistration.OrganizationId = selectedOrganizationId;
                newRegistration.DateCreate = DateTime.Now;
                newRegistration.SemesterId = semesterPeriod.SemesterId;
                newRegistration.StudentUserId = studentUserId;
                newRegistration.State = (int)enu_RegistrationState.NotSent;
                _db.StudentCourseRegistrations.Add(newRegistration);
                _db.SaveChanges();

                registration = newRegistration;

                if (registration == null)
                    throw new Exception("Регистрация не была создана");

                model.StepNumber = 1;
            }
            else
            {
                if (registration.State == (int)enu_RegistrationState.NotSent)
                    model.StepNumber = 1;
                else if (registration.State == (int)enu_RegistrationState.OnApproval ||
                    registration.State == (int)enu_RegistrationState.NotApproved)
                    model.StepNumber = 2;
                else
                    model.StepNumber = 3;
            }

            var departmentGroupId = studentOrgInfo.DepartmentGroupId;
            if (studentOrgInfo.IsPrep && studentOrgInfo.PrepDepartmentGroup != null)
                departmentGroupId = studentOrgInfo.PrepDepartmentGroup.Id;

            var studyCard = _studyCardService.GetStudyCard(semesterPeriod.SemesterId, departmentGroupId);

            model.MaxRegistrationCredits = _envarSettingService.GetMaxRegistrationCredits(selectedOrganizationId);
            model.NoCreditsLimitation = registration.NoCreditsLimitation;
            model.StudentCourseRegistrationId = registration.Id;
            model.RegistrationState = (enu_RegistrationState)registration.State;
            model.RegistrationStateStr = EnumExtentions.GetDisplayName((enu_RegistrationState)registration.State);
            //model.StudentCourses = GetStudentCoursesByRegistrationId(registration.Id);
            model.StudentCourses = GetStudentCoursesByRegistrationIdTemp(registration.Id, studentUserId, studyCard);
            model.ActionTitle = GetStepActionTitle(model.StepNumber, model.RegistrationState);
            model.AdviserComment = registration.AdviserComment;
            model.StudentComment = registration.StudentComment;

            return model;
        }

        /// <summary>
        /// Get current student add/drop course step.
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Model with current step, registration id and state </returns>
        public CourseRegistrationStepsViewModel GetStudentAddDropCourseStep(int selectedOrganizationId, string studentUserId)
        {
            CourseRegistrationStepsViewModel model = new CourseRegistrationStepsViewModel();

            var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(selectedOrganizationId, studentUserId);

            var semesterPeriod = _semesterPeriodService.GetSemesterPeriod(selectedOrganizationId, (int)enu_Period.AddDrop, DateTime.Now);
            if (semesterPeriod == null)
            {
                model.RegistrationUnavailable = true;
                model.WarningMessage = "Add/Drop период закрыт";
                return model;
            }

            if (CheckStudentDebts(model, studentUserId, semesterPeriod.SemesterId))
                return model;


            model.SemesterId = semesterPeriod.SemesterId;
            model.SeasonYear = semesterPeriod.Semester.SeasonYear;
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.SemesterId == semesterPeriod.SemesterId &&
                                        x.StudentUserId == studentUserId);

            if (registration == null)
                throw new Exception("Регистрация не была создана");
            
            if (registration.AddDropState == (int)enu_RegistrationState.NotSent)
                model.StepNumber = 1;
            else if (registration.AddDropState == (int)enu_RegistrationState.OnApproval ||
                registration.AddDropState == (int)enu_RegistrationState.NotApproved)
                model.StepNumber = 2;
            else
                model.StepNumber = 3;

            var departmentGroupId = studentOrgInfo.DepartmentGroupId;
            if (studentOrgInfo.IsPrep && studentOrgInfo.PrepDepartmentGroup != null)
                departmentGroupId = studentOrgInfo.PrepDepartmentGroup.Id;

            var studyCard = _studyCardService.GetStudyCard(semesterPeriod.SemesterId, departmentGroupId);

            model.MaxRegistrationCredits = _envarSettingService.GetMaxRegistrationCredits(selectedOrganizationId);
            model.NoCreditsLimitation = registration.NoCreditsLimitation;
            model.StudentCourseRegistrationId = registration.Id;
            model.RegistrationState = (enu_RegistrationState)registration.AddDropState;
            model.RegistrationStateStr = EnumExtentions.GetDisplayName((enu_RegistrationState)registration.AddDropState);
            //model.StudentCourses = GetStudentCoursesByRegistrationId(registration.Id);
            model.StudentCourses = GetStudentAddDropCoursesByRegistrationIdTemp(registration.Id, studentUserId, studyCard);
            model.ActionTitle = GetAddDropStepActionTitle(model.StepNumber, model.RegistrationState);
            model.AdviserComment = registration.AdviserComment;
            model.StudentComment = registration.StudentComment;

            return model;
        }

        private bool CheckStudentDebts(CourseRegistrationStepsViewModel model, string studentUserId, int semesterId) 
        {
            var debtInfo = _studentDebtService.GetStudentDebtInfo(semesterId, studentUserId);

            if (debtInfo.Count > 0) 
            {
                model.IsDebt = true;
                model.WarningMessage = "Есть задолженности по следующим отделам: " + string.Join(",\n",
                    debtInfo.Select(x => x.DebtName + ": " + x.Comment));
                return true;
            }
            return false;
        }

        private List<StudentCourseStepsViewModel> GetStudentCoursesByRegistrationIdTemp(int studentCourseRegistrationId,
            string studentUserId, StudyCardDTO studyCard)
        {
            List<StudentCourseStepsViewModel> model = new List<StudentCourseStepsViewModel>();

            var studentCourses = _db.StudentCoursesTemp.Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId 
                    && x.State != (int)enu_CourseState.Added)
                    .Include(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                   .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course)
                   .Include(x => x.AnnouncementSection).ThenInclude(x => x.ExtraInstructors)
                   .ToList();


            foreach (var course in studentCourses)
            {
                StudentCourseStepsViewModel courseModel = new StudentCourseStepsViewModel();
                courseModel.RegistrationCourseId = course.AnnouncementSectionId;
                courseModel.StudentCourseId = course.Id;
                courseModel.CourseImportCode = course.AnnouncementSection.Course.ImportCode;
                courseModel.CourseName = course.AnnouncementSection.Course.NameRus + " \\ " + course.AnnouncementSection.Course.NameEng 
                        + " \\ " + course.AnnouncementSection.Course.NameKir + $"({course.AnnouncementSection.Section})";
                courseModel.Code = course.AnnouncementSection.Course.Abbreviation + course.AnnouncementSection.Course.Number;
                courseModel.Points = (int)course.AnnouncementSection.Credits;
                courseModel.IsProcessed = course.IsProcessed;
                courseModel.IsApproved = course.IsApproved;
                courseModel.Comment = course.Comment;
                courseModel.Schedule = course.AnnouncementSection.Schedule;
                courseModel.Queue = GetStudentQueue(course.AnnouncementSectionId, course.AnnouncementSection.Places, studentUserId);
                courseModel.State = (enu_CourseState)course.State;
                courseModel.IsAudit = course.IsAudit;
                courseModel.IsForAll = course.AnnouncementSection.Announcement.IsForAll;
                courseModel.NoCreditsCount = course.AnnouncementSection.Course.CourseType == (int)enu_CourseType.PhysEd;
                courseModel.InstructorName = _userManager.GetUserFullName(course.AnnouncementSection.InstructorUserId);
                courseModel.IsActivated = course.AnnouncementSection.Announcement.IsActivated;
                if (course.AnnouncementSection.ExtraInstructors.Any()) 
                {
                    foreach (var extraInstructor in course.AnnouncementSection.ExtraInstructors) 
                    {
                        courseModel.InstructorName += $", {_userManager.GetUserFullName(extraInstructor.InstructorUserId)}";
                    }
                }

                if (studyCard != null) 
                    courseModel.IsFromStudyCard = studyCard.StudyCardCourses.Select(x => x.RegistrationCourseId).Contains(course.AnnouncementSectionId);

                model.Add(courseModel);
            }

            return model;
        }

        private List<StudentCourseStepsViewModel> GetStudentAddDropCoursesByRegistrationIdTemp(int studentCourseRegistrationId,
            string studentUserId, StudyCardDTO studyCard)
        {
            List<StudentCourseStepsViewModel> model = new List<StudentCourseStepsViewModel>();

            var studentCourses = _db.StudentCoursesTemp.Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId)
                   .Include(x => x.AnnouncementSection)
                   .ThenInclude(x => x.Course)
                   .Include(x => x.AnnouncementSection)
                   .ThenInclude(x => x.Announcement)
                   .Include(x => x.AnnouncementSection)
                   .ThenInclude(x => x.ExtraInstructors)
                   .ToList();

            foreach (var course in studentCourses)
            {
                StudentCourseStepsViewModel courseModel = new StudentCourseStepsViewModel();
                courseModel.RegistrationCourseId = course.AnnouncementSectionId;
                courseModel.StudentCourseId = course.Id;
                courseModel.CourseImportCode = course.AnnouncementSection.Course.ImportCode;
                courseModel.CourseName = course.AnnouncementSection.Course.NameRus + " \\ " + course.AnnouncementSection.Course.NameEng
                        + " \\ " + course.AnnouncementSection.Course.NameKir + $" ({course.AnnouncementSection.Section})";
                courseModel.Code = course.AnnouncementSection.Course.Abbreviation + course.AnnouncementSection.Course.Number;
                courseModel.Points = (int)course.AnnouncementSection.Credits;
                courseModel.IsProcessed = course.IsAddDropProcessed;
                courseModel.IsApproved = course.IsAddDropApproved;
                courseModel.Comment = course.Comment;
                courseModel.Schedule = course.AnnouncementSection.Schedule;
                courseModel.Queue = GetStudentQueue(course.AnnouncementSectionId, course.AnnouncementSection.Places, studentUserId);
                courseModel.State = (enu_CourseState)course.State;
                courseModel.IsAudit = course.IsAudit;
                courseModel.IsForAll = course.AnnouncementSection.Announcement.IsForAll;
                courseModel.NoCreditsCount = course.AnnouncementSection.Course.CourseType == (int)enu_CourseType.PhysEd;
                courseModel.InstructorName = _userManager.GetUserFullName(course.AnnouncementSection.InstructorUserId);
                courseModel.IsActivated = course.AnnouncementSection.Announcement.IsActivated;
                if (course.AnnouncementSection.ExtraInstructors.Any())
                {
                    foreach (var extraInstructor in course.AnnouncementSection.ExtraInstructors)
                    {
                        courseModel.InstructorName += $", {_userManager.GetUserFullName(extraInstructor.InstructorUserId)}";
                    }
                }

                if (studyCard != null)
                    courseModel.IsFromStudyCard = studyCard.StudyCardCourses.Select(x => x.RegistrationCourseId).Contains(course.AnnouncementSectionId);

                model.Add(courseModel);
            }

            return model;
        }

        private List<StudentCourseStepsViewModel> GetStudentCoursesByRegistrationId(int studentCourseRegistrationId) 
        {
            List<StudentCourseStepsViewModel> model = new List<StudentCourseStepsViewModel>();

            var studentCourses = _db.StudentCourses.Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId)
                   .Include(x => x.OldStudyCardCourse).ThenInclude(x => x.CyclePartCourse).ThenInclude(x => x.Course).ToList();

            foreach (var course in studentCourses) 
            {
                StudentCourseStepsViewModel courseModel = new StudentCourseStepsViewModel();
                courseModel.CourseName = course.OldStudyCardCourse.CyclePartCourse.Course.NameRus + "\\" + course.OldStudyCardCourse.CyclePartCourse.Course.NameEng;
                courseModel.Code = course.OldStudyCardCourse.CyclePartCourse.Course.Abbreviation + course.OldStudyCardCourse.CyclePartCourse.Course.Number;
                courseModel.Points = course.OldStudyCardCourse.CyclePartCourse.Points;
                courseModel.InstructorName = _userManager.GetUserFullName(course.OldStudyCardCourse.InstructorUserId);
                courseModel.IsApproved = course.IsApproved;
                courseModel.Comment = course.Comment;
                model.Add(courseModel);
            }
        
            return model;
        }

        private string GetStepActionTitle(int stepNumber, enu_RegistrationState registrationState) 
        {
            string actionTitle = "";

            if (stepNumber == 1)
            {
                actionTitle = "Выберите курсы и отправьте на одобрение эдвайзеру";
            }
            else if (stepNumber == 2)
            {
                if (registrationState == enu_RegistrationState.OnApproval)
                {
                    actionTitle = "Дождитесь подтверждения эдвазейра для завершения регистрации";
                }
                else if (registrationState == enu_RegistrationState.NotApproved)
                {
                    actionTitle = "Исправьте регистрацию по рекомендациям эдвайзера затем отправьте на одобрение повторно";
                }
            }
            else if (stepNumber == 3)
            {
                if (registrationState != enu_RegistrationState.Submitted)
                {
                    actionTitle = "Завершите регистрацию";
                }
                else
                {
                    actionTitle = "Регистрация успешно завершена. Скачайте PDF файл и отправьте на почту ocr@iuca.kg";
                }
            }

            return actionTitle;
        }

        private string GetAddDropStepActionTitle(int stepNumber, enu_RegistrationState registrationState)
        {
            string actionTitle = "";

            if (stepNumber == 1)
            {
                actionTitle = "Исправьте курсы и отправьте на одобрение эдвайзеру";
            }
            else if (stepNumber == 2)
            {
                if (registrationState == enu_RegistrationState.OnApproval)
                {
                    actionTitle = "Дождитесь подтверждения эдвазейра для завершения Add/Drop формы";
                }
                else if (registrationState == enu_RegistrationState.NotApproved)
                {
                    actionTitle = "Исправьте курсы по рекомендациям эдвайзера затем отправьте на одобрение повторно";
                }
            }
            else if (stepNumber == 3)
            {
                if (registrationState != enu_RegistrationState.Submitted)
                {
                    actionTitle = "Завершите Add/Drop форму";
                }
                else
                {
                    actionTitle = "Регистрация Add/Drop формы успешно завершена. Скачайте PDF файл, расечатайте, и отнесите в учебную часть для подтверждения";
                }
            }

            return actionTitle;
        }

        /// <summary>
        /// Save student comment
        /// </summary>
        /// <param name="id">Student course registration id</param>
        /// <param name="comment">Student comment</param>
        public void SaveStudentComment(int id, string comment)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == id);
            if (registration == null)
                throw new Exception($"Registration with id { id } not found");

            if (registration.State != (int)enu_RegistrationState.NotSent && registration.State != (int)enu_RegistrationState.NotApproved)
                throw new Exception("Comment cannot be updated");

            registration.StudentComment = comment;
            _db.StudentCourseRegistrations.Update(registration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Save student comment for add/drop period
        /// </summary>
        /// <param name="id">Student course registration id</param>
        /// <param name="comment">Student comment</param>
        public void SaveStudentAddDropComment(int id, string comment)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == id);
            if (registration == null)
                throw new Exception($"Registration with id {id} not found");

            /*if (registration.AddDropState != (int)enu_RegistrationState.NotSent && registration.State != (int)enu_RegistrationState.NotApproved)
                throw new Exception("Comment cannot be updated");*/

            registration.StudentComment = comment;
            _db.StudentCourseRegistrations.Update(registration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Send registration on approval to advisor
        /// </summary>
        /// <param name="id">Student course registration id</param>
        public void SendOnApproval(int id) 
        {
            var registration = _db.StudentCourseRegistrations.Include(x => x.StudentCoursesTemp).FirstOrDefault(x => x.Id == id);
            if (registration == null)
                throw new Exception($"Registration with id { id } not found");

            if (registration.StudentCoursesTemp.Count == 0)
                throw new Exception("There are no courses in registration");

            registration.State = (int)enu_RegistrationState.OnApproval;
            registration.DateChange = DateTime.Now;
            _db.StudentCourseRegistrations.Update(registration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Send add/drop registration on approval to advisor
        /// </summary>
        /// <param name="id">Student course registration id</param>
        public void SendAddDropOnApproval(int id)
        {
            var registration = _db.StudentCourseRegistrations.Include(x => x.StudentCoursesTemp).FirstOrDefault(x => x.Id == id);
            if (registration == null)
                throw new Exception($"Registration with id {id} not found");

            if (registration.StudentCoursesTemp.Count == 0)
                throw new Exception("There are no courses in registration");

            registration.AddDropState = (int)enu_RegistrationState.OnApproval;
            registration.DateChange = DateTime.Now;
            _db.StudentCourseRegistrations.Update(registration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Submit registration by student
        /// </summary>
        /// <param name="id">Student course registration id</param>
        public void SubmitRegistration(int id)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == id);
            if (registration == null)
                throw new Exception($"Registration with id { id } not found");

            registration.State = (int)enu_RegistrationState.Submitted;
            registration.StudentComment = "";
            registration.DateChange = DateTime.Now;
            _db.StudentCourseRegistrations.Update(registration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Submit add/drop form of a student
        /// </summary>
        /// <param name="id">Student course registration id</param>
        public void SubmitAddDropForm(int id)
        {
            var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.Id == id);
            if (registration == null)
                throw new Exception($"Registration with id {id} not found");

            registration.AddDropState = (int)enu_RegistrationState.Submitted;
            registration.StudentComment = "";
            registration.DateChange = DateTime.Now;
            _db.StudentCourseRegistrations.Update(registration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get student registration information
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="userId">User id</param>
        /// <returns>Inforamtion model</returns>
        public RegistrationInfoViewModel GetStudentRegistrationInfo(int organizationId, string userId) 
        {
            RegistrationInfoViewModel model = new RegistrationInfoViewModel();
            model.IsStudent = _userTypeOrganizationService.UserHasType(organizationId, userId, (int)enu_UserType.Student);
            if (model.IsStudent)
            {
                var registrationSemesterPeriod = _semesterPeriodService.GetSemesterPeriod(organizationId,
                            (int)enu_Period.CourseRegistration, DateTime.Now);
                var addDropSemesterPeriod = _semesterPeriodService.GetSemesterPeriod(organizationId,
                        (int)enu_Period.AddDrop, DateTime.Now);
                
                model.IsCourseRegistrationOpened = registrationSemesterPeriod != null;
                model.IsAddDropOpened = addDropSemesterPeriod != null;

                if (model.IsCourseRegistrationOpened || model.IsAddDropOpened)
                {
                    var semesterId = model.IsCourseRegistrationOpened ? registrationSemesterPeriod.SemesterId :
                        model.IsAddDropOpened ? addDropSemesterPeriod.SemesterId : 0;

                    model.DebtList = _studentDebtService.GetStudentDebtInfo(semesterId, userId);
                    model.IsDebt = model.DebtList.Count > 0;

                    var studentCourseRegistration = _db.StudentCourseRegistrations
                            .FirstOrDefault(x => x.SemesterId == semesterId && x.StudentUserId == userId);

                    if (studentCourseRegistration != null)
                    {
                        model.RegistrationState = EnumExtentions.GetDisplayName((enu_RegistrationState)studentCourseRegistration.State);
                        model.IsRegistrationSubmitted = (enu_RegistrationState)studentCourseRegistration.State == enu_RegistrationState.Submitted;
                        model.AddDropState = EnumExtentions.GetDisplayName((enu_RegistrationState)studentCourseRegistration.AddDropState);
                        model.IsAddDropSubmitted = (enu_RegistrationState)studentCourseRegistration.AddDropState == enu_RegistrationState.Submitted ||
                            (enu_RegistrationState)studentCourseRegistration.AddDropState == enu_RegistrationState.NotSent;
                        model.IsAddDropAllowed = (DateTime.Now - studentCourseRegistration.DateCreate).Days > 20;
                    }
                    else
                    {
                        model.RegistrationState = EnumExtentions.GetDisplayName(enu_RegistrationState.NotSent);
                        model.AddDropState = EnumExtentions.GetDisplayName(enu_RegistrationState.NotSent);
                    }
                    
                }
            }

            return model;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
