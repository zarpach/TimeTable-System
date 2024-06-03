using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_SyllabusStatus
    {
        [Display(Name = "Draft")]
        Draft = 1,
        [Display(Name = "Pending")]
        Pending = 2,
        [Display(Name = "Rejected")]
        Rejected = 3,
        [Display(Name = "Approved")]
        Approved = 4
    }
}
