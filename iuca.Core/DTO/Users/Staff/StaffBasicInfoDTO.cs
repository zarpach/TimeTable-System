using iuca.Application.DTO.Users.UserInfo;
using iuca.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.Staff
{
    public class StaffBasicInfoDTO
    {
        public int Id { get; set; }
        public string StaffInfo { get; set; }
        public bool IsMainOrganization { get; set; }

        public string ApplicationUserId { get; set; }
    }
}
