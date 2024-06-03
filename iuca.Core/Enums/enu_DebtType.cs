using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Enums
{
    public enum enu_DebtType
    {
        [Display(Name = "Бухгалтерия")]
        Accounting = 1,
        [Display(Name = "Библиотека")]
        Library,
        [Display(Name = "Общежитие")]
        Dormitory,
        [Display(Name = "Учебный отдел")]
        RegistarOffice,
        [Display(Name = "Медицинский офис")]
        MedicineOffice
    }
}
