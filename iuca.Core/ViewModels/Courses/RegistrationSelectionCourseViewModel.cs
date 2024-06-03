using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using System.Collections.Generic;


namespace iuca.Application.ViewModels.Courses
{
    public class RegistrationSelectionCourseViewModel
    {
        public int SelectedCoursesNumber { get; set; }
        public int TotalPoints { get; set; }
        public int TotalNoCreditsCount { get; set; }
        public bool ElectiveSelected { get; set; }
        public List<RegistrationCourseRowViewModel> Courses { get; set; } = new List<RegistrationCourseRowViewModel>();
    }

    public class RegistrationCourseRowViewModel 
    {
        public CourseDTO Course { get; set; }
        public int Points { get; set; }
        public string InstructorUserId { get; set; }
        public string InstructorName { get; set; }
        public bool IsSelected { get; set; }
        public int StudentCourseId { get; set; }
        public int RegistrationCourseId { get; set; }
        public string DepartmentGroups { get; set; }
        public string Comment { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsApproved { get; set; }
        public int TotalPlaces { get; set; }
        public int RestPlaces { get; set; }
        public int Queue { get; set; }
        public string Schedule { get; set; }
        public string Section { get; set; }
        public enu_CourseState State { get; set; }
        public bool IsAudit { get; set; }
        public bool IsForAll { get; set; }
        public bool NoCreditsCount { get; set; }
    }
}
