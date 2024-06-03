using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class Cycle
    {
        public int Id { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string NameKir { get; set; }
        public string Code { get; set; }

        public virtual List<CyclePart> CycleParts { get; set; }
    }
}
