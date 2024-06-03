using iuca.Application.DTO.Users.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.Instructors
{
    public interface IInstructorOtherJobInfoService
    {
        /// <summary>
        /// Create, update, or delete instructor other job info
        /// </summary>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <param name="newOtherJobInfoList">List of other job records from new model</param>
        void EditOtherJobInfo(int instructorBasicInfoId, List<InstructorOtherJobInfoDTO> newOtherJobInfoList);

        /// <summary>
        /// Create other job info record
        /// </summary>
        /// <param name="otherJobInfoDTO">Other job info model</param>
        /// <returns>Model of new created record</returns>
        void Create(InstructorOtherJobInfoDTO otherJobInfoDTO);

        /// <summary>
        /// Edit other job info by id
        /// </summary>
        /// <param name="otherJobInfoId">Other job info id</param>
        /// <param name="otherJobInfoDTO">Other job info model</param>
        public void Edit(int otherJobInfoId, InstructorOtherJobInfoDTO otherJobInfoDTO);

        /// <summary>
        /// Delete other job info by id
        /// </summary>
        /// <param name="id">Other job info id</param>
        public void Delete(int id);

        void Dispose();
    }
}
