using iuca.Domain.Entities.Courses;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Common
{
    public class Grade
    {
        public int Id { get; set; }
        public string GradeMark { get; set; }
        public float Gpa { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string NameKir { get; set; }
        public int ImportCode { get; set; }

        public virtual List<StudentCourseGrade> StudentCourseGrades { get; set; }
        public virtual List<TransferCourse> TransferCourses { get; set; }
        public virtual List<StudentCourseTemp> StudentCourseTemps { get; set; }
    }
}
