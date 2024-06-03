using AutoMapper;
using iuca.Application.Converters;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Models;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Courses
{
    public class CourseService : ICourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly ICoursePrerequisiteService _coursePrerequisiteService;
        private readonly IMapper _mapper;

        public CourseService(IApplicationDbContext db,
            ICoursePrerequisiteService coursePrerequisiteService,
            IMapper mapper)
        {
            _db = db;
            _coursePrerequisiteService = coursePrerequisiteService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get course list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="parameters">Paging parameters</param>
        /// <param name="departmentIds">Department ids</param>
        /// <param name="courseName">Course name</param>
        /// <param name="courseAbbr">Course abbreviation</param>
        /// <param name="courseNum">Course number</param>
        /// <param name="courseId">Course id</param>
        /// <param name="isDeleted">Return deleted courses</param>
        /// <returns>Course list</returns>
        public PagedList<CourseDTO> GetCourses(int selectedOrganizationId, QueryStringParameters parameters,
            List<int> departmentIds, string courseName, string courseAbbr, string courseNum, int? courseId, bool isDeleted = false)
        {
            var query = _db.Courses.Include(x => x.Department)
                        .Include(x => x.Language)
                        .Where(x => x.OrganizationId == selectedOrganizationId);

            if (isDeleted)
                query = query.IgnoreQueryFilters();

            if (!string.IsNullOrEmpty(courseName)) 
                query = query.Where(x => x.NameRus.Contains(courseName) || x.NameEng.Contains(courseName) 
                    || x.NameKir.Contains(courseName));

            if (departmentIds != null)
                query = query.Where(x => departmentIds.Contains(x.DepartmentId));

            if (!string.IsNullOrEmpty(courseAbbr))
                query = query.Where(x => x.Abbreviation == courseAbbr);

            if (!string.IsNullOrEmpty(courseNum))
                query = query.Where(x => x.Number == courseNum);

            if (courseId != null)
                query = query.Where(x => x.ImportCode == courseId.Value);

            var courses = PagedList<Course>.ToPagedList(query.OrderBy(x => x.NameRus),
                    parameters.PageNumber, parameters.PageSize);

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<Language, LanguageDTO>();
                cfg.CreateMap<Course, CourseDTO>();
                cfg.CreateMap<PagedList<Course>, PagedList<CourseDTO>>()
                .ConvertUsing(typeof(PagedListConverter<Course, CourseDTO>));
            }).CreateMapper();

            var restult = mapper.Map<PagedList<CourseDTO>>(courses);

            return restult;
        }

        /// <summary>
        /// Get course list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="isDeleted">Return deleted courses</param>
        /// <returns>Course list</returns>
        public IEnumerable<CourseDTO> GetCourses(int selectedOrganizationId, bool isDeleted = false)
        {
            var query = _db.Courses
                .Include(x => x.Department)
                .Include(x => x.Language)
                .Where(x => x.OrganizationId == selectedOrganizationId);

            if (!isDeleted)
                query = query.Where(x => !x.IsDeleted);

            return _mapper.Map<IEnumerable<CourseDTO>>(query.AsEnumerable());
        }

        /// <summary>
        /// Get course list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="ids">Courses ids</param>
        /// <param name="isDeleted">Return deleted courses</param>
        /// <returns>Course list</returns>
        public IEnumerable<CourseDTO> GetCourses(int selectedOrganizationId, int[] ids, bool isDeleted = false)
        {
            var query = _db.Courses
                .Include(x => x.Department)
                .Include(x => x.Language)
                .Where(x => x.OrganizationId == selectedOrganizationId && ids.Contains(x.Id));
            
            if (!isDeleted)
                query = query.Where(x => !x.IsDeleted);

            return _mapper.Map<IEnumerable<CourseDTO>>(query.AsEnumerable());
        }

        /// <summary>
        /// Get course list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="excludedIds">Courses ids to exclude</param>
        /// <param name="isDeleted">Return deleted courses</param>
        /// <returns>Course list</returns>
        public IEnumerable<CourseDTO> GetCoursesWithExclusion(int selectedOrganizationId, int[] excludedIds, bool isDeleted = false)
        {
            var query = _db.Courses
                .Include(x => x.Department)
                .Include(x => x.Language)
                .Where(x => x.OrganizationId == selectedOrganizationId && !excludedIds.Contains(x.Id));

            if (!isDeleted)
                query = query.Where(x => !x.IsDeleted);

            return _mapper.Map<IEnumerable<CourseDTO>>(query.AsEnumerable());
        }

        /// <summary>
        /// Get courses for selection
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="excludedIds">Course ids to exclude</param>
        /// <param name="isDeleted">Return deleted courses</param>
        /// <returns>Course list without excluded ids</returns>
        public IEnumerable<CourseDTO> GetCoursesForSelection(int organizationId, int[] excludedIds, bool isDeleted = false)
        {
            var query = _db.Courses.Include(x => x.Department)
                        .Include(x => x.Language)
                        .Where(x => x.OrganizationId == organizationId && !excludedIds.Contains(x.Id));

            if (!isDeleted)
                query = query.Where(x => !x.IsDeleted);

            return _mapper.Map<IEnumerable<CourseDTO>>(query.AsEnumerable());
        }


        /// <summary>
        /// Get cycle part course list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="departmentId">Department id of academic plan</param>
        /// <param name="year">Year of academic plan</param>
        /// <param name="courseId">Course import Id</param>
        /// <param name="excludedIds">Cycle part courses ids to exclude</param>
        /// <returns>Cycle part courses list</returns>
        public List<CyclePartCourseDTO> GetCyclePartCoursesWithExclusion(int selectedOrganizationId, int departmentId, 
            int year, int courseId, int[] excludedIds)
        {
            List<CyclePartCourseDTO> courseList = new List<CyclePartCourseDTO>();

            var academicPlan = _db.AcademicPlans.FirstOrDefault(x => x.OrganizationId == selectedOrganizationId 
                                    && x.DepartmentId == departmentId && x.Year == year);

            if (academicPlan != null) 
            {
                var mapper = new MapperConfiguration(cfg => {
                    cfg.CreateMap<CyclePart, CyclePartDTO>()
                    .ForMember(x => x.AcademicPlan, opt => opt.Ignore())
                    .ForMember(x => x.Cycle, opt => opt.Ignore())
                    .ForMember(x => x.Cycle, opt => opt.Ignore());
                    cfg.CreateMap<Course, CourseDTO>()
                    .ForMember(x => x.Department, opt => opt.Ignore())
                    .ForMember(x => x.Language, opt => opt.Ignore());
                    cfg.CreateMap<CyclePartCourse, CyclePartCourseDTO>();
                }).CreateMapper();

                var courses = _db.CyclePartCourses
                    .Include(x => x.CyclePart)
                    .Include(x => x.Course)
                    .Where(x => x.CyclePart.AcademicPlanId == academicPlan.Id && !excludedIds.Contains(x.Id));

                if (courseId != 0)
                    courses = courses.Where(x => x.Course.ImportCode == courseId);

                courseList = mapper.Map<List<CyclePartCourse>, List<CyclePartCourseDTO>>(courses.ToList());
            }

            return courseList;
        }


        /// <summary>
        /// Get course by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of course</param>
        /// <param name="isDeleted">Include deleted records</param>
        /// <returns>Course model</returns>
        public CourseDTO GetCourse(int selectedOrganizationId, int id, bool isDeleted = false)
        {
            var query = _db.Courses.AsQueryable();

            if (isDeleted)
                query = query.IgnoreQueryFilters();

            Course course = query.Include(x => x.Department)
                .Include(x => x.Language)
                .Include(x => x.CoursePrerequisites).ThenInclude(x => x.Prerequisite).ThenInclude(x => x.Language)
                .FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            
            if (course == null)
                throw new Exception($"Course with id {id} not found");

            return _mapper.Map<Course, CourseDTO>(course);
        }

        /// <summary>
        /// Create course
        /// </summary>
        /// <param name="courseDTO">Course model</param>
        public void Create(CourseDTO courseDTO)
        {
            if (courseDTO == null)
                throw new Exception($"courseDTO is null");

            var newCourse = _mapper.Map<Course>(courseDTO);
            
            //Import code increment to keep old logic working 
            /*if (_db.Courses.Any())
                newCourse.ImportCode = _db.Courses.Max(x => x.ImportCode)+1;*/

            _db.Courses.Add(newCourse);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit course
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of course</param>
        /// <param name="courseDTO">Course model</param>
        public void Edit(int selectedOrganizationId, int id, CourseDTO courseDTO)
        {
            if (courseDTO == null)
                throw new Exception($"courseDTO is null");

            Course course = _db.Courses.Include(x => x.CoursePrerequisites).FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (course == null)
                throw new Exception($"Course with id {id} not found");

            course.NameEng = courseDTO.NameEng;
            course.NameRus = courseDTO.NameRus;
            course.NameKir = courseDTO.NameKir;
            course.Abbreviation = courseDTO.Abbreviation;
            course.Number = courseDTO.Number;
            course.DepartmentId = courseDTO.DepartmentId;
            course.LanguageId = courseDTO.LanguageId;
            course.IsChanged = true;
            course.CourseType = courseDTO.CourseType; 

            EditCoursePrerequisites(id, course.CoursePrerequisites.ToList(), courseDTO.CoursePrerequisites);

            _db.Courses.Update(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// Create, update, or delete course prerequisites
        /// </summary>
        /// <param name="existingCoursePrerequisiteList">List of existing course prerequisites</param>
        /// <param name="newCoursePrerequisiteList">List of course prerequisites from new model</param>
        private void EditCoursePrerequisites(int courseId, List<CoursePrerequisite> existingCoursePrerequisiteList,
            List<CoursePrerequisiteDTO> newCoursePrerequisiteList)
        {
            if (newCoursePrerequisiteList != null && newCoursePrerequisiteList.Any())
            {
                foreach (CoursePrerequisiteDTO coursePrerequisite in newCoursePrerequisiteList)
                {
                    if (coursePrerequisite.CourseId == 0)
                        coursePrerequisite.CourseId = courseId;
                    CoursePrerequisite existingPrerequisite = existingCoursePrerequisiteList
                        .FirstOrDefault(x => x.PrerequisiteId == coursePrerequisite.PrerequisiteId);
                    if (existingPrerequisite == null)
                        _coursePrerequisiteService.Create(coursePrerequisite);
                    else 
                        existingCoursePrerequisiteList.Remove(existingPrerequisite);
                }
            }

            //Delete course prerequisite if it is removed from model
            if (existingCoursePrerequisiteList.Any())
            {
                foreach (CoursePrerequisite coursePrerequisite in existingCoursePrerequisiteList)
                {
                    _coursePrerequisiteService.Delete(coursePrerequisite.CourseId, coursePrerequisite.PrerequisiteId);
                }
            }
        }

        /// <summary>
        /// Delete course by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of course</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            Course course = _db.Courses.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (course == null)
                throw new Exception($"Course with id {id} not found");

            _db.Courses.Remove(course);
            _db.SaveChanges();
        }

        /// <summary>
        /// UnDelete course by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of course</param>
        public void UnDelete(int selectedOrganizationId, int id)
        {
            Course course = _db.Courses.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (course == null)
                throw new Exception($"Course with id {id} not found");
            
            course.IsDeleted = false;
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        /// <summary>
        /// Get course SelectList
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="excludedCourseId">Course id that must be excluded</param>
        /// <param name="selectedCourseId">Selected course id</param>
        /// <returns>SelectList of courses</returns>
        public List<SelectListItem> GetCourseSelectList(int selectedOrganizationId, int? excludedCourseId, int? selectedCourseId)
        {
            var courses = _db.Courses.Where(x => x.OrganizationId == selectedOrganizationId);
            if (excludedCourseId != null)
                courses = courses.Where(x => x.Id != excludedCourseId.Value);

            var selectCourses = courses.Select(x => new { Id = x.Id, Name = x.Abbreviation + x.Number + " " + x.NameEng })
                .OrderBy(x => x.Name).ToList();

            return new SelectList(selectCourses, "Id", "Name", selectedCourseId).ToList();
        }

        
    }
}
