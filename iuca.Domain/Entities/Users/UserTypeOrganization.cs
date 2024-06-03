using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.UserInfo;

namespace iuca.Domain.Entities.Users
{
    public class UserTypeOrganization
    {
        public string ApplicationUserId { get; set; }
        public int UserType { get; set; }
        public virtual Organization Organization { get; set; }
        public int OrganizationId { get; set; }
        
    }
}
