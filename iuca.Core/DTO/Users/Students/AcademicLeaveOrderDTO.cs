using iuca.Application.ViewModels.Users.Students;
using System;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Users.Students
{
    public class AcademicLeaveOrderDTO
    {
        public int Id { get; set; }

        [Display(Name = "Student user id")]
        public string StudentUserId { get; set; }

        [Display(Name = "Student info")]
        public StudentMinimumInfoViewModel StudentInfo { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Number")]
        public int Number { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; } = new DateTime(DateTime.Today.Year, 01, 01);

        [Display(Name = "Reason")]
        public int Reason { get; set; }

        [Display(Name = "Comment")]
        [MaxLength(255)]
        public string Comment { get; set; }

        [Display(Name = "Start date")]
        public DateTime Start { get; set; } = new DateTime(DateTime.Today.Year, 01, 01);

        [Display(Name = "End date")]
        public DateTime End { get; set; } = new DateTime(DateTime.Today.Year, 01, 01);

        [Display(Name = "Application status")]
        public bool IsApplied { get; set; }
    }
}