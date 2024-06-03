using iuca.Application.Enums;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Courses
{
    public class CheckRegistrationViewModel
    {
        public int StudentCourseRegistrationId { get; set; }
        public int SemesterId { get; set; }
        public string StudentName { get; set; }
        public string RegistrationState { get; set; }
        public bool Disaprove { get; set; }
        public string AdviserComment { get; set; }
        public string StudentComment { get; set; }
        public int MaxRegistrationCredits { get; set; }
        public bool NoCreditsLimitation { get; set; }

        public List<CheckStudentCourse> StudentCourses { get; set; } = new List<CheckStudentCourse>();
    }

    public class CheckStudentCourse 
    { 
        public int StudentCourseId { get; set; }
        public string Name { get; set; }
        public int ImportCode { get; set; }
        public string Code { get; set; }
        public int Points { get; set; }
        public string InstructorName { get; set; }
        public string Comment { get; set; }
        public int Queue { get; set; }
        public bool IsApproved { get; set; }
        public bool IsProcessed { get; set; }
        public bool PassedPrerequisite { get; set; }
        public enu_CourseState State { get; set; }
        public bool IsAudit { get; set; }
        public bool IsFromStudyCard { get; set; }
        public string StudyCardComment { get; set; }
        public bool NoCreditsCount { get; set; }
        public List<CheckCoursePrerequisite> CoursePrerequisites { get; set; }
    }

    public class CheckCoursePrerequisite
    {
        public string Name { get; set; }
        public string Grade { get; set; }
        public bool Passed { get; set; }
    }
}
