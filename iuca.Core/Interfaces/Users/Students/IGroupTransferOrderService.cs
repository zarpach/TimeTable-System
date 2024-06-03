using iuca.Application.DTO.Users.Students;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Users.Students
{
    public interface IGroupTransferOrderService
    {
        /// <summary>
        /// Get group transfer order list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="isApplied">Aplication status</param>
        /// <returns>Group transfer order list</returns>
        IEnumerable<GroupTransferOrderDTO> GetGroupTransferOrders(int organizationId, int isApplied);

        /// <summary>
        /// Get group transfer order
        /// </summary>
        /// <returns>Group transfer order</returns>
        GroupTransferOrderDTO GetGroupTransferOrder(int groupTransferOrderId);

        /// <summary>
        /// Set order application status
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="groupTransferOrderId">Group transfer order id</param>
        /// <param name="applicationStatus">New application status</param>
        /// <returns>Accompanying message</returns>
        string SetOrderApplicationStatus(int organizationId, int groupTransferOrderId, bool applicationStatus);

        /// <summary>
        /// Create group transfer order
        /// </summary>
        /// <param name="groupTransferOrderDTO">Group transfer order</param>
        int CreateGroupTransferOrder(GroupTransferOrderDTO groupTransferOrderDTO);

        /// <summary>
        /// Edit group transfer order by id
        /// </summary>
        /// <param name="groupTransferOrderId">Group transfer order id</param>
        /// <param name="groupTransferOrderDTO">Group transfer order</param>
        void EditGroupTransferOrder(int groupTransferOrderId, GroupTransferOrderDTO groupTransferOrderDTO);

        /// <summary>
        /// Delete group transfer order by id
        /// </summary>
        /// <param name="groupTransferOrderId">Group transfer order id</param>
        void DeleteGroupTransferOrder(int groupTransferOrderId);
    }
}
