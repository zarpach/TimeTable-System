using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.ViewModels.Courses
{
    public class AttendanceChartDataViewModel
    {
        public int CourseCount { get; set; }
        public float OverallAttendancePercentage
        {
            get
            {
                if (DateAttendances == null || DateAttendances.Count == 0)
                {
                    return 0;
                }

                int totalClasses = DateAttendances.Sum(d => d.TotalClasses);
                float attendedClasses = DateAttendances.Sum(d => d.BlankOrLateClasses);

                return totalClasses == 0 ? 0 : (attendedClasses / (float)totalClasses) * 100;
            }
        }

        public List<DateAttendanceViewModel> DateAttendances { get; set; }
    }

    public class DateAttendanceViewModel
    {
        public DateTime Date { get; set; }
        public int TotalClasses { get; set; }
        public float BlankOrLateClasses { get; set; }
    }
}
