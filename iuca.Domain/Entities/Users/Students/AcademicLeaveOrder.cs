using iuca.Domain.Entities.Common;
using System;

namespace iuca.Domain.Entities.Users.Students
{
    public class AcademicLeaveOrder : AuditableEntity
    {
        public int Id { get; set; }
        public string StudentUserId { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public int Reason { get; set; }
        public string Comment { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsApplied { get; set; }
    }
}