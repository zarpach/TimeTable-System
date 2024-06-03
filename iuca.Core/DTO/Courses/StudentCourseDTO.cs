using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class StudentCourseDTO
    {
        public int Id { get; set; }

        [Display(Name = "Registration")]
        public StudentCourseRegistrationDTO StudentCourseRegistration { get; set; }

        [Display(Name = "Registration")]
        public int StudentCourseRegistrationId { get; set; }

        [Display(Name = "Course")]
        public OldStudyCardCourseDTO StudyCardCourse { get; set; }

        [Display(Name = "Course")]
        public int StudyCardCourseId { get; set; }

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
    }
}
