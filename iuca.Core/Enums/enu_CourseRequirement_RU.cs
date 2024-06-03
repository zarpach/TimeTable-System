using System.ComponentModel.DataAnnotations;

namespace iuca.Application.Enums
{
    public enum enu_CourseRequirement_RU
    {
        [Display(Name = "Посещение")]
        Attendance = 0,
        [Display(Name = "Активность на занятиях")]
        classroomActivity,
        [Display(Name = "Эссе")]
        Essays,
        [Display(Name = "СРС - Самост. Работа Студента")]
        ISW,
        [Display(Name = "Задания")]
        Assigments,
        [Display(Name = "Лабораторные работы")]
        LabAssignments,
        [Display(Name = "Практические работы")]
        PracticalAssignments,
        [Display(Name = "Тесты")]
        Test,
        [Display(Name = "Промежуточный экзамен")]
        MidtermExam,
        [Display(Name = "Финальный проект")]
        FinalProject,
        [Display(Name = "Финальный экзамен")]
        FinalExam,
        [Display(Name = "Прочее")]
        Other,
        [Display(Name = "Проекты")]
        Projects
    }
}
