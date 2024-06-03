using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class StudentCourseGrade
    {
        public int Id { get; set; }
        public string StudentUserId { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public int Season { get; set; }
        public int Year { get; set; }
        public float Points { get; set; }
        public Grade Grade { get; set; }
        public int? GradeId { get; set; }
        public Organization Organization { get; set; } 
        public int OrganizationId { get; set; }
        public int ImportCode { get; set; }

    }
}
