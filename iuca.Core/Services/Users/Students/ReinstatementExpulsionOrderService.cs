using AutoMapper;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Users.Students;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Users.Students
{
    public class ReinstatementExpulsionOrderService : IReinstatementExpulsionOrderService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IStudentInfoService _studentInfoService;
        private readonly IAcademicLeaveOrderService _academicLeaveOrderService;

        public ReinstatementExpulsionOrderService(IApplicationDbContext db,
            IMapper mapper,
            IStudentInfoService studentInfoService,
            IAcademicLeaveOrderService academicLeaveOrderService)
        {
            _db = db;
            _mapper = mapper;
            _studentInfoService = studentInfoService;
            _academicLeaveOrderService = academicLeaveOrderService;
        }

        /// <summary>
        /// Get reinstatement/expulsion order list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="type">Type</param>
        /// <param name="isApplied">Aplication status</param>
        /// <returns>Reinstatement/Expulsion order list</returns>
        public IEnumerable<OrdersViewModel> GetReinstatementExpulsionOrders(int organizationId, int type, int isApplied)
        {
            IEnumerable<ReinstatementExpulsionOrder> reinstatementExpulsionOrders = _db.ReinstatementExpulsionOrders
                .Where(x => x.OrganizationId == organizationId);

            if (type != 0)
                reinstatementExpulsionOrders = reinstatementExpulsionOrders.Where(x => x.Type == type);

            if (isApplied != 0)
            {
                if (isApplied == (int)enu_Status.True)
                    reinstatementExpulsionOrders = reinstatementExpulsionOrders.Where(x => x.IsApplied == true);
                if (isApplied == (int)enu_Status.False)
                    reinstatementExpulsionOrders = reinstatementExpulsionOrders.Where(x => x.IsApplied == false);
            }
                
            IEnumerable<OrdersViewModel> reinstatementExpulsionOrdersDTOs = _mapper.Map<IEnumerable<OrdersViewModel>>(reinstatementExpulsionOrders);

            foreach (var reinstatementExpulsionOrderDTO in reinstatementExpulsionOrdersDTOs)
                reinstatementExpulsionOrderDTO.StudentInfo = _studentInfoService.GetStudentMinimumInfo(organizationId, reinstatementExpulsionOrderDTO.StudentUserId);

            return reinstatementExpulsionOrdersDTOs;
        }

        /// <summary>
        /// Get reinstatement/expulsion order
        /// </summary>
        /// <param name="reinstatementExpulsionOrderId">Reinstatement/Expulsion order id</param>
        /// <returns>Reinstatement/Expulsion order</returns>
        public ReinstatementExpulsionOrderDTO GetReinstatementExpulsionOrder(int reinstatementExpulsionOrderId)
        {
            if (reinstatementExpulsionOrderId == 0)
                throw new Exception($"The reinstatement/expulsion order id is 0.");

            ReinstatementExpulsionOrder reinstatementExpulsionOrder = _db.ReinstatementExpulsionOrders
                .Find(reinstatementExpulsionOrderId);
            if (reinstatementExpulsionOrder == null)
                throw new Exception($"The reinstatement/expulsion order with id {reinstatementExpulsionOrderId} does not exist.");

            ReinstatementExpulsionOrderDTO reinstatementExpulsionOrderDTO = _mapper.Map<ReinstatementExpulsionOrderDTO>(reinstatementExpulsionOrder);

            return reinstatementExpulsionOrderDTO;
        }

        /// <summary>
        /// Get older order
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="reinstatementExpulsionOrder">Reinstatement/Expulsion order</param>
        /// <returns>Number of the older order</returns>
        private void OlderOrderExist(int organizationId, ReinstatementExpulsionOrder reinstatementExpulsionOrder)
        {
            if (reinstatementExpulsionOrder == null)
                throw new Exception($"The reinstatement/expulsion order is null.");

            ReinstatementExpulsionOrder olderReinstatementExpulsionOrder = _db.ReinstatementExpulsionOrders
                .FirstOrDefault(x => x.StudentUserId == reinstatementExpulsionOrder.StudentUserId &&
                x.OrganizationId == organizationId && x.IsApplied == true && x.Date > reinstatementExpulsionOrder.Date);

            if (olderReinstatementExpulsionOrder != null)
                throw new ModelValidationException($"There is a newer applied order (number {olderReinstatementExpulsionOrder.Number}).", "");

            AcademicLeaveOrder olderAcademicLeaveOrder = _db.AcademicLeaveOrder
                .FirstOrDefault(x => x.StudentUserId == reinstatementExpulsionOrder.StudentUserId &&
                x.OrganizationId == organizationId && x.IsApplied == true && x.Date > reinstatementExpulsionOrder.Date);

            if (olderAcademicLeaveOrder != null)
                throw new ModelValidationException($"There is a newer applied order (number {olderAcademicLeaveOrder.Number}).", "");
        }

        /// <summary>
        /// Set order application status
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="reinstatementExpulsionOrderId">Reinstatement/Expulsion order id</param>
        /// <param name="applicationStatus">New application status</param>
        public void SetOrderApplicationStatus(int organizationId, int reinstatementExpulsionOrderId, bool applicationStatus)
        {
            if (reinstatementExpulsionOrderId == 0)
                throw new Exception($"The reinstatement/expulsion order id is 0.");

            ReinstatementExpulsionOrder reinstatementExpulsionOrder = _db.ReinstatementExpulsionOrders
                .Find(reinstatementExpulsionOrderId);

            OlderOrderExist(organizationId, reinstatementExpulsionOrder);

            int newStudentState = 0;

            if (reinstatementExpulsionOrder.Type == (int)enu_OrderType.Reinstatement)
            {
                if (applicationStatus == true)
                    newStudentState = (int)enu_StudentState.Active;
                else
                    newStudentState = (int)enu_StudentState.Dismissed;
            }
            else if (reinstatementExpulsionOrder.Type == (int)enu_OrderType.Expulsion)
            {
                if (applicationStatus == true)
                    newStudentState = (int)enu_StudentState.Dismissed;
                else
                    newStudentState = (int)enu_StudentState.Active;
            }

            if (newStudentState != 0)
            {
                _studentInfoService.SetStudentState(organizationId, reinstatementExpulsionOrder.StudentUserId, newStudentState);
                
                reinstatementExpulsionOrder.IsApplied = applicationStatus;
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Create reinstatement/expulsion order
        /// </summary>
        /// <param name="reinstatementExpulsionOrderDTO">Reinstatement/Expulsion order</param>
        public int CreateReinstatementExpulsionOrder(ReinstatementExpulsionOrderDTO reinstatementExpulsionOrderDTO)
        {
            if (reinstatementExpulsionOrderDTO == null)
                throw new Exception($"The reinstatement/expulsion order is null.");

            ReinstatementExpulsionOrder newReinstatementExpulsionOrder = _mapper.Map<ReinstatementExpulsionOrder>(reinstatementExpulsionOrderDTO);

            _db.ReinstatementExpulsionOrders.Add(newReinstatementExpulsionOrder);
            _db.SaveChanges();
            return newReinstatementExpulsionOrder.Id;
        }

        /// <summary>
        /// Edit reinstatement/expulsion order by id
        /// </summary>
        /// <param name="reinstatementExpulsionOrderId">Reinstatement/Expulsion order id</param>
        /// <param name="reinstatementExpulsionOrderDTO">Reinstatement/Expulsion order</param>
        public void EditReinstatementExpulsionOrder(int reinstatementExpulsionOrderId, ReinstatementExpulsionOrderDTO reinstatementExpulsionOrderDTO)
        {
            if (reinstatementExpulsionOrderId == 0)
                throw new Exception($"The reinstatement/expulsion order id is 0.");
            if (reinstatementExpulsionOrderDTO == null)
                throw new Exception($"The reinstatement/expulsion order is null.");

            ReinstatementExpulsionOrder reinstatementExpulsionOrder = _db.ReinstatementExpulsionOrders.Find(reinstatementExpulsionOrderId);
            if (reinstatementExpulsionOrder == null)
                throw new Exception($"The reinstatement/expulsion order with id {reinstatementExpulsionOrderId} does not exist.");

            if (reinstatementExpulsionOrder.IsApplied == true)
                throw new Exception($"An applied order cannot be edited.");

            reinstatementExpulsionOrder.StudentUserId = reinstatementExpulsionOrderDTO.StudentUserId;
            reinstatementExpulsionOrder.Number = reinstatementExpulsionOrderDTO.Number;
            reinstatementExpulsionOrder.Date = reinstatementExpulsionOrderDTO.Date;
            reinstatementExpulsionOrder.Reason = reinstatementExpulsionOrderDTO.Reason;
            reinstatementExpulsionOrder.Comment = reinstatementExpulsionOrderDTO.Comment;

            _db.SaveChanges();
        }

        /// <summary>
        /// Delete reinstatement/expulsion order by id
        /// </summary>
        /// <param name="reinstatementExpulsionOrderId">Reinstatement/Expulsion order id</param>
        public void DeleteReinstatementExpulsionOrder(int reinstatementExpulsionOrderId)
        {
            if (reinstatementExpulsionOrderId == 0)
                throw new Exception($"The reinstatement/expulsion order id is 0.");

            ReinstatementExpulsionOrder reinstatementExpulsionOrder = _db.ReinstatementExpulsionOrders.Find(reinstatementExpulsionOrderId);
            if (reinstatementExpulsionOrder == null)
                throw new Exception($"The reinstatement/expulsion order with id {reinstatementExpulsionOrderId} does not exist.");

            if (reinstatementExpulsionOrder.IsApplied == true)
                throw new Exception($"An applied order cannot be deleted.");

            _db.ReinstatementExpulsionOrders.Remove(reinstatementExpulsionOrder);
            _db.SaveChanges();
        }
    }
}
