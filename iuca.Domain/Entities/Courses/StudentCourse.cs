using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class StudentCourse
    {
        public int Id { get; set; }
        public StudentCourseRegistration StudentCourseRegistration { get; set; }
        public int StudentCourseRegistrationId { get; set; }
        public OldStudyCardCourse OldStudyCardCourse { get; set; }
        public int StudyCardCourseId { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
        public bool IsPassed { get; set; }
        public int Queue { get; set; }
    }
}
