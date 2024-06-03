using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.ViewModels.Users.Students;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Courses
{
    public class MidtermAdviserReportViewModel
    {
        public SemesterDTO Semester { get; set; }
        public string AdviserUserId { get; set; }
        public string AdviserName { get; set; }

        public List<CourseDTO> AllCourses { get; set; } = new List<CourseDTO>();
        public List<MidtermAdviserReportStudentRow> AllStudents { get; set; } = new List<MidtermAdviserReportStudentRow>();
        public Dictionary<string, StudentCourseTempDTO> AllStudentCourses { get; set; } = new Dictionary<string, StudentCourseTempDTO>();
    }

    public class MidtermAdviserReportStudentRow
    {
        public AdviserStudentViewModel StudentInfo { get; set; }
        public int AttentionCount { get; set; }
    }
}
