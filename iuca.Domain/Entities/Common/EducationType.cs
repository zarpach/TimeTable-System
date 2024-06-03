using iuca.Domain.Entities.Users.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Common
{
    public class EducationType
    {
        public int Id { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string NameKir { get; set; }
        public int ImportCode { get; set; }
        
        public virtual List<InstructorEducationInfo> InstructorEducationInfo { get; set; }

    }
}
