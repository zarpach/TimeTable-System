using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_ProposalCourseStatus
    {
        [Display(Name = "New")]
        New = 1,
        [Display(Name = "Pending")]
        Pending,
        [Display(Name = "Rejected")]
        Rejected,
        [Display(Name = "Approved")]
        Approved
    }
}
