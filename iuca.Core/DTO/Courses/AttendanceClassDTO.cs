using System;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class AttendanceClassDTO
    {
        public int Id { get; set; }

        [Display(Name = "Attendance")]
        public AttendanceDTO Attendance { get; set; }

        [Display(Name = "Attendance")]
        public int AttendanceId { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Class number")]
        public int Number { get; set; }

        [Display(Name = "Mark")]
        public int Mark { get; set; }

        [Display(Name = "Data")]
        public string Data { get; set; }
    }
}
