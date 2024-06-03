using iuca.Domain.Entities.Courses;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.ViewModels.Courses
{
    public class StudentAttendanceDetailsViewModel
    {
        public Course Course { get; set; }

        public float AttendanceWeightedGrade { get; set; }
        public float CourseAttendancePercentage {
            get
            {
                if (CourseAttendance == null || CourseAttendance.Count() == 0)
                {
                    return 0;
                }

                float totalClasses = CourseAttendance.Sum(x => x.TotalClasses);
                float attendedClasses = CourseAttendance.Sum(x => x.BlankOrLateClasses);

                return totalClasses == 0 ? 100 : (attendedClasses / totalClasses) * 100;
            }
        }

        public IEnumerable<CourseAttendanceViewModel> CourseAttendance { get; set; }
    }
}
