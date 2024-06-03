using iuca.Domain.Entities.Users.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class OldStudyCardCourse
    {
        public int Id { get; set; }
        public int OldStudyCardId { get; set; }
        public OldStudyCard OldStudyCard { get; set; }
        public int CyclePartCourseId { get; set; }
        public CyclePartCourse CyclePartCourse { get; set; }
        public string InstructorUserId { get; set; }
        public bool IsVacancy { get; set; }
        public int Places { get; set; }

        public virtual List<StudentCourse> StudentCourses { get; set; }
    }
}
