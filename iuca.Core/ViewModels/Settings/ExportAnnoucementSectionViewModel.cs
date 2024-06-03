using System.Collections.Generic;

namespace iuca.Application.ViewModels.Settings
{
    public class ExportAnnoucementSectionViewModel
    {
        public int CourseDetId { get; set; }
        public int CourseImportCode { get; set; }
        public int InstructorImportCode { get; set; }
        public int Season { get; set; }
        public int Year { get; set; }
        public string Section { get; set; }
        public float Points { get; set; }
        public List<int> ExtraInstructors { get; set; } = new List<int>();
    }
}
