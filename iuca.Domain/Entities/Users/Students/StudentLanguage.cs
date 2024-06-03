using iuca.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.Students
{
    public class StudentLanguage
    {
        public StudentBasicInfo StudentBasicInfo { get; set; }
        public int StudentBasicInfoId { get; set; }
        public Language Language { get; set; }
        public int LanguageId { get; set; } 
    }
}
