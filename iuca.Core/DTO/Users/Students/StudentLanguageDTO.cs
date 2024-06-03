using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.Students
{
    public class StudentLanguageDTO
    {
        [Display(Name = "Student basic info")]
        public StudentBasicInfoDTO StudentBasicInfo { get; set; }

        [Display(Name = "Student basic info")]
        public int StudentBasicInfoId { get; set; }

        [Display(Name = "Language")]
        public LanguageDTO Language { get; set; }

        [Display(Name = "Language")]
        public int LanguageId { get; set; }
    }
}
