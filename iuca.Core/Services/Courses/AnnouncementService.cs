using AutoMapper;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using iuca.Application.Interfaces.Common;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;
using AutoMapper.QueryableExtensions;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Interfaces.Users.Students;
using iuca.Domain.Entities.Common;

namespace iuca.Application.Services.Courses
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IAnnouncementSectionService _announcementSectionService;
        private readonly IDepartmentGroupService _departmentGroupService;
        private readonly IEnvarSettingService _envarSettingService;

        public AnnouncementService(IApplicationDbContext db,
            IMapper mapper,
            ApplicationUserManager<ApplicationUser> userManager,
            IAnnouncementSectionService announcementSectionService,
            IDepartmentGroupService departmentGroupService,
            IEnvarSettingService envarSettingService)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _announcementSectionService = announcementSectionService;
            _departmentGroupService = departmentGroupService;
            _envarSettingService = envarSettingService;
        }

        /// <summary>
        /// Get announcement list
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="withAnnouncementSections">Announcement section include</param>
        /// <param name="withSemester">Semester include</param>
        /// <returns>Announcement list</returns>
        public IEnumerable<AnnouncementDTO> GetAnnouncements(int semesterId, bool withAnnouncementSections = false, bool withSemester = false)
        {
            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            IQueryable<Announcement> announcementsQuery = _db.Announcements
                .AsNoTracking()
                .Where(x => x.SemesterId == semesterId);

            // Includes
            if (withAnnouncementSections)
                announcementsQuery = announcementsQuery.Include(x => x.AnnouncementSections);

            if (withSemester)
                announcementsQuery = announcementsQuery.Include(x => x.Semester);

            // To list
            var announcementDTOs = announcementsQuery
                .ProjectTo<AnnouncementDTO>(_mapper.ConfigurationProvider)
                .ToList();

            return announcementDTOs;
        }

        /// <summary>
        /// Get announcement by id for announcement controls
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <returns>Announcement</returns>
        public AnnouncementForAnnouncementControlsDTO GetAnnouncementForAnnouncementControls(int announcementId)
        {
            if (announcementId == 0)
                throw new ArgumentException($"The announcement id is 0.");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Announcement, AnnouncementForAnnouncementControlsDTO>();
                cfg.CreateMap<Course, CourseForAnnouncementControlsDTO>();
                cfg.CreateMap<ProposalCourse, ProposalCourseForAnnouncementControlsDTO>();
                cfg.CreateMap<Proposal, ProposalForAnnouncementControlsDTO>();
                cfg.CreateMap<AnnouncementSection, AnnouncementSectionForAnnouncementControlsDTO>();
                cfg.CreateMap<StudentCourseTemp, StudentCourseTempForAnnouncementControlsDTO>();
            }).CreateMapper();

            var announcement = _db.Announcements
                .AsNoTracking()
                .Include(x => x.Course)
                .Include(x => x.AnnouncementSections)
                .ThenInclude(x => x.StudentCourses)
                .Where(x => x.Id == announcementId)
                .ProjectTo<AnnouncementForAnnouncementControlsDTO>(mapper.ConfigurationProvider)
                .FirstOrDefault();

            announcement.ProposalCourse = _db.ProposalCourses
                .AsNoTracking()
                .Include(x => x.Proposal)
                .Where(x => x.Proposal.SemesterId == announcement.SemesterId &&
                            x.CourseId == announcement.CourseId &&
                            x.Status == (int)enu_ProposalCourseStatus.Approved)
                .ProjectTo<ProposalCourseForAnnouncementControlsDTO>(mapper.ConfigurationProvider)
                .FirstOrDefault();

            return announcement;
        }

        /// <summary>
        /// Get announcement list for announcement info
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="courseType">Course type</param>
        /// <param name="isForAll">For all</param>
        /// <param name="isActivated">Activated</param>
        /// <returns>Announcement list</returns>
        public IEnumerable<AnnouncementForAnnouncementInfoDTO> GetAnnouncementsForAnnouncementInfo(int semesterId, int departmentId = -1, int courseType = -1, int isForAll = (int)enu_Status.All, int isActivated = (int)enu_Status.All)
        {
            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Announcement, AnnouncementForAnnouncementInfoDTO>();
                cfg.CreateMap<Semester, SemesterForAnnouncementInfoDTO>();
                cfg.CreateMap<Course, CourseForAnnouncementInfoDTO>();
                cfg.CreateMap<ProposalCourse, ProposalCourseForAnnouncementInfoDTO>();
                cfg.CreateMap<Proposal, ProposalForAnnouncementInfoDTO>();
                cfg.CreateMap<Department, DepartmentForAnnouncementInfoDTO>();
                cfg.CreateMap<Language, LanguageForAnnouncementInfoDTO>();
                cfg.CreateMap<AnnouncementSection, AnnouncementSectionForAnnouncementInfoDTO>();
            }).CreateMapper();

            var announcements = _db.Announcements
                .AsNoTracking()
                .Include(x => x.Course)
                .ThenInclude(x => x.Department)
                .Include(x => x.Course)
                .ThenInclude(x => x.Language)
                .Include(x => x.AnnouncementSections)
                .Where(x => x.SemesterId == semesterId &&
                    (departmentId == -1 || x.Course.DepartmentId == departmentId) &&
                    (courseType == -1 || x.Course.CourseType == courseType) &&
                    (isForAll == (int)enu_Status.All || x.IsForAll == (isForAll == (int)enu_Status.True)) &&
                    (isActivated == (int)enu_Status.All || x.IsActivated == (isActivated == (int)enu_Status.True)))
                .ProjectTo<AnnouncementForAnnouncementInfoDTO>(mapper.ConfigurationProvider)
                .ToList();

            var courseIds = announcements.Select(x => x.CourseId).ToList();

            var proposalCourses = _db.ProposalCourses
                .AsNoTracking()
                .Include(x => x.Proposal)
                .Where(x => x.Proposal.SemesterId == semesterId &&
                            courseIds.Contains(x.CourseId) &&
                            x.Status == (int)enu_ProposalCourseStatus.Approved)
                .ProjectTo<ProposalCourseForAnnouncementInfoDTO>(mapper.ConfigurationProvider)
                .ToList();

            if (announcements != null && announcements.Any())
            {
                var departmentGroups = _departmentGroupService.GetDepartmentGroups(announcements.First().Semester.OrganizationId)
                .Select(x => new GroupDTO
                {
                    Id = x.Id,
                    Code = x.DepartmentCode
                });

                var proposalCoursesDictionary = proposalCourses.ToDictionary(pc => pc.CourseId);

                foreach (var announcement in announcements)
                {
                    if (proposalCoursesDictionary.TryGetValue(announcement.CourseId, out var proposalCourse))
                        announcement.ProposalCourse = mapper.Map<ProposalCourseForAnnouncementInfoDTO>(proposalCourse);

                    if (announcement.AnnouncementSections != null && announcement.AnnouncementSections.Any())
                    {
                        announcement.Instructors = new List<UserDTO>();
                        announcement.Groups = new List<GroupDTO>();

                        foreach (var announcementSection in announcement.AnnouncementSections)
                        {
                            if (announcementSection.InstructorUserId != null && !announcement.Instructors.Any(i => i.Id == announcementSection.InstructorUserId))
                            {
                                announcementSection.Instructor = new UserDTO
                                {
                                    Id = announcementSection.InstructorUserId,
                                    FullName = _userManager.GetUserFullName(announcementSection.InstructorUserId)
                                };

                                announcement.Instructors.Add(announcementSection.Instructor);
                            }

                            if (announcementSection.ExtraInstructorsJson != null)
                            {
                                announcementSection.ExtraInstructorsList = announcementSection.ExtraInstructorsJson
                                    .Where(x => x != null)
                                    .Select(x => new UserDTO
                                    {
                                        Id = x,
                                        FullName = _userManager.GetUserFullName(x)
                                    })
                                    .ToList();

                                announcement.Instructors.AddRange(announcementSection.ExtraInstructorsList
                                    .Where(i => !announcement.Instructors.Any(existing => existing.Id == i.Id)));
                            }

                            if (announcementSection.GroupsJson != null)
                            {
                                announcementSection.Groups = announcementSection.GroupsJson
                                    .Where(x => x != null)
                                    .Select(x => departmentGroups.FirstOrDefault(y => y.Id == int.Parse(x)))
                                    .ToList();

                                announcement.Groups.AddRange(announcementSection.Groups
                                    .Where(g => !announcement.Groups.Any(existing => existing.Id == g.Id)));
                            }
                        }
                    }
                }

            }
                
            return announcements;
        }

        /// <summary>
        /// Get announcement by id
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <returns>Announcement</returns>
        public AnnouncementDTO GetAnnouncement(int announcementId)
        {
            if (announcementId == 0)
                throw new ArgumentException($"The announcement id is 0.");

            Announcement announcement = _db.Announcements
                .Include(x => x.Course)
                .Include(x => x.AnnouncementSections)
                .FirstOrDefault(x => x.Id == announcementId);

            if (announcement == null)
                throw new ArgumentException($"The announcement with id {announcementId} does not exist.", nameof(announcementId));

            ProposalCourse proposalCourse = _db.ProposalCourses
                .Include(x => x.Proposal)
                .Where(x => x.Proposal.SemesterId == announcement.SemesterId && x.CourseId == announcement.CourseId && x.Status == (int)enu_ProposalCourseStatus.Approved)
                .FirstOrDefault();

            ProposalCourseDTO proposalCourseDTO = _mapper.Map<ProposalCourseDTO>(proposalCourse);

            if (proposalCourseDTO != null && proposalCourseDTO.InstructorsJson != null)
            {
                proposalCourseDTO.Instructors = proposalCourseDTO.InstructorsJson
                    .Select(x => new UserDTO
                    {
                        Id = x,
                        FullName = _userManager.GetUserFullName(x)
                    });
            }

            AnnouncementDTO announcementDTO = _mapper.Map<AnnouncementDTO>(announcement);
            announcementDTO.ProposalCourse = proposalCourseDTO;

            return announcementDTO;
        }

        /// <summary>
        /// Edit announcement sections by announcement id
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <param name="announcementSectionDTOs">Announcement sections</param>
        public void EditAnnouncementSections(int announcementId, IEnumerable<AnnouncementSectionDTO> announcementSectionDTOs)
        {
            if (announcementId == 0)
                throw new ArgumentException($"The announcement id is 0.");

            Announcement announcement = _db.Announcements
                .Include(x => x.AnnouncementSections)
                .FirstOrDefault(x => x.Id == announcementId);

            if (announcement == null)
                throw new ArgumentException($"The announcement with id {announcementId} does not exist.", nameof(announcementId));

            var existingAnnouncementSections = announcement.AnnouncementSections.ToList();

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if (announcementSectionDTOs != null)
                    {
                        var newAnnouncementSections = announcementSectionDTOs.ToList();

                        foreach (AnnouncementSectionDTO newAnnouncementSection in newAnnouncementSections)
                        {
                            AnnouncementSection existingAnnouncementSection = existingAnnouncementSections.FirstOrDefault(x => x.Id == newAnnouncementSection.Id);

                            if (existingAnnouncementSection == null)
                                _announcementSectionService.CreateAnnouncementSection(newAnnouncementSection);
                            else
                            {
                                if (IsAnnouncementSectionChanged(existingAnnouncementSection, newAnnouncementSection))
                                    _announcementSectionService.EditAnnouncementSection(existingAnnouncementSection.Id, newAnnouncementSection);

                                existingAnnouncementSections.Remove(existingAnnouncementSection);
                            }
                        }
                    }

                    foreach (var existingAnnouncementSection in existingAnnouncementSections)
                        _announcementSectionService.DeleteAnnouncementSection(existingAnnouncementSection.Id);

                    transaction.Commit();
                }
                catch (ModelValidationException ex)
                {
                    transaction.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        private bool IsAnnouncementSectionChanged(AnnouncementSection existingAnnouncementSection, AnnouncementSectionDTO newAnnouncementSection)
        {
            return (existingAnnouncementSection.ExtraInstructorsJson != null && newAnnouncementSection.ExtraInstructorsJson != null && 
                !existingAnnouncementSection.ExtraInstructorsJson.SequenceEqual(newAnnouncementSection.ExtraInstructorsJson)) ||
                existingAnnouncementSection.ExtraInstructorsJson != newAnnouncementSection.ExtraInstructorsJson ||
                (existingAnnouncementSection.GroupsJson != null && newAnnouncementSection.GroupsJson != null &&
                !existingAnnouncementSection.GroupsJson.SequenceEqual(newAnnouncementSection.GroupsJson)) ||
                existingAnnouncementSection.GroupsJson != newAnnouncementSection.GroupsJson ||
                existingAnnouncementSection.InstructorUserId != newAnnouncementSection.InstructorUserId ||
                existingAnnouncementSection.Section != newAnnouncementSection.Section ||
                existingAnnouncementSection.Credits != newAnnouncementSection.Credits ||
                existingAnnouncementSection.Places != newAnnouncementSection.Places ||
                existingAnnouncementSection.Schedule != newAnnouncementSection.Schedule;
        }

        /// <summary>
        /// Replace announcememnt instructor for all sections
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="courseId">Course id</param>
        /// <param name="previousInstructorId">Previous instructor id</param>
        /// <param name="futureInstructorId">Future instructor id</param>
        public void ReplaceAnnouncementInstructor(int semesterId, int courseId, string previousInstructorId, string futureInstructorId)
        {
            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            if (courseId == 0)
                throw new ArgumentException("The course id is 0.", nameof(courseId));

            if (string.IsNullOrEmpty(previousInstructorId))
                throw new ArgumentException("The previous instructor id is null.", nameof(previousInstructorId));

            if (string.IsNullOrEmpty(futureInstructorId))
                throw new ArgumentException("The future instructor id is null.", nameof(futureInstructorId));

            Announcement announcement = _db.Announcements
                .Include(x => x.AnnouncementSections)
                .FirstOrDefault(x => x.SemesterId == semesterId && x.CourseId == courseId);

            if (announcement == null)
                throw new ArgumentException($"The announcement with semester id {semesterId} and course id {courseId} does not exist.");

            if (announcement.AnnouncementSections != null && announcement.AnnouncementSections.Any())
            {
                foreach (var section in announcement.AnnouncementSections)
                {
                    if (string.Equals(section.InstructorUserId, previousInstructorId))
                    {
                        section.InstructorUserId = futureInstructorId;
                        section.IsChanged = true;
                    }

                    if (section.ExtraInstructorsJson != null && section.ExtraInstructorsJson.Any())
                    {
                        var existingExtraInstructors = section.ExtraInstructorsJson.ToList();

                        for (int i = 0; i < existingExtraInstructors.Count; i++)
                        {
                            if (string.Equals(existingExtraInstructors[i], previousInstructorId))
                            {
                                existingExtraInstructors[i] = futureInstructorId;
                            }
                        }

                        section.ExtraInstructorsJson = existingExtraInstructors;
                        section.IsChanged = true;
                    }
                }

                _db.Announcements.Update(announcement);
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Remove announcememnt instructor for all sections
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="courseId">Course id</param>
        /// <param name="instructorId">Instructor id</param>
        public void RemoveAnnouncementInstructor(int semesterId, int courseId, string instructorId)
        {
            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            if (courseId == 0)
                throw new ArgumentException("The course id is 0.", nameof(courseId));

            if (string.IsNullOrEmpty(instructorId))
                throw new ArgumentException("The instructor id is null.", nameof(instructorId));

            Announcement announcement = _db.Announcements
                .Include(x => x.AnnouncementSections)
                .Include(x => x.Semester)
                .FirstOrDefault(x => x.SemesterId == semesterId && x.CourseId == courseId);

            if (announcement == null)
                throw new ArgumentException($"The announcement with semester id {semesterId} and course id {courseId} does not exist.");

            if (announcement.AnnouncementSections != null && announcement.AnnouncementSections.Any())
            {
                foreach (var section in announcement.AnnouncementSections)
                {
                    if (string.Equals(section.InstructorUserId, instructorId))
                    {
                        section.InstructorUserId = _envarSettingService.GetDefaultInstructor(announcement.Semester.OrganizationId).Id;
                        section.IsChanged = true;
                    }

                    if (section.ExtraInstructorsJson != null)
                    {
                        section.ExtraInstructorsJson = section.ExtraInstructorsJson
                            .Where(x => !string.Equals(x, instructorId))
                            .ToList();
                        section.IsChanged = true;
                    }
                }

                _db.Announcements.Update(announcement);
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Set announcement for all value
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <param name="isForAll">For all value</param>
        public void SetAnnouncementForAllValue(int announcementId, bool isForAll)
        {
            if (announcementId == 0)
                throw new ArgumentException($"The announcement id is 0.");

            Announcement announcement = _db.Announcements.Find(announcementId);

            if (announcement == null)
                throw new ArgumentException($"The announcement with id {announcementId} does not exist.");

            if (announcement.IsForAll == isForAll)
                return;

            ProposalCourse proposalCourse = _db.ProposalCourses
                .Include(x => x.Proposal)
                .Where(x => x.Proposal.SemesterId == announcement.SemesterId && x.CourseId == announcement.CourseId && x.Status == (int)enu_ProposalCourseStatus.Approved)
                .FirstOrDefault();

            if (proposalCourse != null)
                proposalCourse.IsForAll = isForAll;

            announcement.IsForAll = isForAll;
            _db.SaveChanges();
        }

        /// <summary>
        /// Set announcement status
        /// </summary>
        /// <param name="announcementIds">Announcement ids</param>
        /// <param name="isActivated">Announcement status</param>
        public void SetAnnouncementStatuses(int[] announcementIds, bool isActivated)
        {
            if (announcementIds == null || announcementIds.Length == 0)
                return;

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (int announcementId in announcementIds)
                    {
                        Announcement announcement = _db.Announcements.Find(announcementId);

                        if (announcement == null)
                            throw new ArgumentException($"The announcement with id {announcementId} does not exist.");

                        if (announcement.IsActivated == isActivated)
                            continue;

                        if (isActivated == false)
                        {
                            var announcementDetails = _db.Announcements
                                .Include(x => x.AnnouncementSections)
                                .ThenInclude(x => x.StudyCardCourses)
                                .ThenInclude(x => x.StudyCard)
                                .ThenInclude(x => x.DepartmentGroup)
                                .ThenInclude(x => x.Department)
                                .FirstOrDefault(x => x.Id == announcementId);

                            if (announcementDetails.AnnouncementSections.SelectMany(x => x.StudyCardCourses).Distinct().Count() >= 1)
                            {
                                var studyCardDepartments = string.Join(", ", announcementDetails.AnnouncementSections
                                .SelectMany(x => x.StudyCardCourses)
                                .Select(x => x.StudyCard.DepartmentGroup.Department.Code + x.StudyCard.DepartmentGroup.Code));
                                throw new ModelValidationException($"This course is present in the following study cards: <strong>{studyCardDepartments}</strong>.", "");
                            }
                        }

                        announcement.IsActivated = isActivated;
                    }
                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Create announcements
        /// </summary>
        /// <param name="proposalCourseIds">Proposal course ids</param>
        public void CreateAnnouncements(int[] proposalCourseIds)
        {
            if (proposalCourseIds == null || proposalCourseIds.Length == 0)
                return;

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (int proposalCourseId in proposalCourseIds)
                    {
                        if (proposalCourseId == 0)
                            throw new ArgumentException($"The proposal course id is 0.");

                        ProposalCourse proposalCourse = _db.ProposalCourses
                            .Include(x => x.Proposal)
                            .ThenInclude(x => x.Semester)
                            .FirstOrDefault(x => x.Id == proposalCourseId);
                        if (proposalCourse == null)
                            throw new ArgumentException($"The proposal course with id {proposalCourseId} does not exist.", nameof(proposalCourseId));

                        Announcement announcement = _db.Announcements
                            .FirstOrDefault(x => x.SemesterId == proposalCourse.Proposal.SemesterId && x.CourseId == proposalCourse.CourseId);

                        if (announcement != null)
                            throw new ModelValidationException("Such a course has already been announced this semester.", "");

                        Announcement newAnnouncement = new Announcement()
                        {
                            CourseId = proposalCourse.CourseId,
                            SemesterId = proposalCourse.Proposal.SemesterId,
                            IsForAll = proposalCourse.IsForAll
                        };

                        _db.Announcements.Add(newAnnouncement);
                        _db.SaveChanges();

                        string instructorUserId = _envarSettingService.GetDefaultInstructor(proposalCourse.Proposal.Semester.OrganizationId).Id;
                        float credits = 0;

                        if (proposalCourse.InstructorsJson.Count() == 1)
                        {
                            instructorUserId = proposalCourse.InstructorsJson.First();
                            credits = proposalCourse.Credits;
                        }

                        AnnouncementSectionDTO defaultSection = new AnnouncementSectionDTO()
                        {
                            AnnouncementId = newAnnouncement.Id,
                            InstructorUserId = instructorUserId,
                            Places = 25,
                            Credits = credits,
                            Section = "1"
                        };

                        _announcementSectionService.CreateAnnouncementSection(defaultSection);
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete announcement
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        public void DeleteAnnouncement(int proposalCourseId)
        {
            if (proposalCourseId == 0)
                throw new ArgumentException($"The proposal course id is 0.");

            ProposalCourse proposalCourse = _db.ProposalCourses
                .Include(x => x.Proposal)
                .FirstOrDefault(x => x.Id == proposalCourseId);
            if (proposalCourse == null)
                throw new ArgumentException($"The proposal course with id {proposalCourseId} does not exist.", nameof(proposalCourseId));

            Announcement announcement = _db.Announcements
                .Include(x => x.AnnouncementSections)
                .FirstOrDefault(x => x.SemesterId == proposalCourse.Proposal.SemesterId && x.CourseId == proposalCourse.CourseId);

            if (announcement == null)
                return;
            
            if (announcement.IsActivated == true)
                throw new ModelValidationException("You cannot delete an activated announcement.", "");

            if (announcement.AnnouncementSections.Count() > 1)
                throw new ModelValidationException("You cannot delete an announcement that contains more than one section.", "");

            _db.Announcements.Remove(announcement);
            _db.SaveChanges();
        }
    }
}
