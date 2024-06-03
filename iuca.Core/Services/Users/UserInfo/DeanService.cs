using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Roles;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels.Users.UserInfo;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Users.UserInfo
{
    public class DeanService : IDeanService
    {
        private readonly IApplicationDbContext _db;
        private readonly IUserRolesService _userRolesService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public DeanService(IApplicationDbContext db,
            IUserRolesService userRolesService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userRolesService = userRolesService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get dean departments model list who has dean role
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Dean departments model view model list who has dean role</returns>
        public IEnumerable<DeanDepartmentViewModel> GetDeans(int organizationId)
        {
            List<DeanDepartmentViewModel> model = new List<DeanDepartmentViewModel>();

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
            }).CreateMapper();

            var deans = _userRolesService.GetUserListByRole(organizationId, enu_Role.Dean).ToList();
            foreach (var dean in deans)
            {
                DeanDepartmentViewModel deanDepartment = new DeanDepartmentViewModel();
                deanDepartment.DeanUser = dean;
                deanDepartment.OrganizationId = organizationId;
                deanDepartment.Departments = mapper.Map<List<Department>, List<DepartmentDTO>>(
                    _db.DeanDepartments.Include(x => x.Department).Where(x => x.DeanUserId == dean.Id &&
                    x.OrganizationId == organizationId).Select(x => x.Department).ToList());
                model.Add(deanDepartment);
            }

            return model;
        }

        /// <summary>
        /// Get dean departments
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns>Dean departments model</returns>
        public DeanDepartmentViewModel GetDeanDepartments(int organizationId, string deanUserId) 
        {
            var departmentMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Department, DepartmentDTO>();
            }).CreateMapper();

            var deanDepartments = _db.DeanDepartments
                .Include(x => x.Department)
                .Where(x => x.OrganizationId == organizationId &&
                        x.DeanUserId == deanUserId).ToList();

            DeanDepartmentViewModel deanDepartmentModel = new DeanDepartmentViewModel();
            deanDepartmentModel.DeanUser = _userManager.Users.FirstOrDefault(x => x.Id == deanUserId);
            deanDepartmentModel.OrganizationId = organizationId;
            deanDepartmentModel.Departments = departmentMapper.Map<List<Department>, List<DepartmentDTO>>(
                deanDepartments.Select(x => x.Department).ToList());

            return deanDepartmentModel;
        }


        /// <summary>
        /// Edit dean departments
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <param name="departmentIds">Department id array</param>
        public void EditDeanDepartments(int organizationId, string deanUserId, List<int> departmentIds)
        {
            var modelDepartmentIds = departmentIds.ToList();
            var dbDepartments = _db.DeanDepartments.Where(x => x.OrganizationId == organizationId && 
                    x.DeanUserId == deanUserId).ToList();

            var existingDepartments = dbDepartments.ToList();

            foreach (var dbDepartment in dbDepartments)
            {
                int departmentId = modelDepartmentIds.FirstOrDefault(x => x == dbDepartment.DepartmentId);

                if (departmentId == 0)
                    _db.DeanDepartments.Remove(dbDepartment);
                else
                    modelDepartmentIds.Remove(departmentId);
            }

            if (modelDepartmentIds.Any())
                foreach (var departmentId in modelDepartmentIds)
                    AddDepartmentToDean(organizationId, deanUserId, departmentId);

            _db.SaveChanges();
        }

        private void AddDepartmentToDean(int organizationId, string deanUserId, int departmentId)
        {
            DeanDepartment deanDepartment = new DeanDepartment();
            deanDepartment.OrganizationId = organizationId;
            deanDepartment.DeanUserId = deanUserId;
            deanDepartment.DepartmentId = departmentId;
            _db.DeanDepartments.Add(deanDepartment);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get instructor brief view model list of advisers by dean user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns>Instructor brief view model of user list who has adviser role</returns>
        public IEnumerable<DeanAdviserViewModel> GetDeanAdvisers(int organizationId, string deanUserId)
        {
            List<DeanAdviserViewModel> adviserListModel = new List<DeanAdviserViewModel>();
            var adviserIds = _db.DeanAdvisers.Where(x => x.OrganizationId == organizationId && x.DeanUserId == deanUserId)
                .Select(x => x.AdviserUserId).ToList();

            var orgInfo = _db.InstructorOrgInfo.Include(x => x.InstructorBasicInfo)
                .Where(x => adviserIds.Contains(x.InstructorBasicInfo.InstructorUserId) && 
                    x.OrganizationId == organizationId).ToList();

            var advisers = _userManager.Users.Where(x => adviserIds.Contains(x.Id)).ToList();

            foreach (var adviser in advisers)
            {
                DeanAdviserViewModel model = new DeanAdviserViewModel();
                model.Instructor = adviser;
                var instructorOrgInfo = orgInfo.FirstOrDefault(x => x.InstructorBasicInfo.InstructorUserId == adviser.Id);
                if(instructorOrgInfo != null)
                    model.State = (enu_InstructorState)instructorOrgInfo.State;
                adviserListModel.Add(model);
            }

            return adviserListModel;
        }

        /// <summary>
        /// Get list of instructors that not belong to dean
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <param name="onlyActive">Only active instructors</param>
        /// <returns>List of instructors</returns>
        public List<DeanAdviserViewModel> GetFreeInstructorsForDean(int organizationId, string deanUserId, bool onlyActive = true)
        {
            List<DeanAdviserViewModel> deanAdviserList = new List<DeanAdviserViewModel>();

            if (!string.IsNullOrEmpty(deanUserId)) 
            {
                var selectedInstructors = _db.DeanAdvisers
                    .Where(x => x.DeanUserId == deanUserId && x.OrganizationId == organizationId)
                    .Select(x => x.AdviserUserId)
                    .ToList();

                var orgInfoes = _db.InstructorOrgInfo.Include(x => x.InstructorBasicInfo)
                    .Where(x => !selectedInstructors.Contains(x.InstructorBasicInfo.InstructorUserId) && 
                        x.OrganizationId == organizationId);

                if (onlyActive)
                    orgInfoes = orgInfoes.Where(x => x.State == (int)enu_InstructorState.Active);

                foreach (var orgInfo in orgInfoes.ToList())
                {
                    DeanAdviserViewModel model = new DeanAdviserViewModel();
                    model.Instructor = _userManager.Users.FirstOrDefault(x => x.Id == orgInfo.InstructorBasicInfo.InstructorUserId);
                    model.State = (enu_InstructorState)orgInfo.State;
                    deanAdviserList.Add(model);
                }
            }

            return deanAdviserList;
        }

        /// <summary>
        /// Add or remove adviser from dean
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <param name="instructorUserId">Instructor user id</param>
        /// <param name="isSelected">If true - add, else - remove</param>
        public void SetDeanAdviser(int organizationId, string deanUserId, string instructorUserId, bool isSelected) 
        {
            var deanAdviser = _db.DeanAdvisers
                .FirstOrDefault(x => x.OrganizationId == organizationId && 
                x.DeanUserId == deanUserId && x.AdviserUserId == instructorUserId);

            if (isSelected)
            {
                if (deanAdviser == null)
                    AddAdviserToDean(deanUserId, organizationId, instructorUserId);

                if (!_userRolesService.IsUserInRole(instructorUserId, organizationId, enu_Role.Adviser))
                    _userRolesService.AddToRole(instructorUserId, organizationId, enu_Role.Adviser);
            }
            else 
            {
                if (deanAdviser != null) 
                {
                    _db.DeanAdvisers.Remove(deanAdviser);
                    _db.SaveChanges();
                }

                if (!_db.DeanAdvisers.Any(x => x.OrganizationId == organizationId &&
                    x.AdviserUserId == instructorUserId)) 
                {
                    var adviserStudents = _db.AdviserStudents.Where(x => x.OrganizationId == organizationId &&
                        x.InstructorUserId == instructorUserId).ToList();

                    if (adviserStudents.Count > 0) 
                    {
                        foreach (var adviserStudent in adviserStudents)
                            _db.AdviserStudents.Remove(adviserStudent);

                        _db.SaveChanges();
                    }

                    if (_userRolesService.IsUserInRole(instructorUserId, organizationId, enu_Role.Adviser))
                        _userRolesService.RemoveFromRole(instructorUserId, organizationId, enu_Role.Adviser);
                }
            }
        }

        private void AddAdviserToDean(string deanUserId, int organizationId, string adviserUserId)
        {
            DeanAdviser deanAdviser = new DeanAdviser();
            deanAdviser.OrganizationId = organizationId;
            deanAdviser.DeanUserId = deanUserId;
            deanAdviser.AdviserUserId = adviserUserId;
            _db.DeanAdvisers.Add(deanAdviser);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get dean SelectList
        /// </summary>
        /// <param name="selectedOrganizationId">Id of selected organization</param>
        /// <param name="deanUserId">Selected dean user id</param>
        /// <returns>SelectList of deans</returns>
        public List<SelectListItem> GetDeanSelectList(int selectedOrganizationId, string deanUserId = null)
        {
            var deans = _userRolesService.GetUserListByRole(selectedOrganizationId, enu_Role.Dean).ToList();
            return new SelectList(deans, "Id", "FullNameEng", deanUserId).ToList();
        }

        /// <summary>
        /// Get advisers by dean user id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="deanUserId">Dean user id</param>
        /// <returns>List of advisers</returns>
        public List<ApplicationUser> GetAdvisersByDeanUserId(int organizationId, string deanUserId) 
        {
            var adviserIds = _db.DeanAdvisers.Where(x => x.OrganizationId == organizationId &&
                x.DeanUserId == deanUserId)
                .Select(x => x.AdviserUserId)
                .ToList();

            return _userManager.Users.Where(x => adviserIds.Contains(x.Id)).ToList();
        }
    }
}
