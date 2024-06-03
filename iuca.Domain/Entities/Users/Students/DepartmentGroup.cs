using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.Students
{
    public class DepartmentGroup
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public int Year { get; set; }
        public string Code { get; set; }

        public virtual List<OldStudyCard> OldStudyCards { get; set; }
        public virtual List<StudyCard> StudyCards { get; set; }
        public virtual List<StudentOrgInfo> StudentOrgInfo { get; set; }
        public virtual List<StudentOrgInfo> PrepStudentOrgInfo { get; set; }
        public virtual List<GroupTransferOrder> SourseGroupTransferOrders { get; set; }
        public virtual List<GroupTransferOrder> TargetGroupTransferOrders { get; set; }
    }
}
