using iuca.Domain.Entities.Common;
using System;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Users.Students
{
    public class GroupTransferOrder : AuditableEntity
    {
        public int Id { get; set; }
        public string StudentUserId { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int SemesterId { get; set; }
        public Semester Semester { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public int SourceGroupId { get; set; }
        public DepartmentGroup SourceGroup { get; set; }
        public int TargetGroupId { get; set; }
        public DepartmentGroup TargetGroup { get; set; }
        public bool IsApplied { get; set; }

        public IEnumerable<string> PreviousAdvisorsJson { get; set; }
        public IEnumerable<string> FutureAdvisorsJson { get; set; }
    }
}
