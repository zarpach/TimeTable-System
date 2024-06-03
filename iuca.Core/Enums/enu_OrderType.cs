using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_OrderType
    {
        [Display(Name = "Expulsion")]
        Expulsion = 1,
        [Display(Name = "Reinstatement")]
        Reinstatement = 2,
        [Display(Name = "Acad Leave")]
        AcadLeave = 3
    }
}
