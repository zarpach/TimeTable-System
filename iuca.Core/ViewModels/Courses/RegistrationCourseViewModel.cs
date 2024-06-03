using iuca.Application.DTO.Courses;
using System.ComponentModel.DataAnnotations;


namespace iuca.Application.ViewModels.Courses
{
    public class RegistrationCourseViewModel
    {
        [Display(Name = "Course")]
        public AnnouncementSectionDTO AnnouncementSection { get; set; }

        [Display(Name = "Instructor name")]
        public string InstructorName { get; set; }

        [Display(Name = "Students number")]
        public int StudentsNumber { get; set; }
    }
}
