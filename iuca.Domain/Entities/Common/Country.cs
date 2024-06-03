using iuca.Domain.Entities.Users.UserInfo;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Common
{
    public class Country
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string NameKir { get; set; }
        public int SortNum { get; set; }
        public int ImportCode { get; set; }

        public virtual List<UserBasicInfo> UserBasicInfo { get; set; }
        public virtual List<University> Universities { get; set; }
        public virtual List<InstructorContactInfo> InstructorCountries { get; set; }
        public virtual List<InstructorContactInfo> InstructorCitizenshipCountries { get; set; }
        public virtual List<StudentContactInfo> StudentContactInfo { get; set; }
        public virtual List<StudentContactInfo> CitizenshipStudentContactInfo { get; set; }
    }
}
