using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class ProposalCourseDTO
    {
        public int Id { get; set; }

        [Display(Name = "Proposal")]
        public int ProposalId { get; set; }

        [Display(Name = "Proposal")]
        public ProposalDTO Proposal { get; set; }

        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Display(Name = "Course")]
        public CourseDTO Course { get; set; }

        [Display(Name = "Credits")]
        [Range(0, 50, ErrorMessage = "Value must be equal or greater than 0")]
        public float Credits { get; set; } = -1;

        [Display(Name = "For all")]
        public bool IsForAll { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; } = (int)enu_ProposalCourseStatus.New;

        [Display(Name = "Comment")]
        [MaxLength(255)]
        public string Comment { get; set; }

        [Display(Name = "Schedule")]
        [MaxLength(50)]
        public string Schedule { get; set; }

        [Display(Name = "Instructors")]
        [Required(ErrorMessage = "Select instructor")]
        public IEnumerable<string> InstructorsJson { get; set; }
        public IEnumerable<UserDTO> Instructors { get; set; }

        [Display(Name = "Years of study")]
        public IEnumerable<int> YearsOfStudyJson { get; set; }
    }
}
