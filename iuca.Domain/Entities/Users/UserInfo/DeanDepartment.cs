using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.UserInfo
{
    public class DeanDepartment
    {
        public string DeanUserId { get; set; }
        public Department Department { get; set; }
        public int DepartmentId { get; set; }
        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }
    }
}
