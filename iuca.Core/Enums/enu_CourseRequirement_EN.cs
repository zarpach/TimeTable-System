using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_CourseRequirement_EN
    {
        [Display(Name = "Attendance")]
        Attendance = 0,
        [Display(Name = "Classroom activity")]
        classroomActivity,
        [Display(Name = "Essays")]
        Essays,
        [Display(Name = "ISW - Independent Student Work")]
        ISW,
        [Display(Name = "Assignments")]
        Assigments,
        [Display(Name = "Lab assignments")]
        LabAssignments,
        [Display(Name = "Practical assignments")]
        PracticalAssignments,
        [Display(Name = "Tests")]
        Tests,
        [Display(Name = "Midterm exam")]
        MidtermExam,
        [Display(Name = "Final project")]
        FinalProject,
        [Display(Name = "Final exam")]
        FinalExam,
        [Display(Name = "Other")]
        Other,
        [Display(Name = "Projects")]
        Projects
    }
}
