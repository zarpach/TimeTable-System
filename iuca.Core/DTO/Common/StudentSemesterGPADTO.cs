using iuca.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Common
{
    public class StudentSemesterGPADTO
    {
        public string StudentUserId { get; set; }

        [Display(Name = "Season")]
        public int Season { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "GPA")]
        public float GPA { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Organization")]
        public Organization Organization { get; set; }
    }
}
