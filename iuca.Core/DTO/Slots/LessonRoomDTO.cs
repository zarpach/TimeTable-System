using System;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Slots
{
    public class LessonRoomDTO
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name = "Номер или название аудитории")]
        public string RoomName { get; set; }

        [Display(Name = "Вместительность аудитории")]
        public int? RoomCapacity { get; set; }

        [Display(Name = "Этаж")]
        public int? Floor { get; set; }
    }
}

