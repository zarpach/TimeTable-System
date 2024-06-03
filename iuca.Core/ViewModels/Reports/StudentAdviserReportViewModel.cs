using iuca.Application.Enums;
using iuca.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Reports
{
    public class DeanAdviserStudentReportViewModel
    {
        public ApplicationUser Dean { get; set; }
        public List<AdviserStudentRowViewModel> AdviserStudents { get; set; } = new List<AdviserStudentRowViewModel>();
    }

    public class AdviserStudentRowViewModel
    {
        public ApplicationUser Adviser { get; set; }
        public List<StudentRowViewModel> Students { get; set; } = new List<StudentRowViewModel>();
    }

    public class StudentRowViewModel 
    {
        public ApplicationUser Student { get; set; }
        public string DepartmentGroup { get; set; }
        public enu_StudentState State { get; set; }
    }
}
