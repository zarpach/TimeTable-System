using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Courses;
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
    public class RegistrationCourseManagementService : IRegistrationCourseManagementService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ApplicationUserManager<ApplicationUser> _userManager; 

        public RegistrationCourseManagementService(IApplicationDbContext db,
            IMapper mapper,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Get registration course students
        /// </summary>
        /// <param name="registrationCourseId">Registration course id</param>
        public List<TransferCourseStudentViewModel> GetRegistrationCourseStudents(int registrationCourseId) 
        {
            List<TransferCourseStudentViewModel> model = new List<TransferCourseStudentViewModel>();
            
            var courses = _db.StudentCoursesTemp.Include(x => x.StudentCourseRegistration)
                    .Where(x => x.AnnouncementSectionId == registrationCourseId && 
                        x.State != (int)enu_CourseState.Dropped).ToList();

            foreach (var course in courses) 
            {
                TransferCourseStudentViewModel studentModel = new TransferCourseStudentViewModel();
                studentModel.UserId = course.StudentCourseRegistration.StudentUserId;

                var student = _userManager.Users
                    .Include(x => x.StudentBasicInfo)
                    .ThenInclude(x => x.StudentOrgInfo)
                    .ThenInclude(x => x.DepartmentGroup)
                    .ThenInclude(x => x.Department)
                    .FirstOrDefault(x => x.Id == course.StudentCourseRegistration.StudentUserId);
                
                if (student != null) 
                {
                    studentModel.Name = student.FullNameEng;
                    var orgInfo = student.StudentBasicInfo.StudentOrgInfo
                        .FirstOrDefault(x => x.OrganizationId == course.StudentCourseRegistration.OrganizationId);
                    
                    if (orgInfo != null)
                        studentModel.Group = orgInfo.DepartmentGroup.Department.Code + orgInfo.DepartmentGroup.Code;
                }
                model.Add(studentModel);
            }

            return model;
        }

        /// <summary>
        /// Save transfered students for registration courses
        /// </summary>
        /// <param name="courseIdFrom">Registration course id from</param>
        /// <param name="courseIdTo">Registration course id to</param>
        /// <param name="transferStudentUserIds">Student user ids to transfer</param>
        public void SaveTransferCourseStudents(int courseIdFrom, int courseIdTo, string[] transferStudentUserIds) 
        {
            var courseFrom = _db.AnnouncementSections.FirstOrDefault(x => x.Id == courseIdFrom);
            if (courseFrom == null)
                throw new Exception($"Registration course with id {courseIdFrom} was not found");

            var courseTo = _db.AnnouncementSections.FirstOrDefault(x => x.Id == courseIdTo);
            if (courseTo == null)
                throw new Exception($"Registration course with id {courseIdTo} was not found");

            var coureFromStudents = _db.StudentCoursesTemp.Include(x => x.StudentCourseRegistration)
                                        .Where(x => x.AnnouncementSectionId == courseIdFrom &&
                                            transferStudentUserIds.Contains(x.StudentCourseRegistration.StudentUserId))
                                        .ToList();

            if (coureFromStudents.Count != transferStudentUserIds.Length)
                throw new Exception("Something wrong with student user ids");

            foreach (var student in coureFromStudents) 
            {
                student.AnnouncementSectionId = courseIdTo;
                _db.StudentCoursesTemp.Update(student);
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Get announcement list for assigning
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="deanDepartments">Dean departments</param>
        /// <returns>Announcement list</returns>
        public IEnumerable<AnnouncementDTO> GetAnnouncementsForAssigning(int semesterId, List<DepartmentDTO> deanDepartments)
        {
            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            var announcements = _db.Announcements
                .Include(x => x.Course)
                .ThenInclude(x => x.Department)
                .Include(x => x.AnnouncementSections)
                .Where(x => x.SemesterId == semesterId && x.AnnouncementSections.Count() > 1 && x.IsActivated == true);

            if (deanDepartments != null && deanDepartments.Any())
                announcements = announcements.Where(x => deanDepartments.Select(x => x.Id).Contains(x.Course.DepartmentId));

            return _mapper.Map<IEnumerable<AnnouncementDTO>>(announcements.ToList());
        }

        /// <summary>
        /// Get sections with students
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <returns>Sections with students</returns>
        public List<StudentsInSectionViewModel> GetSectionsWithStudents(int announcementId)
        {
            if (announcementId == 0)
                throw new ArgumentException("The announcement id is 0.", nameof(announcementId));

            var studentsInSections = _db.AnnouncementSections
                .Include(x => x.StudentCourses)
                .ThenInclude(x => x.StudentCourseRegistration)
                .Where(x => x.AnnouncementId == announcementId)
                .ToList()
                .Select(section => new StudentsInSectionViewModel
                {
                    AnnouncementSectionId = section.Id,
                    AnnouncementSection = _mapper.Map<AnnouncementSectionDTO>(section),
                    Students = section.StudentCourses
                        .Where(student => student.State != (int)enu_CourseState.Dropped &&
                        ((student.StudentCourseRegistration.State == (int)enu_RegistrationState.Submitted &&
                        student.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.NotSent) ||
                        student.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.Submitted))
                        .Select(student => MapStudentInfo(student))
                        .OrderBy(x => x.ShortName)
                })
                .OrderBy(x => x.AnnouncementSection.Section)
                .ToList();

            return studentsInSections;
        }

        /// <summary>
        /// Set student section
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="oldAnnouncementSectionId">Old announcement section id</param>
        /// <param name="newAnnouncementSectionId">New announcement section id</param>
        public void SetStudentSection(string studentUserId, int oldAnnouncementSectionId, int newAnnouncementSectionId)
        {
            if (string.IsNullOrEmpty(studentUserId))
                throw new ArgumentException("The student user id is null.", nameof(studentUserId));

            if (oldAnnouncementSectionId == 0)
                throw new ArgumentException("The old announcement section id is 0.", nameof(oldAnnouncementSectionId));

            if (newAnnouncementSectionId == 0)
                throw new ArgumentException("The new announcement section id is 0.", nameof(newAnnouncementSectionId));

            var studentCourse = _db.StudentCoursesTemp
                .Include(x => x.StudentCourseRegistration)
                .FirstOrDefault(x => x.StudentCourseRegistration.StudentUserId == studentUserId &&
                x.AnnouncementSectionId == oldAnnouncementSectionId);

            if (studentCourse == null)
                throw new ArgumentException($"There is no student with user id {studentUserId} who is in section {oldAnnouncementSectionId}.");

            studentCourse.AnnouncementSectionId = newAnnouncementSectionId;
            _db.SaveChanges();
        }

        private StudentInfoViewModel MapStudentInfo(StudentCourseTemp student)
        {
            var studentInfo = _userManager.Users
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .ThenInclude(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .FirstOrDefault(x => x.Id == student.StudentCourseRegistration.StudentUserId);

            var orgInfo = studentInfo?.StudentBasicInfo?.StudentOrgInfo
                .FirstOrDefault(x => x.OrganizationId == student.StudentCourseRegistration.OrganizationId);

            if (studentInfo == null || orgInfo == null)
                throw new Exception($"Student with id {student.StudentCourseRegistration.StudentUserId} was not found");

            return new StudentInfoViewModel
            {
                UserId = student.StudentCourseRegistration.StudentUserId,
                ShortName = studentInfo.LastNameEng + " " + studentInfo.FirstNameEng.Substring(0, 1) + ".",
                Group = orgInfo.DepartmentGroup?.Department?.Code + orgInfo.DepartmentGroup?.Code,
                StudentId = orgInfo.StudentId
            };
        }

    }
}
