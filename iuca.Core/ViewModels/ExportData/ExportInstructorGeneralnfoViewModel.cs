using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.ExportData
{
    public class ExportInstructorGeneralnfoViewModel
    {
        public int ImportCode { get; set; } 
        public string LastNameEng { get; set; }
        public string FirstNameEng { get; set; }
        public string MiddleNameEng { get; set; }
        public string LastNameRus { get; set; }
        public string FirstNameRus { get; set; }
        public string MiddleNameRus { get; set; }
        public bool Sex { get; set; }
        public string DateOfBirth { get; set; }
        public int? NationalityImportCode { get; set; }
        public bool IsMarried { get; set; }
        public int ChildrenQty { get; set; }
        public int? CitizenshipImportCode { get; set; }
        public string Status { get; set; }
        public int? DepartmentImportCode { get; set; }
        public bool FullPart { get; set; }
    }
}
