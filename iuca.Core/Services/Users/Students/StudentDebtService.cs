using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Users.Students;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.Users.Students
{
    public class StudentDebtService : IStudentDebtService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public StudentDebtService(IApplicationDbContext db,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Get student list with their debts by department group id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="debtType">Debt type</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentGroupId">Department group id</param>
        /// <param name="lastName">Student last name</param>
        /// <param name="firstName">Student first name</param>
        /// <param name="studentId">StudentId</param>
        /// <param name="debtorType">Debtor type</param>
        /// <param name="activeOnly">Only active students</param>
        /// <param name="noRegistrations">Students that didn't submit registration on the semester</param>
        /// <returns>Student list with their debts</returns>
        public List<StudentDebtViewModel> GetStudentDebtList(int organizationId, enu_DebtType debtType, int semesterId, 
            int departmentGroupId,string lastName, string firstName, int studentId, enu_DebtorType debtorType,
            bool activeOnly, bool noRegistrations)
        {
            List<StudentDebtViewModel> studentDebtList = new List<StudentDebtViewModel>();

            var semesterMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Semester, SemesterDTO>();
            }).CreateMapper();

            var studentIds = _db.UserTypeOrganizations.Where(x => x.OrganizationId == organizationId &&
                x.UserType == (int)enu_UserType.Student).Select(x => x.ApplicationUserId).ToList();

            var studentList = _userManager.Users
                .Include(x => x.UserBasicInfo)
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .ThenInclude(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .Where(x => studentIds.Contains(x.Id));

            if (departmentGroupId != 0)
                studentList = studentList.Where(x => x.StudentBasicInfo.StudentOrgInfo
                                                .Any(y => y.DepartmentGroupId == departmentGroupId &&
                                                          y.OrganizationId == organizationId));

            if (!string.IsNullOrEmpty(lastName))
                studentList = studentList.Where(x => x.LastNameEng.Contains(lastName) ||
                    x.UserBasicInfo.LastNameRus.Contains(lastName));

            if (!string.IsNullOrEmpty(firstName))
                studentList = studentList.Where(x => x.FirstNameEng.Contains(firstName) ||
                     x.UserBasicInfo.FirstNameRus.Contains(firstName));

            if (studentId != 0)
                studentList = studentList.Where(x => x.StudentBasicInfo.StudentOrgInfo.Any(y => y.StudentId == studentId));

            if (activeOnly)
                studentList = studentList.Where(x => x.StudentBasicInfo.StudentOrgInfo
                    .Any(y => y.State == (int)enu_StudentState.Active || y.State == (int)enu_StudentState.AcadLeave));

            if (noRegistrations)
            {
                var submittedStudentIds = _db.StudentCourseRegistrations.Where(x => x.SemesterId == semesterId 
                    && (x.State == (int)enu_RegistrationState.Submitted && x.AddDropState == (int)enu_RegistrationState.NotSent
                    || x.AddDropState == (int)enu_RegistrationState.Submitted))
                    .Select(x => x.StudentUserId).ToList();

                studentList = studentList.Where(x => !submittedStudentIds.Contains(x.Id));
            }

            var studentUserIds = studentList.Select(x => x.Id).ToList();

            var studentDebts = _db.StudentDebts
                .Include(x => x.Semester)
                .Where(x => x.SemesterId == semesterId && studentUserIds.Contains(x.StudentUserId)
                    && x.DebtType == (int)debtType).ToList();

            
            foreach (var student in studentList)
            {
                var studentDebt = studentDebts.FirstOrDefault(x => x.StudentUserId == student.Id);

                if (studentDebt == null && debtorType == enu_DebtorType.NotDebtor)
                    continue;

                var studentOrgInfo = student.StudentBasicInfo.StudentOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId);

                StudentDebtViewModel model = new StudentDebtViewModel();
                model.StudentUserId = student.Id;
                model.StudentName = student.FullNameEng;
                model.StudentId = studentOrgInfo.StudentId;
                model.StudentMajor = studentOrgInfo.DepartmentGroup.Department.Code;
                model.StudentGroup = studentOrgInfo.DepartmentGroup.Code;
                model.DebtType = (int)debtType;
                model.SemesterId = semesterId;

                if (studentDebt != null)
                {
                    if (debtorType == enu_DebtorType.NotDebtor && studentDebt.IsDebt)
                        continue;

                    if (debtorType == enu_DebtorType.Debtor && !studentDebt.IsDebt)
                        continue;

                    model.DebtId = studentDebt.Id;
                    model.IsDebt = studentDebt.IsDebt;
                    model.Comment = studentDebt.Comment;
                    model.DebtAmount = studentDebt.DebtAmount;
                }
                studentDebtList.Add(model);
            }

            return studentDebtList;
        }

        /// <summary>
        /// Set student debt
        /// </summary>
        /// <param name="debtList">Debt list</param>
        public void SetStudentDebts(List<StudentDebtViewModel> debtList) 
        {
            foreach (StudentDebtViewModel debt in debtList) 
            {
                var studentDebt = _db.StudentDebts.FirstOrDefault(x => x.DebtType == debt.DebtType && 
                        x.SemesterId == debt.SemesterId && x.StudentUserId == debt.StudentUserId);
                
                if (studentDebt == null)
                {
                    studentDebt = new StudentDebt();
                    studentDebt.DebtType = debt.DebtType;
                    studentDebt.SemesterId = debt.SemesterId;
                    studentDebt.StudentUserId = debt.StudentUserId;
                    studentDebt.IsDebt = debt.IsDebt;
                    studentDebt.Comment = debt.Comment;
                    studentDebt.DebtAmount = debt.DebtAmount;
                    _db.StudentDebts.Add(studentDebt);
                }
                else
                {
                    studentDebt.IsDebt = debt.IsDebt;
                    studentDebt.Comment = debt.Comment;
                    studentDebt.DebtAmount = debt.DebtAmount;
                    _db.StudentDebts.Update(studentDebt);
                }
            }
            
            _db.SaveChanges();
        }

        /// <summary>
        /// Get student debt info list
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>List of student debts</returns>
        public List<DebtInfoViewModel> GetStudentDebtInfo(int semesterId, string studentUserId) 
        {
            List<DebtInfoViewModel> debtList = new List<DebtInfoViewModel>();
            var debtInfo = _db.StudentDebts.Where(x => x.SemesterId == semesterId &&
                    x.StudentUserId == studentUserId).ToList();

            foreach (enu_DebtType debtType in Enum.GetValues(typeof(enu_DebtType))) 
            {
                var debt = debtInfo.FirstOrDefault(x => x.DebtType == (int)debtType);
                if (debt == null || (debt != null && debt.IsDebt)) 
                {
                    DebtInfoViewModel debtModel = new DebtInfoViewModel();
                    debtModel.DebtName = debtType.GetDisplayName();
                    debtModel.Comment = debt?.Comment;
                    debtModel.DebtAmount = debt != null ? debt.DebtAmount : 0;
                    debtList.Add(debtModel);
                }
            }
            return debtList;
        }

        /// <summary>
        /// Get student debt marks
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Students debt marks</returns>
        public StudentDebtMarksViewModel GetStudentDebtMarks(int semesterId, string studentUserId)
        {
            var debtInfo = _db.StudentDebts.Where(x => x.SemesterId == semesterId &&
                    x.StudentUserId == studentUserId).ToList();

            StudentDebtMarksViewModel debtMarks = new StudentDebtMarksViewModel();

            if (debtInfo.Any())
            {
                debtMarks.Accounting = debtInfo.Any(x => x.DebtType == (int)enu_DebtType.Accounting && x.IsDebt);
                debtMarks.Library = debtInfo.Any(x => x.DebtType == (int)enu_DebtType.Library && x.IsDebt);
                debtMarks.Dormitory = debtInfo.Any(x => x.DebtType == (int)enu_DebtType.Dormitory && x.IsDebt);
                debtMarks.RegistarOffice = debtInfo.Any(x => x.DebtType == (int)enu_DebtType.RegistarOffice && x.IsDebt);
                debtMarks.MedicineOffice = debtInfo.Any(x => x.DebtType == (int)enu_DebtType.MedicineOffice && x.IsDebt);
            }

            return debtMarks;
        }
    }
}
