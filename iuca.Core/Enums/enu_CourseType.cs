using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_CourseType
    {
        [Display(Name = "Standart")]
        Standart = 0,
        [Display(Name = "General")]
        General = 1,
        [Display(Name = "Term paper")]
        TermPaper = 2,
        [Display(Name = "Phys. Ed")]
        PhysEd = 3
    }
}
