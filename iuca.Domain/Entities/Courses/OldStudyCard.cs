using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class OldStudyCard
    {
        public int Id { get; set; }
        public int SemesterId { get; set; }
        public Semester Semester { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int DepartmentGroupId { get; set; }
        public DepartmentGroup DepartmentGroup { get; set; }

        public virtual List<OldStudyCardCourse> OldStudyCardCourses { get; set; }
    }
}
