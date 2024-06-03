using iuca.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.UserInfo
{
    public class DeanAdviser
    {
        public string DeanUserId { get; set; }
        public string AdviserUserId { get; set; }
        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }
    }
}
