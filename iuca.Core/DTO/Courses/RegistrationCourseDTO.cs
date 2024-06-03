using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class RegistrationCourseDTO
    {
        public int Id { get; set; }

        [Display(Name = "Organization")]
        public OrganizationDTO Organization { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Course")]
        public CourseDTO Course { get; set; }

        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Display(Name = "Syllabus")]
        public SyllabusDTO Syllabus { get; set; }

        [Display(Name = "Instructor")]
        public string InstructorUserId { get; set; }

        [Display(Name = "Instructor")]
        public string InstructorUserName { get; set; }

        [Display(Name = "Course Details")]
        public int CourseDetId { get; set; }

        [Display(Name = "Section")]
        [MaxLength(50)]
        public string Section { get; set; }

        [Display(Name = "Points")]
        public float Points { get; set; }

        [Display(Name = "Language")]
        [MaxLength(100)]
        public string Language { get; set; }

        [Display(Name = "GeneralEducation")]
        public bool GeneralEducation { get; set; }

        [Display(Name = "Season")]
        public int Season { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "NumberOfGroups")]
        public int NumberOfGroups { get; set; }

        [Display(Name = "NumberOfGroups")]
        public int Places { get; set; }

        [Display(Name = "Schedule")]
        [MaxLength(100)]
        public string Schedule { get; set; }

        [Display(Name = "Description (En)")]
        public string DescEng { get; set; }

        [Display(Name = "Description (Ru)")]
        public string DescRus { get; set; }

        [Display(Name = "Description (Kg)")]
        public string DescKir { get; set; }

        [Display(Name = "Requirements")]
        public string Requirements { get; set; }

        [Display(Name = "Is Changed")]
        public bool IsChanged { get; set; }

        [Display(Name = "Marked as deleted")]
        public bool MarkedDeleted { get; set; }

        public List<ExtraInstructorDTO> ExtraInstructors { get; set; }
    }
}
