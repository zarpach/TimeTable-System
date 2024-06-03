using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Common
{
    public class EducationTypeDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Name eng")]
        public string NameEng { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Name rus")]
        public string NameRus { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Name kir")]
        public string NameKir { get; set; }

        [Display(Name = "Import Code")]
        public int ImportCode { get; set; }
    }
}
