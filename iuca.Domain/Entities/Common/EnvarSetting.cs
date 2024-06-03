
using System;

namespace iuca.Domain.Entities.Common
{
    public class EnvarSetting
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int MaxRegistrationCredits { get; set; }
        public string DefaultInstructor { get; set; }
        public int CurrentSemester { get; set; }
        public int UpcomingSemester { get; set; }
        public DateTime LastAttendanceUpdate { get; set; }
    }
}
