using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.UserInfo;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Users.Students;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Common
{
    public class Department
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string NameKir { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int ImportCode { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<InstructorOrgInfo> InstructorOrgInfo { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual List<AcademicPlan> AcademicPlans { get; set; }
        public virtual List<DepartmentGroup> DepartmentGroups { get; set; }
        public virtual List<DeanDepartment> DeanDepartments { get; set; }
        public virtual List<StudentMinorInfo> StudentMinorInfo { get; set; }
        public virtual IEnumerable<Proposal> Proposals { get; set; }
    }
}
