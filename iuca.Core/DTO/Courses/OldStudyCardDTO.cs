using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Instructors;
using iuca.Application.DTO.Users.Students;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class OldStudyCardDTO
    {
        public int Id { get; set; }

        [Display(Name = "Semester")]
        public int SemesterId { get; set; }
        [Display(Name = "Semester")]
        public SemesterDTO Semester { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }
        [Display(Name = "Organization")]
        public OrganizationDTO Organization { get; set; }

        [Display(Name = "Group")]
        public int DepartmentGroupId { get; set; }
        [Display(Name = "Group")]
        public DepartmentGroupDTO DepartmentGroup { get; set; }
        
        public virtual List<OldStudyCardCourseDTO> OldStudyCardCourses { get; set; }
    }
}
