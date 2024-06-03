using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Students;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Common
{
    public class Language
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string NameKir { get; set; }
        public int SortNum { get; set; }
        public int ImportCode { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
        public virtual List<StudentLanguage> StudentLanguages { get; set; }
    }
}
