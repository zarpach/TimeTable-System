using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Students;
using iuca.Application.Enums;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Courses
{
    public class StudyCardCoursesViewModel
    {
        public SemesterDTO Semester { get; set; }
        public DepartmentGroupDTO DepartmentGroup { get; set; }
        public int StudentCourseRegistrationId { get; set; }
        public int StudyCardId { get; set; }
        public bool DisplayIUCAElectives { get; set; }

        public List<StudyCardCourseRowViewModel> StudyCardCourses = new List<StudyCardCourseRowViewModel>();
        public List<StudyCardCourseRowViewModel> ElectiveCourses = new List<StudyCardCourseRowViewModel>();
    }

    public class StudyCardCourseRowViewModel
    {
        public AnnouncementSectionDTO AnnouncementSection { get; set; }
        public string InstructorName { get; set; }
        public bool IsSelected { get; set; }
        public string Comment { get; set; }
        public int TotalPlaces { get; set; }
        public int RestPlaces { get; set; }
        public int Queue { get; set; }
        public enu_CourseState State { get; set; }
        public bool IsForAll { get; set; }
    }
}
