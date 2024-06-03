using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Users.Students
{
    public class StudentGroupInfoViewModel
    {
        public int StudentId { get; set; }
        public int DepartmentImportId { get; set; }
        public string Code { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
        public bool CurrentGroup { get; set; }
    }
}
