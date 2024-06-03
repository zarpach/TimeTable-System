using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using iuca.Application.DTO.Common;

namespace iuca.Application.DTO.Courses
{
    public class ProposalDTO
    {
        public int Id { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Department")]
        public DepartmentDTO Department { get; set; }

        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        [Display(Name = "Semester")]
        public SemesterDTO Semester { get; set; }

        public IEnumerable<ProposalCourseDTO> ProposalCourses { get; set; }

    }
}
