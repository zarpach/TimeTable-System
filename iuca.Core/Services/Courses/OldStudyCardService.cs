using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Instructors;
using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Interfaces.Courses;
using iuca.Application.ViewModels.Courses;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using iuca.Application.Enums;
using System.Reflection;
using iuca.Application.Exceptions;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;

namespace iuca.Application.Services.Courses
{
    public class OldStudyCardService : IOldStudyCardService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public OldStudyCardService(IApplicationDbContext db,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Get study card list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Study card list</returns>
        public IEnumerable<OldStudyCardDTO> GetStudyCards(int selectedOrganizationId)
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Semester, SemesterDTO>();
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<OldStudyCard, OldStudyCardDTO>();
            }).CreateMapper();
            return mapper.Map<IEnumerable<OldStudyCard>, IEnumerable<OldStudyCardDTO>>(_db.OldStudyCards
                .Where(x => x.OrganizationId == selectedOrganizationId)
                .Include(x => x.Semester)
                .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department));
        }

        /// <summary>
        /// Get study card list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <returns>Study card list</returns>
        public IEnumerable<OldStudyCardDTO> GetStudyCards(int selectedOrganizationId, int departmentGroupId)
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Semester, SemesterDTO>();
                cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>();
                cfg.CreateMap<InstructorBasicInfo, InstructorBasicInfoDTO>();
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<OldStudyCard, OldStudyCardDTO>();
            }).CreateMapper();

            return mapper.Map<IEnumerable<OldStudyCard>, IEnumerable<OldStudyCardDTO>>(_db.OldStudyCards
                .Where(x => x.OrganizationId == selectedOrganizationId && x.DepartmentGroupId == departmentGroupId)
                .Include(x => x.Semester)
                .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department));
        }

        /// <summary>
        /// Get study card by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of study card</param>
        /// <returns>Study card model</returns>
        public OldStudyCardDTO GetStudyCard(int selectedOrganizationId, int id)
        {
            OldStudyCard studyCard = _db.OldStudyCards
                .Include(x => x.Semester)
                .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                .Include(x => x.OldStudyCardCourses).ThenInclude(x => x.CyclePartCourse).ThenInclude(x => x.Course)
                .FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (studyCard == null)
                throw new Exception($"Study card with id {id} not found");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Semester, SemesterDTO>();
                cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>();
                cfg.CreateMap<InstructorBasicInfo, InstructorBasicInfoDTO>();
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<OldStudyCardCourse, OldStudyCardCourseDTO>();
                cfg.CreateMap<Course, CourseDTO>();
                cfg.CreateMap<CyclePartCourse, CyclePartCourseDTO>();
                cfg.CreateMap<OldStudyCard, OldStudyCardDTO>();
            }).CreateMapper();
            return mapper.Map<OldStudyCard, OldStudyCardDTO>(studyCard);
        }

        /// <summary>
        /// Get study card courses for selection window
        /// </summary>
        /// <param name="semester">Semester to select courses</param>
        /// <param name="group">Group for courses selection</param>
        /// <param name="academicPlan">Academic plan with courses for group</param>
        /// <returns>List of courses with recommendations</returns>
        public List<StudyCardSelectionCourseViewModel> GetCoursesForSelection(SemesterDTO semester, DepartmentGroupDTO group,
            AcademicPlanDTO academicPlan, int[] excludedIds)
        {
            if (semester == null) 
                throw new Exception("semester is null");

            List<StudyCardSelectionCourseViewModel> courseList = new List<StudyCardSelectionCourseViewModel>();

            if (academicPlan != null && academicPlan.CycleParts != null && academicPlan.CycleParts.Any()) 
            {
                List<OldStudyCardCourse> existingStudyCardCourses = new List<OldStudyCardCourse>();
                
                    existingStudyCardCourses = _db.OldStudyCards
                        .Include(x => x.Semester)
                        .Where(x => (x.Semester.Year < semester.Year && x.DepartmentGroupId == group.Id) || 
                        (x.Semester.Year == semester.Year && x.Semester.Season < semester.Season))
                        .Include(x => x.OldStudyCardCourses)
                        .SelectMany(x => x.OldStudyCardCourses)
                        .ToList();

                int courseNum = semester.Year + 1 - academicPlan.Year;
                int semesterNumber = (enu_Season)semester.Season == enu_Season.Fall ? 1 : 2;

                foreach (var cyclePart in academicPlan.CycleParts) 
                {
                    foreach (var partCourse in cyclePart.CyclePartCourses) 
                    {
                        if (excludedIds.Contains(partCourse.Id))
                            continue;

                        var course = new StudyCardSelectionCourseViewModel();
                        course.AcademicPlanId = academicPlan.Id;
                        course.CylcePart = cyclePart;
                        course.Course = partCourse.Course;
                        course.Points = partCourse.Points;
                        course.CyclePartCourseId = partCourse.Id;

                        course.SelectionStatus = enu_CourseSelectionStatus.Neutral;

                        if (existingStudyCardCourses.Any(x => x.CyclePartCourseId == partCourse.Id))
                        {
                            course.SelectionStatus = enu_CourseSelectionStatus.NotRecommended;
                            course.Comment = "Был добавлен в предыдущих стади кард";
                        }
                        else 
                        {
                            string ptsName = $"PtsCrs{courseNum}Sem{semesterNumber}";

                            PropertyInfo ptsProperty = typeof(CyclePartCourseDTO).GetProperty(ptsName);

                            if (ptsProperty != null)
                            {
                                int pts = 0;
                                int.TryParse(ptsProperty.GetValue(partCourse).ToString(), out pts);
                                if (pts > 0)
                                {
                                    course.SelectionStatus = enu_CourseSelectionStatus.Recommended;
                                    course.Comment = "Читается в этом семестре";
                                }
                            }
                        }
                        courseList.Add(course);
                    }
                }
            }

            return courseList;
        }

        /// <summary>
        /// Get courses from selection window
        /// </summary>
        /// <param name="cyclePartCourseIds">Array of cycle part courses ids</param>
        /// <returns>Listcourses from selection window</returns>
        public List<OldStudyCardCourseDTO> GetCoursesFromSelection(int studyCardId, int[] cyclePartCourseIds)
        {
            List<OldStudyCardCourseDTO> studyCardCourses = new List<OldStudyCardCourseDTO>();

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Course, CourseDTO>();
                cfg.CreateMap<CyclePartCourse, CyclePartCourseDTO>();
            }).CreateMapper();

            var cyclePartCourses = mapper.Map<List<CyclePartCourse>, List<CyclePartCourseDTO>>(_db.CyclePartCourses
                .Include(x => x.Course).Where(x => cyclePartCourseIds.Contains(x.Id)).ToList());

            foreach (var cyclePart in cyclePartCourses) 
            {
                studyCardCourses.Add(new OldStudyCardCourseDTO
                {
                    CyclePartCourseId = cyclePart.Id,
                    CyclePartCourse = cyclePart,
                    OldStudyCardId = studyCardId
                }); ;
            }

            return studyCardCourses;
        }

        /// <summary>
        /// Create study card
        /// </summary>
        /// <param name="studyCardDTO">Study card model</param>
        public void Create(OldStudyCardDTO studyCardDTO)
        {
            if (studyCardDTO == null)
                throw new Exception($"studyCardDTO is null");

            var existingStudyCard = _db.OldStudyCards.FirstOrDefault(x => x.OrganizationId == studyCardDTO.OrganizationId &&
                    x.SemesterId == studyCardDTO.SemesterId && x.DepartmentGroupId == studyCardDTO.DepartmentGroupId);

            if (existingStudyCard != null)
                throw new ModelValidationException("Study card for this semester and group already exists", "ErrorMsg");

            var mapperToDTO = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Semester, SemesterDTO>();
                cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>();
                cfg.CreateMap<InstructorBasicInfo, InstructorBasicInfoDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
                cfg.CreateMap<OldStudyCard, OldStudyCardDTO>();
            }).CreateMapper();

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<SemesterDTO, Semester>();
                cfg.CreateMap<UserBasicInfoDTO, UserBasicInfo>();
                cfg.CreateMap<InstructorBasicInfoDTO, InstructorBasicInfo>();
                cfg.CreateMap<DepartmentGroupDTO, DepartmentGroup>();
                cfg.CreateMap<OldStudyCardDTO, OldStudyCard>();
            }).CreateMapper();

            OldStudyCard newStudyCard = mapperFromDTO.Map<OldStudyCardDTO, OldStudyCard>(studyCardDTO);

            _db.OldStudyCards.Add(newStudyCard);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit study card
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of study card</param>
        /// <param name="studyCardDTO">Study card model</param>
        public void Edit(int selectedOrganizationId, int id, OldStudyCardDTO studyCardDTO)
        {
            if (studyCardDTO == null)
                throw new Exception($"studyCardDTO is null");

            OldStudyCard studyCard = _db.OldStudyCards.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (studyCard == null)
                throw new Exception($"Study card with id {id} not found");

            var existingStudyCard = _db.OldStudyCards.FirstOrDefault(x => x.OrganizationId == selectedOrganizationId && x.Id != id &&
                    x.SemesterId == studyCardDTO.SemesterId && x.DepartmentGroupId == studyCardDTO.DepartmentGroupId);

            if (existingStudyCard != null)
                throw new ModelValidationException("Study card for this semester and group already exists", "ErrorMsg");

            studyCard.SemesterId = studyCardDTO.SemesterId;
            studyCard.DepartmentGroupId = studyCardDTO.DepartmentGroupId;

            _db.OldStudyCards.Update(studyCard);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete study card by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of study card</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            OldStudyCard studyCard = _db.OldStudyCards.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (studyCard == null)
                throw new Exception($"Study card with id {id} not found");

            _db.OldStudyCards.Remove(studyCard);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit study card courses
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studyCardId">Id of study card</param>
        /// <param name="modelCourses">List of study card courses</param>
        public void EditStudyCardCourses(int selectedOrganizationId, int studyCardId, List<OldStudyCardCourseDTO> modelCourses)
        {
            OldStudyCard studyCard = _db.OldStudyCards
                .FirstOrDefault(x => x.Id == studyCardId && x.OrganizationId == selectedOrganizationId);

            if (studyCard == null)
                throw new Exception($"Academic plan with id {studyCardId} not found");


            var dbCourses = _db.OldStudyCardCourses.Include(x => x.CyclePartCourse)
                .ThenInclude(x => x.Course).Where(x => x.OldStudyCardId == studyCardId).ToList();


            var existingCourses = dbCourses.ToList();

            foreach (OldStudyCardCourse dbCourse in dbCourses)
            {
                var course = modelCourses.FirstOrDefault(x => x.CyclePartCourseId == dbCourse.CyclePartCourseId);

                if (course == null)
                {
                    _db.OldStudyCardCourses.Remove(dbCourse);
                }
                else 
                {
                    dbCourse.IsVacancy = course.IsVacancy;
                    if (course.IsVacancy)
                        course.InstructorUserId = null;

                    dbCourse.InstructorUserId = course.InstructorUserId;
                    modelCourses.Remove(course);
                }
            }

            if (modelCourses.Any())
            {
                var courseMapper = new MapperConfiguration(cfg => {
                    cfg.CreateMap<OldStudyCardCourseDTO, OldStudyCardCourse>();
                }).CreateMapper();

                _db.OldStudyCardCourses.AddRange(courseMapper.Map<List<OldStudyCardCourseDTO>, List<OldStudyCardCourse>>(modelCourses));
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// Get study card places list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Study card places list</returns>
        public IEnumerable<StudyCardPlacesViewModel> GetStudyCardPlaces(int selectedOrganizationId, int semesterId)
        {
            var cyclePartCourseMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Course, CourseDTO>();
                cfg.CreateMap<CyclePartCourse, CyclePartCourseDTO>();
            }).CreateMapper();
            
            var instructorBasicInfoMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserBasicInfo, UserBasicInfoDTO>();
                cfg.CreateMap<InstructorBasicInfo, InstructorBasicInfoDTO>();
            }).CreateMapper();

            var studyCardCourses = _db.OldStudyCards.Where(x => x.OrganizationId == selectedOrganizationId &&
                            x.SemesterId == semesterId).Include(x => x.OldStudyCardCourses)
                            .SelectMany(x => x.OldStudyCardCourses)
                            .Include(x => x.CyclePartCourse).ThenInclude(x => x.Course).ToList()
                            .GroupBy(x => new { x.CyclePartCourse.CourseId, x.InstructorUserId })
                            .Select(x => new StudyCardPlacesViewModel
                            {
                                CyclePartCourse = cyclePartCourseMapper.Map<CyclePartCourse, CyclePartCourseDTO>(x.FirstOrDefault().CyclePartCourse),
                                InstructorUserId = x.FirstOrDefault().InstructorUserId,
                                InstructorName =  _userManager.GetUserFullName(x.FirstOrDefault().InstructorUserId),
                                Places = x.FirstOrDefault().Places
                            }); 

            return studyCardCourses;
        }

        /// <summary>
        /// Set study card places for given list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="courses">List of grouped courses</param>
        public void SetStudyCardPlaces(int selectedOrganizationId, int semesterId, List<StudyCardPlacesViewModel> courses) 
        {
            if (courses == null || courses.Count == 0)
                return;

            var dbStudyCardCourses = _db.OldStudyCards.Where(x => x.OrganizationId == selectedOrganizationId &&
                            x.SemesterId == semesterId).Include(x => x.OldStudyCardCourses)
                            .SelectMany(x => x.OldStudyCardCourses).Include(x => x.CyclePartCourse).ToList();

            foreach (var course in courses) 
            {
                var dbCourses = dbStudyCardCourses.Where(x => x.CyclePartCourse.CourseId == course.CyclePartCourse.CourseId);
                if (!string.IsNullOrEmpty(course.InstructorUserId))
                    dbCourses = dbCourses.Where(x => x.InstructorUserId == course.InstructorUserId);
                else
                    dbCourses = dbCourses.Where(x => string.IsNullOrEmpty(x.InstructorUserId));

                if (dbCourses != null && dbCourses.Any()) 
                {
                    foreach (var dbCourse in dbCourses) 
                    {
                        dbCourse.Places = course.Places;
                        _db.OldStudyCardCourses.Update(dbCourse);
                    }
                }
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Get department groups of existing study cards on semester
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>List of department groups</returns>
        public List<DepartmentGroupDTO> GetDepartmentGroupsForSemester(int organizationId, int semesterId) 
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
            }).CreateMapper();

            var groups = _db.OldStudyCards.Where(x => x.SemesterId == semesterId && x.OrganizationId == organizationId)
                .Include(x => x.DepartmentGroup).ThenInclude(x => x.Department)
                .Select(x => x.DepartmentGroup).ToList();

            return mapper.Map<List<DepartmentGroup>, List<DepartmentGroupDTO>>(groups);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
