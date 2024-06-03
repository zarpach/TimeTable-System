using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Users.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.Instructors
{
    public class AdviserStudent
    {
        public string InstructorUserId { get; set; }
        public string StudentUserId { get; set; }
        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }
    }
}
