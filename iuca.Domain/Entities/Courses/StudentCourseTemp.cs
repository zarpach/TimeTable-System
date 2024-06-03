using iuca.Domain.Entities.Common;
using System;

namespace iuca.Domain.Entities.Courses
{
    public class StudentCourseTemp
    {
        public int Id { get; set; }
        public StudentCourseRegistration StudentCourseRegistration { get; set; }
        public int StudentCourseRegistrationId { get; set; }
        public AnnouncementSection AnnouncementSection { get; set; }
        public int AnnouncementSectionId { get; set; }
        public int? GradeId { get; set; }
        public Grade Grade { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
        public bool IsPassed { get; set; }
        public int State { get; set; }
        public bool IsAddDropApproved { get; set; }
        public bool IsAddDropProcessed { get; set; }
        public bool MarkedDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsAudit { get; set; }

        public virtual StudentMidterm StudentMidterm { get; set; }
    }
}
