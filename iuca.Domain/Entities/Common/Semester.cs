using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Students;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Common
{
    public class Semester
    {
        public int Id { get; set; }
        public int Season { get; set; }
        public int Year { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public virtual List<OldStudyCard> OldStudyCards { get; set; }
        public virtual List<StudyCard> StudyCards { get; set; }
        public virtual List<SemesterPeriod> SemesterPeriods { get; set; }
        public virtual List<StudentCourseRegistration> StudentCourseRegistrations { get; set; }
        public virtual List<StudentDebt> StudentDebts { get; set; }
        public virtual IEnumerable<Proposal> Proposals { get; set; }
        public virtual IEnumerable<Announcement> Announcements { get; set; }
        public virtual List<GroupTransferOrder> GroupTransferOrders { get; set; }
        public virtual List<AttendanceFolder> AttendanceFolders { get; set; }
    }
}
