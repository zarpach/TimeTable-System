using iuca.Application.DTO.Users.Staff;
using iuca.Application.DTO.Users.UserInfo;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace iuca.Application.ViewModels.Users.Staff
{
    public class StaffInfoDetailsViewModel
    {
        public string StaffUserId { get; set; }
        public string FullNameEng { get; set; }
        public bool IsReadOnly { get; set; }
        public UserBasicInfoDTO UserBasicInfo { get; set; }
        public StaffBasicInfoDTO StaffBasicInfo { get; set; }
    }
}
