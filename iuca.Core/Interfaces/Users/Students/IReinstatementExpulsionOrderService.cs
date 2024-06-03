using iuca.Application.DTO.Users.Students;
using iuca.Application.ViewModels.Users.Students;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Users.Students
{
    public interface IReinstatementExpulsionOrderService
    {
        /// <summary>
        /// Get reinstatement/expulsion order list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="type">Type</param>
        /// <param name="isApplied">Aplication status</param>
        /// <returns>Reinstatement/Expulsion order list</returns>
        IEnumerable<OrdersViewModel> GetReinstatementExpulsionOrders(int organizationId, int type, int isApplied);

        /// <summary>
        /// Get reinstatement/expulsion order
        /// </summary>
        /// <param name="reinstatementExpulsionOrderId">Reinstatement/Expulsion order id</param>
        /// <returns>Reinstatement/Expulsion order</returns>
        ReinstatementExpulsionOrderDTO GetReinstatementExpulsionOrder(int reinstatementExpulsionOrderId);

        /// <summary>
        /// Set order application status
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="reinstatementExpulsionOrderId">Reinstatement/Expulsion order id</param>
        /// <param name="applicationStatus">New application status</param>
        void SetOrderApplicationStatus(int organizationId, int reinstatementExpulsionOrderId, bool applicationStatus);

        /// <summary>
        /// Create reinstatement/expulsion order
        /// </summary>
        /// <param name="reinstatementExpulsionOrderDTO">Reinstatement/Expulsion order</param>
        int CreateReinstatementExpulsionOrder(ReinstatementExpulsionOrderDTO reinstatementExpulsionOrderDTO);

        /// <summary>
        /// Edit reinstatement/expulsion order by id
        /// </summary>
        /// <param name="reinstatementExpulsionOrderId">Reinstatement/Expulsion order id</param>
        /// <param name="reinstatementExpulsionOrderDTO">Reinstatement/Expulsion order</param>
        void EditReinstatementExpulsionOrder(int reinstatementExpulsionOrderId, ReinstatementExpulsionOrderDTO reinstatementExpulsionOrderDTO);

        /// <summary>
        /// Delete reinstatement/expulsion order by id
        /// </summary>
        /// <param name="reinstatementExpulsionOrderId">Reinstatement/Expulsion order id</param>
        void DeleteReinstatementExpulsionOrder(int reinstatementExpulsionOrderId);
    }
}
