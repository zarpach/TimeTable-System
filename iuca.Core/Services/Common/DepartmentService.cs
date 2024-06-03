using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Common
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IApplicationDbContext _db;

        public DepartmentService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get department list
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="activeOnly">Active only</param>
        /// <returns>Department list</returns>
        public IEnumerable<DepartmentDTO> GetDepartments(int selectedOrganizationId, bool activeOnly = false)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Department, DepartmentDTO>()).CreateMapper();

            var departments = _db.Departments.Where(x => x.OrganizationId == selectedOrganizationId);
            if (activeOnly)
                departments = departments.Where(x => x.IsActive);

            return mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentDTO>>(departments).OrderBy(x => x.NameEng);
        }

        /// <summary>
        /// Get department by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department/param>
        /// <returns>Department model</returns>
        public DepartmentDTO GetDepartment(int selectedOrganizationId, int id)
        {
            Department department = _db.Departments.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (department == null)
                throw new Exception($"Department with id {id} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Department, DepartmentDTO>()).CreateMapper();
            return mapper.Map<Department, DepartmentDTO>(department);
        }

        /// <summary>
        /// Create department
        /// </summary>
        /// <param name="departmentDTO">Department model</param>
        public void Create(DepartmentDTO departmentDTO)
        {
            if (departmentDTO == null)
                throw new Exception($"departmentDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<Department, DepartmentDTO>()).CreateMapper();
            var mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<DepartmentDTO, Department>()).CreateMapper();

            Department newDepartment = mapperFromDTO.Map<DepartmentDTO, Department>(departmentDTO);

            _db.Departments.Add(newDepartment);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit department
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department</param>
        /// <param name="departmentDTO">Department model</param>
        public void Edit(int selectedOrganizationId, int id, DepartmentDTO departmentDTO)
        {
            if (departmentDTO == null)
                throw new Exception($"departmentDTO is null");

            Department department = _db.Departments.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (department == null)
                throw new Exception($"Department with id {id} not found");

            department.NameEng = departmentDTO.NameEng;
            department.NameRus = departmentDTO.NameRus;
            department.NameKir = departmentDTO.NameKir;
            department.Code = departmentDTO.Code;
            department.IsActive= departmentDTO.IsActive;

            _db.Departments.Update(department);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete department by id
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="id">Id of department</param>
        public void Delete(int selectedOrganizationId, int id)
        {
            Department department = _db.Departments.FirstOrDefault(x => x.Id == id && x.OrganizationId == selectedOrganizationId);
            if (department == null)
                throw new Exception($"Department with id {id} not found");

            _db.Departments.Remove(department);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get department SelectList
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="selectedDepartment">Selected department id</param>
        /// <returns>SelectList of departments</returns>
        public List<SelectListItem> GetDepartmentSelectList(int organizationId, int? selectedDepartment)
        {
            return new SelectList(_db.Departments.Where(x => x.OrganizationId == organizationId)
                .OrderBy(x => x.Code), "Id", "Code", selectedDepartment).ToList();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
