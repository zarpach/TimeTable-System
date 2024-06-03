using iuca.Application.Enums;

namespace iuca.Application.ViewModels.Users.Students
{
    public class SelectStudentViewModel
    {
        public string StudentUserId { get; set; }
        public string FullNameRus { get; set; }
        public string FullNameEng { get; set; }
        public string Group { get; set; }
        public enu_StudentState State { get; set; }
        public int DepartmentGroupId { get; set; }
        public int StudentId { get; set; }
    }
}
