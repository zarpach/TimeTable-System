using System;
using System.ComponentModel.DataAnnotations;


namespace iuca.Application.DTO.Common
{
    public class SemesterPeriodDTO
    {
        public int Id { get; set; }

        [Display(Name = "Semester")]
        public SemesterDTO Semester { get; set; }
        
        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        [Display(Name = "Period")]
        public int Period { get; set; }

        [Display(Name = "Date begin")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy HH:mm:ss}")]
        public DateTime DateBegin { get; set; }
        
        [Display(Name = "Date end")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy HH:mm:ss}")]
        public DateTime DateEnd { get; set; }

        public int OrganizationId { get; set; }

        public bool IsEnabed()
        {
            return IsEnabed(DateTime.Now);
        }

        public bool IsEnabed(DateTime date)
        { 
            return date >= DateBegin && date <= DateEnd;
        }
    }
}
