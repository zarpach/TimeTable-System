using iuca.Application.Enums;
using iuca.Application.ViewModels.Users.Students;

namespace iuca.Application.ViewModels.Courses
{
    public class AdviserStudentRegistrationViewModel
    {
        public int StudentCourseRegistrationId { get; set; }
        public string StudentName { get; set; }
        public string StudentGroup { get; set; }
        public enu_StudentState StudentState { get; set; }
        public string RegistrationStateDesc { get; set; }
        public enu_RegistrationState RegistrationState { get; set; }
        public string RegistrationAddDropStateDesc { get; set; }
        public enu_RegistrationState RegistrationAddDropState { get; set; }
        public StudentDebtMarksViewModel DebtMarks { get; set; }
    }
}
