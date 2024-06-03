using iuca.Application.DTO.Users.Instructors;
using iuca.Application.DTO.Users.UserInfo;

namespace iuca.Application.ViewModels.Users.Instructors
{
    public class InstructorInfoDetailsViewModel
    {
        public string InstructorUserId { get; set; }
        public bool IsReadOnly { get; set; }
        public InstructorUserInfoViewModel UserInfo { get; set; }
        public UserBasicInfoDTO UserBasicInfo { get; set; }
        public InstructorBasicInfoDTO InstructorBasicInfo { get; set; }
        public InstructorOrgInfoDTO InstructorOrgInfo { get; set; }
    }
}
