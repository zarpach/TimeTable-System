using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Domain.Entities.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Reports
{
    public class GradeReportViewModel
    {
        public string CourseName { get; set; }
        public int AnnouncementSectionId { get; set; }
        public int CourseImportCode { get; set; }
        public string InstructorName { get; set; }
        public float Credits { get; set; }
        public bool GradeSheetSubmitted { get; set; }
        public List<GradeReportStudentRow> Students { get; set; } = new List<GradeReportStudentRow>();
    }

    public class GradeReportStudentRow 
    {
        public string StudentUserId { get; set; }
        public string StudentName { get; set; }
        public string StudentGroup { get; set; }
        public int StudentId { get; set; }
        public enu_StudentState StudentState { get; set; }
        public int? GradeId { get; set; }
    }
}
