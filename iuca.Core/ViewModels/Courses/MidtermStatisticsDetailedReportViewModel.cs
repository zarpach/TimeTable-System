using iuca.Application.DTO.Courses;
using iuca.Application.Enums;

namespace iuca.Application.ViewModels.Courses
{
    public class MidtermStatisticsDetailedReportViewModel
    {
        public AnnouncementSectionDTO AnnouncementSection { get; set; }
        public int CountStudents { get; set; }
        public int CountMidterms { get; set; }
        public enu_MidtermReportState State { get; set; }
    }
}
