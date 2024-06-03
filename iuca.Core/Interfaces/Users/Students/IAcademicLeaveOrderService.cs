using iuca.Application.DTO.Users.Students;
using iuca.Application.ViewModels.Users.Students;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Users.Students
{
    public interface IAcademicLeaveOrderService
    {
        /// <summary>
        /// Get academic leave order list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="isApplied">Aplication status</param>
        /// <returns>Academic leave order list</returns>
        IEnumerable<OrdersViewModel> GetAcademicLeaveOrders(int organizationId, int isApplied);

        /// <summary>
        /// Get academic leave order
        /// </summary>
        /// <returns>Academic leave order</returns>
        AcademicLeaveOrderDTO GetAcademicLeaveOrder(int academicLeaveOrderId);

        /// <summary>
        /// Set order application status
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="academicLeaveOrderId">Academic leave order id</param>
        /// <param name="applicationStatus">New application status</param>
        void SetOrderApplicationStatus(int organizationId, int academicLeaveOrderId, bool applicationStatus);

        /// <summary>
        /// Create academic leave order
        /// </summary>
        /// <param name="academicLeaveOrderDTO">Academic leave order</param>
        int CreateAcademicLeaveOrder(AcademicLeaveOrderDTO academicLeaveOrderDTO);

        /// <summary>
        /// Edit academic leave order by id
        /// </summary>
        /// <param name="academicLeaveOrderId">Academic leave order id</param>
        /// <param name="academicLeaveOrderDTO">Academic leave order</param>
        void EditAcademicLeaveOrder(int academicLeaveOrderId, AcademicLeaveOrderDTO academicLeaveOrderDTO);

        /// <summary>
        /// Delete academic leave order by id
        /// </summary>
        /// <param name="academicLeaveOrderId">Academic leave order id</param>
        void DeleteAcademicLeaveOrder(int academicLeaveOrderId);
    }
}
