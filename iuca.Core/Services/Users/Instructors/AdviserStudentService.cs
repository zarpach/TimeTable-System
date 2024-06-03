using iuca.Application.DTO.Users.Students;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.ViewModels.Courses;
using iuca.Application.ViewModels.Users.Instructors;
using iuca.Application.ViewModels.Users.Students;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using iuca.Application.Enums;
using iuca.Domain.Entities.Courses;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.DTO.Common;
using AutoMapper;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Common;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Interfaces.Roles;

namespace iuca.Application.Services.Users.Instructors
{
    public class AdviserStudentService : IAdviserStudentService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IStudentOrgInfoService _studentOrgInfoService;
        private readonly IStudentCourseRegistrationService _studentCourseRegistrationService;
        private readonly IEnvarSettingService _envarSettingService;
        private readonly IStudyCardService _studyCardService;
        private readonly IUserRolesService _userRolesService;
        private readonly IStudentDebtService _studentDebtService;

        public AdviserStudentService(IApplicationDbContext db,
            ApplicationUserManager<ApplicationUser> userManager,
            IStudentOrgInfoService studentOrgInfoService,
            IStudentCourseRegistrationService studentCourseRegistrationService,
            IEnvarSettingService envarSettingService,
            IStudyCardService studyCardService,
            IUserRolesService userRolesService,
            IStudentDebtService studentDebtService)
        {
            _db = db;
            _userManager = userManager;
            _studentOrgInfoService = studentOrgInfoService;
            _studentCourseRegistrationService = studentCourseRegistrationService;
            _envarSettingService = envarSettingService;
            _studyCardService = studyCardService;
            _userRolesService = userRolesService;
            _studentDebtService = studentDebtService;
        }

        /// <summary>
        /// Get adviser list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Adviser list</returns>
        public IEnumerable<UserDTO> GetAdvisers(int organizationId)
        {
            var advisers = _userRolesService.GetUserListByRole(organizationId, enu_Role.Adviser)
                .Select(x => new UserDTO { Id = x.Id, FullName = x.FullNameEng });

            return advisers;
        }

        /// <summary>
        /// Get instructor brief view model list of advisers by dean user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns>Instructor brief view model of user list who has adviser role</returns>
        public IEnumerable<InstructorInfoBriefViewModel> GetAdvisers(int organizationId, string deanUserId) 
        {
            List<InstructorInfoBriefViewModel> adviserListModel = new List<InstructorInfoBriefViewModel>();
            var adviserIds = _db.DeanAdvisers.Where(x => x.OrganizationId == organizationId && x.DeanUserId == deanUserId)
                .Select(x => x.AdviserUserId).ToList();

            var advisers = _userManager.Users.Where(x => adviserIds.Contains(x.Id)).ToList();

            foreach (var adviser in advisers) 
            {
                InstructorInfoBriefViewModel model = new InstructorInfoBriefViewModel();
                model.InstructorUserId = adviser.Id;
                model.FullNameEng = adviser.FullNameEng;
                model.Email = adviser.Email;
                adviserListModel.Add(model);
            }

            return adviserListModel;
        }

        /// <summary>
        /// Get students for intructor-adviser
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <returns>List of students</returns>
        public List<AdviserStudentViewModel> GetAdviserStudentsByInstuctorId(int organizationId, string instructorUserId)
        {
            return GetAdviserStudentsByInstuctorId(organizationId, new List<string> { instructorUserId });
        }

        /// <summary>
        /// Get students for intructor-adviser
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserIds">Instructor user ids</param>
        /// <returns>List of students</returns>
        public List<AdviserStudentViewModel> GetAdviserStudentsByInstuctorId(int organizationId,  List<string> instructorUserIds)
        {
            List<AdviserStudentViewModel> studentList = new List<AdviserStudentViewModel>();

            var studentIds = _db.AdviserStudents
                .Where(x => instructorUserIds.Contains(x.InstructorUserId) && x.OrganizationId == organizationId)
                .Select(x => x.StudentUserId)
                .ToList();

            var students = _userManager.Users
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .ThenInclude(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .ThenInclude(x => x.PrepDepartmentGroup)
                .ThenInclude(x => x.Department)
                .Where(x => studentIds.Contains(x.Id)).ToList();

            foreach (var student in students)
            {
                AdviserStudentViewModel model = new AdviserStudentViewModel();
                model.StudentUserId = student.Id;
                model.Name = student.FullNameEng;
                model.ShortName = $"{student.LastNameEng} {student.FirstNameEng}";
                
                if (student.StudentBasicInfo != null && student.StudentBasicInfo.StudentOrgInfo != null)
                {
                    var orgInfo = student.StudentBasicInfo.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId);
                    if (orgInfo != null)
                    {
                        model.Group = orgInfo.DepartmentGroup.Department.Code + orgInfo.DepartmentGroup.Code;
                        if (orgInfo.PrepDepartmentGroup != null)
                            model.Group += $"/{orgInfo.PrepDepartmentGroup.Department.Code + orgInfo.PrepDepartmentGroup.Code}";
                        model.DepartmentGroupId = orgInfo.DepartmentGroupId;
                        model.PrepDepartmentGroupId = orgInfo.PrepDepartmentGroupId;
                        model.State = (enu_StudentState)orgInfo.State;
                        model.StudentId = orgInfo.StudentId.ToString();
                    }
                }

                studentList.Add(model);
            }

            return studentList;
        }

