using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Enums
{
    public enum enu_AcademicPlanPart
    {
        [Display(Name = "Базовая/обязательная часть / Base/required part")]
        BasePart = 1,
        [Display(Name = "Элективная/вариативная часть / Elective/variable part")]
        ElectivePart = 2
    }
}
