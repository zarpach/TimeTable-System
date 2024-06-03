using iuca.Application.DTO.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Courses
{
    public interface ITransferCourseService
    {
        /// <summary>
        /// Get transfer course list for student
        /// </summary>
        /// <param name="selectedOrganizationId">Selected organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <returns>Transfer course list for student</returns>
        public IEnumerable<TransferCourseDTO> GetTransferCourses(int selectedOrganizationId, string studentUserId);

        /// <summary>
        /// Edit transfer courses of student
        /// </summary>
        /// <param name="selectedOrganizationId">Organization id</param>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="transferCourses">Transfer courses list</param>
        public void EditStudentTransferCourses(int selectedOrganizationId, string studentUserId,
                                                            List<TransferCourseDTO> transferCourses);

        
        void Dispose();
    }
}
