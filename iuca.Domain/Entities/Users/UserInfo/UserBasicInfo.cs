using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Users.Staff;
using iuca.Domain.Entities.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.UserInfo
{
    public class UserBasicInfo
    {
        public int Id { get; set; }
        public string LastNameRus { get; set; }
        public string FirstNameRus { get; set; }
        public string MiddleNameRus { get; set; }
        public int Sex { get; set; }
        public DateTime DateOfBirth { get; set; }

        public bool IsMainOrganization { get; set; }

        public string ApplicationUserId { get; set; }

        public int? NationalityId { get; set; }
        public Nationality Nationality { get; set; }

        public int? CitizenshipId { get; set; }
        public Country Citizenship { get; set; }
    }
}
