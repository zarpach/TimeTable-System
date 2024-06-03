using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;

namespace iuca.Application.DTO.Courses
{
    public class StudyCardDTO
    {
        public int Id { get; set; }

        [Display(Name = "Semester")]
        public int SemesterId { get; set; }
        [Display(Name = "Semester")]
        public SemesterDTO Semester { get; set; }

        [Display(Name = "Group")]
        public int DepartmentGroupId { get; set; }
        [Display(Name = "Group")]
        public DepartmentGroupDTO DepartmentGroup { get; set; }

        [Display(Name = "Display IUCA electives")]
        public bool DisplayIUCAElectives { get; set; }

        public virtual List<StudyCardCourseDTO> StudyCardCourses { get; set; }
    }
}
