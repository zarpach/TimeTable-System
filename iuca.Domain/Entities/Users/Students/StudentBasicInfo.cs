using System.Collections.Generic;

namespace iuca.Domain.Entities.Users.Students
{
    public class StudentBasicInfo
    {
        public int Id { get; set; }
        public bool IsMainOrganization { get; set; }

        public string ApplicationUserId { get; set; }

        public bool ArmyService { get; set; }
        public int Toefl { get; set; }

        public virtual List<StudentOrgInfo> StudentOrgInfo { get; set; }
        public virtual StudentContactInfo StudentContactInfo { get; set; }
        public virtual List<StudentLanguage> StudentLanguages { get; set; }
        public virtual List<StudentParentsInfo> StudentParentsInfo { get; set; }
        public virtual List<StudentMinorInfo> StudentMinorInfo { get; set;}
    }
}
