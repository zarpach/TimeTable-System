using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Common
{
    public class GradeDTO
    {
        public int Id { get; set; }

        [Display(Name = "Grade")]
        public string GradeMark { get; set; }

        [Display(Name = "GPA")]
        public float Gpa { get; set; }

        [Display(Name = "Name eng")]
        public string NameEng { get; set; }

        [Display(Name = "Name rus")]
        public string NameRus { get; set; }

        [Display(Name = "Name kir")]
        public string NameKir { get; set; }

        [Display(Name = "Import code")]
        public int ImportCode { get; set; }
    }
}
