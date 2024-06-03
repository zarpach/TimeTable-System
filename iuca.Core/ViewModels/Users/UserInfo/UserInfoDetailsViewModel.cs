using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.ViewModels.Users.UserInfo
{
    public class UserInfoDetailsViewModel
    {
        public UserBasicInfoDTO UserBasicInfo { get; set; }
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsStaff { get; set; }
        public bool IsStudent { get; set; }
        public bool IsInstructor { get; set; }
    }
}
