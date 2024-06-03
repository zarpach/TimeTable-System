using System;

namespace iuca.Domain.Entities.Courses
{
    public class AttendanceClass
    {
        public int Id { get; set; }
        public Attendance Attendance { get; set; }
        public int AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }
        public int Mark { get; set; }
        public string Data { get; set; }
    }
}
