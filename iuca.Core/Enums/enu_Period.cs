using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Enums
{
    public enum enu_Period
    {
        [Display(Name = "Семестр")]
        Semester = 1,
        [Display(Name = "Регистрация на курсы")]
        CourseRegistration,
        [Display(Name = "Выставление оценок")]
        Grading,
        [Display(Name = "Add/Drop период")]
        AddDrop,
        [Display(Name = "Выставление Midterm")]
        Midterm
    }
}
