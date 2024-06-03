using System;
using System.ComponentModel.DataAnnotations;
using iuca.Domain.Entities.Common;

namespace iuca.Application.DTO.Slots
{
	public class LessonPeriodDTO
	{
		public int Id { get; set; }

        public string Name
        {
            get
            {
                return $"{TimeBegin.ToString(@"hh\:mm")} – {TimeEnd.ToString(@"hh\:mm")}";
            }
        }

        [Display(Name = "Время начала")]
        [Required]
        public TimeSpan TimeBegin { get; set; }

		[Display(Name = "Время конца")]
        [Required]
        public TimeSpan TimeEnd { get; set; }

        
    }
}

