using iuca.Application.Enums;

namespace iuca.Application.ViewModels.Users.Instructors
{
    public class InstructorInfoBriefViewModel
    {
        public string InstructorUserId { get; set; }
        public int InstructorBasicInfoId { get; set; }
        public string FullNameEng { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public enu_InstructorState State { get; set; }
        public bool IsReadOnly { get; set; }
        public bool BasicInfoExists { get; set; }
        public int ImportCode { get; set; }
        public bool IsChanged { get; set; }
    }
}
