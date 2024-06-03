using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class StudyCardCourseDTO
    {
        public int Id { get; set; }

        [Display(Name = "Study Card")]
        public int StudyCardId { get; set; }
        [Display(Name = "Study Card")]
        public StudyCardDTO StudyCard { get; set; }

        [Display(Name = "Registration Course")]
        public int RegistrationCourseId { get; set; }
        [Display(Name = "Registration Course")]
        public AnnouncementSectionDTO AnnouncementSection { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(255)]
        public string Comment { get; set; }
    }
}
