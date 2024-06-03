using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.Students
{
    public class StudentParentsInfo
    {
        public int Id { get; set; }
        public StudentBasicInfo StudentBasicInfo { get; set; }
        public int StudentBasicInfoId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string WorkPlace { get; set; }
        public string Relation { get; set; }
        public int? DeadYear { get; set; }
    }
}
