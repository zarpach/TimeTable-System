using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_ReasonReinstatement
    {
        [Display(Name = "Причин нет")]
        NoReason = 0,

        [Display(Name = "Возврат с академического отпуска")]
        ReturnFromAcademicLeave,

        [Display(Name = "Ошибка при отчислении")]
        ErrorDuringDismissal,

        [Display(Name = "Другие причины")]
        OtherReason
    }
}
