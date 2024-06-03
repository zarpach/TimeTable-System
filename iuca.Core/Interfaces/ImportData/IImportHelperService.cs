using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.ImportData
{
    public interface IImportHelperService
    {
        /// <summary>
        /// Get id of language matched by import code
        /// </summary>
        /// <param name="languageImportCodeStr">Import code of language</param>
        /// <returns>Language id</returns>
        int GetLanguageId(string languageImportCodeStr);

        /// <summary>
        /// Get id of department matched by import code
        /// </summary>
        /// <param name="departmentImportCodeStr">Import code of department</param>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Department id</returns>
        int GetDepartmentId(string departmentImportCodeStr, int organizationId);

        /// <summary>
        /// Get id of nationalty matched by import code
        /// </summary>
        /// <param name="nationalityImportCodeStr">Import code of nationality</param>
        /// <returns>Nationality id</returns>
        int GetNationalityId(string nationalityImportCodeStr);

        /// <summary>
        /// Get id of country matched by import code
        /// </summary>
        /// <param name="countryImportCodeStr">Import code of country</param>
        /// <returns>Country id</returns>
        int GetCountryId(string countryImportCodeStr);

        /// <summary>
        /// Get id of country matched by import code
        /// </summary>
        /// <param name="universityImportCodeStr">Import code of university</param>
        /// <returns>University id</returns>
        int GetUniversityId(string universityImportCodeStr);

        /// <summary>
        /// Get student user id by studentId
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="studentId">Student id</param>
        /// <returns>Student basic info id</returns>
        public string GetStudentUserIdByStudentId(int organizationId, int studentId);

        /// <summary>
        /// Get season by import season value
        /// </summary>
        /// <param name="importSeasonId">Import season number</param>
        /// <returns>Season value from new DB</returns>
        int GetSeason(string importSeasonId);

        /// <summary>
        /// Get id of education type matched by import code
        /// </summary>
        /// <param name="educationTypeImportCodeStr">Import code of education type</param>
        /// <returns>Education type id</returns>
        int GetEducationTypeId(string educationTypeImportCodeStr);

        /// <summary>
        /// Get grade id by grade import code
        /// </summary>
        /// <param name="gradeImportCodeStr">Grade import code</param>
        /// <returns>Grade id</returns>
        int? GetGradeId(string gradeImportCodeStr);
    }
}
