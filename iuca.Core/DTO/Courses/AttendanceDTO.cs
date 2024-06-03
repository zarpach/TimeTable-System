using iuca.Application.ViewModels.Users.Students;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class AttendanceDTO
    {
        public int Id { get; set; }

        [Display(Name = "Student")]
        public string StudentUserId { get; set; }

        [Display(Name = "Student")]
        public StudentMinimumInfoViewModel Student { get; set; }

        [Display(Name = "Course")]
        public AnnouncementSectionDTO AnnouncementSection { get; set; }

        [Display(Name = "Course")]
        public int AnnouncementSectionId { get; set; }

        public virtual IEnumerable<AttendanceClassDTO> AttendanceClasses { get; set; }
    }
}
