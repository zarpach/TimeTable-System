using iuca.Domain.Entities.Common;

namespace iuca.Domain.Entities.Users.Students
{
    public class StudentDebt
    {
        public int Id { get; set; }
        public int DebtType { get; set; }
        public string StudentUserId { get; set; }
        public Semester Semester { get; set; }
        public int SemesterId { get; set; }
        public bool IsDebt { get; set; } = true;
        public string Comment { get; set; }
        public float DebtAmount { get; set; }
    }
}
