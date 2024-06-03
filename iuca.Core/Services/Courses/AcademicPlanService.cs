using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Courses;
using iuca.Application.ViewModels.Courses;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Courses
{
    public class AcademicPlanService : IAcademicPlanService
    {
        private readonly IApplicationDbContext _db;
        private readonly ICycleService _cycleService;

        public AcademicPlanService(IApplicationDbContext db, 
            ICycleService cycleService)
        {
            _db = db;
            _cycleService = cycleService;
        }

        /// <summary>
        /// Get academic plan list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Academic plan list</returns>
        public IEnumerable<AcademicPlanDTO> GetAcademicPlans(int selectedOrganizationId)
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<AcademicPlan, AcademicPlanDTO>();
                }).CreateMapper();
            return mapper.Map<IEnumerable<AcademicPlan>, IEnumerable<AcademicPlanDTO>>
                (_db.AcademicPlans.Include(x => x.Department).Where(x => x.OrganizationId == selectedOrganizationId));
        }

        /// <summary>
        /// Get academic plan by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of academic plan/param>
        /// <returns>Academic plan model</returns>
        public AcademicPlanDTO GetAcademicPlan(int selectedOrganizationId, int id)
        {
            AcademicPlan academicPlan = _db.AcademicPlans.Include(x => x.Department)
                .FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (academicPlan == null)
                throw new Exception($"Academic plan with id {id} not found");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<AcademicPlan, AcademicPlanDTO>();
            }).CreateMapper();
            return mapper.Map<AcademicPlan, AcademicPlanDTO>(academicPlan);
        }

        /// <summary>
        /// Get academic plans by year and department
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="departmentGroup">Department group/param>
        /// <returns>Academic plans for department group</returns>
        public AcademicPlanDTO GetAcademicPlanForDepartmentGroup(int selectedOrganizationId, DepartmentGroupDTO departmentGroup)
        {
            var academicPlan = _db.AcademicPlans.Include(x => x.Department)
                .Include(x => x.CycleParts).ThenInclude(x => x.CyclePartCourses).ThenInclude(x => x.Course)
                .Include(x => x.CycleParts).ThenInclude(x => x.Cycle)
                .FirstOrDefault(x => x.OrganizationId == selectedOrganizationId && x.Year == departmentGroup.Year && x.DepartmentId == departmentGroup.DepartmentId);

            if (academicPlan == null)
                throw new Exception($"Academic plan not found");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Course, CourseDTO>();
                cfg.CreateMap<CyclePartCourse, CyclePartCourseDTO>();
                cfg.CreateMap<Cycle, CycleDTO>();
                cfg.CreateMap<CyclePart, CyclePartDTO>();
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<AcademicPlan, AcademicPlanDTO>();
            }).CreateMapper();
            return mapper.Map<AcademicPlan, AcademicPlanDTO>(academicPlan);
        }

        /// <summary>
        /// Create academic plan
        /// </summary>
        /// <param name="academicPlanDTO">Academic plan model</param>
        public void Create(AcademicPlanDTO academicPlanDTO)
        {
            if (academicPlanDTO == null)
                throw new Exception($"academicPlanDTO is null");

            var existingAcademicPlan = _db.AcademicPlans.Where(x => x.DepartmentId == academicPlanDTO.DepartmentId
                                && x.Year == academicPlanDTO.Year).FirstOrDefault();
            if (existingAcademicPlan != null)
                throw new ModelValidationException("Academic plan for this department and year already exists", "ErrorMsg");

            var mapperToDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<AcademicPlan, AcademicPlanDTO>();
            }).CreateMapper();

            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<DepartmentDTO, Department>();
                cfg.CreateMap<AcademicPlanDTO, AcademicPlan>();
            }).CreateMapper();

            AcademicPlan newAcademicPlan = mapperFromDTO.Map<AcademicPlanDTO, AcademicPlan>(academicPlanDTO);

            _db.AcademicPlans.Add(newAcademicPlan);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit academic plan
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of academic plan</param>
        /// <param name="academicPlanDTO">Academic plan model</param>
        public void Edit(int selectedOrganizationId, int id, AcademicPlanDTO academicPlanDTO)
        {
            if (academicPlanDTO == null)
                throw new Exception($"academicPlanDTO is null");

            AcademicPlan academicPlan = _db.AcademicPlans.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (academicPlan == null)
                throw new Exception($"Academic plan with id {id} not found");

            var existingAcademicPlan = _db.AcademicPlans.Where(x => x.DepartmentId == academicPlanDTO.DepartmentId
                                && x.Year == academicPlanDTO.Year && x.Id != id).FirstOrDefault();
            if (existingAcademicPlan != null)
                throw new ModelValidationException("Academic plan for this department and year already exists", "ErrorMsg");

            academicPlan.Year = academicPlanDTO.Year;
            academicPlan.DepartmentId = academicPlanDTO.DepartmentId;

            _db.AcademicPlans.Update(academicPlan);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete academic plan by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of academic plan</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            AcademicPlan academicPlan = _db.AcademicPlans.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (academicPlan == null)
                throw new Exception($"Academic plan with id {id} not found");

            _db.AcademicPlans.Remove(academicPlan);
            _db.SaveChanges();
        }

        
        /// <summary>
        /// Get academic plan by id to edit courses
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="academicPlanId">Id of academic plan/param>
        /// <returns>Academic plan edit model</returns>
        public AcademicPlanViewModel GetAcademicPlanEditModel(int selectedOrganizationId, int academicPlanId) 
        {
            var academicPlan = _db.AcademicPlans
                .Include(x => x.Department)
                .Include(x => x.CycleParts).ThenInclude(x => x.Cycle)
                .Include(x => x.CycleParts).ThenInclude(x => x.CyclePartCourses).ThenInclude(x => x.Course)
                .FirstOrDefault(x => x.Id == academicPlanId && x.OrganizationId == selectedOrganizationId);
            
            if (academicPlan == null)
                throw new Exception($"Academic plan with id {academicPlanId} not found");

            AcademicPlanViewModel model = new AcademicPlanViewModel();

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<Cycle, CycleDTO>();
                cfg.CreateMap<Course, CourseDTO>();
                cfg.CreateMap<CyclePart, CyclePartDTO>();
                cfg.CreateMap<CyclePartCourse, CyclePartCourseDTO>();
                cfg.CreateMap<AcademicPlan, AcademicPlanDTO>();
            }).CreateMapper();

            model.AcademicPlan = mapper.Map<AcademicPlan, AcademicPlanDTO>(academicPlan);
            model.Cycles = _cycleService.GetCycles().ToList();
            model.Parts = ((enu_AcademicPlanPart[])Enum.GetValues(typeof(enu_AcademicPlanPart))).ToList();

            return model;
        }

        /// <summary>
        /// Get academic plan model filled by courses of selected year
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="academicPlanId">Id of academic plan/param>
        /// <param name="year">Year of the academic plan to get courses from<param>
        /// <returns>Academic plan edit model</returns>
        public AcademicPlanViewModel FillAcademicPlanByYear(int selectedOrganizationId, int academicPlanId, int year)
        {
            var academicPlan = _db.AcademicPlans
                .Include(x => x.Department)
                .FirstOrDefault(x => x.Id == academicPlanId && x.OrganizationId == selectedOrganizationId);

            if (academicPlan == null)
                throw new Exception($"Academic plan with id {academicPlanId} not found");

            var existingAcademicPlan = _db.AcademicPlans
                .Include(x => x.CycleParts).ThenInclude(x => x.Cycle)
                .Include(x => x.CycleParts).ThenInclude(x => x.CyclePartCourses).ThenInclude(x => x.Course)
                .FirstOrDefault(x => x.DepartmentId == academicPlan.DepartmentId && x.Year == year);

            if (existingAcademicPlan == null)
                throw new ModelValidationException($"Academic plan for { academicPlan.Department.Code } { year } not found", "ErrorMsg");

            academicPlan.CycleParts = existingAcademicPlan.CycleParts;

            AcademicPlanViewModel model = new AcademicPlanViewModel();

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<Cycle, CycleDTO>();
                cfg.CreateMap<Course, CourseDTO>();
                cfg.CreateMap<CyclePart, CyclePartDTO>();
                cfg.CreateMap<CyclePartCourse, CyclePartCourseDTO>();
                cfg.CreateMap<AcademicPlan, AcademicPlanDTO>();
            }).CreateMapper();

            model.AcademicPlan = mapper.Map<AcademicPlan, AcademicPlanDTO>(academicPlan);
            model.Cycles = _cycleService.GetCycles().ToList();
            model.Parts = ((enu_AcademicPlanPart[])Enum.GetValues(typeof(enu_AcademicPlanPart))).ToList();

            return model;
        }

        /// <summary>
        /// Edit academic plan courses
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="academicPlanId">Id of academic plan</param>
        /// <param name="modelCycleParts">List of academic plan cycle parts</param>
        public void EditAcademicPlanCourses(int selectedOrganizationId, int academicPlanId, List<CyclePartDTO> modelCycleParts)
        {
            AcademicPlan academicPlan = _db.AcademicPlans
                .FirstOrDefault(x => x.Id == academicPlanId && x.OrganizationId == selectedOrganizationId);

            if (academicPlan == null)
                throw new Exception($"Academic plan with id {academicPlanId} not found");


            var dbCycleParts = _db.CycleParts.Include(x => x.CyclePartCourses).Where(x => x.AcademicPlanId == academicPlanId).ToList();

            modelCycleParts = modelCycleParts.Where(x => x.CyclePartCourses != null && x.CyclePartCourses.Any()).ToList();

            foreach (CyclePart dbCyclePart in dbCycleParts)
            {
                var cyclePart = modelCycleParts.FirstOrDefault(x => x.AcademicPlanPart == dbCyclePart.AcademicPlanPart
                    && x.CycleId == dbCyclePart.CycleId);

                if (cyclePart == null || cyclePart.CyclePartCourses == null || !cyclePart.CyclePartCourses.Any()) 
                {
                    _db.CycleParts.Remove(dbCyclePart);
                }
                else
                {
                    dbCyclePart.ReqPts = cyclePart.ReqPts;
                    dbCyclePart.ReqPtsCrs1Sem1 = cyclePart.ReqPtsCrs1Sem1;
                    dbCyclePart.ReqPtsCrs1Sem2 = cyclePart.ReqPtsCrs1Sem2;
                    dbCyclePart.ReqPtsCrs2Sem1 = cyclePart.ReqPtsCrs2Sem1;
                    dbCyclePart.ReqPtsCrs2Sem2 = cyclePart.ReqPtsCrs2Sem2;
                    dbCyclePart.ReqPtsCrs3Sem1 = cyclePart.ReqPtsCrs3Sem1;
                    dbCyclePart.ReqPtsCrs3Sem2 = cyclePart.ReqPtsCrs3Sem2;
                    dbCyclePart.ReqPtsCrs4Sem1 = cyclePart.ReqPtsCrs4Sem1;
                    dbCyclePart.ReqPtsCrs4Sem2 = cyclePart.ReqPtsCrs4Sem2;

                    var existingCourses = dbCyclePart.CyclePartCourses.ToList();

                    foreach (CyclePartCourseDTO course in cyclePart.CyclePartCourses) 
                    {
                        var dbCourse = existingCourses.FirstOrDefault(x => x.CourseId == course.CourseId);
                        if (dbCourse != null)
                        {
                            dbCourse.GroupName = course.GroupName;
                            dbCourse.GroupId = course.GroupId;
                            dbCourse.Points = course.Points;
                            dbCourse.PtsCrs1Sem1 = course.PtsCrs1Sem1;
                            dbCourse.PtsCrs1Sem2 = course.PtsCrs1Sem2;
                            dbCourse.PtsCrs2Sem1 = course.PtsCrs2Sem1;
                            dbCourse.PtsCrs2Sem2 = course.PtsCrs2Sem2;
                            dbCourse.PtsCrs3Sem1 = course.PtsCrs3Sem1;
                            dbCourse.PtsCrs3Sem2 = course.PtsCrs3Sem2;
                            dbCourse.PtsCrs4Sem1 = course.PtsCrs4Sem1;
                            dbCourse.PtsCrs4Sem2 = course.PtsCrs4Sem2;

                            existingCourses.Remove(dbCourse);
                        }
                        else 
                        {
                            CyclePartCourse newCourse = new CyclePartCourse();
                            newCourse.CourseId = course.CourseId;
                            newCourse.GroupName = course.GroupName;
                            newCourse.GroupId = course.GroupId;
                            newCourse.CyclePartId = dbCyclePart.Id;
                            newCourse.Points = course.Points;
                            newCourse.PtsCrs1Sem1 = course.PtsCrs1Sem1;
                            newCourse.PtsCrs1Sem2 = course.PtsCrs1Sem2;
                            newCourse.PtsCrs2Sem1 = course.PtsCrs2Sem1;
                            newCourse.PtsCrs2Sem2 = course.PtsCrs2Sem2;
                            newCourse.PtsCrs3Sem1 = course.PtsCrs3Sem1;
                            newCourse.PtsCrs3Sem2 = course.PtsCrs3Sem2;
                            newCourse.PtsCrs4Sem1 = course.PtsCrs4Sem1;
                            newCourse.PtsCrs4Sem2 = course.PtsCrs4Sem2;
                            _db.CyclePartCourses.Add(newCourse);
                        }
                    }

                    if (existingCourses.Any()) 
                        _db.CyclePartCourses.RemoveRange(existingCourses);

                    _db.CycleParts.Update(dbCyclePart);
                }

                modelCycleParts.Remove(cyclePart);
            }

            if (modelCycleParts.Any())
            {
                var cyclePartMapper = new MapperConfiguration(cfg => {
                    cfg.CreateMap<CyclePartCourseDTO, CyclePartCourse>();
                    cfg.CreateMap<CyclePartDTO, CyclePart>();
                }).CreateMapper();

                _db.CycleParts.AddRange(cyclePartMapper.Map<List<CyclePartDTO>, List<CyclePart>>(modelCycleParts));
            }
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
