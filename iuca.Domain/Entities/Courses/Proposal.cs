using System.Collections.Generic;
using iuca.Domain.Entities.Common;

namespace iuca.Domain.Entities.Courses
{
    public class Proposal : AuditableEntity
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int SemesterId { get; set; }
        public Semester Semester { get; set; }

        public virtual IEnumerable<ProposalCourse> ProposalCourses { get; set; }
    }
}
