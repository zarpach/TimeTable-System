using iuca.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.Instructors
{
    public class InstructorOrgInfo
    {
        public InstructorBasicInfo InstructorBasicInfo { get; set; }
        public int InstructorBasicInfoId { get; set; }
        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int State { get; set; }
        public bool PartTime { get; set; }
        public int ImportCode { get; set; }
    }
}
