using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Users.Roles
{
    public class RoleViewModel
    {
        public string RoleName { get; set; }
        public List<string> OrganizationIds { get; set; }
        public List<string> RoleIds { get; set; }
    }
}
