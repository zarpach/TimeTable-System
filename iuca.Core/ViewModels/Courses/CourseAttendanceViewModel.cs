using System;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Courses
{
    public class CourseAttendanceViewModel
    {
        public DateTime Date { get; set; }
        public int TotalClasses { get; set; }
        public float BlankOrLateClasses { get; set; }

        public IEnumerable<int> Marks { get; set; }
    }
}
