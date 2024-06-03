using System;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.DTO.Users.Students;
using System.ComponentModel.DataAnnotations;
using iuca.Infrastructure.Identity.Entities;

namespace iuca.Application.DTO.Slots
{
    public class SlotDTO
    {
        public Guid Id { get; set; }

        [Display(Name = "Департамент")]
        [Required(ErrorMessage = "Поле '{0}' обязательноe")]
        public int[] DepartmentIds { get; set; }

        [Display(Name = "Департамент")]
        public DepartmentDTO Department { get; set; }

        [Display(Name = "Группа")]
        [Required(ErrorMessage = "Поле '{0}' обязательноe")]
        public int[] GroupIds { get; set; }

        [Display(Name = "Группа")]
        public DepartmentGroupDTO Group { get; set; }

        [Display(Name = "Семестр")]
        [Required(ErrorMessage = "Поле '{0}' обязательноe")]
        public int SemesterId { get; set; }

        public SemesterDTO Semester { get; set; }

        [Display(Name = "Преподаватель")]
        [Required(ErrorMessage = "Поле '{0}' обязательноe")]
        public string InstructorUserId { get; set; }

        //[Display(Name = "Преподаватель")]
        //public ApplicationUser InstructorUser { get; set; }

        [Display(Name = "День недели")]
        [Required(ErrorMessage = "Поле '{0}' обязательноe")]
        public int[] SlotDaysOfWeek { get; set; }

        public int DayOfWeek { get; set; }

        [Display(Name = "Аудитория")]
        [Required(ErrorMessage = "Поле '{0}' обязательноe")]
        public string LessonRoomId { get; set; }

        [Display(Name = "Аудитория")]
        public LessonRoomDTO LessonRoom { get; set; }

        [Display(Name = "Время занятия")]
        [Required(ErrorMessage = "Поле '{0}' обязательноe")]
        public int[] LessonPeriodIds { get; set; }

        [Display(Name = "Время занятия")]
        public LessonPeriodDTO LessonPeriod { get; set; }

        [Display(Name = "Предмет")]
        [Required(ErrorMessage = "Поле '{0}' обязательноe")]
        public int AnnouncementSectionId { get; set; }

        [Display(Name = "Предмет")]
        public AnnouncementSectionDTO AnnouncementSection { get; set; }
    }
}

