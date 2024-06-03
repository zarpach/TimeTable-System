using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Instructors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Courses
{
    public class StudyCardPlacesViewModel
    {

        [Display(Name = "Course")]
        public CyclePartCourseDTO CyclePartCourse { get; set; }

        [Display(Name = "Instructor user id")]
        public string InstructorUserId { get; set; }

        [Display(Name = "Instructor name")]
        public string InstructorName { get; set; }

        [Display(Name = "Places")]
        public int Places { get; set; }
    }
}
