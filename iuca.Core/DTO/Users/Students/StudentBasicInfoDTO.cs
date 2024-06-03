using iuca.Application.DTO.Users.UserInfo;
using iuca.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.Students
{
    public class StudentBasicInfoDTO
    {
        public int Id { get; set; }
        public bool IsMainOrganization { get; set; }

        public string ApplicationUserId { get; set; }

        public bool ArmyService { get; set; }
        public int Toefl { get; set; }

        public virtual StudentContactInfoDTO StudentContactInfo { get; set; }
        public virtual List<StudentLanguageDTO> StudentLanguages { get; set; }
        public virtual List<StudentParentsInfoDTO> StudentParentsInfo { get; set; }
        public virtual List<StudentOrgInfoDTO> StudentOrgInfo { get; set; }
    }
}
