using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Users.Students;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


namespace iuca.Application.Services.Users.Students
{
    public class StudentInfoService : IStudentInfoService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IOrganizationService _organizationService;
        private readonly IStudentBasicInfoService _studentBasicInfoService;
        private readonly IStudentOrgInfoService _studentOrgInfoService;
        private readonly IStudentMinorInfoService _studentMinorInfoService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;


        public StudentInfoService(IApplicationDbContext db,
            IMapper mapper,
            IOrganizationService organizationService,
            IStudentBasicInfoService studentBasicInfoService,
            IStudentOrgInfoService studentOrgInfoService,
            IStudentMinorInfoService studentMinorInfoService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _organizationService = organizationService;
            _studentBasicInfoService = studentBasicInfoService;
            _studentOrgInfoService = studentOrgInfoService;
            _studentMinorInfoService = studentMinorInfoService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get student info list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>List of student info</returns>
        public IEnumerable<StudentInfoBriefViewModel> GetStudentInfoList(int organizationId)
        {
            return GetStudentInfoList(organizationId, false);
        }

        /// <summary>
        /// Get student info list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="onlyActive">Only active students</param>
        /// <returns>List of student info</returns>
        public IEnumerable<StudentInfoBriefViewModel> GetStudentInfoList(int organizationId, bool onlyActive)
        {
            List<StudentInfoBriefViewModel> studentInfoList = new List<StudentInfoBriefViewModel>();

            OrganizationDTO organization = _organizationService.GetOrganization(organizationId);
            if (organization == null)
                throw new Exception("organization is null");

            List<string> userIds = _db.UserTypeOrganizations
                                .Where(x => x.OrganizationId == organizationId && x.UserType == (int)enu_UserType.Student)
                                .Select(x => x.ApplicationUserId).ToList();

            var studentListQuery = _userManager.Users
                    .Include(x => x.StudentBasicInfo)
                    .ThenInclude(x => x.StudentOrgInfo)
                    .ThenInclude(x => x.DepartmentGroup)
                    .ThenInclude(x => x.Department)
                    .Where(x => userIds.Contains(x.Id));

            if (onlyActive) 
            {
                studentListQuery = studentListQuery.Where(x => x.StudentBasicInfo.StudentOrgInfo
                    .Any(y => y.State == (int)enu_StudentState.Active));
            }

            var studentList = studentListQuery.ToList();

            foreach (var student in studentList) 
            {
                StudentInfoBriefViewModel studentInfoVM = new StudentInfoBriefViewModel();
                studentInfoVM.StudentUserId = student.Id;
                studentInfoVM.FullNameEng = student.FullNameEng;
                studentInfoVM.BasicInfoExists = student.StudentBasicInfo != null;

                if (studentInfoVM.BasicInfoExists)
                {

                    //Only main organization is allowed to modify data if IsMainOrganization = true
                    studentInfoVM.IsReadOnly = student.StudentBasicInfo.IsMainOrganization && !organization.IsMain;

                    var studentOrgInfo = student.StudentBasicInfo.StudentOrgInfo
                                        .FirstOrDefault(x => x.StudentBasicInfoId == student.StudentBasicInfo.Id &&
                                            x.OrganizationId == organizationId);

                    //Student org info
                    if (studentOrgInfo != null)
                    {
                        studentInfoVM.DepartmentGroup = studentOrgInfo.DepartmentGroup.Department.Code + " " + studentOrgInfo.DepartmentGroup.Code;
                        studentInfoVM.State = (enu_StudentState)studentOrgInfo.State;
                    }
                }
                    studentInfoList.Add(studentInfoVM);
            }
                
            return studentInfoList.OrderBy(x => x.FullNameEng);
        }

        /// <summary>
        /// Get minimum student info list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentStates">Student state list</param>
        /// <returns>List of minimum student info</returns>
        public IEnumerable<StudentMinimumInfoViewModel> GetStudentInfoList(int organizationId, int[] studentStates)
        {
            List<StudentMinimumInfoViewModel> studentInfoList = new List<StudentMinimumInfoViewModel>();

            OrganizationDTO organization = _organizationService.GetOrganization(organizationId);
            if (organization == null)
                throw new Exception("Organization is null.");

            List<string> userIds = _db.UserTypeOrganizations
                                .Where(x => x.OrganizationId == organizationId && x.UserType == (int)enu_UserType.Student)
                                .Select(x => x.ApplicationUserId).ToList();

            var studentList = _userManager.Users
                    .Include(x => x.StudentBasicInfo)
                    .ThenInclude(x => x.StudentOrgInfo)
                    .ThenInclude(x => x.DepartmentGroup)
                    .ThenInclude(x => x.Department)
                    .Where(x => userIds.Contains(x.Id) && x.StudentBasicInfo.StudentOrgInfo
                    .Any(x => studentStates.Contains(x.State)))
                    .ToList();

            foreach (var student in studentList)
            {
                StudentMinimumInfoViewModel studentInfoVM = new StudentMinimumInfoViewModel();
                studentInfoVM.StudentUserId = student.Id;
                studentInfoVM.FullNameEng = student.FullNameEng;

                if (student.StudentBasicInfo != null)
                {
                    var studentOrgInfo = student.StudentBasicInfo.StudentOrgInfo
                                        .FirstOrDefault(x => x.StudentBasicInfoId == student.StudentBasicInfo.Id &&
                                            x.OrganizationId == organizationId);

                    if (studentOrgInfo != null)
                    {
                        studentInfoVM.StudentId = studentOrgInfo.StudentId;
                        studentInfoVM.Group = studentOrgInfo.DepartmentGroup.Department.Code + "-" + studentOrgInfo.DepartmentGroup.Code;
                    }
                }

                studentInfoList.Add(studentInfoVM);
            }

            return studentInfoList.OrderBy(x => x.FullNameEng).ToList();
        }

        /// <summary>
        /// Get student info by user basic info id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student info model</returns>
        public StudentInfoDetailsViewModel GetStudentDetailsInfo(int organizationId, string studentUserId)
        {
            var student = _userManager.Users
                .Include(x => x.UserBasicInfo).ThenInclude(x => x.Citizenship)
                .Include(x => x.UserBasicInfo).ThenInclude(x => x.Nationality)
                .FirstOrDefault(x => x.Id == studentUserId);

            if (student == null)
                throw new Exception("User not found");

            StudentInfoDetailsViewModel studentInfoVM = new StudentInfoDetailsViewModel();
            studentInfoVM.StudentUserId = student.Id;
            studentInfoVM.FullNameEng = student.FullNameEng;

            //User basic info
            if (student.UserBasicInfo != null)
            {
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Nationality, NationalityDTO>();
                    cfg.CreateMap<Country, CountryDTO>();
                    cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>();
                }).CreateMapper();

                studentInfoVM.UserBasicInfo = mapper.Map<UserBasicInfo, UserBasicInfoDTO>(student.UserBasicInfo);
            }
            else 
            {
                studentInfoVM.UserBasicInfo = new UserBasicInfoDTO { ApplicationUserId = studentUserId };
            }

