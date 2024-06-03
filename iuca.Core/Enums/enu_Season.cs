using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_Season
    {
        [Display(Name = "Осенний (Fall)")]
        Fall = 1,
        [Display(Name = "Зимний (Winter)")]
        Winter,
        [Display(Name = "Весенний (Spring)")]
        Spring,
        [Display(Name = "Летний (Summer)")]
        Summer
    }
}
