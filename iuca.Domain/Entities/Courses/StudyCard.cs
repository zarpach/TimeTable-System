using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Courses
{
    public class StudyCard
    {
        public int Id { get; set; }
        public int SemesterId { get; set; }
        public Semester Semester { get; set; }
        public int DepartmentGroupId { get; set; }
        public DepartmentGroup DepartmentGroup { get; set; }
        public bool DisplayIUCAElectives { get; set; }

        public virtual List<StudyCardCourse> StudyCardCourses { get; set; }
    }
}
