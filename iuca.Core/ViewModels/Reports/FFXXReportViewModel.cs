
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;

namespace iuca.Application.ViewModels.Reports
{
    public class FFXXReportViewModel
    {
        public string StudentName { get; set; }
        public int StudentId { get; set; }
        public string StudentGroup { get; set; }
        public CourseDTO Course { get; set; }
        public string GradeMark { get; set; }
        public SemesterDTO Semester { get; set; }
        public bool IsClosed { get; set; }

    }
}
