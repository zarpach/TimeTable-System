using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_ReasonAcadLeave
    {
        [Display(Name = "По состоянию здоровья")]
        HealthCondition = 0,

        [Display(Name = "По семейным обстоятельствам")]
        FamilyCircumstances,

        [Display(Name = "Другие причины")]
        OtherReason
    }
}
