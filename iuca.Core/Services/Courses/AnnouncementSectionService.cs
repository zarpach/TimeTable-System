using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace iuca.Application.Services.Courses
{
    public class AnnouncementSectionService : IAnnouncementSectionService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ISemesterService _semesterService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public AnnouncementSectionService(IApplicationDbContext db,
            IMapper mapper,
            ISemesterService semesterService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _semesterService = semesterService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get announcement section by course import code, semester id and section number
        /// </summary>
        /// <param name="importCode">Course import code</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="sectionNumber">Section number</param>
        /// <returns>Announcement section</returns>
        public AnnouncementSectionDTO GetAnnouncementSection(int importCode, int semesterId, string sectionNumber)
        {
            if (importCode == 0)
                throw new ArgumentException("The import code is 0.", nameof(importCode));

            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            var announcementSection = _db.AnnouncementSections
                .Include(x => x.Announcement)
                .ThenInclude(x => x.Semester)
                .Include(x => x.Course)
                .FirstOrDefault(x => x.Course.ImportCode == importCode &&
                x.Announcement.SemesterId == semesterId && x.Section == sectionNumber);

            if (announcementSection == null)
                throw new ArgumentException($"The announcement section with course import code {importCode}, semester {semesterId} and section number {sectionNumber} does not exist.");

            var announcementSectionDTO = _mapper.Map<AnnouncementSection, AnnouncementSectionDTO>(announcementSection);
            announcementSectionDTO.InstructorUserName = _userManager.GetUserFullName(announcementSectionDTO.InstructorUserId);

            return announcementSectionDTO;
        }

        /// <summary>
        /// Create announcement section
        /// </summary>
        /// <param name="announcementSectionDTO">Announcement section</param>
        public void CreateAnnouncementSection(AnnouncementSectionDTO announcementSectionDTO)
        {
            if (announcementSectionDTO == null)
                throw new ArgumentException("The announcement section is null.");

            if (announcementSectionDTO.AnnouncementId == 0)
                throw new ArgumentException("The announcement id is null.");

            Announcement announcement = _db.Announcements
                .Include(x => x.AnnouncementSections)
                .FirstOrDefault(x => x.Id == announcementSectionDTO.AnnouncementId);

            if (announcement == null)
                throw new ArgumentException($"The announcement with id {announcementSectionDTO.AnnouncementId} does not exist.", nameof(announcementSectionDTO.AnnouncementId));

            if (announcement.AnnouncementSections != null && announcement.AnnouncementSections.Any(x => x.Section == announcementSectionDTO.Section))
                throw new ModelValidationException($"Semester number {announcementSectionDTO.Section} already exists.", "");

            SemesterDTO semester = _semesterService.GetSemester(announcement.SemesterId);

            announcementSectionDTO.OrganizationId = semester.OrganizationId;
            announcementSectionDTO.Season = semester.Season;
            announcementSectionDTO.Year = semester.Year;
            announcementSectionDTO.CourseId = announcement.CourseId;

            AnnouncementSection newAnnouncementSection = _mapper.Map<AnnouncementSection>(announcementSectionDTO);

            _db.AnnouncementSections.Add(newAnnouncementSection);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit announcement section by id
        /// </summary>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <param name="announcementSectionDTO">Announcement section</param>
        public void EditAnnouncementSection(int announcementSectionId, AnnouncementSectionDTO announcementSectionDTO)
        {
            if (announcementSectionId == 0)
                throw new ArgumentException($"The announcement section id is 0.");

            var announcementSections = _db.AnnouncementSections
                .Where(x => x.AnnouncementId == announcementSectionDTO.AnnouncementId).ToList();

            if (announcementSections != null && announcementSections.Any(x => x.Id != announcementSectionId && x.Section == announcementSectionDTO.Section))
                throw new ModelValidationException($"Section number {announcementSectionDTO.Section} already exists.", "");

            var announcementSection = announcementSections.FirstOrDefault( x => x.Id == announcementSectionId);

            if (announcementSection == null)
                throw new ArgumentException($"The announcement section with id {announcementSectionId} does not exist.");

            announcementSection.Section = announcementSectionDTO.Section;
            announcementSection.Credits = announcementSectionDTO.Credits;
            announcementSection.Places = announcementSectionDTO.Places;
            announcementSection.Schedule = announcementSectionDTO.Schedule;
            announcementSection.InstructorUserId = announcementSectionDTO.InstructorUserId;
            announcementSection.ExtraInstructorsJson = announcementSectionDTO.ExtraInstructorsJson;
            announcementSection.GroupsJson = announcementSectionDTO.GroupsJson;
            announcementSection.IsChanged = true;

            _db.SaveChanges();
        }

        /// <summary>
        /// Delete announcement section by id
        /// </summary>
        /// <param name="announcementSectionId">Announcement section id</param>
        public void DeleteAnnouncementSection(int announcementSectionId)
        {
            if (announcementSectionId == 0)
                throw new ArgumentException($"The announcement section id is 0.");

            AnnouncementSection announcementSection = _db.AnnouncementSections
                .Include(x => x.StudentCourses)
                .Include(x => x.StudyCardCourses)
                .FirstOrDefault(x => x.Id == announcementSectionId);

            if (announcementSection == null)
                throw new ArgumentException($"The announcement section with id {announcementSectionId} does not exist.");

            if (announcementSection.StudyCardCourses != null && announcementSection.StudyCardCourses.Any())
                throw new ModelValidationException($"This section (number {announcementSection.Section}) is available in some study card.", "");

            if (announcementSection.StudentCourses != null && announcementSection.StudentCourses.Any(x => x.State != (int)enu_CourseState.Dropped))
                throw new ModelValidationException($"This section (number {announcementSection.Section}) is registered with some student.", "");

            _db.AnnouncementSections.Remove(announcementSection);
            _db.SaveChanges();
        }
    }
}
