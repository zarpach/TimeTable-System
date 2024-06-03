using iuca.Application.DTO.Common;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Users.Students
{
    public class DepartmentGroupDTO
    {
        public int Id { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [Display(Name = "Department")]
        public DepartmentDTO Department { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }
        [Display(Name = "Organization")]
        public OrganizationDTO Organization { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        public string DepartmentCode
        {
            get 
            {
                return Department != null ? $"{Department.Code}{Code}" : Code;
            }
        }
    }
}
