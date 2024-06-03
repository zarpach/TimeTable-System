using iuca.Application.DTO.Users.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.Instructors
{
    public interface IInstructorContactInfoService
    {
        /// <summary>
        /// Create instructor contact info record
        /// </summary>
        /// <param name="instructorContactInfoDTO">Instructor contact info model</param>
        InstructorContactInfoDTO Create(InstructorContactInfoDTO instructorContactInfoDTO);

        /// <summary>
        /// Edit instructor contact info by id
        /// </summary>
        /// <param name="instructorContactInfoDTO">Instructor contact info model</param>
        void Edit(InstructorContactInfoDTO instructorContactInfoDTO);

        /// <summary>
        /// Delete instructor contact info by id
        /// </summary>
        /// <param name="id">Instructor contact info id</param>
        void Delete(int id);

        void Dispose();
    }
}
