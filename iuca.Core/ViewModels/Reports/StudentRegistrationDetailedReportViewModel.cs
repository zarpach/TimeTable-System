using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Reports
{
    public class StudentRegistrationDetailedReportViewModel
    {
        public SemesterDTO Semester { get; set; }
        public string ReportDate { get; set; }
        public string DeanName { get; set; }
        public List<StudentReportRowViewModel> StudentReportRows { get; set; } = new List<StudentReportRowViewModel>();
    }

    public class StudentReportRowViewModel 
    {
        public string Department { get; set; }
        public string StudentName { get; set; }
        public string Group { get; set; }
        public enu_RegistrationState RegistrationState { get; set; }
        public enu_RegistrationState AddDropState { get; set; }
        public string AdviserName { get; set; }

    }
}
