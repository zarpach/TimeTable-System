
namespace iuca.Application.ViewModels.Users.Students
{
    public class StudentMinimumInfoViewModel
    {
        public string StudentUserId { get; set; }
        public string FullNameEng { get; set; }
        public string ShortNameEng { get; set; }
        public int StudentId { get; set; }
        public string Group { get; set; }

        public string StudentInfo
        {
            get { return $"{StudentId} {FullNameEng}  ({Group})"; }
        }
    }
}
