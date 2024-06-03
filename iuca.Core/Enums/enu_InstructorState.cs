using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Enums
{
    public enum enu_InstructorState
    {
        [Display(Name = "Not set")]
        NotSet = 0,
        [Display(Name = "Active")]
        Active = 1,
        [Display(Name = "Inactive")]
        Inactive = 2
    }
}
