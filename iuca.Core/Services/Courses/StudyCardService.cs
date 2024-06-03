using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Infrastructure.Persistence;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Interfaces.Users.Students;

namespace iuca.Application.Services.Courses
{
    public class StudyCardService : IStudyCardService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IStudyCardCourseService _studyCardCourseService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public StudyCardService(IApplicationDbContext db, 
            IMapper mapper, 
            IStudyCardCourseService studyCardCourseService,
            IDepartmentGroupService departmentGroupService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _studyCardCourseService = studyCardCourseService;
            _departmentGroupService = departmentGroupService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get study card list
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="deanDepartments">Dean departments</param>
        /// <returns>Study card list</returns>
        public IEnumerable<StudyCardDTO> GetStudyCards(int semesterId, int departmentId, IEnumerable<DepartmentDTO> deanDepartments)
        {
            IEnumerable<StudyCard> studyCards = _db.StudyCards
                .Include(x => x.StudyCardCourses)
                .Include(x => x.Semester)
                .Include(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department);

            if (deanDepartments != null && deanDepartments.Any())
                studyCards = studyCards.Where(x => deanDepartments.Select(x => x.Id).Contains(x.DepartmentGroup.DepartmentId));

            if (semesterId != 0)
                studyCards = studyCards.Where(x => x.SemesterId == semesterId);

            if (departmentId != 0)
                studyCards = studyCards.Where(x => x.DepartmentGroup.DepartmentId == departmentId);

            return _mapper.Map<IEnumerable<StudyCardDTO>>(studyCards);
        }

        /// <summary>
        /// Get study card by id
        /// </summary>
        /// <param name="studyCardId">Study card id</param>
        /// <returns>Study card</returns>
        public StudyCardDTO GetStudyCard(int studyCardId)
        {
            if (studyCardId == 0)
                throw new Exception($"The study card id is 0.");

            StudyCard studyCard = _db.StudyCards
                .Include(x => x.StudyCardCourses)
                .ThenInclude(x => x.AnnouncementSection).ThenInclude(x => x.Announcement)
                .ThenInclude(x => x.Course)
                .Include(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .Include(x => x.Semester)
                .FirstOrDefault(x => x.Id == studyCardId);
            if (studyCard == null)
                throw new Exception($"The study card with id {studyCardId} does not exist.");

            StudyCardDTO studyCardDTO = _mapper.Map<StudyCardDTO>(studyCard);
            foreach (var studyCardCourseDTO in studyCardDTO.StudyCardCourses)
                studyCardCourseDTO.AnnouncementSection.InstructorUserName = _userManager.GetUserFullName(studyCardCourseDTO.AnnouncementSection.InstructorUserId);

            return studyCardDTO;
        }

        /// <summary>
        /// Get study card by semester id and department group id
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <returns>Study card</returns>
        public StudyCardDTO GetStudyCard(int semesterId, int departmentGroupId)
        {
            if (semesterId == 0)
                throw new Exception($"The semester id is 0.");

            if (departmentGroupId == 0)
                throw new Exception($"The department group id is 0.");

            StudyCard studyCard = _db.StudyCards
                .Include(x => x.StudyCardCourses)
                .ThenInclude(x => x.AnnouncementSection.Course)
                .Include(x => x.StudyCardCourses)
                .ThenInclude(x => x.AnnouncementSection.Announcement)
                .Include(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .Include(x => x.Semester)
                .FirstOrDefault(x => x.SemesterId == semesterId && x.DepartmentGroupId == departmentGroupId);

            StudyCardDTO studyCardDTO = _mapper.Map<StudyCardDTO>(studyCard);
            if (studyCardDTO != null) 
            {
                foreach (var studyCardCourseDTO in studyCardDTO.StudyCardCourses)
                    studyCardCourseDTO.AnnouncementSection.InstructorUserName = _userManager.GetUserFullName(studyCardCourseDTO.AnnouncementSection.InstructorUserId);
            }

            return studyCardDTO;
        }

        /// <summary>
        /// Create study card
        /// </summary>
        /// <param name="studyCardDTO">Study card</param>
        public void CreateStudyCard(StudyCardDTO studyCardDTO)
        {
            if (studyCardDTO == null)
                throw new Exception($"The study card is null.");

            StudyCard newStudyCard = _mapper.Map<StudyCard>(studyCardDTO);

            _db.StudyCards.Add(newStudyCard);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit study card by id
        /// </summary>
        /// <param name="studyCardId">Study card id</param>
        /// <param name="studyCardDTO">Study card</param>
        public void EditStudyCard(int studyCardId, StudyCardDTO studyCardDTO)
        {
            if (studyCardId == 0)
                throw new Exception($"The study card id is 0.");
            if (studyCardDTO == null)
                throw new Exception($"The study card is null.");

            StudyCard studyCard = _db.StudyCards.Find(studyCardId);
            if (studyCard == null)
                throw new Exception($"The study card with id {studyCardId} does not exist.");

            studyCard.SemesterId = studyCardDTO.SemesterId;
            studyCard.DepartmentGroupId = studyCardDTO.DepartmentGroupId;
            studyCard.DisplayIUCAElectives = studyCardDTO.DisplayIUCAElectives;

            _db.SaveChanges();
        }

        /// <summary>
        /// Edit study card courses by id
        /// </summary>
        /// <param name="studyCardId">Study card id</param>
        /// <param name="studyCardCourseDTOList">Study card course list</param>
        public void EditStudyCardCourses(int studyCardId, IEnumerable<StudyCardCourseDTO> studyCardCourseDTOList)
        {
            if (studyCardId == 0)
                throw new Exception($"The study card id is 0.");

            StudyCard studyCard = _db.StudyCards
                .Include(x => x.StudyCardCourses)
                .Include(x => x.Semester)
                .FirstOrDefault(x => x.Id == studyCardId);
            if (studyCard == null)
                throw new Exception($"The study card with id {studyCardId} does not exist.");

            IEnumerable<StudyCardCourse> existingStudyCardCourseList = studyCard.StudyCardCourses.ToList();

            if (studyCardCourseDTOList != null && studyCardCourseDTOList.Any())
            {
                foreach (StudyCardCourseDTO newStudyCardCourse in studyCardCourseDTOList)
                {
                    newStudyCardCourse.StudyCardId = studyCardId;
                    StudyCardCourse existingStudyCardCourse = existingStudyCardCourseList.FirstOrDefault(x => x.Id == newStudyCardCourse.Id);

                    if (existingStudyCardCourse == null)
                        _studyCardCourseService.CreateStudyCardCourse(newStudyCardCourse);
                    else if (existingStudyCardCourse.Comment != newStudyCardCourse.Comment)
                        _studyCardCourseService.EditStudyCardCourse(existingStudyCardCourse.Id, newStudyCardCourse);
                }
            }
            if (existingStudyCardCourseList != null && existingStudyCardCourseList.Any())
            {
                foreach (StudyCardCourse existingStudyCardCourse in existingStudyCardCourseList)
                {
                    if (studyCardCourseDTOList == null || !studyCardCourseDTOList.Any(x => x.Id == existingStudyCardCourse.Id))
                        _studyCardCourseService.DeleteStudyCardCourse(existingStudyCardCourse.Id);
                }
            }
        }

        /// <summary>
        /// Delete study card by id
        /// </summary>
        /// <param name="studyCardId">Study card id</param>
        public void DeleteStudyCard(int studyCardId)
        {
            if (studyCardId == 0)
                throw new Exception($"The study card id is 0.");

            StudyCard studyCard = _db.StudyCards.Find(studyCardId);
            if (studyCard == null)
                throw new Exception($"The study card with id {studyCardId} does not exist.");

            _db.StudyCards.Remove(studyCard);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get courses for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="season">Season</param>
        /// <param name="year">Year</param>
        /// <param name="excludedAnnouncementSectionIds">Announcement section ids to exclude</param>
        /// <returns>Announcement section list without excluded announcement section ids</returns>
        public IEnumerable<AnnouncementSectionDTO> GetCoursesForSelection(int organizationId, int year, int season, int[] excludedAnnouncementSectionIds)
        {
            var announcementSections = _db.AnnouncementSections
                .Include(x => x.Announcement)
                .Include(x => x.Course)
                .Where(x => x.OrganizationId == organizationId && x.Season == season && x.Year == year &&
                !excludedAnnouncementSectionIds.Contains(x.Id) && x.Announcement.IsActivated == true)
                .OrderBy(x => x.Course.NameRus)
                .ThenBy(x => x.Course.NameEng)
                .ToList();

            IEnumerable<AnnouncementSectionDTO> announcementSectionsDTO = _mapper.Map<IEnumerable<AnnouncementSectionDTO>>(announcementSections);
            foreach (var announcementSectionDTO in announcementSectionsDTO)
            {
                announcementSectionDTO.Instructor = new UserDTO
                {
                    Id = announcementSectionDTO.InstructorUserId,
                    FullName = _userManager.GetUserFullName(announcementSectionDTO.InstructorUserId)
                };

                announcementSectionDTO.Groups = announcementSectionDTO.GroupsJson.Select(x => new GroupDTO
                {
                    Id = int.Parse(x),
                    Code = _departmentGroupService.GetDepartmentGroup(organizationId, int.Parse(x)).DepartmentCode
                }); ;
            }
                
            return announcementSectionsDTO;
        }

        /// <summary>
        /// Get course from selection
        /// </summary>
        /// <param name="selectedRegistrationCourseId">Selected registration course id</param>
        /// <returns>Registartion course</returns>
        public AnnouncementSectionDTO GetCourseFromSelection(int selectedRegistrationCourseId)
        {
            if (selectedRegistrationCourseId == 0)
                throw new Exception($"The registration course id is 0.");

            AnnouncementSection announcementSection = _db.AnnouncementSections
                .Include(x => x.Course)
                .Include(x => x.Announcement)
                .FirstOrDefault(x => x.Id == selectedRegistrationCourseId);
            if (announcementSection == null)
                throw new Exception($"The registration course with id {selectedRegistrationCourseId} does not exist.");

            return _mapper.Map<AnnouncementSectionDTO>(announcementSection);
        }

        /// <summary>
        /// Get announcement section with ForAll flag
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Announcement section collection</returns>
        public List<AnnouncementSectionDTO> GetForAllAnnouncementSections(int semesterId)
        {
            var sections = _db.AnnouncementSections.Include(x => x.Announcement)
                .Include(x => x.Course)
                .Where(x => x.Announcement.SemesterId == semesterId && x.Announcement.IsForAll)
                .ToList();

            return _mapper.Map<List<AnnouncementSectionDTO>>(sections);
        }
    }
}
