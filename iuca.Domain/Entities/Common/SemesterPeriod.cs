using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Common
{
    public class SemesterPeriod
    {
        public int Id { get; set; }
        public Semester Semester { get; set; }
        public int SemesterId { get; set; }
        public int Period { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }

        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }


    }
}
