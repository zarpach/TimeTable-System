using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace iuca.Application.Enums
{
    public enum enu_SlotDayOfWeek
    {
        [Display(Name = "Понедельник")]
        Monday = DayOfWeek.Monday,

        [Display(Name = "Вторник")]
        Tuesday = DayOfWeek.Tuesday,

        [Display(Name = "Среда")]
        Wednesday = DayOfWeek.Wednesday,

        [Display(Name = "Четверг")]
        Thursday = DayOfWeek.Thursday,

        [Display(Name = "Пятница")]
        Friday = DayOfWeek.Friday,

        [Display(Name = "Суббота")]
        Saturday = DayOfWeek.Saturday,
    }

}
