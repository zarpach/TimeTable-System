using iuca.Application.DTO.Users.UserInfo;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using iuca.Application.ViewModels.Users.Students;
using iuca.Application.DTO.Common;

namespace iuca.Application.DTO.Users.Students
{
    public class GroupTransferOrderDTO
    {
        public int Id { get; set; }

        [Display(Name = "Student user id")]
        public string StudentUserId { get; set; }

        [Display(Name = "Student info")]
        public StudentMinimumInfoViewModel StudentInfo { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        [Display(Name = "Semester")]
        public SemesterDTO Semester { get; set; }

        [Display(Name = "Number")]
        public int Number { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; } = new DateTime(DateTime.Today.Year, 01, 01);

        [Display(Name = "Comment")]
        [MaxLength(255)]
        public string Comment { get; set; }

        [Display(Name = "Sourse group")]
        public int SourceGroupId { get; set; }

        [Display(Name = "Sourse group")]
        public DepartmentGroupDTO SourceGroup { get; set; }

        [Display(Name = "Target group")]
        public int TargetGroupId { get; set; }

        [Display(Name = "Target group")]
        public DepartmentGroupDTO TargetGroup { get; set; }

        [Display(Name = "Application status")]
        public bool IsApplied { get; set; }

        [Display(Name = "Previous advisors")]
        public IEnumerable<string> PreviousAdvisorsJson { get; set; }
        public IEnumerable<UserDTO> PreviousAdvisors { get; set; }

        [Display(Name = "Future advisors")]
        public IEnumerable<string> FutureAdvisorsJson { get; set; }
        public IEnumerable<UserDTO> FutureAdvisors { get; set; }
    }
}
