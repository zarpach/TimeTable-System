using iuca.Application.Enums;

namespace iuca.Application.ViewModels.Users.Students
{
    public class AdviserStudentViewModel
    {
        public string StudentUserId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Group { get; set; }
        public string StudentId { get; set; }
        public enu_StudentState State { get; set; }
        public int DepartmentGroupId { get; set; }
        public int? PrepDepartmentGroupId { get; set; }
        public float SemsterGPA { get; set; }
        public float TotalGPA { get; set; }
    }
}
