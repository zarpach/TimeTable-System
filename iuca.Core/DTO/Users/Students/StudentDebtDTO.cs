using iuca.Application.DTO.Common;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Users.Students
{
    public class StudentDebtDTO
    {
        public int Id { get; set; }

        public int DebtType { get; set; }

        [Display(Name = "Student")]
        public string StudentUserId { get; set; }

        [Display(Name = "Semester")]
        public SemesterDTO Semester { get; set; }

        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        [Display(Name = "Is Debt")]
        public bool IsDebt { get; set; } = true;

        [Display(Name = "Is Debt")]
        public string Comment { get; set; }

        [Display(Name = "DebtAmount")]
        public int DebtAmount { get; set; }
    }
}
