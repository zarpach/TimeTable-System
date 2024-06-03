using iuca.Application.DTO.Common;
using System;
using System.ComponentModel.DataAnnotations;


namespace iuca.Application.DTO.Courses
{
    public class StudentCourseTempDTO
    {
        public int Id { get; set; }

        [Display(Name = "Registration")]
        public StudentCourseRegistrationDTO StudentCourseRegistration { get; set; }

        [Display(Name = "Registration")]
        public int StudentCourseRegistrationId { get; set; }

        [Display(Name = "Course")]
        public AnnouncementSectionDTO AnnouncementSection { get; set; }

        [Display(Name = "Course")]
        public int AnnouncementSectionId { get; set; }

        [Display(Name = "Grade")]
        public int? GradeId { get; set; }

        [Display(Name = "Grade")]
        public GradeDTO Grade { get; set; }

        [Display(Name = "Is processed")]
        public bool IsProcessed { get; set; }

        [Display(Name = "Is approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string Comment { get; set; }

        [Display(Name = "Is passed")]
        public bool IsPassed { get; set; }

        [Display(Name = "Queue")]
        public int Queue { get; set; }

        [Display(Name = "State")]
        public int State { get; set; }

        [Display(Name = "Is add/drop approved")]
        public bool IsAddDropApproved { get; set; }

        [Display(Name = "Is add/drop processed")]
        public bool IsAddDropProcessed { get; set; }

        [Display(Name = "Marked as deleted")]
        public bool MarkedDeleted { get; set; }

        [Display(Name = "Date created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Is audit")]
        public bool IsAudit { get; set; }

        [Display(Name = "Sudent midterm")]
        public virtual StudentMidtermDTO StudentMidterm { get; set; }
    }
}
