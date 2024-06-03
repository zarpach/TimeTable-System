using iuca.Application.DTO.Users.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.Instructors
{
    public interface IInstructorEducationInfoService
    {
        /// <summary>
        /// Create, update, or delete instructor education info
        /// </summary>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <param name="newEducationInfoList">List of instructor education records from new model</param>
        void EditEducationInfo(int instructorBasicInfoId, List<InstructorEducationInfoDTO> newEducationInfoList);

        /// <summary>
        /// Create instructor education info record
        /// </summary>
        /// <param name="instructorEducationInfoDTO">Instructor education info model</param>
        void Create(InstructorEducationInfoDTO instructorEducationInfoDTO);

        /// <summary>
        /// Edit instructor education info by id
        /// </summary>
        /// <param name="instructorEducationInfoId">Instructor education info id</param>
        /// <param name="instructorEducationInfoDTO">Instructor education info model</param>
        void Edit(int instructorEducationInfoId, InstructorEducationInfoDTO instructorEducationInfoDTO);

        /// <summary>
        /// Delete instructor education info by id
        /// </summary>
        /// <param name="id">Instructor education info id</param>
        void Delete(int id);

        void Dispose();
    }
}
