
using iuca.Application.Enums;

namespace iuca.Application.ViewModels.Users.Instructors
{
    public class InstructorCourseStudentViewModel
    {
        public string StudentUserId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentStatus { get; set; }
        public string StudentMajor { get; set; }
        public string StudentGroup { get; set; }
        public enu_RegistrationState RegistrationState { get; set; }
        
    }
}
