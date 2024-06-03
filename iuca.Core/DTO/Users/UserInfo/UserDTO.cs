using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Users.UserInfo
{
    public class UserDTO
    {
        public string Id { get; set; }

        [Display(Name = "User full name")]
        public string FullName { get; set; }
    }
}
