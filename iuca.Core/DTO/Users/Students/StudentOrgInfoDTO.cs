using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.Students
{
    public class StudentOrgInfoDTO
    {
        [Display(Name = "Student Basic Info")]
        public StudentBasicInfoDTO StudentBasicInfo { get; set; }

        [Display(Name = "Student Basic Info")]
        public int StudentBasicInfoId { get; set; }

        [Display(Name = "Organization")]
        public OrganizationDTO Organization { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Department Group")]
        public DepartmentGroupDTO DepartmentGroup { get; set; }

        [Display(Name = "Department Group")]
        public int DepartmentGroupId { get; set; }

        [Display(Name = "PREP Department Group")]
        public DepartmentGroupDTO PrepDepartmentGroup { get; set; }

        [Display(Name = "PREP Department Group")]
        public int? PrepDepartmentGroupId { get; set; }

        [Display(Name = "Student Id")]
        public int StudentId { get; set; }

        [Display(Name = "PREP")]
        public bool IsPrep { get; set; }

        [Display(Name = "State")]
        public int State { get; set; }

    }
}
