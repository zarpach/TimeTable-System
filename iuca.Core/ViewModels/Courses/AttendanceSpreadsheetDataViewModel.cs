using System.Collections.Generic;

namespace iuca.Application.ViewModels.Courses
{
    public class AttendanceSpreadsheetDataViewModel
    {
        public int AnnouncementId { get; set; }
        public string CourseId { get; set; }
        public string CourseAbbreviation { get; set; }
        public string CourseNumber { get; set; }
        public string CourseName { get; set; }
        public string Season { get; set; }
        public string Year { get; set; }

        public List<AttendanceSheetDataViewModel> AttendanceSheetsData { get; set; }
    }

    public class AttendanceSheetDataViewModel
    {
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Section { get; set; }
        public string Instructor { get; set; }

        public List<StudentInfoViewModel> StudentsData { get; set; }
    }
}
