using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Common
{
    public class UniversityDTO
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Code")]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "NameEng")]
        [MaxLength(200, ErrorMessage = "The field {0} length must be less than {1}")]
        public string NameEng { get; set; }

        [Required]
        [Display(Name = "NameRus")]
        [MaxLength(200, ErrorMessage = "The field {0} length must be less than {1}")]
        public string NameRus { get; set; }

        [Display(Name = "NameKir")]
        [MaxLength(200, ErrorMessage = "The field {0} length must be less than {1}")]
        public string NameKir { get; set; }

        [Display(Name = "Country")]
        public CountryDTO Country { get; set; }

        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Display(Name = "Import code")]
        public int ImportCode { get; set; }

    }
}
