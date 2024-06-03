using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.ViewModels.Courses
{
    public class StudyCardSelectionCourseViewModel
    {
        public int AcademicPlanId { get; set; }
        public CyclePartDTO CylcePart { get; set; }
        public CourseDTO Course { get; set; }
        public int CyclePartCourseId { get; set; }
        public int Points { get; set; }
        public enu_CourseSelectionStatus SelectionStatus { get; set; }
        public string Comment { get; set; }
    }
}
