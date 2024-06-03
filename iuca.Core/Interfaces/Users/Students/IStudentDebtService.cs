using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using iuca.Application.ViewModels.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.Students
{
    public interface IStudentDebtService
    {
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
            int departmentGroupId, string lastName, string firstName, int studentId, enu_DebtorType debtorType,
            bool activeOnly, bool noRegistrations);

        /// <summary>
        /// Set student debt
        /// </summary>
        /// <param name="debtList">Debt list</param>
        void SetStudentDebts(List<StudentDebtViewModel> debtList);

        /// <summary>
        /// Get student debt info list
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>List of student debts</returns>
        List<DebtInfoViewModel> GetStudentDebtInfo(int semesterId, string studentUserId);

        /// <summary>
        /// Get student debt marks
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Students debt marks</returns>
        StudentDebtMarksViewModel GetStudentDebtMarks(int semesterId, string studentUserId);
    }
}
