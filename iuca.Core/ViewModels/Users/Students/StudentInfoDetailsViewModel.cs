using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;

namespace iuca.Application.ViewModels.Users.Students
{
    public class StudentInfoDetailsViewModel
    {
        public string FullNameEng { get; set; }
        public string StudentUserId { get; set; }
        public bool IsReadOnly { get; set; }
        public UserBasicInfoDTO UserBasicInfo { get; set; }
        public StudentBasicInfoDTO StudentBasicInfo { get; set; }
        public StudentOrgInfoDTO StudentOrgInfo { get; set; }
        public StudentMinorInfoViewModel StudentMinorInfo { get; set; }
    }
}
