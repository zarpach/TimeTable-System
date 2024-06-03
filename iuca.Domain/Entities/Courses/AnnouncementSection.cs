using iuca.Domain.Entities.Common;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Courses
{
    public class AnnouncementSection : AuditableEntity
    {
        public int Id { get; set; }
        public Announcement Announcement { get; set; }
        public int AnnouncementId { get; set; }
        public string InstructorUserId { get; set; }
        public Syllabus Syllabus { get; set; }
        public int Places { get; set; }
        public string Schedule { get; set; }
        public float Credits { get; set; }
        public string Section { get; set; }
        public bool GradeSheetSubmitted { get; set; }

        public IEnumerable<string> ExtraInstructorsJson { get; set; }
        public IEnumerable<string> GroupsJson { get; set; }

        public virtual List<StudentCourseTemp> StudentCourses { get; set; }
        public virtual List<StudyCardCourse> StudyCardCourses { get; set; }
        public virtual List<Attendance> Attendances { get; set; }

        // to delete
        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        public int Season { get; set; }
        public int Year { get; set; }
        public bool IsChanged { get; set; }
        public bool MarkedDeleted { get; set; }
        public int CourseDetId { get; set; }

        public virtual List<ExtraInstructor> ExtraInstructors { get; set; } = new List<ExtraInstructor>();
    }
}
