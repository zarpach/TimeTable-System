using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class CourseRequirementDTO
    {
        public int Id { get; set; }

        [Display(Name = "Syllabus")]
        public SyllabusDTO Syllabus { get; set; }

        [Display(Name = "Syllabus")]
        public int SyllabusId { get; set; }

        [Display(Name = "Name")]
        [Required]
        public int Name { get; set; }

        [Display(Name = "Description")]
        [MaxLength(5000)]
        public string Description { get; set; }

        [Display(Name = "Points")]
        [Required]
        public float Points { get; set; }
    }
}
