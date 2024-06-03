
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Common
{
    public class EnvarSettingDTO
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public OrganizationDTO Organization { get; set; }

        [Display(Name = "Max registration credits")]
        public int MaxRegistrationCredits { get; set; }

        [Display(Name = "Default instructor")]
        public string DefaultInstructor { get; set; }

        [Display(Name = "Current semester")]
        public int CurrentSemester { get; set; }

        [Display(Name = "Upcoming semester")]
        public int UpcomingSemester { get; set; }
    }
}
