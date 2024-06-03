using System;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Students;
using iuca.Domain.Entities.Users.UserInfo;

namespace iuca.Domain.Entities.Slots
{
    public class Slot : AuditableEntity
    {
        public Guid Id { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public int GroupId { get; set; }
        public DepartmentGroup Group { get; set; }

        public string InstructorUserId { get; set; }
        //public ApplicationUser InstructorUser { get; set; }

        public int DayOfWeek { get; set; }

        public int LessonPeriodId { get; set; }
        public LessonPeriod LessonPeriod { get; set; }

        public Guid LessonRoomId { get; set; }
        public LessonRoom LessonRoom { get; set; }

        public int SemesterId { get; set; }
        public Semester Semester { get; set; }

        public int AnnouncementSectionId { get; set; }
        public AnnouncementSection AnnouncementSection { get; set; }
    }
}

