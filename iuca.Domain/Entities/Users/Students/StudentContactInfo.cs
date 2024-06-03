using iuca.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Domain.Entities.Users.Students
{
    public class StudentContactInfo
    {
        public int Id { get; set; }
        public StudentBasicInfo StudentBasicInfo { get; set; }
        public int StudentBasicInfoId { get; set; }
        public string StreetEng { get; set; }
        public string CityEng { get; set; }
        public string StreetRus { get; set; }
        public string CityRus { get; set; }
        public Country Country { get; set; }
        public int CountryId { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string CitizenshipStreetEng { get; set; }
        public string CitizenshipCityEng { get; set; }
        public string CitizenshipStreetRus { get; set; }
        public string CitizenshipCityRus { get; set; }
        public Country CitizenshipCountry { get; set; }
        public int CitizenshipCountryId { get; set; }
        public string CitizenshipZip { get; set; }
        public string CitizenshipPhone { get; set; }
        public string ContactNameEng { get; set; }
        public string ContactNameRus { get; set; }
        public string ContactPhone { get; set; }
        public string RelationEng { get; set; }
        public string RelationRus { get; set; }
        public string RelationKir { get; set; }

    }
}
