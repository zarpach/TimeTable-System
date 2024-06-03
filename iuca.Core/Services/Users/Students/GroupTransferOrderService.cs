using AutoMapper;
using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.Students;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Users.Students
{
    public class GroupTransferOrderService : IGroupTransferOrderService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IStudentInfoService _studentInfoService;
        private readonly IAdviserStudentService _adviserStudentService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public GroupTransferOrderService(IApplicationDbContext db,
            IMapper mapper,
            IStudentInfoService studentInfoService,
            IAdviserStudentService adviserStudentService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _studentInfoService = studentInfoService;
            _adviserStudentService = adviserStudentService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get group transfer order list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="isApplied">Aplication status</param>
        /// <returns>Group transfer order list</returns>
        public IEnumerable<GroupTransferOrderDTO> GetGroupTransferOrders(int organizationId, int isApplied)
        {
            IEnumerable<GroupTransferOrder> groupTransferOrders = _db.GroupTransferOrders
                .Include(x => x.TargetGroup)
                .ThenInclude(x => x.Department)
                .Include(x => x.SourceGroup)
                .ThenInclude(x => x.Department)
                .Include(x => x.Semester)
                .Where(x => x.OrganizationId == organizationId);

            if (isApplied != 0)
            {
                bool isAppliedValue = isApplied == (int)enu_Status.True;
                groupTransferOrders = groupTransferOrders.Where(x => x.IsApplied == isAppliedValue);
            }

            IEnumerable<GroupTransferOrderDTO> groupTransferOrdersDTOs = _mapper.Map<IEnumerable<GroupTransferOrderDTO>>(groupTransferOrders);

            foreach (var groupTransferOrderDTO in groupTransferOrdersDTOs)
            {
                groupTransferOrderDTO.StudentInfo = _studentInfoService.GetStudentMinimumInfo(organizationId, groupTransferOrderDTO.StudentUserId);
                groupTransferOrderDTO.FutureAdvisors = groupTransferOrderDTO.FutureAdvisorsJson
                    .Select(x => new UserDTO
                    {
                        Id = x,
                        FullName = _userManager.GetUserFullName(x)
                    });
                groupTransferOrderDTO.PreviousAdvisors = groupTransferOrderDTO.PreviousAdvisorsJson
                    .Select(x => new UserDTO
                    {
                        Id = x,
                        FullName = _userManager.GetUserFullName(x)
                    });
            }

            return groupTransferOrdersDTOs;
        }

        /// <summary>
        /// Get group transfer order
        /// </summary>
        /// <returns>Group transfer order</returns>
        public GroupTransferOrderDTO GetGroupTransferOrder(int groupTransferOrderId)
        {
            if (groupTransferOrderId == 0)
                throw new ArgumentException("The group transfer order id is 0.", nameof(groupTransferOrderId));

            GroupTransferOrder groupTransferOrder = _db.GroupTransferOrders.Find(groupTransferOrderId);
            if (groupTransferOrder == null)
                throw new InvalidOperationException($"The group transfer order with id {groupTransferOrderId} does not exist.");

            GroupTransferOrderDTO groupTransferOrderDTO = _mapper.Map<GroupTransferOrderDTO>(groupTransferOrder);

            return groupTransferOrderDTO;
        }

        /// <summary>
        /// Get older order
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="groupTransferOrder">Group transfer order</param>
        private void OlderOrderExist(int organizationId, GroupTransferOrder groupTransferOrder)
        {
            if (groupTransferOrder == null)
                throw new ArgumentNullException(nameof(groupTransferOrder), "The group transfer order is null.");

            GroupTransferOrder olderGroupTransferOrder = _db.GroupTransferOrders
                .FirstOrDefault(x => x.StudentUserId == groupTransferOrder.StudentUserId &&
                x.OrganizationId == organizationId && x.IsApplied == true && x.Date > groupTransferOrder.Date);

            if (olderGroupTransferOrder != null)
                throw new ModelValidationException($"There is a newer applied order (number {olderGroupTransferOrder.Number}).", "");
        }

        /// <summary>
        /// Set order application status
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="groupTransferOrderId">Group transfer order id</param>
        /// <param name="applicationStatus">New application status</param>
        /// <returns>Accompanying message</returns>
        public string SetOrderApplicationStatus(int organizationId, int groupTransferOrderId, bool applicationStatus)
        {
            string message = "";

            if (groupTransferOrderId == 0)
                throw new ArgumentException("The group transfer order id is 0.", nameof(groupTransferOrderId));

            GroupTransferOrder groupTransferOrder = _db.GroupTransferOrders.Find(groupTransferOrderId);
            if (groupTransferOrder == null)
                throw new InvalidOperationException($"The group transfer order with id {groupTransferOrderId} does not exist.");

            OlderOrderExist(organizationId, groupTransferOrder);

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if (applicationStatus == true)
                    {
                        _studentInfoService.SetStudentDepartmentGroup(organizationId, groupTransferOrder.StudentUserId, groupTransferOrder.TargetGroupId, groupTransferOrder.SourceGroupId);
                        message = _adviserStudentService.SetStudentAdvisers(organizationId, groupTransferOrder.StudentUserId, groupTransferOrder.FutureAdvisorsJson);
                    }else
                    {
                        _studentInfoService.SetStudentDepartmentGroup(organizationId, groupTransferOrder.StudentUserId, groupTransferOrder.SourceGroupId, groupTransferOrder.TargetGroupId);
                        message = _adviserStudentService.SetStudentAdvisers(organizationId, groupTransferOrder.StudentUserId, groupTransferOrder.PreviousAdvisorsJson);
                    }

                    groupTransferOrder.IsApplied = applicationStatus;
                    _db.SaveChanges();

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

            return message;
        }

        /// <summary>
        /// Create group transfer order
        /// </summary>
        /// <param name="groupTransferOrderDTO">Group transfer order</param>
        public int CreateGroupTransferOrder(GroupTransferOrderDTO groupTransferOrderDTO)
        {
            if (groupTransferOrderDTO == null)
                throw new ArgumentNullException(nameof(groupTransferOrderDTO), "The group transfer order is null.");

            groupTransferOrderDTO.SourceGroupId = _studentInfoService.GetStudentDepartmentGroup(groupTransferOrderDTO.OrganizationId, groupTransferOrderDTO.StudentUserId).Id;
            groupTransferOrderDTO.PreviousAdvisorsJson = _adviserStudentService.GetStudentAdvisers(groupTransferOrderDTO.OrganizationId, groupTransferOrderDTO.StudentUserId)
                .Select(x => x.Id);

            var newGroupTransferOrder = _mapper.Map<GroupTransferOrder>(groupTransferOrderDTO);

            _db.GroupTransferOrders.Add(newGroupTransferOrder);
            _db.SaveChanges();

            return newGroupTransferOrder.Id;
        }

        /// <summary>
        /// Edit group transfer order by id
        /// </summary>
        /// <param name="groupTransferOrderId">Group transfer order id</param>
        /// <param name="groupTransferOrderDTO">Group transfer order</param>
        public void EditGroupTransferOrder(int groupTransferOrderId, GroupTransferOrderDTO groupTransferOrderDTO)
        {
            if (groupTransferOrderId == 0)
                throw new ArgumentException("The group transfer order id is 0.", nameof(groupTransferOrderId));

            if (groupTransferOrderDTO == null)
                throw new ArgumentNullException(nameof(groupTransferOrderDTO), "The group transfer order is null.");

            GroupTransferOrder groupTransferOrder = _db.GroupTransferOrders.Find(groupTransferOrderId);
            if (groupTransferOrder == null)
                throw new InvalidOperationException($"The group transfer order with id {groupTransferOrderId} does not exist.");

            if (groupTransferOrder.IsApplied)
                throw new InvalidOperationException("An applied order cannot be edited.");

            var sourseGroupId = _studentInfoService.GetStudentDepartmentGroup(groupTransferOrderDTO.OrganizationId, groupTransferOrderDTO.StudentUserId).Id;
            var previousAdvisorsIds = _adviserStudentService.GetStudentAdvisers(groupTransferOrderDTO.OrganizationId, groupTransferOrderDTO.StudentUserId)
                .Select(x => x.Id);

            groupTransferOrder.StudentUserId = groupTransferOrderDTO.StudentUserId;
            groupTransferOrder.Number = groupTransferOrderDTO.Number;
            groupTransferOrder.Date = groupTransferOrderDTO.Date;
            groupTransferOrder.Comment = groupTransferOrderDTO.Comment;
            groupTransferOrder.TargetGroupId = groupTransferOrderDTO.TargetGroupId;
            groupTransferOrder.FutureAdvisorsJson = groupTransferOrderDTO.FutureAdvisorsJson;
            groupTransferOrder.SourceGroupId = sourseGroupId;
            groupTransferOrder.PreviousAdvisorsJson = previousAdvisorsIds;

            _db.SaveChanges();
        }

        /// <summary>
        /// Delete group transfer order by id
        /// </summary>
        /// <param name="groupTransferOrderId">Group transfer order id</param>
        public void DeleteGroupTransferOrder(int groupTransferOrderId)
        {
            if (groupTransferOrderId == 0)
                throw new ArgumentException("The group transfer order id is 0.", nameof(groupTransferOrderId));

            GroupTransferOrder groupTransferOrder = _db.GroupTransferOrders.Find(groupTransferOrderId);

            if (groupTransferOrder == null)
                throw new InvalidOperationException($"The group transfer order with id {groupTransferOrderId} does not exist.");

            if (groupTransferOrder.IsApplied)
                throw new InvalidOperationException("An applied order cannot be deleted.");

            _db.GroupTransferOrders.Remove(groupTransferOrder);
            _db.SaveChanges();
        }
    }
}
