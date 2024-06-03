using iuca.Application.ViewModels.Users.Students;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Courses
{
    public class AttendanceInconsistencyViewModel
    {
        public int UndefinedMarkCount { get; set; }

        public IEnumerable<StudentMinimumInfoViewModel> MissingStudents { get; set; }
        public IEnumerable<StudentMinimumInfoViewModel> SurplusStudents { get; set; }
    }
}
