using iuca.Application.DTO.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.Students
{
    public interface IStudentContactInfoService
    {
        /// <summary>
        /// Create student contact info record
        /// </summary>
        /// <param name="studentContactInfoDTO">Student contact info model</param>
        void Create(StudentContactInfoDTO studentContactInfoDTO);

        /// <summary>
        /// Edit student contact info by id
        /// </summary>
        /// <param name="studentContactInfoId">Student contact info id</param>
        /// <param name="studentContactInfoDTO">Student contact info model</param>
        void Edit(int studentContactInfoId, StudentContactInfoDTO studentContactInfoDTO);

        /// <summary>
        /// Delete student contact info by id
        /// </summary>
        /// <param name="id">Student contact info id</param>
        void Delete(int id);

        void Dispose();
    }
}
