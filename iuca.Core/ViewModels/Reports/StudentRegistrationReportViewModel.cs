using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Reports
{
    public class StudentRegistrationReportViewModel
    {
        public SemesterDTO Semester { get; set; }
        public string ReportDate { get; set; }
        public List<DepartmentRegistrationReportViewModel> DepartmentRegistrations { get; set; } 
            = new List<DepartmentRegistrationReportViewModel>();
    }

    public class DepartmentRegistrationReportViewModel 
    {
        public DepartmentDTO Department { get; set;}
        public int TotalActiveStudents { get; set; }
        public int SubmittedRegistrations { get; set; }
        public int NotSubmittedRegistrtations { get; set; }
        public int NotSentRegistrtations { get; set; }
    }
}
