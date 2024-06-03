using iuca.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class AcademicPlan
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public virtual List<CyclePart> CycleParts { get; set; }
    }
}
