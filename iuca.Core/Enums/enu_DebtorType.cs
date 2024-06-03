using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Enums
{
    public enum enu_DebtorType
    {
        [Display(Name = "Все")]
        All = 0,
        [Display(Name = "Есть задолженность")]
        Debtor,
        [Display(Name = "Нет задолженности")]
        NotDebtor
    }
}
