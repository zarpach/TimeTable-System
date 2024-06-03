using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_MidtermReportSorting
    {
        [Display(Name = "Group")]
        Group = 0,
        [Display(Name = "Student name")]
        StudentName,
        [Display(Name = "Attention")]
        Attention
    }
}
