using System.ComponentModel.DataAnnotations;

namespace iuca.Application.ViewModels.Users.Instructors
{
    public class InstructorUserInfoViewModel
    {
        public string ApplicationUserId { get; set; }

        [Required]
        [Display(Name = "Last name eng")]
        public string LastNameEng { get; set; }

        [Required]
        [Display(Name = "First name eng")]
        public string FirstNameEng { get; set; }

        [Display(Name = "Middle name eng")]
        public string MiddleNameEng { get; set; }

        [Display(Name = "Full name eng")]
        public string FullNameEng
        {
            get
            {
                return LastNameEng + " " + FirstNameEng + " " + MiddleNameEng;
            }
        }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
