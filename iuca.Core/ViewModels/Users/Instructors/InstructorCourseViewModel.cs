using System.Collections.Generic;

namespace iuca.Application.ViewModels.Users.Instructors
{
    public class InstructorCourseViewModel
    {
        public int AnnouncementSetcionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Section { get; set; }
        public int ImportCode { get; set; }
        public float Points { get; set; }
        public int Places { get; set; }
        public int StudentCount { get; set; }
        public string Schedule { get; set; }
        public string InstructorUserId { get; set; }
        public string InstructorName { get; set; }
        public int SyllabusStatus { get; set; }
        public bool GradeSheetSubmitted { get; set; }
        public string AttendanceSpreadsheetId { get; set; }

        public List<string> ExtraInstructorIds { get; set; } = new List<string>();
    }
}
