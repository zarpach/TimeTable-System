using System;

namespace iuca.Domain.Entities.Courses
{
    public class CourseCalendarRow
    {
        public int Id { get; set; }
        public Syllabus Syllabus { get; set; }
        public int SyllabusId { get; set; }
        public int Week { get; set; }
        public DateTime Date { get; set; }
        public string Topics { get; set; }
        public string Assignments { get; set; }
    }
}
