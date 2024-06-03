using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class StudentCourseRegistrationDTO
    {
        public int Id { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Organization")]
        public OrganizationDTO Organization { get; set; }

        [Display(Name = "Student")]
        public string StudentUserId { get; set; }

        [Display(Name = "Semester")]
        public SemesterDTO Semester { get; set; }
        
        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        [Display(Name = "State")]
        public int State { get; set; }

        [Display(Name = "Is approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Adviser comment")]
        public string AdviserComment { get; set; }

        [Display(Name = "Student comment")]
        public string StudentComment { get; set; }

        [Display(Name = "Date create")]
        public DateTime DateCreate { get; set; }

        [Display(Name = "Date change")]
        public DateTime DateChange { get; set; }

        [Display(Name = "Add/Drop state")]
        public int AddDropState { get; set; } = 1;

        [Display(Name = "Is add/drop approved")]
        public bool IsAddDropApproved { get; set; }

        [Display(Name = "No credits limitation")]
        public bool NoCreditsLimitation { get; set; }

        public virtual List<StudentCourseDTO> StudentCourses { get; set; }
        public virtual List<StudentCourseTempDTO> StudentCoursesTemp { get; set; }
    }
}
