using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.Instructors
{
    public class InstructorOtherJobInfoDTO
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Place name eng")]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string PlaceNameEng { get; set; }

        [Required]
        [Display(Name = "Place name rus")]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string PlaceNameRus { get; set; }

        [Display(Name = "Place name kir")]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string PlaceNameKir { get; set; }

        [Required]
        [Display(Name = "Position eng")]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string PositionEng { get; set; }

        [Required]
        [Display(Name = "Position rus")]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string PositionRus { get; set; }

        [Display(Name = "Position kir")]
        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        public string PositionKir { get; set; }

        [Display(Name = "Phone")]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string Phone { get; set; }

        public int InstructorBasicInfoId { get; set; }

        //virtual field for import instructor info
        public int InstructorImportCode { get; set; }
    }
}
