using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iuca.Application.Enums;

namespace iuca.Application.DTO.Common
{
    public class SemesterDTO
    {
        public int Id { get; set; }

        [Display(Name = "Season")]
        public int Season { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        public int OrganizationId { get; set; }

        public string SeasonYear
        {
            get 
            {
                return EnumExtentions.GetDisplayName((enu_Season)Season) + " " + Year;
            }
        }
    }
}
