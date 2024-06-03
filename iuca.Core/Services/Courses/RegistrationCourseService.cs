using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Courses;
using iuca.Application.ViewModels.Users.Students;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Courses
{
    public class RegistrationCourseService : IRegistrationCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IStudentCourseTempService _studentCourseTempService;
        private readonly IDepartmentGroupService _departmentGroupService;

        public RegistrationCourseService(IApplicationDbContext db,
            IMapper mapper,
            ApplicationUserManager<ApplicationUser> userManager,
            IStudentCourseTempService studentCourseTempService,
            IDepartmentGroupService departmentGroupService) 
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _studentCourseTempService = studentCourseTempService;
            _departmentGroupService = departmentGroupService;
        }

        /// <summary>
        /// Get registration course
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Registration course</returns>
        public RegistrationCourseDTO GetRegistrationCourse(int selectedOrganizationId, int registrationCourseId)
        {
            var registrationCourse = _db.AnnouncementSections.Include(x => x.Course)
                .ThenInclude(x => x.CoursePrerequisites)
                .ThenInclude(x => x.Prerequisite)
                .Include(x => x.ExtraInstructors)
                .Include(x => x.Syllabus)
                .FirstOrDefault(x => x.OrganizationId == selectedOrganizationId &&
                            x.Id == registrationCourseId);

            return _mapper.Map<RegistrationCourseDTO>(registrationCourse);
        }

        /// <summary>
        /// Get students for selection window
        /// </summary>
        /// <param name="organizationId">Organiaztion id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="excludedIds">Studnet ids to exclude</param>
        /// <param name="onlyActive">Select only active and academic leave students</param>
        /// <returns>List of students</returns>
        public List<SelectStudentViewModel> GetStudentsForSelection(int organizationId, int semesterId, 
            string[] excludedIds, bool onlyActive = true)
        {
            List<SelectStudentViewModel> studentList = new List<SelectStudentViewModel>();

            var studentIds = _db.UserTypeOrganizations.Where(x => x.OrganizationId == organizationId &&
                            x.UserType == (int)enu_UserType.Student && !excludedIds.Contains(x.ApplicationUserId))
                            .Select(x => x.ApplicationUserId).ToList();

            studentIds = _db.StudentCourseRegistrations.Where(x => x.OrganizationId == organizationId && 
                    x.SemesterId == semesterId && studentIds.Contains(x.StudentUserId)).Select(x => x.StudentUserId)
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
                                .Where(x => studentIds.Contains(x.Id));

            if (onlyActive)
                students = students.Where(x => x.StudentBasicInfo.StudentOrgInfo == null ||
                    x.StudentBasicInfo.StudentOrgInfo.Any(x => x.State == (int)enu_StudentState.Active ||
                    x.State == (int)enu_StudentState.AcadLeave));

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
        /// <returns>List of students from selection window</returns>
        public List<RegistrationCourseStudentViewModel> AddStudentsFromSelection(int organizationId, int semesterId,
            int registrationCourseId, string[] studentUserIds)
        {
            List<RegistrationCourseStudentViewModel> studentList = new List<RegistrationCourseStudentViewModel>();
            
            var studentRegistrationIds = _db.StudentCourseRegistrations.Where(x => x.OrganizationId == organizationId &&
                x.SemesterId == semesterId && studentUserIds.Contains(x.StudentUserId)).Select(x => x.Id).ToList();

            if (studentRegistrationIds.Count != studentUserIds.Length)
                throw new Exception("Some registrations not found");

            var students = _userManager.Users
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .ThenInclude(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .Where(x => studentUserIds.Contains(x.Id))
                .ToList();

            if (students.Count != studentUserIds.Length)
                throw new Exception("Some students not found");

            foreach (var studentRegistrationId in studentRegistrationIds) 
                _studentCourseTempService.AddCourseToRegistrationByAdmin(studentRegistrationId, registrationCourseId, enu_CourseState.Regular);

            foreach (var student in students)
            {
                RegistrationCourseStudentViewModel model = new RegistrationCourseStudentViewModel();

                model.StudentName = student.FullNameEng;
                model.StudentUserId = student.Id;

                if (student.StudentBasicInfo != null && student.StudentBasicInfo.StudentOrgInfo != null)
                {
                    var orgInfo = student.StudentBasicInfo.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId);
                    if (orgInfo != null)
                    {
                        model.Group = $"{orgInfo.DepartmentGroup.Department.Code}{orgInfo.DepartmentGroup.Code}";
                        model.StudentState = (enu_StudentState)orgInfo.State;
                    }
                }

                studentList.Add(model);
            }

            return studentList;
        }

        /// <summary>
        /// Get registration courses list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="season">Season</param>
        /// <param name="year">Year</param>
        /// <returns>Registration courses list</returns>
        public IEnumerable<RegistrationCourseDTO> GetRegistrationCourses(int selectedOrganizationId, int year, int season)
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Course, CourseDTO>().ForMember(x => x.Department, options => options.Ignore());
                cfg.CreateMap<AnnouncementSection, RegistrationCourseDTO>().ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Credits));
            }).CreateMapper();

            var registrationCourses = _db.AnnouncementSections.Where(x => x.OrganizationId == selectedOrganizationId && x.Season == season && x.Year == year)
                .Include(x => x.Course)
                .OrderBy(x => x.Course.NameRus)
                .ThenBy(x => x.Course.NameEng)
                .ToList();

            return mapper.Map<List<AnnouncementSection>, List<RegistrationCourseDTO>>(registrationCourses);
        }

        /// <summary>
        /// Get registration courses information
        /// </summary>
        /// <param name = "semesterId" > Semester id</param>
        /// <returns>Registration courses info list</returns>
        public IEnumerable<RegistrationCourseViewModel> GetRegistrationCoursesInfo(int semesterId)
        {
            var announcementSections = _db.AnnouncementSections
                .Include(x => x.Course)
                .Include(x => x.Announcement)
                .ThenInclude(x => x.Semester)
                .Where(x => x.Announcement.SemesterId == semesterId && x.Announcement.IsActivated == true)
                .ToList();

            var courseStudents = _db.StudentCoursesTemp.Include(x => x.StudentCourseRegistration)
                .Where(x => announcementSections.Select(x => x.Id).Contains(x.AnnouncementSectionId) &&
                    x.State != (int)enu_CourseState.Dropped).ToList();

            List<RegistrationCourseViewModel> registrationCoursesModel = new List<RegistrationCourseViewModel>();
            foreach (var announcementSection in announcementSections) 
            {
                var row = new RegistrationCourseViewModel();
                row.AnnouncementSection = _mapper.Map<AnnouncementSectionDTO>(announcementSection);
                row.StudentsNumber = courseStudents.Count(y => y.AnnouncementSectionId == announcementSection.Id);
                row.InstructorName = _userManager.GetUserFullName(announcementSection.InstructorUserId);

                if (row.AnnouncementSection.ExtraInstructorsJson != null && row.AnnouncementSection.ExtraInstructorsJson.Any())
                    row.AnnouncementSection.ExtraInstructorsList = row.AnnouncementSection.ExtraInstructorsJson.Select(x => new UserDTO
                    {
                        Id = x,
                        FullName = _userManager.GetUserFullName(x)
                    });

                if (row.AnnouncementSection.GroupsJson != null && row.AnnouncementSection.GroupsJson.Any())
                    row.AnnouncementSection.Groups = row.AnnouncementSection.GroupsJson.Select(x => new GroupDTO
                    {
                        Id = int.Parse(x),
                        Code = _departmentGroupService.GetDepartmentGroup(announcementSection.Announcement.Semester.OrganizationId, int.Parse(x)).DepartmentCode
                    });

                registrationCoursesModel.Add(row);
            }

            return registrationCoursesModel;
        }

        /// <summary>
        /// Get registration courses with syllabi and syllabus status counts
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="deanDepartments">Dean departments</param>
        /// <param name="status">Syllabus status</param>
        /// <returns>Registration courses list with syllabi and syllabus status counts</returns>
        public RegistrationCoursesSyllabusStatusesViewModel GetRegistrationCoursesWithSyllabi(int selectedOrganizationId, 
            int semesterId, int departmentId, IEnumerable<DepartmentDTO> deanDepartments, int? status)
        {
            var semester = _db.Semesters.Find(semesterId);

            var registrationCourses = _db.AnnouncementSections
                .Include(x => x.Course)
                .ThenInclude(x => x.Department)
                .Include(x => x.Course)
                .ThenInclude(x => x.Language)
                .Include(x => x.Announcement)
                .Include(x => x.Syllabus)
                .Where(x => x.OrganizationId == selectedOrganizationId &&
                           x.Season == semester.Season && x.Year == semester.Year && 
                           x.Announcement.IsActivated == true);

            if (deanDepartments != null && deanDepartments.Any())
                registrationCourses = registrationCourses.Where(x => deanDepartments.Select(x => x.Id).Contains(x.Course.DepartmentId));

            if (departmentId != 0)
                registrationCourses = registrationCourses.Where(x => x.Course.DepartmentId == departmentId);

            var registrationCoursesModel = new RegistrationCoursesSyllabusStatusesViewModel();

            registrationCoursesModel.notAddedCount = registrationCourses.Count(x => x.Syllabus == null);
            registrationCoursesModel.notSubmittedCount = registrationCourses.Count(x => x.Syllabus != null && x.Syllabus.Status == (int)enu_SyllabusStatus.Draft);
            registrationCoursesModel.rejectedCount = registrationCourses.Count(x => x.Syllabus != null && x.Syllabus.Status == (int)enu_SyllabusStatus.Rejected);
            registrationCoursesModel.pendingCount = registrationCourses.Count(x => x.Syllabus != null && x.Syllabus.Status == (int)enu_SyllabusStatus.Pending);
            registrationCoursesModel.approvedCount = registrationCourses.Count(x => x.Syllabus != null && x.Syllabus.Status == (int)enu_SyllabusStatus.Approved);

            if (status == 0)
                registrationCourses = registrationCourses.Where(x => x.Syllabus == null);
            else if (status != null)
                registrationCourses = registrationCourses.Where(x => x.Syllabus.Status == status);

            var filteredRegistrationCourses = registrationCourses.ToList();

            List<RegistrationCourseViewModel> registrationCourseViewModel = new List<RegistrationCourseViewModel>();

            foreach (var registrationCourse in filteredRegistrationCourses)
            {
                var row = new RegistrationCourseViewModel();
                row.AnnouncementSection = _mapper.Map<AnnouncementSectionDTO>(registrationCourse);
                row.InstructorName = _userManager.GetUserFullName(registrationCourse.InstructorUserId);
                registrationCourseViewModel.Add(row);
            }

            registrationCoursesModel.RegistrationCourses = registrationCourseViewModel
                .OrderBy(x => x.AnnouncementSection.Course.Abbreviation)
                .ThenBy(x => x.AnnouncementSection.Course.Number)
                .ThenBy(x => x.AnnouncementSection.Course.Name)
                .ToList();

            return registrationCoursesModel;
        }

        /// <summary>
        /// Edit registration course
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="registrationCourse">Registration course</param>
        public void EditRegistrationCourse(int organizationId, RegistrationCourseDTO registrationCourse)
        {
            if (registrationCourse == null)
                return;

            var dbRegistrationCourse = _db.AnnouncementSections.Include(x => x.ExtraInstructors)
                        .FirstOrDefault(x => x.Id == registrationCourse.Id);

            if (dbRegistrationCourse == null)
                throw new Exception($"Registration course with id {registrationCourse.Id} not found");

            dbRegistrationCourse.Places = registrationCourse.Places;
            dbRegistrationCourse.Schedule = registrationCourse.Schedule;

            if (registrationCourse.Points != dbRegistrationCourse.Credits) 
            {
                dbRegistrationCourse.Credits = registrationCourse.Points;
                dbRegistrationCourse.IsChanged = true;
            }

            if (registrationCourse.InstructorUserId != dbRegistrationCourse.InstructorUserId)
            {
                var instructor = _db.InstructorOrgInfo.Include(x => x.InstructorBasicInfo)
                    .FirstOrDefault(x => x.OrganizationId == organizationId &&
                        x.InstructorBasicInfo.InstructorUserId == registrationCourse.InstructorUserId);
                
                if (instructor == null)
                    throw new Exception($"Instructor with id {registrationCourse.InstructorUserId} not found");

                if (instructor.ImportCode == 0)
                    throw new Exception("Instructor import code is wrong");

                dbRegistrationCourse.InstructorUserId = registrationCourse.InstructorUserId;
                dbRegistrationCourse.IsChanged = true;
            }

            EditExtraInstructors(dbRegistrationCourse, dbRegistrationCourse.ExtraInstructors.ToList(), 
                registrationCourse.ExtraInstructors);

            _db.AnnouncementSections.Update(dbRegistrationCourse);
            _db.SaveChanges();
        }


        private void EditExtraInstructors(AnnouncementSection dbRegistrationCourse, List<ExtraInstructor> existingExtraInstructors,
            List<ExtraInstructorDTO> newExtraInstructors)
        {
            if (newExtraInstructors != null) 
            {
                foreach (ExtraInstructorDTO extraInstructor in newExtraInstructors)
                {
                    if (extraInstructor.RegistrationCourseId == 0)
                        extraInstructor.RegistrationCourseId = dbRegistrationCourse.Id;

                    ExtraInstructor existingExtraInstructor = _db.ExtraInstructors
                        .FirstOrDefault(x => x.AnnouncementSectionId == dbRegistrationCourse.Id
                            && x.InstructorUserId == extraInstructor.InstructorUserId);

                    if (existingExtraInstructor == null) 
                    {
                        _db.ExtraInstructors.Add(_mapper.Map<ExtraInstructor>(extraInstructor));
                        dbRegistrationCourse.IsChanged = true;
                    }
                }
            }

            if (existingExtraInstructors.Any())
            {
                foreach (ExtraInstructor extraInstructor in existingExtraInstructors)
                {
                    if (newExtraInstructors == null || !newExtraInstructors.Any(x => x.RegistrationCourseId ==
                                extraInstructor.AnnouncementSectionId && x.InstructorUserId == extraInstructor.InstructorUserId)) 
                    {
                        _db.ExtraInstructors.Remove(extraInstructor);
                        dbRegistrationCourse.IsChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Mark registration course as deleted/undeleted
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="courseDetId">Course details id</param>
        /// <param name="isDeleted">Is deleted flag</param>
        public void MarkRegistrationCourseDeleted(int organizationId, int courseDetId, bool isDeleted) 
        {
            var dbRegistrationCourse = _db.AnnouncementSections.FirstOrDefault(x => x.OrganizationId == organizationId &&
                    x.CourseDetId == courseDetId);

            if (dbRegistrationCourse == null)
                throw new Exception("Registration course not found");

            if (isDeleted && _db.StudentCoursesTemp.Any(x => x.AnnouncementSectionId == dbRegistrationCourse.Id))
                throw new Exception("Students exist for deleting registration course");

            dbRegistrationCourse.MarkedDeleted = isDeleted;
            _db.AnnouncementSections.Update(dbRegistrationCourse);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get registration course details with registered students
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <returns>Registration course details view model</returns>
        public RegistrationCourseDetailsViewModel GetRegistrationCourseDetails(int organizationId, int registrationCourseId) 
        {
            RegistrationCourseDetailsViewModel model = new RegistrationCourseDetailsViewModel();
            model.RegistrationCourse = GetRegistrationCourse(organizationId, registrationCourseId);
            model.InstructorName = _userManager.GetUserFullName(model.RegistrationCourse.InstructorUserId);

            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.StudentCourseRegistration)
                .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId &&
                    x.AnnouncementSectionId == registrationCourseId &&
                    x.State != (int)enu_CourseState.Dropped).ToList();

            var sudentQueues = GetStudentQueues(studentCourses, model.RegistrationCourse.Places);

            var studentIds = studentCourses.Select(x => x.StudentCourseRegistration.StudentUserId)
                .ToList();

            var studentOrgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                .Include(x => x.PrepDepartmentGroup).ThenInclude(x => x.Department)
                .Where(x => x.OrganizationId == organizationId && studentIds.Contains(x.StudentBasicInfo.ApplicationUserId)).ToList();

            foreach (var orgInfo in studentOrgInfo) 
            {
                RegistrationCourseStudentViewModel studentViewModel = new RegistrationCourseStudentViewModel();
                var studentCourse = studentCourses
                    .FirstOrDefault(x => x.StudentCourseRegistration.StudentUserId == orgInfo.StudentBasicInfo.ApplicationUserId);

                if (studentCourse == null)
                    throw new Exception("Student course is not found");

                studentViewModel.StudentUserId = orgInfo.StudentBasicInfo.ApplicationUserId;
                studentViewModel.StudentCourseId = studentCourse.Id;
                studentViewModel.StudentName = _userManager.GetUserFullName(orgInfo.StudentBasicInfo.ApplicationUserId);
                studentViewModel.Group = GetDepartmentGroup(orgInfo);
                studentViewModel.StudentState = (enu_StudentState)orgInfo.State;
                studentViewModel.Queue = model.RegistrationCourse.Places > 0 ? GetStudentQueue(sudentQueues, orgInfo.StudentBasicInfo.ApplicationUserId) : 0;
                studentViewModel.IsDeleted = studentCourse.MarkedDeleted;
                studentViewModel.RegistrationState = (enu_RegistrationState)studentCourse.StudentCourseRegistration.AddDropState != enu_RegistrationState.NotSent ?
                            (enu_RegistrationState)studentCourse.StudentCourseRegistration.AddDropState : (enu_RegistrationState)studentCourse.StudentCourseRegistration.State;

                model.CourseStudents.Add(studentViewModel);
            }

            return model;

        }

        private string GetDepartmentGroup(StudentOrgInfo orgInfo) 
        {
            string departmentGroup = orgInfo.DepartmentGroup.Department.Code + orgInfo.DepartmentGroup.Code;
            if (orgInfo.PrepDepartmentGroup != null) 
                departmentGroup += "/" + orgInfo.PrepDepartmentGroup.Department.Code + orgInfo.PrepDepartmentGroup.Code;

            return departmentGroup;
        }

        private List<StudentQueueViewModel> GetStudentQueues(List<StudentCourseTemp> studentCourses, int places) 
        {
            List<StudentQueueViewModel> model = new List<StudentQueueViewModel>();

            var sortedStudents = studentCourses.OrderBy(x => x.DateCreated).ToList();
            var acceptedStudents = sortedStudents.Take(places);
            model.AddRange(acceptedStudents.Select(x => new StudentQueueViewModel
            {
                StudentUserId = x.StudentCourseRegistration.StudentUserId,
                Queue = 0
            }));

            var queuedStudents = sortedStudents.Skip(places).ToList();
            for (int i = 0; i < queuedStudents.Count; i++) 
            {
                StudentQueueViewModel queueRow = new StudentQueueViewModel();
                queueRow.StudentUserId = queuedStudents[i].StudentCourseRegistration.StudentUserId;
                queueRow.Queue = i + 1;
                model.Add(queueRow);
            }

            return model;
        }

        private int GetStudentQueue(List<StudentQueueViewModel> studentQueues, string studentUserId) 
        {
            int queue = 0;
            var studentQueue = studentQueues.FirstOrDefault(x => x.StudentUserId == studentUserId);
            if (studentQueue != null)
                queue = studentQueue.Queue;
            
            return queue;
        }
    }
}
