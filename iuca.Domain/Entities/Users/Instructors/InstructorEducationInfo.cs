using iuca.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.Instructors
{
    public class InstructorEducationInfo
    {
        public int Id { get; set; }
        public string MajorEng { get; set; }
        public string MajorRus { get; set; }
        public string MajorKir { get; set; }
        public int GraduateYear { get; set; }
        public InstructorBasicInfo InstructorBasicInfo { get; set; }
        public int InstructorBasicInfoId { get; set; }
        public University University { get; set; }
        public int? UniversityId { get; set; }
        public EducationType EducationType { get; set; }
        public int? EducationTypeId { get; set; }

    }
}
