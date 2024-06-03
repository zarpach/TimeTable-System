using iuca.Application.DTO.Common;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Users.Students
{
    public class StudentMinorInfoDTO
    {
        public int Id { get; set; }

        [Display(Name = "Student Basic Info")]
        public StudentBasicInfoDTO StudentBasicInfo { get; set; }

        [Display(Name = "Student Basic Info")]
        public int StudentBasicInfoId { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Department")]
        public DepartmentDTO Department { get; set; }
    }
}
  