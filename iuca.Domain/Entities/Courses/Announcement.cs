using iuca.Domain.Entities.Common;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Courses
{
    public class Announcement : AuditableEntity
    {
        public int Id { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public Semester Semester { get; set; }
        public int SemesterId { get; set; }
        public bool IsForAll { get; set; }
        public bool IsActivated { get; set; }
        public string AttendanceSpreadsheetId { get; set; }

        public virtual IEnumerable<AnnouncementSection> AnnouncementSections { get; set; }
    }
}
