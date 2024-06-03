using iuca.Application.ViewModels.Users.Students;

namespace iuca.Application.ViewModels.Courses
{
    public class StudentAttendanceViewModel
    {
        public string StudentUserId { get; set; }
        public StudentMinimumInfoViewModel Student { get; set; }

        public int RegisteredСoursesCount { get; set; }
        public int AttendanceTrackedCoursesCount { get; set; }

        public float OverallAttendancePercentage { get; set; }
    }
}
