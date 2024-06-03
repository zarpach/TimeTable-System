using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class StudentCourseRegistration
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string StudentUserId { get; set; }
        public Semester Semester { get; set; }
        public int SemesterId { get; set; }
        public int State { get; set; }
        public bool IsApproved { get; set; }
        public string AdviserComment { get; set; }
        public string StudentComment { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateChange { get; set; }
        public int AddDropState { get; set; } = 1;
        public bool IsAddDropApproved { get; set; }
        public bool NoCreditsLimitation { get; set; }
        public virtual List<StudentCourse> StudentCourses { get; set; }
        public virtual List<StudentCourseTemp> StudentCoursesTemp { get; set; }
    }
}
