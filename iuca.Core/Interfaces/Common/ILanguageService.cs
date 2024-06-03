using iuca.Application.DTO.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.Common
{
    public interface ILanguageService
    {
        /// <summary>
        /// Get language list
        /// </summary>
        /// <returns>Language list</returns>
        IEnumerable<LanguageDTO> GetLanguages();

        /// <summary>
        /// Get language by id
        /// </summary>
        /// <param name="id">Id of language</param>
        /// <returns>Language model</returns>
        LanguageDTO GetLanguage(int id);

        /// <summary>
        /// Create language
        /// </summary>
        /// <param name="languageDTO">Language model</param>
        void Create(LanguageDTO languageDTO);

        /// <summary>
        /// Edit language
        /// </summary>
        /// <param name="id">Id of language</param>
        /// <param name="languageDTO">Language model</param>
        void Edit(int id, LanguageDTO languageDTO);

        /// <summary>
        /// Delete language by id
        /// </summary>
        /// <param name="id">Id of language</param>
        void Delete(int id);

        /// <summary>
        /// Get language SelectList
        /// </summary>
        /// <param name="selectedLanguage">Selected language id</param>
        /// <returns>SelectList of languages</returns>
        List<SelectListItem> GetLanguageSelectList(int? selectedLanguage);

        void Dispose();
    }
}
