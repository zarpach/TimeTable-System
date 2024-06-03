using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.Instructors
{
    public class InstructorEducationInfoDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Major eng")]
        public string MajorEng { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Major rus")]
        public string MajorRus { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Major kir")]
        public string MajorKir { get; set; }

        [Required]
        [Range(1, 3000)]
        [Display(Name = "Graduate year")]
        public int GraduateYear { get; set; }

        public int InstructorBasicInfoId { get; set; }

        [Display(Name = "University")]
        public UniversityDTO University { get; set; }
        [Display(Name = "University")]
        public int? UniversityId { get; set; }

        [Display(Name = "Education type")]
        public EducationTypeDTO EducationType { get; set; }
        [Display(Name = "Education type")]
        public int? EducationTypeId { get; set; }

        //virtual field for import instructor info
        public int InstructorImportCode { get; set; }
    }
}
