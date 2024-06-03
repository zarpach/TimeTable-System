using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.ViewModels.Users.Students;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Courses
{
    public class GradeAdviserReportViewModel
    {
        public SemesterDTO Semester { get; set; }
        public string AdviserUserId { get; set; }
        public string AdviserName { get; set; }

        public List<CourseDTO> AllCourses { get; set; } = new List<CourseDTO>();
        public List<AdviserStudentViewModel> AllStudents { get; set; } = new List<AdviserStudentViewModel>();
        public Dictionary<string, StudentCourseTempDTO> AllStudentCourses { get; set; } = new Dictionary<string, StudentCourseTempDTO>();
    }
}
