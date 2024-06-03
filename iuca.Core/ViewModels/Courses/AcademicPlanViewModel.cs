using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Courses
{
    public class AcademicPlanViewModel
    {
        public AcademicPlanDTO AcademicPlan { get; set; }
        public List<CycleDTO> Cycles { get; set; }
        public List<enu_AcademicPlanPart> Parts { get; set; }
    }
}
