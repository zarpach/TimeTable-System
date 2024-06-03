using iuca.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Settings
{
    public class ExportCourseViewModel
    {
        public int StudentCourseId { get; set; }
        public int StudentId { get; set; }
        public int CourseDetId { get; set; }
        public int GradeImportCode { get; set; }
        public enu_CourseState State { get; set; }
        public bool IsDismissed { get; set; }
        public bool IsDeleted { get; set; }
        public int ProgramId { get; set; }
    }
}
