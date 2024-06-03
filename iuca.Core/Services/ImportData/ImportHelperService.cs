using iuca.Application.Enums;
using iuca.Application.Interfaces.ImportData;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.ImportData
{
    public class ImportHelperService : IImportHelperService
    {
        private readonly IApplicationDbContext _db;

        private int defaultLangugeId;
        private int defaultDepartmentId;
        private int defaultNationalityId;
        private int defaultCountryId;
        private int defaultUniversityId;
        private int defaultEducationTypeId;

        public ImportHelperService(IApplicationDbContext db)
        {
            _db = db;

            var defaultLanguge = _db.Languages.FirstOrDefault(x => x.Code == "Na");
            if (defaultLanguge == null)
                throw new Exception("Default language not found");

            var defaultDepartment = _db.Departments.FirstOrDefault(x => x.Code == "NA");
            if (defaultDepartment == null)
                throw new Exception("Default department not found");

            var defaultNationality = _db.Nationalities.FirstOrDefault(x => x.NameEng == "Not assigned");
            if (defaultNationality == null)
                throw new Exception("Default nationality not found");

            var defaultCountry = _db.Countries.FirstOrDefault(x => x.Code == "NA");
            if (defaultCountry == null)
                throw new Exception("Default country not found");

            var defaultUniversity = _db.Universities.FirstOrDefault(x => x.Code == "NA");
            if (defaultUniversity == null)
                throw new Exception("Default university not found");

            var defaultEducationType = _db.EducationTypes.FirstOrDefault(x => x.NameEng == "Not assigned");
            if (defaultEducationType == null)
                throw new Exception("Default education type not found");

            defaultLangugeId = defaultLanguge.Id;
            defaultDepartmentId = defaultDepartment.Id;
            defaultNationalityId = defaultNationality.Id;
            defaultCountryId = defaultCountry.Id;
            defaultUniversityId = defaultUniversity.Id;
            defaultEducationTypeId = defaultEducationType.Id;
        }

        /// <summary>
        /// Get id of language matched by import code
        /// </summary>
        /// <param name="languageImportCodeStr">Import code of language</param>
        /// <returns>Language id</returns>
        public int GetLanguageId(string languageImportCodeStr)
        {
            int languageId;
            if (int.TryParse(languageImportCodeStr, out languageId))
            {
                var language = _db.Languages.FirstOrDefault(x => x.ImportCode == languageId);
                if (language != null)
                    languageId = language.Id;
            }
            else
                languageId = defaultLangugeId;

            return languageId;
        }

        /// <summary>
        /// Get id of department matched by import code
        /// </summary>
        /// <param name="departmentImportCodeStr">Import code of department</param>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Department id</returns>
        public int GetDepartmentId(string departmentImportCodeStr, int organizationId)
        {
            int departmentId;
            if (int.TryParse(departmentImportCodeStr, out departmentId))
            {
                var department = _db.Departments.FirstOrDefault(x => x.ImportCode == departmentId 
                    && x.OrganizationId == organizationId);
                if (department != null)
                    departmentId = department.Id;
            }
            else
                departmentId = defaultDepartmentId;

            return departmentId;
        }

        /// <summary>
        /// Get id of nationalty matched by import code
        /// </summary>
        /// <param name="nationalityImportCodeStr">Import code of nationality</param>
        /// <returns>Nationality id</returns>
        public int GetNationalityId(string nationalityImportCodeStr)
        {
            int nationalityId;
            if (int.TryParse(nationalityImportCodeStr, out nationalityId))
            {
                var nationality = _db.Nationalities.FirstOrDefault(x => x.ImportCode == nationalityId);
                if (nationality != null)
                    nationalityId = nationality.Id;
            }
            else
                nationalityId = defaultNationalityId;

            return nationalityId;
        }

        /// <summary>
        /// Get id of country matched by import code
        /// </summary>
        /// <param name="countryImportCodeStr">Import code of country</param>
        /// <returns>Country id</returns>
        public int GetCountryId(string countryImportCodeStr)
        {
            int countryId;
            if (int.TryParse(countryImportCodeStr, out countryId))
            {
                var country = _db.Countries.FirstOrDefault(x => x.ImportCode == countryId);
                if (country != null)
                    countryId = country.Id;
            }
            else
                countryId = defaultCountryId;

            return countryId;
        }

        /// <summary>
        /// Get id of country matched by import code
        /// </summary>
        /// <param name="universityImportCodeStr">Import code of university</param>
        /// <returns>University id</returns>
        public int GetUniversityId(string universityImportCodeStr)
        {
            int universityId;
            if (int.TryParse(universityImportCodeStr, out universityId))
            {
                var university = _db.Universities.FirstOrDefault(x => x.ImportCode == universityId);
                if (university != null)
                    universityId = university.Id;
            }
            else
                universityId = defaultUniversityId;

            return universityId;
        }

        /// <summary>
        /// Get student user id by studentId
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentId">Student id</param>
        /// <returns>Student basic info id</returns>
        public string GetStudentUserIdByStudentId(int organizationId, int studentId)
        {
            var orgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.OrganizationId == organizationId && x.StudentId == studentId);
            
            if (orgInfo == null)
                throw new Exception($"Student with id {studentId} not found in organization with id {organizationId}");

            if (orgInfo.StudentBasicInfo == null)
                throw new Exception("Student basic info is null");

            return orgInfo.StudentBasicInfo.ApplicationUserId;
        }

        /// <summary>
        /// Get season by import season value
        /// </summary>
        /// <param name="importSeasonId">Import season number</param>
        /// <returns>Season value from new DB</returns>
        public int GetSeason(string importSeasonId)
        {
            if (string.IsNullOrEmpty(importSeasonId))
                return (int)enu_Season.Fall;

            int importSeason = int.Parse(importSeasonId);

            int season;
            switch (importSeason)
            {
                case 1:
                    season = (int)enu_Season.Fall;
                    break;
                case 2:
                    season = (int)enu_Season.Spring;
                    break;
                case 3:
                    season = (int)enu_Season.Summer;
                    break;
                case 29:
                    season = (int)enu_Season.Winter;
                    break;
                default:
                    throw new Exception("Season not defined");
            }
            return season;
        }

        /// <summary>
        /// Get id of education type matched by import code
        /// </summary>
        /// <param name="educationTypeImportCodeStr">Import code of education type</param>
        /// <returns>Education type id</returns>
        public int GetEducationTypeId(string educationTypeImportCodeStr)
        {
            int educationTypeId;
            if (int.TryParse(educationTypeImportCodeStr, out educationTypeId))
            {
                var educationType = _db.EducationTypes.FirstOrDefault(x => x.ImportCode == educationTypeId);
                if (educationType != null)
                    educationTypeId = educationType.Id;
            }
            else
                educationTypeId = defaultEducationTypeId;

            return educationTypeId;
        }

        /// <summary>
        /// Get grade id by grade import code
        /// </summary>
        /// <param name="gradeImportCodeStr">Grade import code</param>
        /// <returns>Grade id</returns>
        public int? GetGradeId(string gradeImportCodeStr)
        {
            int? gradeId = null;

            if (!string.IsNullOrEmpty(gradeImportCodeStr))
            {
                int gradeImportCode = int.Parse(gradeImportCodeStr);
                var grade = _db.Grades.FirstOrDefault(x => x.ImportCode == gradeImportCode);
                if (grade == null)
                    throw new Exception($"Grade with import code {gradeImportCodeStr} not found");
                gradeId = grade.Id;
            }

            return gradeId;
        }
    }
}
