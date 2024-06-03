using iuca.Application.DTO.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.DTO.Users.Students
{
    public class StudentContactInfoDTO
    {
        public int Id { get; set; }

        [Display(Name = "Student Basic Info")]
        public StudentBasicInfoDTO StudentBasicInfo { get; set; }

        [Display(Name = "Student Basic Info")]
        public int StudentBasicInfoId { get; set; }

        [Display(Name = "Street Eng")]
        [MaxLength(100)]
        public string StreetEng { get; set; }

        [Display(Name = "City Eng")]
        [MaxLength(100)]
        public string CityEng { get; set; }

        [Display(Name = "Street Rus")]
        [MaxLength(100)]
        public string StreetRus { get; set; }

        [Display(Name = "City Rus")]
        [MaxLength(100)]
        public string CityRus { get; set; }

        [Display(Name = "Country")]
        public CountryDTO Country { get; set; }

        [Display(Name = "Country")]
        public int CountryId { get; set; }

        [Display(Name = "Zip")]
        [MaxLength(50)]
        public string Zip { get; set; }

        [Display(Name = "Phone")]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Display(Name = "Citizenship Street Eng")]
        [MaxLength(100)]
        public string CitizenshipStreetEng { get; set; }

        [Display(Name = "Citizenship City Eng")]
        [MaxLength(100)]
        public string CitizenshipCityEng { get; set; }

        [Display(Name = "Citizenship Street Rus")]
        [MaxLength(100)]
        public string CitizenshipStreetRus { get; set; }

        [Display(Name = "Citizenship City Rus")]
        [MaxLength(100)]
        public string CitizenshipCityRus { get; set; }

        [Display(Name = "Citizenship Country")]
        public CountryDTO CitizenshipCountry { get; set; }

        [Display(Name = "Citizenship Country")]
        public int CitizenshipCountryId { get; set; }

        [Display(Name = "Citizenship Zip")]
        [MaxLength(50)]
        public string CitizenshipZip { get; set; }

        [Display(Name = "Citizenship Phone")]
        [MaxLength(50)]
        public string CitizenshipPhone { get; set; }

        [Display(Name = "Contact Name Eng")]
        [MaxLength(100)]
        public string ContactNameEng { get; set; }

        [Display(Name = "Contact Name Rus")]
        [MaxLength(100)]
        public string ContactNameRus { get; set; }

        [Display(Name = "Contact Phone")]
        [MaxLength(50)]
        public string ContactPhone { get; set; }

        [Display(Name = "Relation Eng")]
        [MaxLength(50)]
        public string RelationEng { get; set; }

        [Display(Name = "Relation Rus")]
        [MaxLength(50)]
        public string RelationRus { get; set; }

        [Display(Name = "Relation Kir")]
        [MaxLength(50)]
        public string RelationKir { get; set; }
    }
}
