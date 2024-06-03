using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.Instructors
{
    public class InstructorContactInfoDTO
    {
        public int Id { get; set; }
        public int InstructorBasicInfoId { get; set; }
        
        [Display(Name = "Country")]
        public CountryDTO Country { get; set; }
        
        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "City eng")]
        public string CityEng { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Street eng")]
        public string StreetEng { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Address eng")]
        public string AddressEng { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "City rus")]
        public string CityRus { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Street rus")]
        public string StreetRus { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Address rus")]
        public string AddressRus { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Zip code")]
        public string ZipCode { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Country")]
        public CountryDTO CitizenshipCountry { get; set; }

        [Display(Name = "Country")]
        public int? CitizenshipCountryId { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "City eng")]
        public string CitizenshipCityEng { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Street eng")]
        public string CitizenshipStreetEng { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Address eng")]
        public string CitizenshipAddressEng { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "City rus")]
        public string CitizenshipCityRus { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Street rus")]
        public string CitizenshipStreetRus { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Address rus")]
        public string CitizenshipAddressRus { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Zip code")]
        public string CitizenshipZipCode { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Phone")]
        public string CitizenshipPhone { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Contact name eng")]
        public string ContactNameEng { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Contact name rus")]
        public string ContactNameRus { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Contact phone")]
        public string ContactPhone { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Relation eng")]
        public string RelationEng { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Relation rus")]
        public string RelationRus { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} length must be less than {1}")]
        [Display(Name = "Relation kir")]
        public string RelationKir { get; set; }

        //virtual field for import instructor info
        public int InstructorImportCode { get; set; }
    }
}
