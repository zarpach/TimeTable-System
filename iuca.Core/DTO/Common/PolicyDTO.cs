using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Common
{
    public class PolicyDTO
    {
        public int Id { get; set; }

        [Display(Name = "Name (Ru)")]
        [MaxLength(100)]
        [Required]
        public string NameRus { get; set; }

        [Display(Name = "Name (En)")]
        [MaxLength(100)]
        [Required]
        public string NameEng { get; set; }

        [Display(Name = "Name (Kg)")]
        [MaxLength(100)]
        [Required]
        public string NameKir { get; set; }

        [Display(Name = "Description (Ru)")]
        [MaxLength(5000)]
        [Required]
        public string DescriptionRus { get; set; }

        [Display(Name = "Description (En)")]
        [MaxLength(5000)]
        [Required]
        public string DescriptionEng { get; set; }

        [Display(Name = "Description (Kg)")]
        [MaxLength(5000)]
        [Required]
        public string DescriptionKir { get; set; }
    }
}
