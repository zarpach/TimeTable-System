using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.Students
{
    public class StudentOrgInfo
    {
        public StudentBasicInfo StudentBasicInfo { get; set; }
        public int StudentBasicInfoId { get; set; }
        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }
        public DepartmentGroup DepartmentGroup { get; set; }
        public int DepartmentGroupId { get; set; }
        public DepartmentGroup PrepDepartmentGroup { get; set; }
        public int? PrepDepartmentGroupId { get; set; }
        public int StudentId { get; set; }
        public bool IsPrep { get; set; }
        public int State { get; set; }

    }
}