        /// <summary>
        /// Get departments for intructor-adviser
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <returns>List of departments</returns>
        public List<DepartmentGroupDTO> GetAdviserDepartmentGroupsByInstuctorId(int organizationId, string instructorUserId)
        {
            List<AdviserStudentViewModel> studentList = new List<AdviserStudentViewModel>();

            var studentIds = _db.AdviserStudents
                .Where(x => x.InstructorUserId == instructorUserId && x.OrganizationId == organizationId)
                .Select(x => x.StudentUserId)
                .ToList();

            var studentOrgInfo = _db.StudentOrgInfo
                .Include(x => x.StudentBasicInfo)
                .Include(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .Include(x => x.PrepDepartmentGroup)
                .ThenInclude(x => x.Department)
                .Where(x => x.StudentBasicInfo != null && 
                    studentIds.Contains(x.StudentBasicInfo.ApplicationUserId))
                .ToList();

            var departmentGroups = studentOrgInfo.Where(x => x.PrepDepartmentGroup == null)
                .Select(x => x.DepartmentGroup).Distinct().ToList();

            var prepDepartmentGroups = studentOrgInfo.Where(x => x.PrepDepartmentGroup != null)
                .Select(x => x.PrepDepartmentGroup).Distinct().ToList();

            departmentGroups.AddRange(prepDepartmentGroups.Except(departmentGroups));

            var mapper = new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
            }).CreateMapper();

