using iuca.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Common
{
    public class StudentTotalGPADTO
    {
        public string StudentUserId { get; set; }

        [Display(Name = "Total GPA")]
        public float TotalGPA { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Organization")]
        public Organization Organization { get; set; }
    }
}
