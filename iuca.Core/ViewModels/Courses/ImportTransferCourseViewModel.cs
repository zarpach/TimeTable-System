using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Courses
{
    public class ImportTransferCourseViewModel
    {
        public string StudentUserId { get; set; }
        public int UniversityId { get; set; }
        public int Year { get; set; }
        public int Season { get; set; }
        public string NameEng { get; set; }
    }
}
