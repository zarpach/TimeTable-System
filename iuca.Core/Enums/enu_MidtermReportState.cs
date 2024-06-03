using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_MidtermReportState
    {
        [Display(Name = "Not selected")]
        NotSelected = -1,
        [Display(Name = "Not started")]
        NotStarted = 0,
        [Display(Name = "In progress")]
        InProgress,
        [Display(Name = "Submitted")]
        Submitted
    }
}
