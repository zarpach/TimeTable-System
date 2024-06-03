using iuca.Application.DTO.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Users.Students
{
    public interface IStudentParentsInfoService
    {
        /// <summary>
        /// Edit student parents info
        /// </summary>
        /// <param name="studentBasicInfoId">Student basic info id</param>
        /// <param name="studentParentsInfoList">Student parents info list from model</param>
        void EditStudentParentsInfo(int studentBasicInfoId, List<StudentParentsInfoDTO> studentParentsInfoList);

        void Dispose();
    }
}
