using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Users.Students;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Users.Students
{
    public class DepartmentGroupService : IDepartmentGroupService
    {
        private readonly IApplicationDbContext _db;

        public DepartmentGroupService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get department group list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <returns>Department group list</returns>
        public IEnumerable<DepartmentGroupDTO> GetDepartmentGroups(int selectedOrganizationId)
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Organization, OrganizationDTO>();
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>(); 
            }).CreateMapper();
            var model = mapper.Map<IEnumerable<DepartmentGroup>, IEnumerable<DepartmentGroupDTO>>(_db.DepartmentGroups.Include(x => x.Department)
                .Where(x => x.OrganizationId == selectedOrganizationId));
            return model;
        }

        public IEnumerable<DepartmentGroupDTO> GetDepartmentGroupsByParam(int selectedOrganizaionId, int departmentId)
        {
            var AllDepartmentGroups = GetDepartmentGroups(selectedOrganizaionId);
            AllDepartmentGroups = AllDepartmentGroups
                .Where(x => x.DepartmentId == departmentId)
                .OrderBy(x => x.Code);

            return AllDepartmentGroups;
        }

        /// <summary>
        /// Get department group by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department group</param>
        /// <returns>Department group model</returns>
        public DepartmentGroupDTO GetDepartmentGroup(int selectedOrganizationId, int id)
        {
            DepartmentGroup departmentGroup = _db.DepartmentGroups.Include(x => x.Department)
                .FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (departmentGroup == null)
                throw new Exception($"Department group with id {id} not found");

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>();
            }).CreateMapper();
            return mapper.Map<DepartmentGroup, DepartmentGroupDTO>(departmentGroup);
        }

        /// <summary>
        /// Create department group
        /// </summary>
        /// <param name="departmentGroupDTO">Department group model</param>
        public void Create(DepartmentGroupDTO departmentGroupDTO)
        {
            if (departmentGroupDTO == null)
                throw new Exception($"departmentGroupDTO is null");

            if (_db.DepartmentGroups.Any(x => x.OrganizationId == departmentGroupDTO.OrganizationId &&
                x.DepartmentId == departmentGroupDTO.DepartmentId && x.Year == departmentGroupDTO.Year && x.Code == departmentGroupDTO.Code))
                    throw new ModelValidationException($"Department group with these department and year already exists", "");

            var mapperToDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<DepartmentGroup, DepartmentGroupDTO>(); 
            }).CreateMapper();
            
            var mapperFromDTO = new MapperConfiguration(cfg => {
                cfg.CreateMap<DepartmentDTO, Department>();
                cfg.CreateMap<DepartmentGroupDTO, DepartmentGroup>(); 
            }).CreateMapper();

            DepartmentGroup newDepartmentGroup = mapperFromDTO.Map<DepartmentGroupDTO, DepartmentGroup>(departmentGroupDTO);

            _db.DepartmentGroups.Add(newDepartmentGroup);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit department group
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department group</param>
        /// <param name="departmentGroupDTO">Department group model</param>
        public void Edit(int selectedOrganizationId, int id, DepartmentGroupDTO departmentGroupDTO)
        {
            if (departmentGroupDTO == null)
                throw new Exception("departmentGroupDTO is null");

            DepartmentGroup departmentGroup = _db.DepartmentGroups.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (departmentGroup == null)
                throw new ModelValidationException($"Department group with id {id} not found", "");

            if (_db.DepartmentGroups.Any(x => x.OrganizationId == selectedOrganizationId &&
                    x.DepartmentId == departmentGroupDTO.DepartmentId && x.Year == departmentGroupDTO.Year && 
                    x.Code == departmentGroupDTO.Code && x.Id != id))
                throw new ModelValidationException($"Department group with these department and year already exists", "");

            departmentGroup.DepartmentId = departmentGroupDTO.DepartmentId;
            departmentGroup.Year = departmentGroupDTO.Year;
            departmentGroup.Code = departmentGroupDTO.Code;

            _db.DepartmentGroups.Update(departmentGroup);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete department group by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department group</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            DepartmentGroup departmentGroup = _db.DepartmentGroups.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (departmentGroup == null)
                throw new Exception($"Department group with id {id} not found");

            _db.DepartmentGroups.Remove(departmentGroup);
            _db.SaveChanges();
        }

        /// <summary>
        /// Generate department groups for selected year
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="year">Year</param>
        public void GenerateDepartmentGroupsForYear(int selectedOrganizationId, int year)
        {
            if (year.ToString().Length != 4)
                throw new Exception("Year is wrong");

            var activeDepartments = _db.Departments.Where(x => x.OrganizationId == selectedOrganizationId && x.IsActive)
                .ToList();

            foreach (var department in activeDepartments) 
            {
                if (!_db.DepartmentGroups.Any(x => x.OrganizationId == selectedOrganizationId && 
                    x.DepartmentId == department.Id && x.Year == year)) 
                {
                    DepartmentGroup departmentGroup = new DepartmentGroup();
                    departmentGroup.OrganizationId = selectedOrganizationId;
                    departmentGroup.Year = year;
                    departmentGroup.DepartmentId = department.Id;
                    departmentGroup.Code = "1" + year.ToString().Substring(2);

                    _db.DepartmentGroups.Add(departmentGroup);
                }
            }
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
