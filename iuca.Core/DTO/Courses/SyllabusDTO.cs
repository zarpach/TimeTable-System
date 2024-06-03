using iuca.Application.Enums;
using iuca.Application.ViewModels.Courses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class SyllabusDTO
    {
        public int Id { get; set; }

        [Display(Name = "Registration Course")]
        public RegistrationCourseDTO RegistrationCourse { get; set; }

        [Display(Name = "Registration Course")]
        public int RegistrationCourseId { get; set; }

        [Display(Name = "Instructor Phone")]
        [MaxLength(100)]
        public string InstructorPhone { get; set; }

        [Display(Name = "Office Hours")]
        [MaxLength(255)]
        public string OfficeHours { get; set; }

        [Display(Name = "Course Description")]
        [MaxLength(5000)]
        [Required]
        public string CourseDescription { get; set; }


        [Display(Name = "Objectives")]
        [MaxLength(5000)]
        public string Objectives { get; set; }

        [Display(Name = "Teach Methods")]
        [MaxLength(5000)]
        public string TeachMethods { get; set; }

        [Display(Name = "Primary Resources")]
        [MaxLength(5000)]
        public string PrimaryResources { get; set; }

        [Display(Name = "Additional Resources")]
        [MaxLength(5000)]
        public string AdditionalResources { get; set; }


        [Display(Name = "Link")]
        [MaxLength(255)]
        [Required]
        public string Link { get; set; }

        [Display(Name = "Grading Comment")]
        [MaxLength(5000)]
        public string GradingComment { get; set; }


        [Display(Name = "Instructor Comment")]
        [MaxLength(5000)]
        public string InstructorComment { get; set; }

        [Display(Name = "Approver Comment")]
        [MaxLength(5000)]
        public string ApproverComment { get; set; }

        [Display(Name = "Syllabus Language")]
        public int Language { get; set; } = (int)enu_SyllabusLanguage.English;

        [Display(Name = "Syllabus Status")]
        public int Status { get; set; }

        [Display(Name = "Date Created")]
        public DateTime Created { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime Modified { get; set; }
        
        public List<CourseRequirementDTO> CourseRequirements { get; set; }


        public List<AcademicPolicyDTO> AcademicPolicies { get; set; }
        public List<CourseCalendarRowDTO> CourseCalendar { get; set; }


        [Display(Name = "Syllabus Details")]
        public SyllabusDetailsViewModel SyllabusDetails { get; set; }
    }
}
