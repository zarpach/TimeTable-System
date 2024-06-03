using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Courses
{
    public class CycleDTO
    {
        public int Id { get; set; }

        [Display(Name = "Name eng")]
        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string NameEng { get; set; }

        [Display(Name = "Name rus")]
        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string NameRus { get; set; }

        [Display(Name = "Name kir")]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string NameKir { get; set; }

        [Display(Name = "Code")]
        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string Code { get; set; }
    }
}
