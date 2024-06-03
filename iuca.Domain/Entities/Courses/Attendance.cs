using System.Collections.Generic;

namespace iuca.Domain.Entities.Courses
{
    public class Attendance : AuditableEntity
    {
        public int Id { get; set; }
        public string StudentUserId { get; set; }
        public AnnouncementSection AnnouncementSection { get; set; }
        public int AnnouncementSectionId { get; set; }

        public virtual IEnumerable<AttendanceClass> AttendanceClasses { get; set; }
    }
}
