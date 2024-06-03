using iuca.Application.Enums;
using iuca.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Users.UserInfo
{
    public class DeanAdviserViewModel
    {
        public ApplicationUser Instructor { get; set; }
        public enu_InstructorState State { get; set; }
    }
}
