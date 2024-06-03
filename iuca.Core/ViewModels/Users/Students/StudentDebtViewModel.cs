
namespace iuca.Application.ViewModels.Users.Students
{
    public class StudentDebtViewModel
    {
        public int DebtId { get; set; }
        public int DebtType { get; set; }
        public string StudentUserId { get; set; }
        public string StudentName { get; set; }
        public int StudentId { get; set; }
        public string StudentMajor { get; set; }
        public string StudentGroup { get; set; }
        public int SemesterId { get; set; }
        public string Comment { get; set; }
        public bool IsDebt { get; set; } = true;
        public float DebtAmount { get; set; }
    }
}
