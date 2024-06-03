using iuca.Application.DTO.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.ViewModels.Courses
{
    public class SyllabusDetailsViewModel
    {
        [Display(Name = "Course Name")]
        public string CourseNameEng { get; set; }

        [Display(Name = "Course Name")]
        public string CourseNameRus { get; set; }

        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Display(Name = "Semester Season")]
        public int SemesterSeason { get; set; }

        [Display(Name = "Semester Year")]
        public int SemesterYear { get; set; }

        [Display(Name = "Department Name")]
        public string DepartmentNameEng { get; set; }

        [Display(Name = "Department Name")]
        public string DepartmentNameRus { get; set; }

        [Display(Name = "Course Credits")]
        public float CourseCredits { get; set; }

        [Display(Name = "Instructor Name")]
        public string InstructorName { get; set; }

        [Display(Name = "Instructor Email")]
        public string InstructorEmail { get; set; }

        [Display(Name = "Course Prerequisites")]
        public List<string> CoursePrerequisitesEng { get; set; }

        [Display(Name = "Course Prerequisites")]
        public List<string> CoursePrerequisitesRus { get; set; }

        [Display(Name = "Course Policies")]
        public IEnumerable<PolicyDTO> CoursePolicies { get; set; }
    }
}
