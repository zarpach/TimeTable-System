using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.Students
{
    public class StudentParentsInfoDTO
    {
        public int Id { get; set; }
        
        [Display(Name = "Student basic info")]
        public StudentBasicInfoDTO StudentBasicInfo { get; set; }

        [Display(Name = "Student basic info")]
        public int StudentBasicInfoId { get; set; }

        [Display(Name = "Last name")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Display(Name = "First name")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Display(Name = "Middle name")]
        [MaxLength(100)]
        public string MiddleName { get; set; }

        [Display(Name = "Phone")]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Display(Name = "Work place")]
        [MaxLength(100)]
        public string WorkPlace { get; set; }

        [Display(Name = "Relation")]
        [MaxLength(50)]
        public string Relation { get; set; }

        [Display(Name = "Dead year")]
        public int? DeadYear { get; set; }
    }
}
