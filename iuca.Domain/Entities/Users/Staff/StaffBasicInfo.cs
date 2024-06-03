using iuca.Domain.Entities.Users.UserInfo;

namespace iuca.Domain.Entities.Users.Staff
{
    public class StaffBasicInfo
    {
        public int Id { get; set; }
        public string StaffInfo { get; set; }
        public bool IsMainOrganization { get; set; }

        public string ApplicationUserId { get; set; }

    }
}
