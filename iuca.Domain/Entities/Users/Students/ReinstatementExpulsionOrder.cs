using iuca.Domain.Entities.Common;
using System;

namespace iuca.Domain.Entities.Users.Students
{
    public class ReinstatementExpulsionOrder : AuditableEntity
    {
        public int Id { get; set; }
        public string StudentUserId { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public int Reason { get; set; }
        public string Comment { get; set; }
        public bool IsApplied { get; set; }
    }
}
