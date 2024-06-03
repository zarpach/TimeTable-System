using iuca.Application.DTO.Common;
using iuca.Application.Enums;

namespace iuca.Application.ViewModels.Courses
{
    public class MidtermStatisticsReportViewModel
    {
        public DepartmentDTO Department { get; set; }
        public int CountCourses { get; set; }
        public int CountMidterms { get; set; }
        public enu_MidtermReportState State { get; set; }
    }
}
