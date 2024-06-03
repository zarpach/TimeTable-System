using System;
using System.Collections.Generic;

namespace iuca.Domain.Entities.Courses
{
    public class Syllabus
    {
        public int Id { get; set; }
        public AnnouncementSection AnnouncementSection { get; set; }
        public int AnnouncementSectionId { get; set; }
        public string InstructorPhone { get; set; }
        public string OfficeHours { get; set; }
        public string CourseDescription { get; set; }

        public string Objectives { get; set; }
        public string TeachMethods { get; set; }
        public string PrimaryResources { get; set; }
        public string AdditionalResources { get; set; }

        public string Link { get; set; }
        public string GradingComment { get; set; }

        public string InstructorComment { get; set; }
        public string ApproverComment { get; set; }
        public int Language { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual List<CourseRequirement> CourseRequirements { get; set; }

        public virtual List<AcademicPolicy> AcademicPolicies { get; set; }
        public virtual List<CourseCalendarRow> CourseCalendar { get; set; }
    }
}
