using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iuca.Application.DTO.Courses;
using iuca.Application.Enums;

namespace iuca.Application.ViewModels.Courses
{
    public class CourseRegistrationStepsViewModel
    {
        public bool RegistrationUnavailable { get; set; }
        public bool IsDebt { get; set; }
        public string WarningMessage { get; set; }

        public int StudentCourseRegistrationId { get; set; }
        public int StepNumber { get; set; }
        public enu_RegistrationState RegistrationState { get; set; }
        public string RegistrationStateStr { get; set; }
        public string SeasonYear { get; set; }
        public int SemesterId { get; set; }
        public string ActionTitle { get; set; }
        public string AdviserComment { get; set; }
        public string StudentComment { get; set; }
        public int MaxRegistrationCredits { get; set; }
        public bool NoCreditsLimitation { get; set; }

        public List<StudentCourseStepsViewModel> StudentCourses { get; set; } = new List<StudentCourseStepsViewModel>();
    }

    public class StudentCourseStepsViewModel 
    {
        public int RegistrationCourseId { get; set; }
        public int StudentCourseId { get; set; }
        public string CourseName { get; set; }
        public string Code { get; set; }
        public int CourseImportCode { get; set; }
        public int Points { get; set; }
        public string InstructorName { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
        public string Schedule { get; set; }
        public int Queue { get; set; }
        public enu_CourseState State { get; set; }
        public bool IsAudit { get; set; }
        public bool IsFromStudyCard { get; set; }
        public bool IsForAll { get; set; }
        public bool NoCreditsCount { get; set; }
        public bool IsActivated { get; set; }
    }
}
