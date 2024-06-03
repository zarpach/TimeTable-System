using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.Instructors
{
    public class InstructorOtherJobInfo
    {
        public int Id { get; set; }
        public string PlaceNameEng { get; set; }
        public string PlaceNameRus { get; set; }
        public string PlaceNameKir { get; set; }
        public string PositionEng { get; set; }
        public string PositionRus { get; set; }
        public string PositionKir { get; set; }
        public string Phone { get; set; }
        public int InstructorBasicInfoId { get; set; }
        public InstructorBasicInfo instructorBasicInfo { get; set; }
    }
}
