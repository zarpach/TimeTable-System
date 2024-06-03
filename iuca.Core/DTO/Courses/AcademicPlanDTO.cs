using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class AcademicPlanDTO
    {
        public int Id { get; set; }
        
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Organization")]
        public OrganizationDTO Organization { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Department")]
        public DepartmentDTO Department { get; set; }

        public virtual List<CyclePartDTO> CycleParts { get; set; }
    }
}