            studentInfoVM.StudentBasicInfo = _studentBasicInfoService.GetStudentFullInfo(studentUserId, false);
            if (studentInfoVM.StudentBasicInfo != null) 
            {
                OrganizationDTO organization = _organizationService.GetOrganization(organizationId);
                if (organization == null)
                    throw new Exception("organization is null");

                //Only main organization is allowed to modify data if IsMainOrganization = true
                studentInfoVM.IsReadOnly = studentInfoVM.StudentBasicInfo.IsMainOrganization && !organization.IsMain;

                //Get student organization information
                studentInfoVM.StudentOrgInfo = _studentOrgInfoService.GetStudentOrgInfo(organizationId, studentInfoVM.StudentBasicInfo.Id);

                //Get student minor information
                studentInfoVM.StudentMinorInfo = _studentMinorInfoService.GetStudentMinorInfo(studentInfoVM.StudentBasicInfo.Id);
            }

            return studentInfoVM;
        }

        /// <summary>
        /// Get student info by student user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student info model</returns>
        public StudentMinimumInfoViewModel GetStudentMinimumInfo(int organizationId, string studentUserId)
        {
            var student = _userManager.Users
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .ThenInclude(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .FirstOrDefault(x => x.Id == studentUserId);

            if (student == null)
                throw new Exception("User not found");

            StudentMinimumInfoViewModel studentInfoVM = new StudentMinimumInfoViewModel();
            studentInfoVM.StudentUserId = student.Id;
            studentInfoVM.FullNameEng = student.FullNameEng;
            studentInfoVM.ShortNameEng = student.LastNameEng + " " + student.FirstNameEng.Substring(0, 1) + ".";

            if (student.StudentBasicInfo != null)
            {
                var studentOrgInfo = student.StudentBasicInfo.StudentOrgInfo
                                    .FirstOrDefault(x => x.StudentBasicInfoId == student.StudentBasicInfo.Id &&
                                        x.OrganizationId == organizationId);

                if (studentOrgInfo != null)
                {
                    studentInfoVM.StudentId = studentOrgInfo.StudentId;
                    studentInfoVM.Group = studentOrgInfo.DepartmentGroup.Department.Code + "-" + studentOrgInfo.DepartmentGroup.Code;
                }
            }

            return studentInfoVM;
        }

        /// <summary>
        /// Get students list for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="lastName">Student last name</param>
        /// <param name="firstName">Student first name</param>
        /// <param name="studentId">StudentId</param>
        /// <param name="onlyActive">Only active students</param>
        /// <returns>Student list for selection</returns>
        public List<SelectStudentViewModel> GetStudentSelectList(int organizationId, int departmentId, int departmentGroupId,
            string lastName, string firstName, int studentId, bool onlyActive)
        {
            var departmentIds = new List<int>();
            if (departmentId != 0)
                departmentIds.Add(departmentId);

            return GetStudentSelectList(organizationId, departmentIds, departmentGroupId,
                lastName, firstName, studentId, onlyActive);
        }

        /// <summary>
        /// Get students list for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="departmentIds">List of department id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="lastName">Student last name</param>
        /// <param name="firstName">Student first name</param>
        /// <param name="studentId">StudentId</param>
        /// <param name="onlyActive">Only active students</param>
        /// <returns>Student list for selection</returns>
        public List<SelectStudentViewModel> GetStudentSelectList(int organizationId, List<int> departmentIds, int departmentGroupId,
            string lastName, string firstName, int studentId, bool onlyActive)
        {
            List<SelectStudentViewModel> model = new List<SelectStudentViewModel>();

            var studentIds = _db.UserTypeOrganizations.Where(x => x.OrganizationId == organizationId &&
                x.UserType == (int)enu_UserType.Student).Select(x => x.ApplicationUserId).ToList();

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

            if (departmentIds.Count != 0)
                studentList = studentList.Where(x => x.StudentBasicInfo.StudentOrgInfo
                            .Any(y => (departmentIds.Contains(y.DepartmentGroup.DepartmentId) ||
                                (y.PrepDepartmentGroup != null && departmentIds.Contains(y.PrepDepartmentGroup.DepartmentId))) &&
                                y.OrganizationId == organizationId));

            if (departmentGroupId != 0)
                studentList = studentList.Where(x => x.StudentBasicInfo.StudentOrgInfo
                    .Any(y => (y.DepartmentGroupId == departmentGroupId || 
                    y.PrepDepartmentGroupId != null && y.PrepDepartmentGroupId == departmentGroupId) && 
                    y.OrganizationId == organizationId));

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
                        studentModel.StudentId = orgInfo.StudentId;
                        studentModel.State = (enu_StudentState)orgInfo.State;
                    }
                }

                model.Add(studentModel);
            }

            return model;
        }

        /// <summary>
        /// Set student state
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="state">New student state</param>
        public void SetStudentState(int organizationId, string studentUserId, int state)
        {
            if (string.IsNullOrEmpty(studentUserId))
                throw new ArgumentNullException(nameof(studentUserId), "The student user id is null.");

            var student = _userManager.Users
                .Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.Id == studentUserId);

            if (student == null)
                throw new InvalidOperationException("Student not found.");

            var orgInfo = _db.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId &&
                x.StudentBasicInfoId == student.StudentBasicInfo.Id);

            if (orgInfo == null)
                throw new InvalidOperationException("Student orgInfo not found.");

            if (orgInfo.State == state)
                throw new ModelValidationException("Student status is already the same.", "");

            if (orgInfo.State == (int)enu_StudentState.Dismissed)
            {
                if (state == (int)enu_StudentState.AcadLeave)
                    throw new ModelValidationException("A dismissed student cannot obtain 'AcadLeave' status.", "");
            }

            orgInfo.State = state;
            _db.SaveChanges();
        }

        /// <summary>
        /// Get student department group
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Student department group</returns>
        public GroupDTO GetStudentDepartmentGroup(int organizationId, string studentUserId)
        {
            if (string.IsNullOrEmpty(studentUserId))
                throw new ArgumentNullException(nameof(studentUserId), "The student user id is null.");

            var student = _userManager.Users
                .Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.Id == studentUserId);

            if (student == null)
                throw new InvalidOperationException("Student not found.");

            var orgInfo = _db.StudentOrgInfo
                .Include(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .FirstOrDefault(x => x.OrganizationId == organizationId && x.StudentBasicInfoId == student.StudentBasicInfo.Id);

            if (orgInfo == null)
                throw new InvalidOperationException("Student orgInfo not found.");

            var departmentGroup = new GroupDTO
            {
                Id = orgInfo.DepartmentGroupId,
                Code = _mapper.Map<DepartmentGroupDTO>(orgInfo.DepartmentGroup).DepartmentCode
            };

            return departmentGroup;
        }

        /// <summary>
        /// Set student department group
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="newDepartmentGroupId">New student department group</param>
        /// <param name="oldDepartmentGroupId">Old student department group</param>
        public void SetStudentDepartmentGroup(int organizationId, string studentUserId, int newDepartmentGroupId, int oldDepartmentGroupId = 0)
        {
            if (string.IsNullOrEmpty(studentUserId))
                throw new ArgumentNullException(nameof(studentUserId), "The student user id is null.");

            if (newDepartmentGroupId == 0)
                throw new ArgumentException("The new department group id is 0.", nameof(newDepartmentGroupId));

            var student = _userManager.Users
                .Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.Id == studentUserId);

            if (student == null)
                throw new InvalidOperationException("Student not found.");

            var orgInfo = _db.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId &&
                x.StudentBasicInfoId == student.StudentBasicInfo.Id);

            if (orgInfo == null)
                throw new InvalidOperationException("Student orgInfo not found.");

            if (orgInfo.DepartmentGroupId == newDepartmentGroupId)
                throw new ModelValidationException("This is already the student's group.", "");

            if (oldDepartmentGroupId != 0)
            {
                if (oldDepartmentGroupId != orgInfo.DepartmentGroupId)
                    throw new ModelValidationException("The old group does not match the student's current group.", "");
            }

            orgInfo.DepartmentGroupId = newDepartmentGroupId;
            _db.SaveChanges();
        }

        /// <summary>
        /// Create student info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentInfo">Student info model</param>
        public void Create(int organizationId, StudentInfoDetailsViewModel studentInfo)
        {
            if (studentInfo == null)
                throw new Exception("studentInfo is null");

            var student = _db.StudentBasicInfo.FirstOrDefault(x => x.ApplicationUserId == studentInfo.StudentUserId);
            if (student != null)
                throw new ModelValidationException("Student info already exists", "ErrorMsg");

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var newStudentBasicInfo = _studentBasicInfoService.Create(organizationId, studentInfo.StudentBasicInfo);

                    //Create student organization information
                    studentInfo.StudentOrgInfo.StudentBasicInfoId = newStudentBasicInfo.Id;
                    studentInfo.StudentOrgInfo.OrganizationId = organizationId;
                    _studentOrgInfoService.Create(studentInfo.StudentOrgInfo);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Edit student info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentInfo">Student info model</param>
        public void Edit(int organizationId, StudentInfoDetailsViewModel studentInfo)
        {
            if (studentInfo == null)
                throw new Exception("studentInfo is null");

            int studentBasicInfoId = studentInfo.StudentBasicInfo.Id;

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _studentBasicInfoService.Edit(organizationId, studentInfo.StudentBasicInfo);

                    if (_studentOrgInfoService.IfExists(organizationId, studentBasicInfoId))
                        _studentOrgInfoService.Edit(organizationId, studentInfo.StudentOrgInfo);
                    else
                    {
                        //Create student organization information
                        studentInfo.StudentOrgInfo.StudentBasicInfoId = studentBasicInfoId;
                        studentInfo.StudentOrgInfo.OrganizationId = organizationId;
                        _studentOrgInfoService.Create(studentInfo.StudentOrgInfo);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete student info
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="id">Student basic info id</param>
        public void Delete(int organizationId, int id)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _studentOrgInfoService.Delete(organizationId, id);
                    _studentBasicInfoService.Delete(organizationId, id);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Get user list with empty student
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>List of users</returns>
        public IEnumerable<ApplicationUser> GetEmptyStudents(int selectedOrganizationId)
        {
            List<string> userIds = _db.UserTypeOrganizations
                            .Where(x => x.OrganizationId == selectedOrganizationId && x.UserType == (int)enu_UserType.Student)
                            .Select(x => x.ApplicationUserId).ToList();

            return _userManager.Users.Include(x => x.StudentBasicInfo)
                .Where(x => userIds.Contains(x.Id) && x.StudentBasicInfo == null);
        }

        /// <summary>
        /// Get student user ids by department
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="activeOnly">If true - returns only active students</param>
        /// <returns></returns>
        public List<string> GetStudentIdsByDepartment(int organizationId, int departmentId, bool activeOnly = true) 
        {
            var department = _db.Departments.FirstOrDefault(x => x.Id == departmentId);
            if (department == null)
                throw new Exception($"Department with id {departmentId} not found");

            var studentOrgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .Include(x => x.DepartmentGroup)
                .Include(x => x.PrepDepartmentGroup)
                .Where(x => x.OrganizationId == organizationId && x.StudentBasicInfo != null).AsQueryable();

            if (department.Code.StartsWith("PREP"))
                studentOrgInfo = studentOrgInfo.Where(x => x.DepartmentGroup.DepartmentId == departmentId &&
                    x.PrepDepartmentGroup == null);
            else 
                studentOrgInfo = studentOrgInfo.Where(x => x.DepartmentGroup.DepartmentId == departmentId ||
                    x.PrepDepartmentGroup != null && x.PrepDepartmentGroup.DepartmentId == departmentId);

            if (activeOnly) 
                studentOrgInfo = studentOrgInfo.Where(x => x.State == (int)enu_StudentState.Active);

            return studentOrgInfo.Select(x => x.StudentBasicInfo.ApplicationUserId).ToList();
        }

        /// <summary>
        /// Get student user id by student id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentId">Student id</param>
        /// <returns>Student user id</returns>
        public string GetUserIdByStudentId(int organizationId, int studentId)
        {
            if (studentId == 0)
                throw new ArgumentException("The student id is 0.", nameof(studentId));

            var student = _db.StudentOrgInfo
                .Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.StudentId == studentId && x.OrganizationId == organizationId);

            if (student == null)
                throw new ArgumentException($"The student with student id {studentId} and organization id {organizationId} does not exist.");

            return student.StudentBasicInfo.ApplicationUserId;
        }

        /// <summary>
        /// Get prep student list
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="activeOnly">Only active students</param>
        /// <returns>Prep student list</returns>
        public List<PrepStudentViewModel> GetPrepStudents(int organizationId, bool activeOnly = true) 
        {
            List<PrepStudentViewModel> studentList = new List<PrepStudentViewModel>();

            var prepDepartment = _db.Departments.FirstOrDefault(x => x.OrganizationId == organizationId && x.Code == "PREP");
            if (prepDepartment == null)
                throw new Exception("PREP department group not found");

            var prepDepartmentGroupIds = _db.DepartmentGroups.Where(x => x.DepartmentId == prepDepartment.Id)
                .Select(x => x.Id);

            var prepStudentOrgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                .Where(x => x.OrganizationId == organizationId && prepDepartmentGroupIds.Contains(x.DepartmentGroupId));

            if (activeOnly)
                prepStudentOrgInfo = prepStudentOrgInfo.Where(x => x.State == (int)enu_StudentState.Active);

            foreach (var orgInfo in prepStudentOrgInfo.ToList()) 
            {
                PrepStudentViewModel model = new PrepStudentViewModel();
                model.OrganizationId = organizationId;
                model.StudentBasicInfoId = orgInfo.StudentBasicInfoId;
                model.StudentState = (enu_StudentState)orgInfo.State;
                model.DepartmentGroup = orgInfo.DepartmentGroup.Department.Code + orgInfo.DepartmentGroup.Code;
                model.PrepDepartmentGroupId = orgInfo.PrepDepartmentGroupId;
                model.StudentName = _userManager.GetUserFullName(orgInfo.StudentBasicInfo.ApplicationUserId);
                studentList.Add(model);
            }

            return studentList;
        }

        /// <summary>
        /// Save PREP student department group
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="studentBasicInfoId">Student Basic Info Id</param>
        /// <param name="prepDepartmentGroupId">PREP Department Group Id</param>
        public void SavePrepStudentDepartmentGroup(int organizationId, int studentBasicInfoId, int? prepDepartmentGroupId) 
        {
            var orgInfo =_db.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId && 
                    x.StudentBasicInfoId == studentBasicInfoId);

            if (orgInfo == null)
                throw new Exception("Student orgInfo not found");

            orgInfo.PrepDepartmentGroupId = prepDepartmentGroupId;
            _db.SaveChanges();
        }

        /// <summary>
        /// Get students list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="onlyActive">Only active students</param>
        /// <returns>Student list</returns>
        public List<UserDTO> GetStudents(int organizationId, bool onlyActive)
        {
            List<UserDTO> model = new List<UserDTO>();

            var studentList = _userManager.Users
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .Where(x => x.StudentBasicInfo.StudentOrgInfo.Any(x => x.OrganizationId == organizationId));

            if (onlyActive)
                studentList = studentList.Where(x => x.StudentBasicInfo.StudentOrgInfo
                    .Any(y => y.State == (int)enu_StudentState.Active));

            foreach (var student in studentList)
            {
                UserDTO userModel = new UserDTO();
                userModel.Id = student.Id;
                userModel.FullName = student.FullNameEng;
                
                model.Add(userModel);
            }

            return model;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

    }
}
