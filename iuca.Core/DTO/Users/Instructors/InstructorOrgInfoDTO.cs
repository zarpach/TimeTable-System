using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.Instructors
{
    public class InstructorOrgInfoDTO
    {
        public InstructorBasicInfoDTO InstructorBasicInfo { get; set; }
        public int InstructorBasicInfoId { get; set; }
        public OrganizationDTO Organization { get; set; }
        public int OrganizationId { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public DepartmentDTO Department { get; set; }
        public int State { get; set; }
        public bool PartTime { get; set; }
        public int ImportCode { get; set; }
    }
}
