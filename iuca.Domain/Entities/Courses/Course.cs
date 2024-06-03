using iuca.Domain.Entities.Common;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Courses
{
    public class Course : AuditableEntity
    {
        public int Id { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public string NameKir { get; set; }
        public string Abbreviation { get; set; }
        public string Number { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int LanguageId { get; set; }
        public Language Language { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int ImportCode { get; set; }
        public int SortNum { get; set; }
        public bool IsChanged { get; set; }
        public int CourseType { get; set; }

        public virtual List<CoursePrerequisite> CoursePrerequisites { get; set; }
        public virtual List<CoursePrerequisite> Prerequisites { get; set; }
        public virtual List<CyclePartCourse> CyclePartCourses { get; set; }
        public virtual List<StudentCourseGrade> StudentCourseGrades { get; set; }
        public virtual List<AnnouncementSection> AnnouncementSections { get; set; }
        public virtual IEnumerable<ProposalCourse> ProposalCourses { get; set; }
        public virtual IEnumerable<Announcement> Announcements { get; set; }
    }
}
