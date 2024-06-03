using iuca.Application.DTO.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class CourseDTO
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
        public string NameKir { get; set; }

        [Display(Name = "Abbreviation")]
        //[Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string Abbreviation { get; set; }

        [Display(Name = "Number")]
        //[Required]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string Number { get; set; }

        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [Display(Name = "Department")]
        public DepartmentDTO Department { get; set; }

        [Display(Name = "Language")]
        public int LanguageId { get; set; }

        [Display(Name = "Language")]
        public LanguageDTO Language { get; set; }

        public int OrganizationId { get; set; }

        [Display(Name = "Import Code")]
        public int ImportCode { get; set; }
        
        [Display(Name = "Sort number")]
        public int SortNum { get; set; }

        public bool IsChanged { get; set; }

        [Display(Name = "Is deleted")]
        public bool IsDeleted { get; set; }

        [Display(Name = "Course type")]
        public int CourseType { get; set; }

        public List<CoursePrerequisiteDTO> CoursePrerequisites { get; set; }

        public string Name 
        { 
            get 
            {
                return NameRus + " \\ " + NameEng + " \\ " + NameKir;
            } 
        }
    }
}
