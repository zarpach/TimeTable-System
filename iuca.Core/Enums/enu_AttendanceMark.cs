using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_AttendanceMark
    {
        [Display(Name = "undefined")]
        undefined = 0,
        [Display(Name = "present")]
        blank = 1,
        [Display(Name = "absent")]
        abs = 2,
        [Display(Name = "sick")]
        sick = 3,
        [Display(Name = "excused")]
        exc = 4,
        [Display(Name = "n/a")]
        na = 5,
        [Display(Name = "late")]
        late = 6
    }
}
