using iuca.Application.DTO.Users.Instructors;
using iuca.Application.DTO.Users.Staff;
using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.ViewModels.Users.UserInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Users
{
    public class AccountInfoViewModel
    {
        public UserInfoViewModel UserInfo { get; set; }
        public UserBasicInfoDTO UserBasicInfo { get; set; }
        public StaffBasicInfoDTO StaffBasicInfo { get; set; }
        public InstructorBasicInfoDTO InstructorBasicInfo { get; set; }
        public InstructorOrgInfoDTO InstructorOrgInfo { get; set; }
        public StudentBasicInfoDTO StudentBasicInfo { get; set; }
        public StudentOrgInfoDTO StudentOrgInfo { get; set; }
    }
}
