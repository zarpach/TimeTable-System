using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Users.Roles
{
    public class PermissionViewModel
    {
        public string RoleNamePrefix { get; set; }
        public List<RoleClaimsViewModel> RoleClaims { get; set; }
    }

    public class RoleClaimsViewModel 
    {
        public string ModuleName { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}
