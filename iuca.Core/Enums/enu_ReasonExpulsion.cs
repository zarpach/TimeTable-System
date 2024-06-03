using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_ReasonExpulsion
    {
        [Display(Name = "По собственному желанию")]
        OwnWish = 0,

        [Display(Name = "Перевод в другой ВУЗ")]
        TransferToAnotherUniversity,

        [Display(Name = "Потеря связи")]
        LossOfContact,

        [Display(Name = "Отсутствие регистрации на курсы")]
        CourseRegistrationMissing,

        [Display(Name = "Академическая неуспеваемость")]
        AcademicUnderperformance,

        [Display(Name = "Нарушение академических правил и дисциплины")]
        AcademicMisconduct,

        [Display(Name = "Нарушение финансовых обязательств")]
        FinancialObligationsViolation,

        [Display(Name = "Смерть")]
        Death,

        [Display(Name = "Другие причины")]
        OtherReason
    }
}
