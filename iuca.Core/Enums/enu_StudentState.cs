using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Enums
{
    public enum enu_StudentState
    {
        [Display(Name = "Not assigned")]
        NotAssigned = 0,
        [Display(Name = "Active")]
        Active = 1,
        [Display(Name = "Dismissed")]
        Dismissed = 2,
        [Display(Name = "Acad. leave")]
        AcadLeave = 3,
        [Display(Name = "Graduate")]
        Graduate = 6,
        [Display(Name = "Completed")]
        Completed = 8
    }
}
