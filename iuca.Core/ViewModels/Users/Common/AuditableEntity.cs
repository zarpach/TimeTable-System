using System;
using System.ComponentModel.DataAnnotations;

namespace iuca.Domain
{
    public class AuditableEntity
    {
        [ScaffoldColumn(false)]
        public DateTime DateCreated { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? LastModified { get; set; }
        [ScaffoldColumn(false)]
        public string CreatedById { get; set; }
        [ScaffoldColumn(false)]
        public string ModifiedById { get; set; }
        [ScaffoldColumn(false)]
        public bool IsDeleted { get; set; }
    }
}
