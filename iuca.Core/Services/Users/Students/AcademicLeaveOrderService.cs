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
    public class AcademicLeaveOrderService : IAcademicLeaveOrderService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IStudentInfoService _studentInfoService;

        public AcademicLeaveOrderService(IApplicationDbContext db,
            IMapper mapper,
            IStudentInfoService studentInfoService)
        {
            _db = db;
            _mapper = mapper;
            _studentInfoService = studentInfoService;
        }

        /// <summary>
        /// Get academic leave order list
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="isApplied">Aplication status</param>
        /// <returns>Academic leave order list</returns>
        public IEnumerable<OrdersViewModel> GetAcademicLeaveOrders(int organizationId, int isApplied)
        {
            IEnumerable<AcademicLeaveOrder> academicLeaveOrders = _db.AcademicLeaveOrder
                .Where(x => x.OrganizationId == organizationId);
            
            if (isApplied != 0)
            {
                if (isApplied == (int)enu_Status.True)
                    academicLeaveOrders = academicLeaveOrders.Where(x => x.IsApplied == true);
                if (isApplied == (int)enu_Status.False)
                    academicLeaveOrders = academicLeaveOrders.Where(x => x.IsApplied == false);
            }

            IEnumerable<OrdersViewModel> academicLeaveOrdersDTOs = _mapper.Map<IEnumerable<OrdersViewModel>>(academicLeaveOrders);

            foreach (var academicLeaveOrderDTO in academicLeaveOrdersDTOs)
                academicLeaveOrderDTO.StudentInfo = _studentInfoService.GetStudentMinimumInfo(organizationId, academicLeaveOrderDTO.StudentUserId);

            return academicLeaveOrdersDTOs;
        }

        /// <summary>
        /// Get academic leave order
        /// </summary>
        /// <returns>Academic leave order</returns>
        public AcademicLeaveOrderDTO GetAcademicLeaveOrder(int academicLeaveOrderId)
        {
            if (academicLeaveOrderId == 0)
                throw new Exception($"The academic leave order id is 0.");

            AcademicLeaveOrder academicLeaveOrder = _db.AcademicLeaveOrder
                .Find(academicLeaveOrderId);
            if (academicLeaveOrder == null)
                throw new Exception($"The academic leave order with id {academicLeaveOrderId} does not exist.");

            AcademicLeaveOrderDTO academicLeaveOrderDTO = _mapper.Map<AcademicLeaveOrderDTO>(academicLeaveOrder);

            return academicLeaveOrderDTO;
        }

        /// <summary>
        /// Get older order
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="academicLeaveOrder">Academic leave order</param>
        private void OlderOrderExist(int organizationId, AcademicLeaveOrder academicLeaveOrder)
        {
            if (academicLeaveOrder == null)
                throw new Exception($"The academic leave order is null.");

            AcademicLeaveOrder olderAcademicLeaveOrder = _db.AcademicLeaveOrder
                .FirstOrDefault(x => x.StudentUserId == academicLeaveOrder.StudentUserId &&
                x.OrganizationId == organizationId && x.IsApplied == true && x.Date > academicLeaveOrder.Date);

            if (olderAcademicLeaveOrder != null)
                throw new ModelValidationException($"There is a newer applied order (number {olderAcademicLeaveOrder.Number}).", "");

            ReinstatementExpulsionOrder olderReinstatementExpulsionOrder = _db.ReinstatementExpulsionOrders
                .FirstOrDefault(x => x.StudentUserId == academicLeaveOrder.StudentUserId &&
                x.OrganizationId == organizationId && x.IsApplied == true && x.Date > academicLeaveOrder.Date);

            if (olderReinstatementExpulsionOrder != null)
                throw new ModelValidationException($"There is a newer applied order (number {olderReinstatementExpulsionOrder.Number}).", "");
        }

        /// <summary>
        /// Set order application status
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="academicLeaveOrderId">Academic leave order id</param>
        /// <param name="applicationStatus">New application status</param>
        public void SetOrderApplicationStatus(int organizationId, int academicLeaveOrderId, bool applicationStatus)
        {
            if (academicLeaveOrderId == 0)
                throw new Exception($"The academic leave order id is 0.");

            AcademicLeaveOrder academicLeaveOrder = _db.AcademicLeaveOrder
                .Find(academicLeaveOrderId);

            OlderOrderExist(organizationId, academicLeaveOrder);

            int newStudentState = 0;

            if (applicationStatus == true)
                newStudentState = (int)enu_StudentState.AcadLeave;
            else
                newStudentState = (int)enu_StudentState.Active;

            if (newStudentState != 0)
            {
                _studentInfoService.SetStudentState(organizationId, academicLeaveOrder.StudentUserId, newStudentState);

                academicLeaveOrder.IsApplied = applicationStatus;
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Check dates
        /// </summary>
        /// <param name="academicLeaveOrderDTO">Academic leave order</param>
        private void CheckDates(AcademicLeaveOrderDTO academicLeaveOrderDTO)
        {
            if (academicLeaveOrderDTO == null)
                throw new Exception($"The academic leave order is null.");

            if (academicLeaveOrderDTO.Date > academicLeaveOrderDTO.Start)
                throw new ModelValidationException($"The start date of academic leave cannot be earlier than the date of the order.", "");

            if (academicLeaveOrderDTO.Date > academicLeaveOrderDTO.End)
                throw new ModelValidationException($"The end date of academic leave cannot be earlier than the date of the order.", "");

            if (academicLeaveOrderDTO.Start > academicLeaveOrderDTO.End)
                throw new ModelValidationException($"The end date of academic leave cannot be earlier than the start date of academic leave.", "");
        }

        /// <summary>
        /// Create academic leave order
        /// </summary>
        /// <param name="academicLeaveOrderDTO">Academic leave order</param>
        public int CreateAcademicLeaveOrder(AcademicLeaveOrderDTO academicLeaveOrderDTO)
        {
            if (academicLeaveOrderDTO == null)
                throw new Exception($"The academic leave order is null.");

            CheckDates(academicLeaveOrderDTO);

            AcademicLeaveOrder newAcademicLeaveOrder = _mapper.Map<AcademicLeaveOrder>(academicLeaveOrderDTO);

            _db.AcademicLeaveOrder.Add(newAcademicLeaveOrder);
            _db.SaveChanges();
            return newAcademicLeaveOrder.Id;
        }

        /// <summary>
        /// Edit academic leave order by id
        /// </summary>
        /// <param name="academicLeaveOrderId">Academic leave order id</param>
        /// <param name="academicLeaveOrderDTO">Academic leave order</param>
        public void EditAcademicLeaveOrder(int academicLeaveOrderId, AcademicLeaveOrderDTO academicLeaveOrderDTO)
        {
            if (academicLeaveOrderId == 0)
                throw new Exception($"The academic leave order id is 0.");
            if (academicLeaveOrderDTO == null)
                throw new Exception($"The academic leave order is null.");

            CheckDates(academicLeaveOrderDTO);

            AcademicLeaveOrder academicLeaveOrder = _db.AcademicLeaveOrder.Find(academicLeaveOrderId);
            if (academicLeaveOrder == null)
                throw new Exception($"The academic leave order with id {academicLeaveOrderId} does not exist.");

            if (academicLeaveOrder.IsApplied == true)
                throw new Exception($"An applied order cannot be edited.");

            academicLeaveOrder.StudentUserId = academicLeaveOrderDTO.StudentUserId;
            academicLeaveOrder.Number = academicLeaveOrderDTO.Number;
            academicLeaveOrder.Date = academicLeaveOrderDTO.Date;
            academicLeaveOrder.Start = academicLeaveOrderDTO.Start;
            academicLeaveOrder.End = academicLeaveOrderDTO.End;
            academicLeaveOrder.Reason = academicLeaveOrderDTO.Reason;
            academicLeaveOrder.Comment = academicLeaveOrderDTO.Comment;

            _db.SaveChanges();
        }

        /// <summary>
        /// Delete academic leave order by id
        /// </summary>
        /// <param name="academicLeaveOrderId">Academic leave order id</param>
        public void DeleteAcademicLeaveOrder(int academicLeaveOrderId)
        {
            if (academicLeaveOrderId == 0)
                throw new Exception($"The academic leave order id is 0.");

            AcademicLeaveOrder academicLeaveOrder = _db.AcademicLeaveOrder.Find(academicLeaveOrderId);
            if (academicLeaveOrder == null)
                throw new Exception($"The academic leave order with id {academicLeaveOrderId} does not exist.");

            if (academicLeaveOrder.IsApplied == true)
                throw new Exception($"An applied order cannot be deleted.");

            _db.AcademicLeaveOrder.Remove(academicLeaveOrder);
            _db.SaveChanges();
        }
    }
}
