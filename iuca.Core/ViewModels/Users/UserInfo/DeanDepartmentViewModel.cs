using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Users.UserInfo
{
    public class DeanDepartmentViewModel
    {
        public ApplicationUser DeanUser { get; set; }
        public int OrganizationId { get; set; }
        public List<DepartmentDTO> Departments { get; set; }
    }
}
