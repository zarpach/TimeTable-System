using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Users.Students
{
    public class GroupDTO
    {
        public int Id { get; set; }

        [Display(Name = "Group code")]
        [Required]
        public string Code { get; set; }
    }
}
