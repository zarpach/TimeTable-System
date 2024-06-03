using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using iuca.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Users.UserInfo
{
    public class UserInfoOrgTypesViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public bool IsReadOnly { get; set; }
        public List<OrganizationUserType> OrganizationUserTypes { get; set; } = new List<OrganizationUserType>();
    }

    public class OrganizationUserType
    {
        public OrganizationDTO Organization { get; set; }
        public List<SelectedUserType> SelectedUserTypes { get; set; } = new List<SelectedUserType>();
    }

    public class SelectedUserType 
    {
        public enu_UserType UserType { get; set; }
        public bool IsSelected { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