            return mapper.Map<List<DepartmentGroup>, List<DepartmentGroupDTO>>(departmentGroups);
        }

        /// <summary>
        /// Get students for selection window
        /// </summary>
        /// <param name="departmentId">Department ids</param>
        /// <param name="departmentGroupId">Group id for courses selection</param>
        /// <param name="organizationId">Organiaztion id</param>
        /// <param name="excludedIds">Studnet ids to exclude</param>
        /// <param name="onlyActive">Select only active and academic leave students</param>
        /// <returns>List of students</returns>
        public List<SelectStudentViewModel> GetStudentsForSelection(int[] departmentId, int departmentGroupId, 
            int organizationId, string[] excludedIds, bool onlyActive = true)
        {
            List<SelectStudentViewModel> studentList = new List<SelectStudentViewModel>();

            var studentIds = _db.UserTypeOrganizations.Where(x => x.OrganizationId == organizationId &&
                            x.UserType == (int)enu_UserType.Student && !excludedIds.Contains(x.ApplicationUserId))
                            .Select(x => x.ApplicationUserId).ToList();

            var students = _userManager.Users
                                .Include(x => x.StudentBasicInfo)
                                .ThenInclude(x => x.StudentOrgInfo)
                                .ThenInclude(x => x.DepartmentGroup)
                                .ThenInclude(x => x.Department)
                                .Include(x => x.StudentBasicInfo)
                                .ThenInclude(x => x.StudentOrgInfo)
                                .ThenInclude(x => x.PrepDepartmentGroup)
                                .ThenInclude(x => x.Department)
                                .Where(x => studentIds.Contains(x.Id));

            students = students.Where(x => x.StudentBasicInfo.StudentOrgInfo == null || 
                    x.StudentBasicInfo.StudentOrgInfo.Any(x => x.OrganizationId == organizationId
                        && (departmentId.Contains(x.DepartmentGroup.DepartmentId) || 
                        x.PrepDepartmentGroup != null && departmentId.Contains(x.PrepDepartmentGroup.DepartmentId))));

            if (onlyActive)
                students = students.Where(x => x.StudentBasicInfo.StudentOrgInfo == null ||
                    x.StudentBasicInfo.StudentOrgInfo.Any(x => x.State == (int)enu_StudentState.Active ||
                    x.State == (int)enu_StudentState.AcadLeave));

            if (departmentGroupId != 0)
                students = students.Where(x => x.StudentBasicInfo.StudentOrgInfo.Any(x => x.OrganizationId == organizationId
                    && (x.DepartmentGroupId == departmentGroupId || x.PrepDepartmentGroupId == departmentGroupId)));

            foreach (var student in students.ToList()) 
            {
                SelectStudentViewModel model = new SelectStudentViewModel();
                model.StudentUserId = student.Id;
                model.FullNameEng = student.FullNameEng;

                if (student.StudentBasicInfo != null && student.StudentBasicInfo.StudentOrgInfo != null) 
                {
                    var orgInfo = student.StudentBasicInfo.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId);
                    if (orgInfo != null) 
                    {
                        model.Group = $"{ orgInfo.DepartmentGroup.Department.Code }{ orgInfo.DepartmentGroup.Code }";
                        if (orgInfo.PrepDepartmentGroup != null)
                            model.Group += $"/{orgInfo.PrepDepartmentGroup.Department.Code }{ orgInfo.PrepDepartmentGroup.Code}";
                        model.State = (enu_StudentState)orgInfo.State;
                        model.DepartmentGroupId = orgInfo.DepartmentGroupId;
                    }
                }

                studentList.Add(model);
            }

            return studentList;
        }

        /// <summary>
        /// Get students from selection window
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserIds">Array of student user ids</param>
        /// <returns>List of adviser students from selection window</returns>
        public List<AdviserStudentViewModel> GetStudentsFromSelection(int organizationId, string[] studentUserIds)
        {
            List<AdviserStudentViewModel> studentList = new List<AdviserStudentViewModel>();

            var students = _userManager.Users
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .ThenInclude(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .Where(x => studentUserIds.Contains(x.Id))
                .ToList();

            foreach (var student in students)
            {
                AdviserStudentViewModel model = new AdviserStudentViewModel();

                model.Name = student.FullNameEng;
                model.StudentUserId = student.Id;

                if (student.StudentBasicInfo != null && student.StudentBasicInfo.StudentOrgInfo != null) 
                {
                    var orgInfo = student.StudentBasicInfo.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId);
                    if (orgInfo != null) 
                    {
                        model.Group = $"{orgInfo.DepartmentGroup.Department.Code}{orgInfo.DepartmentGroup.Code}";
                        model.State = (enu_StudentState)orgInfo.State;
                    }
                }
                
                studentList.Add(model);
            }

            return studentList;
        }

        /// <summary>
        /// Get adviser list for student
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student advisers</returns>
        public IEnumerable<UserDTO> GetStudentAdvisers(int organizationId, string studentUserId)
        {
            if (string.IsNullOrEmpty(studentUserId))
                throw new ArgumentNullException(nameof(studentUserId), "The student user id is null.");

            var studentAdvisers = _db.AdviserStudents
                .Where(x => x.StudentUserId == studentUserId && x.OrganizationId == organizationId)
                .ToList()
                .Select(x => new UserDTO() { Id = x.InstructorUserId, FullName = _userManager.GetUserFullName(x.InstructorUserId) })
                .ToList();

            return studentAdvisers;
        }

        /// <summary>
        /// Set new advisers for a student
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="newAdviserIds">List of new advisers</param>
        /// <returns>Non adviser user message</returns>
        public string SetStudentAdvisers(int organizationId, string studentUserId, IEnumerable<string> newAdviserIds)
        {
            var message = "";

            if (string.IsNullOrEmpty(studentUserId))
                throw new ArgumentNullException(nameof(studentUserId), "The student user id is null");

            newAdviserIds = newAdviserIds ?? Enumerable.Empty<string>();

            var currentAdviserStudents = _db.AdviserStudents
                .Where(x => x.StudentUserId == studentUserId && x.OrganizationId == organizationId)
                .ToList();

            var newAdviserStudents = new List<AdviserStudent>();
            var nonAdvisers = new List<string>();

            foreach (var adviserId in newAdviserIds)
            {
                if (_userRolesService.IsUserInRole(adviserId, organizationId, enu_Role.Adviser))
                {
                    newAdviserStudents.Add(new AdviserStudent
                    {
                        OrganizationId = organizationId,
                        StudentUserId = studentUserId,
                        InstructorUserId = adviserId
                    });
                }
                else
                {
                    nonAdvisers.Add(_userManager.GetUserFullName(adviserId));
                }
            }

            _db.AdviserStudents.RemoveRange(currentAdviserStudents);
            _db.AdviserStudents.AddRange(newAdviserStudents);

            if (nonAdvisers.Any())
                message = $"These users are no longer advisors: {string.Join(", ", nonAdvisers)}";

            return message;
        }

        /// <summary>
        /// Edit adviser students
        /// </summary>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserIds">Student user id list</param>
        public void EditAdviserStudents(string instructorUserId, int organizationId, List<string> studentUserIds)
        {
            var modelStudentIds = studentUserIds.ToList();
            var dbStudents = _db.AdviserStudents.Where(x => x.InstructorUserId == instructorUserId &&
                x.OrganizationId == organizationId).ToList();

            var existingStudents = dbStudents.ToList();

            foreach (var dbStudent in dbStudents)
            {
                string studentId = modelStudentIds.FirstOrDefault(x => x == dbStudent.StudentUserId);

                if (string.IsNullOrEmpty(studentId))
                    _db.AdviserStudents.Remove(dbStudent);
                else
                    modelStudentIds.Remove(studentId);
            }

            if (modelStudentIds.Any())
                foreach (var studentUserId in modelStudentIds) 
                    AddStudentToAdviser(instructorUserId, organizationId, studentUserId);

            _db.SaveChanges();
        }

        private void AddStudentToAdviser(string instructorUserId, int organizationId, string studentUserId)
        {
            AdviserStudent adviserStudent = new AdviserStudent();
            adviserStudent.OrganizationId = organizationId;
            adviserStudent.InstructorUserId = instructorUserId;
            adviserStudent.StudentUserId = studentUserId;
            _db.AdviserStudents.Add(adviserStudent);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get students list for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="lastName">Student last name</param>
        /// <param name="firstName">Student first name</param>
        /// <param name="studentId">StudentId</param>
        /// <param name="onlyActive">Only active students</param>
        /// <returns>Student list for selection</returns>
        public List<SelectStudentViewModel> GetStudentSelectList(int organizationId, string instructorUserId, int departmentGroupId,
            string lastName, string firstName, int studentId, bool onlyActive)
        {
            List<SelectStudentViewModel> model = new List<SelectStudentViewModel>();

            var studentIds = _db.AdviserStudents.Where(x => x.OrganizationId == organizationId &&
                 x.InstructorUserId == instructorUserId).Select(x => x.StudentUserId).ToList();

            var studentList = _userManager.Users
                .Include(x => x.UserBasicInfo)
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .ThenInclude(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .ThenInclude(x => x.PrepDepartmentGroup)
                .ThenInclude(x => x.Department)
                .Where(x => studentIds.Contains(x.Id));

            if (onlyActive)
                studentList = studentList.Where(x => x.StudentBasicInfo.StudentOrgInfo
                    .Any(y => y.State == (int)enu_StudentState.Active && y.OrganizationId == organizationId));

            if (departmentGroupId != 0)
                studentList = studentList.Where(x => x.StudentBasicInfo.StudentOrgInfo
                    .Any(y => (y.DepartmentGroupId == departmentGroupId || y.PrepDepartmentGroupId == departmentGroupId) 
                        && y.OrganizationId == organizationId));

            if (!string.IsNullOrEmpty(lastName))
                studentList = studentList.Where(x => x.LastNameEng.Contains(lastName) ||
                    x.UserBasicInfo.LastNameRus.Contains(lastName));

            if (!string.IsNullOrEmpty(firstName))
                studentList = studentList.Where(x => x.FirstNameEng.Contains(firstName) ||
                     x.UserBasicInfo.FirstNameRus.Contains(firstName));

            if (studentId != 0)
                studentList = studentList.Where(x => x.StudentBasicInfo.StudentOrgInfo
                    .Any(y => y.StudentId == studentId && y.OrganizationId == organizationId));

            foreach (var student in studentList)
            {
                SelectStudentViewModel studentModel = new SelectStudentViewModel();
                studentModel.StudentUserId = student.Id;
                studentModel.FullNameEng = student.FullNameEng;
                if (student.StudentBasicInfo != null && student.StudentBasicInfo.StudentOrgInfo != null)
                {
                    var orgInfo = student.StudentBasicInfo.StudentOrgInfo.FirstOrDefault();
                    if (orgInfo != null)
                    {
                        studentModel.Group = orgInfo.DepartmentGroup.Department.Code + orgInfo.DepartmentGroup.Code;
                        if (orgInfo.PrepDepartmentGroup != null) 
                            studentModel.Group += $"/{orgInfo.PrepDepartmentGroup.Department.Code + orgInfo.PrepDepartmentGroup.Code}";
                        studentModel.DepartmentGroupId = orgInfo.DepartmentGroup.Id;
                        studentModel.StudentId = orgInfo.StudentId;
                        studentModel.State = (enu_StudentState)orgInfo.State;
                    }
                }

                model.Add(studentModel);
            }

            return model;
        }

        /// <summary>
        /// Get list of students with their course registrations
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="activeOnly">If true - only active students</param>
        /// <param name="withDebtsOnly">If true - students with debts only</param>
        /// <param name="incompleteRegistration">If true - students with incomplete registartion only</param>
        /// <returns>List of students with their course registrations</returns>
        public List<AdviserStudentRegistrationViewModel> GetAdviserStudentRegistrations(int organizationId, string instructorUserId, 
                        int? departmentGroupId, int semesterId, bool activeOnly = true, bool withDebtsOnly = false, bool incompleteRegistration = false) 
        {
            List<AdviserStudentRegistrationViewModel> registrationList = new List<AdviserStudentRegistrationViewModel>();
            var adviserStudents = _db.AdviserStudents.Where(x => x.InstructorUserId == instructorUserId
                                && x.OrganizationId == organizationId).ToList();

            foreach (var adviserStudent in adviserStudents) 
            {
                var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(organizationId, adviserStudent.StudentUserId);

                if (studentOrgInfo != null) 
                {
                    if (activeOnly && studentOrgInfo.State != (int)enu_StudentState.Active)
                        continue;

                    if (departmentGroupId != null && !(studentOrgInfo.DepartmentGroupId == departmentGroupId.Value ||
                            studentOrgInfo.PrepDepartmentGroupId == departmentGroupId.Value))
                        continue;
                }

                var studentInfo = _userManager.Users
                    .FirstOrDefault(x => x.Id == adviserStudent.StudentUserId);
                if (studentInfo == null)
                    throw new Exception($"Student with id {adviserStudent.StudentUserId} not found");

                AdviserStudentRegistrationViewModel model = new AdviserStudentRegistrationViewModel();
                model.StudentName = studentInfo.LastNameEng + " " + studentInfo.FirstNameEng.Substring(0, 1) + ".";
                model.StudentGroup = studentOrgInfo?.DepartmentGroup?.DepartmentCode;
                if (studentOrgInfo?.PrepDepartmentGroup != null) 
                    model.StudentGroup += $"/{studentOrgInfo?.PrepDepartmentGroup.DepartmentCode}";
                model.StudentState = (enu_StudentState)studentOrgInfo?.State;

                var registration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.SemesterId == semesterId &&
                                            x.StudentUserId == adviserStudent.StudentUserId);

                string regStateDesc = "Не создана";
                string regAddDropStateDesc = "Не создана";
                if (registration != null)
                {
                    if (incompleteRegistration && registration.State == (int)enu_RegistrationState.Submitted)
                        continue;

                    model.StudentCourseRegistrationId = registration.Id;
                    model.RegistrationState = (enu_RegistrationState)registration.State;
                    model.RegistrationAddDropState = (enu_RegistrationState)registration.AddDropState;
                    regStateDesc = EnumExtentions.GetDisplayName((enu_RegistrationState)registration.State);
                    regAddDropStateDesc = EnumExtentions.GetDisplayName((enu_RegistrationState)registration.AddDropState);
                }

                model.RegistrationStateDesc = regStateDesc;
                model.RegistrationAddDropStateDesc = regAddDropStateDesc;
                model.DebtMarks = _studentDebtService.GetStudentDebtMarks(semesterId, adviserStudent.StudentUserId);

                if (withDebtsOnly &&
                    !model.DebtMarks.Accounting &&
                    !model.DebtMarks.Library &&
                    !model.DebtMarks.Dormitory &&
                    !model.DebtMarks.RegistarOffice &&
                    !model.DebtMarks.MedicineOffice)
                    continue;

                registrationList.Add(model);
            }

            return registrationList.OrderBy(x => x.StudentGroup).ThenBy(x => x.StudentName).ToList();
        }

        /// <summary>
        /// Get student registration check model by registration id
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <returns>Check registration model</returns>
        public CheckRegistrationViewModel GetStudentRegistrationCheckModel(int studentCourseRegistrationId) 
        {
            CheckRegistrationViewModel model = new CheckRegistrationViewModel();

            var registration = _db.StudentCourseRegistrations
                .FirstOrDefault(x => x.Id == studentCourseRegistrationId);

            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            model.StudentCourseRegistrationId = studentCourseRegistrationId;
            model.SemesterId = registration.SemesterId;
            model.StudentName = _userManager.GetUserFullName(registration.StudentUserId);

            model.RegistrationState = EnumExtentions.GetDisplayName((enu_RegistrationState)registration.State);
            model.Disaprove = !registration.IsApproved;
            model.AdviserComment = registration.AdviserComment;
            model.StudentComment = registration.StudentComment;
            model.MaxRegistrationCredits = _envarSettingService.GetMaxRegistrationCredits(registration.OrganizationId);
            model.NoCreditsLimitation = registration.NoCreditsLimitation;
            /*var studentCourses = _db.StudentCourses
                .Include(x => x.StudyCardCourse).ThenInclude(x => x.CyclePartCourse).ThenInclude(x => x.Course).ThenInclude(x => x.CoursePrerequisites)
                .Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId)
                .ToList();*/

            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.AnnouncementSection)
                .ThenInclude(x => x.ExtraInstructors)
                .Include(x => x.AnnouncementSection)
                .ThenInclude(x => x.Course)
                .ThenInclude(x => x.CoursePrerequisites)
                .ThenInclude(x => x.Prerequisite)
                .Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId && x.State != (int)enu_CourseState.Added)
                .ToList();

            var passedGradeIds = GetPassedGradeIds();

            var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(registration.OrganizationId, registration.StudentUserId);
            var studyCard = _studyCardService.GetStudyCard(registration.SemesterId, studentOrgInfo.DepartmentGroupId);

            foreach (var course in studentCourses) 
            {
                CheckStudentCourse checkCourse = new CheckStudentCourse();
                checkCourse.StudentCourseId = course.Id;
                checkCourse.Name = course.AnnouncementSection.Course.NameRus + " \\ " + course.AnnouncementSection.Course.NameEng 
                        + " \\ " + course.AnnouncementSection.Course.NameKir + $", sec - {course.AnnouncementSection.Season}";
                checkCourse.Code = course.AnnouncementSection.Course.Abbreviation + course.AnnouncementSection.Course.Number;
                checkCourse.ImportCode = course.AnnouncementSection.Course.ImportCode;
                checkCourse.Points = (int)course.AnnouncementSection.Credits;
                checkCourse.Comment = course.Comment;
                checkCourse.IsApproved = course.IsApproved;
                checkCourse.IsProcessed = course.IsProcessed;
                checkCourse.Queue = _studentCourseRegistrationService.GetStudentQueue(course.AnnouncementSection.Id, 
                    course.AnnouncementSection.Places, registration.StudentUserId);
                checkCourse.IsAudit = course.IsAudit;
                checkCourse.NoCreditsCount = course.AnnouncementSection.Course.CourseType == (int)enu_CourseType.PhysEd; 
                checkCourse.InstructorName = _userManager.GetUserFullName(course.AnnouncementSection.InstructorUserId);
                if (studyCard != null) 
                {
                    var studyCardCourse = studyCard.StudyCardCourses.FirstOrDefault(x => x.RegistrationCourseId == course.AnnouncementSectionId);
                    if (studyCardCourse != null) 
                    {
                        checkCourse.IsFromStudyCard = true;
                        checkCourse.StudyCardComment = studyCardCourse.Comment;
                    }
                }
                    

                if (course.AnnouncementSection.ExtraInstructors.Any()) 
                {
                    foreach (var extraInstructor in course.AnnouncementSection.ExtraInstructors) 
                        checkCourse.InstructorName += $", {_userManager.GetUserFullName(extraInstructor.InstructorUserId)}";
                }

                CheckPrerequisiteTemp(checkCourse, course, registration.StudentUserId, passedGradeIds);

                model.StudentCourses.Add(checkCourse);
            }

            return model;
        }

        /// <summary>
        /// Get student add/drop check model by registration id
        /// </summary>
        /// <param name="studentCourseRegistrationId">Student course registration id</param>
        /// <returns>Check registration model</returns>
        public CheckRegistrationViewModel GetStudentAddDropCheckModel(int studentCourseRegistrationId)
        {
            CheckRegistrationViewModel model = new CheckRegistrationViewModel();

            var registration = _db.StudentCourseRegistrations
                .FirstOrDefault(x => x.Id == studentCourseRegistrationId);

            if (registration == null)
                throw new Exception($"Registration with id {studentCourseRegistrationId} not found");

            model.StudentCourseRegistrationId = studentCourseRegistrationId;
            model.SemesterId = registration.SemesterId;
            model.StudentName = _userManager.GetUserFullName(registration.StudentUserId);
            model.RegistrationState = EnumExtentions.GetDisplayName((enu_RegistrationState)registration.AddDropState);
            model.Disaprove = !registration.IsAddDropApproved;
            model.AdviserComment = registration.AdviserComment;
            model.StudentComment = registration.StudentComment;
            model.MaxRegistrationCredits = _envarSettingService.GetMaxRegistrationCredits(registration.OrganizationId);
            model.NoCreditsLimitation = registration.NoCreditsLimitation;

            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.AnnouncementSection)
                .ThenInclude(x => x.ExtraInstructors)
                .Include(x => x.AnnouncementSection)
                .ThenInclude(x => x.Course)
                .ThenInclude(x => x.CoursePrerequisites)
                .ThenInclude(x => x.Prerequisite)
                .Where(x => x.StudentCourseRegistrationId == studentCourseRegistrationId)
                .ToList();

            var passedGradeIds = GetPassedGradeIds();

            var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(registration.OrganizationId, registration.StudentUserId);
            var studyCard = _studyCardService.GetStudyCard(registration.SemesterId, studentOrgInfo.DepartmentGroupId);

            foreach (var course in studentCourses)
            {
                CheckStudentCourse checkCourse = new CheckStudentCourse();
                checkCourse.StudentCourseId = course.Id;
                checkCourse.Name = course.AnnouncementSection.Course.NameRus + " \\ " + course.AnnouncementSection.Course.NameEng
                        + " \\ " + course.AnnouncementSection.Course.NameKir + $", sec - {course.AnnouncementSection.Season}";
                checkCourse.Code = course.AnnouncementSection.Course.Abbreviation + course.AnnouncementSection.Course.Number;
                checkCourse.ImportCode = course.AnnouncementSection.Course.ImportCode;
                checkCourse.Points = (int)course.AnnouncementSection.Credits;
                checkCourse.Comment = course.Comment;
                checkCourse.IsApproved = course.IsAddDropApproved;
                checkCourse.IsProcessed = course.IsAddDropProcessed;
                checkCourse.Queue = _studentCourseRegistrationService.GetStudentQueue(course.AnnouncementSection.Id,
                    course.AnnouncementSection.Places, registration.StudentUserId);
                CheckPrerequisiteTemp(checkCourse, course, registration.StudentUserId, passedGradeIds);
                checkCourse.State = (enu_CourseState)course.State;
                checkCourse.IsAudit = course.IsAudit;
                checkCourse.NoCreditsCount = course.AnnouncementSection.Course.CourseType == (int)enu_CourseType.PhysEd;
                checkCourse.InstructorName = _userManager.GetUserFullName(course.AnnouncementSection.InstructorUserId);

                if (studyCard != null)
                {
                    var studyCardCourse = studyCard.StudyCardCourses.FirstOrDefault(x => x.RegistrationCourseId == course.AnnouncementSectionId);
                    if (studyCardCourse != null)
                    {
                        checkCourse.IsFromStudyCard = true;
                        checkCourse.StudyCardComment = studyCardCourse.Comment;
                    }
                }

                if (course.AnnouncementSection.ExtraInstructors.Any())
                {
                    foreach (var extraInstructor in course.AnnouncementSection.ExtraInstructors)
                    {
                        checkCourse.InstructorName += $", {_userManager.GetUserFullName(extraInstructor.InstructorUserId)}";
                    }
                }

                model.StudentCourses.Add(checkCourse);
            }

            return model;
        }

        private void CheckPrerequisiteTemp(CheckStudentCourse checkCourse, StudentCourseTemp course, string studentUserId,
            List<int> passedGradeIds)
        {
            bool passed = true;
            List<CheckCoursePrerequisite> coursePrerequsites = new List<CheckCoursePrerequisite>();
            var coursePrerequisites = course.AnnouncementSection.Course.CoursePrerequisites?.ToList();
            if (coursePrerequisites != null && coursePrerequisites.Count > 0)
            {
                var passedCourses = _db.StudentCourseGrades
                    .Include(x => x.Grade)
                    .Where(x => x.StudentUserId == studentUserId && passedGradeIds.Contains(x.GradeId.Value));

                foreach (var prerequisite in coursePrerequisites)
                {
                    CheckCoursePrerequisite checkCoursePrerequisite = new CheckCoursePrerequisite();
                    checkCoursePrerequisite.Name = prerequisite.Prerequisite.NameRus + "\\" + prerequisite.Prerequisite.NameEng;
                    var passedCourse = passedCourses.FirstOrDefault(x => x.CourseId == prerequisite.PrerequisiteId);
                    if (passedCourse != null)
                    {
                        checkCoursePrerequisite.Grade = passedCourse.Grade.GradeMark;
                        checkCoursePrerequisite.Passed = true;
                    }
                    else 
                    {
                        checkCoursePrerequisite.Grade = "*";
                        checkCoursePrerequisite.Passed = false;
                        passed = false;
                    }
                    coursePrerequsites.Add(checkCoursePrerequisite);
                }
            }
            checkCourse.PassedPrerequisite = passed;
            checkCourse.CoursePrerequisites = coursePrerequsites;
        }

        private List<int> GetPassedGradeIds() 
        {
            List<string> notPassedGradeMarks = new List<string> { "*", "NP", "W", "I", "X", "F" };
            return _db.Grades.Where(x => !notPassedGradeMarks.Contains(x.GradeMark)).Select(x => x.Id).ToList();
        }

        private bool CheckPrerequisite(StudentCourse course, string studentUserId) 
        {
            var passed = true;
            if (course.OldStudyCardCourse.CyclePartCourse.Course.CoursePrerequisites != null &&
                    course.OldStudyCardCourse.CyclePartCourse.Course.CoursePrerequisites.Count > 0)
            {
                var passedCoursesIds = _db.StudentCourses
                    .Include(x => x.StudentCourseRegistration)
                    .Where(x => x.StudentCourseRegistration.StudentUserId == studentUserId && x.IsPassed)
                    .Select(x => x.OldStudyCardCourse.CyclePartCourse.CourseId)
                    .ToList();

                foreach (var prerequisite in course.OldStudyCardCourse.CyclePartCourse.Course.CoursePrerequisites)
                {
                    if (!passedCoursesIds.Contains(prerequisite.CourseId))
                    {
                        passed = false;
                        break;
                    }
                }
            }

            return passed;
        }

        /// <summary>
        /// Save changes of student registration checking
        /// </summary>
        /// <param name="model">Check registration post view model</param>
        public void CheckStudentRegistration(CheckRegistrationPostViewModel model)
        {
            if (model == null)
                throw new Exception("Model is null");

            var registration = _db.StudentCourseRegistrations
                .FirstOrDefault(x => x.Id == model.StudentCourseRegistrationId);

            if (registration == null)
                throw new Exception($"Registration with id {model.StudentCourseRegistrationId} not found");

            /*var studentCourses = _db.StudentCourses
                .Include(x => x.StudyCardCourse).ThenInclude(x => x.CyclePartCourse).ThenInclude(x => x.Course)
                .Where(x => x.StudentCourseRegistrationId == model.StudentCourseRegistrationId)
                .ToList();*/

            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course)
                .Where(x => x.StudentCourseRegistrationId == model.StudentCourseRegistrationId)
                .ToList();

            if (!registration.NoCreditsLimitation && !model.Disaprove)
            {
                var maxregistrationCredits = _envarSettingService.GetMaxRegistrationCredits(registration.OrganizationId);
                float totalCredits = studentCourses.Where(x => x.State == (int)enu_CourseState.Regular && !x.IsAudit && 
                    x.AnnouncementSection.Course.CourseType != (int)enu_CourseType.PhysEd)
                    .Sum(x => x.AnnouncementSection.Credits);
                if (totalCredits > maxregistrationCredits)
                    throw new Exception($"Total credits more than {maxregistrationCredits}");
            }

            foreach (var modelCourse in model.StudentCourses)
            {
                var course = studentCourses.FirstOrDefault(x => x.Id == modelCourse.StudentCourseId);
                if (course == null)
                    throw new Exception($"Course with id {modelCourse.StudentCourseId} not found");

                course.IsProcessed = true;
                course.IsApproved = modelCourse.IsApproved;
                if (modelCourse.IsApproved)
                    course.Comment = "";
                else 
                {
                    model.Disaprove = true;
                    if (string.IsNullOrEmpty(model.AdviserComment))
                        model.AdviserComment = "Исправьте курсы";

                    course.Comment = modelCourse.Comment;
                }

                //_db.StudentCourses.Update(course);
                _db.StudentCoursesTemp.Update(course);
            }

            if (model.Disaprove)
            {
                registration.IsApproved = false;
                registration.AdviserComment = model.AdviserComment;
                registration.State = (int)enu_RegistrationState.NotApproved;
            }
            else
            {
                registration.IsApproved = true;
                registration.AdviserComment = "";
                registration.State = (int)enu_RegistrationState.Approved;
            }

            registration.DateChange = DateTime.Now;

            _db.StudentCourseRegistrations.Update(registration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Save changes of student add/drop form
        /// </summary>
        /// <param name="model">Check registration post view model</param>
        public void CheckStudentAddDropForm(CheckRegistrationPostViewModel model)
        {
            if (model == null)
                throw new Exception("Model is null");

            var registration = _db.StudentCourseRegistrations
                .FirstOrDefault(x => x.Id == model.StudentCourseRegistrationId);

            if (registration == null)
                throw new Exception($"Registration with id {model.StudentCourseRegistrationId} not found");


            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.AnnouncementSection).ThenInclude(x => x.Course)
                .Where(x => x.StudentCourseRegistrationId == model.StudentCourseRegistrationId)
                .ToList();

            if (!registration.NoCreditsLimitation && !model.Disaprove)
            {
                var maxregistrationCredits = _envarSettingService.GetMaxRegistrationCredits(registration.OrganizationId);
                float totalCredits = studentCourses.Where(x => x.State != (int)enu_CourseState.Dropped && !x.IsAudit &&
                    x.AnnouncementSection.Course.CourseType != (int)enu_CourseType.PhysEd)
                    .Sum(x => x.AnnouncementSection.Credits);
                if (totalCredits > maxregistrationCredits)
                    throw new Exception($"Total credits more than {maxregistrationCredits}");
            }

            foreach (var modelCourse in model.StudentCourses.Where(x => x.State != (int)enu_CourseState.Regular))
            {
                var course = studentCourses.FirstOrDefault(x => x.Id == modelCourse.StudentCourseId);
                if (course == null)
                    throw new Exception($"Course with id {modelCourse.StudentCourseId} not found");

                course.IsAddDropProcessed = true;
                course.IsAddDropApproved = modelCourse.IsApproved;
                if (modelCourse.IsApproved)
                    course.Comment = "";
                else
                {
                    model.Disaprove = true;
                    if (string.IsNullOrEmpty(model.AdviserComment))
                        model.AdviserComment = "Исправьте курсы";

                    course.Comment = modelCourse.Comment;
                }

                //_db.StudentCourses.Update(course);
                _db.StudentCoursesTemp.Update(course);
            }

            if (model.Disaprove)
            {
                registration.IsAddDropApproved = false;
                registration.AdviserComment = model.AdviserComment;
                registration.AddDropState = (int)enu_RegistrationState.NotApproved;
            }
            else
            {
                registration.IsAddDropApproved = true;
                registration.AdviserComment = "";
                registration.AddDropState = (int)enu_RegistrationState.Approved;
            }

            registration.DateChange = DateTime.Now;

            _db.StudentCourseRegistrations.Update(registration);
            _db.SaveChanges();
        }

        /// <summary>
        /// Check if adviser has student
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns></returns>
        public bool HasStudent(int organizationId, string adviserUserId, string studentUserId) 
        {
            return _db.AdviserStudents.Any(x => x.OrganizationId == organizationId &&
                    x.InstructorUserId == adviserUserId && x.StudentUserId == studentUserId);
        }

    }
}
