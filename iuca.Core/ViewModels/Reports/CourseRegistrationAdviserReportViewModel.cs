using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.ViewModels.Users.Students;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Reports
{
    public class CourseRegistrationAdviserReportViewModel
    {
        public SemesterDTO Semester { get; set; }
        public string AdviserUserId { get; set; }
        public string AdviserName { get; set; }

        public List<CourseRegistrationAdviserReportCourse> AllCourses { get; set; } = new List<CourseRegistrationAdviserReportCourse>();
        public List<CourseRegistrationAdviserReportStudent> AllStudents { get; set; } = new List<CourseRegistrationAdviserReportStudent>();
        public Dictionary<string, StudentCourseTempDTO> AllStudentCourses { get; set; } = new Dictionary<string, StudentCourseTempDTO>();
    }

    public class CourseRegistrationAdviserReportCourse 
    {
        public CourseDTO Course { get; set; }
        public string Credits { get; set; }
        public bool IsFromStudyCard { get; set; }
    }

    public class CourseRegistrationAdviserReportStudent
    {
        public AdviserStudentViewModel StudentInfo { get; set; }
        public enu_RegistrationState RegistrationState { get; set; }
        public float SemesterCredits { get; set; }
        public float EarnedCredits { get; set; }
    }
}
