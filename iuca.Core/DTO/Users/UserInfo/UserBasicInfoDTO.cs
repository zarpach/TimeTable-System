using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Instructors;
using iuca.Application.DTO.Users.Staff;
using iuca.Application.DTO.Users.Students;
using System;
using System.ComponentModel.DataAnnotations;


namespace iuca.Application.DTO.Users.UserInfo
{
    public class UserBasicInfoDTO
    {
        public int Id { get; set; }

        [Display(Name = "Last name rus")]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string LastNameRus { get; set; }

        [Display(Name = "First name rus")]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string FirstNameRus { get; set; }

        [Display(Name = "Middle name rus")]
        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        public string MiddleNameRus { get; set; }

        [Display(Name = "Sex")]
        [Required]
        [Range(1, 2, ErrorMessage = "The field {0} must be 1 or 2")]
        public int Sex { get; set; }

        [Display(Name = "Date of birth")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}")]
        public DateTime DateOfBirth { get; set; }
        
        public string ApplicationUserId { get; set; }

        public bool IsMainOrganization { get; set; }

        [Display(Name = "Nationality")]
        public NationalityDTO Nationality { get; set; }

        public int? NationalityId { get; set; }

        [Display(Name = "Citizenship")]
        public CountryDTO Citizenship { get; set; }

        public int? CitizenshipId { get; set; }

        public string FullNameRus
        {
            get { return $"{LastNameRus} {FirstNameRus} {MiddleNameRus}"; }
        }

    }
}
