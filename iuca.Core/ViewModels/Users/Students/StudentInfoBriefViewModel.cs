
using iuca.Application.Enums;

namespace iuca.Application.ViewModels.Users.Students
{
    public class StudentInfoBriefViewModel
    {
        public string StudentUserId { get; set; }
        public string FullNameEng { get; set; }
        public string Email { get; set; }
        public bool IsReadOnly { get; set; }
        public string DepartmentGroup { get; set; }
        public enu_StudentState State { get; set; }
        public bool BasicInfoExists { get; set; }
    }
}
