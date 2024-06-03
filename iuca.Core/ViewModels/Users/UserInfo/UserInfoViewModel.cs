using iuca.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Users.UserInfo
{
    public class UserInfoViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsStaff { get; set; }
        public bool IsStudent { get; set; }
        public bool IsInstructor { get; set; }
    }
}
