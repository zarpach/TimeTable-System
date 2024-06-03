using iuca.Application.DTO.Courses;

namespace iuca.Application.ViewModels.Reports
{
    public class RegistrationCourseReportViewModel
    {
        public AnnouncementSectionDTO AnnouncementSection { get; set; }
        public string InstructorName { get; set; }
        public int TotalStudents { get; set; }
        public int TotalAudits { get; set; }
        public int NotSubmittedRegistration { get; set; }
    }
}
