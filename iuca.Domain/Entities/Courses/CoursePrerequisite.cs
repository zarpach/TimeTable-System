using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Courses
{
    public class CoursePrerequisite
    {
        public virtual Course Course { get; set; }
        public int CourseId { get; set; }

        public virtual Course Prerequisite { get; set; }
        public int PrerequisiteId { get; set; }
    }
}
