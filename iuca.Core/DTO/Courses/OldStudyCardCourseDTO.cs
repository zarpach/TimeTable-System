using iuca.Application.DTO.Users.Instructors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class OldStudyCardCourseDTO
    {
        public int Id { get; set; }

        [Display(Name = "Study Card")]
        public int OldStudyCardId { get; set; }
        
        [Display(Name = "Study Card")]
        public OldStudyCardDTO OldStudyCard { get; set; }

        [Display(Name = "Course")]
        public int CyclePartCourseId { get; set; }
        
        [Display(Name = "Course")]
        public CyclePartCourseDTO CyclePartCourse { get; set; }

        [Display(Name = "Instructor")]
        public string InstructorUserId { get; set; }

        [Display(Name = "Vacancy")]
        public bool IsVacancy { get; set; }

        [Display(Name = "Places")]
        public int Places { get; set; }
    }
}
