using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Instructors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Common
{
    public class University
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string NameKir { get; set; }
        public int? CountryId { get; set; }
        public Country Country { get; set; }
        public int ImportCode { get; set; }

        public virtual List<InstructorEducationInfo> InstructorEducationInfo { get; set; }
        public virtual List<TransferCourse> TransferCourses { get; set; }

    }
}
