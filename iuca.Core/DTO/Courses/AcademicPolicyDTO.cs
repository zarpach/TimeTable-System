using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class AcademicPolicyDTO
    {
        public int Id { get; set; }

        [Display(Name = "Syllabus")]
        public SyllabusDTO Syllabus { get; set; }

        [Display(Name = "Syllabus")]
        public int SyllabusId { get; set; }

        [Display(Name = "Name")]
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [MaxLength(5000)]
        [Required]
        public string Description { get; set; }
    }
}
