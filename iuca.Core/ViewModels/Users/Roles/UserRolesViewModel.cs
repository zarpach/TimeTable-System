using iuca.Application.DTO.Common;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Users.Roles
{
    public class UserRolesViewModel
    {
        public string ApplicationUserId { get; set; }
        public string FullNameEng { get; set; }
        public string Email { get; set; }
        public OrganizationDTO Organization { get; set; }
        public List<SelectedRole> SelectedRoles { get; set; } = new List<SelectedRole>();
    }

    public class SelectedRole 
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsReadonly { get; set; }
    }
}
