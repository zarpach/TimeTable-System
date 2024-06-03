using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Enums
{
    public enum enu_RegistrationState
    {
        [Display(Name = "Не указан")]
        NotSpecified = 0,
        [Display(Name = "Не отправлена")]
        NotSent = 1,
        [Display(Name = "На утверждении")]
        OnApproval,
        [Display(Name = "Не одобрена")]
        NotApproved,
        [Display(Name = "Одобрена")]
        Approved,
        [Display(Name = "Завершена")]
        Submitted
    }
}
