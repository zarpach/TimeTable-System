using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.UserInfo
{
    public class DeanDepartmentDTO
    {
        [Display(Name = "Dean")]
        public string DeanUserId { get; set; }

        [Display(Name = "Department")]
        public DepartmentDTO Department { get; set; }
        public int DepartmentId { get; set; }

        [Display(Name = "Organization")]
        public OrganizationDTO Organization { get; set; }
        public int OrganizationId { get; set; }
    }
}
