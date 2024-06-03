using System;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class CourseCalendarRowDTO
    {
        public int Id { get; set; }

        [Display(Name = "Syllabus")]
        public SyllabusDTO Syllabus { get; set; }

        [Display(Name = "Syllabus")]
        public int SyllabusId { get; set; }

        [Display(Name = "Week")]
        [Required]
        public int Week { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; } = new DateTime(DateTime.Today.Year, 01, 01);

        [Display(Name = "Topics")]
        [MaxLength(255)]
        [Required]
        public string Topics { get; set; }

        [Display(Name = "Assigments")]
        [MaxLength(255)]
        public string Assignments { get; set; }
    }
}
