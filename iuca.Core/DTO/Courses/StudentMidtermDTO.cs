using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class StudentMidtermDTO
    {
        public int Id { get; set; }

        [Display(Name = "Student course")]
        public int StudentCourseId { get; set; }

        [Display(Name = "Student course")]
        public StudentCourseTempDTO StudentCourse { get; set; }

        [Display(Name = "Score (today)")]
        [Range(0, 200, ErrorMessage = "Value must be greater than or equal to 0")]
        public int Score { get; set; } = -1;

        [Display(Name = "Max score (today)")]
        [Range(1,200, ErrorMessage = "Value must be greater than 0")]
        public int MaxScore { get; set; }

        [Display(Name = "Attention")]
        public bool Attention { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(1000)]
        public string Comment { get; set; }

        [Display(Name = "Recommendation")]
        [MaxLength(1000)]
        public string Recommendation { get; set; }

        [Display(Name = "Adviser comment")]
        [MaxLength(1000)]
        public string AdviserComment { get; set; }
    }
}
