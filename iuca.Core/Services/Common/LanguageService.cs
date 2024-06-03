using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.Common
{
    public class LanguageService : ILanguageService
    {
        private readonly IApplicationDbContext _db;

        public LanguageService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get language list
        /// </summary>
        /// <returns>Language list</returns>
        public IEnumerable<LanguageDTO> GetLanguages()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Language, LanguageDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Language>, IEnumerable<LanguageDTO>>(_db.Languages);
        }

        /// <summary>
        /// Get language by id
        /// </summary>
        /// <param name="id">Id of language</param>
        /// <returns>Language model</returns>
        public LanguageDTO GetLanguage(int id)
        {
            Language language = _db.Languages.FirstOrDefault(x => x.Id == id);
            if (language == null)
                throw new Exception($"Language with id {id} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Language, LanguageDTO>()).CreateMapper();
            return mapper.Map<Language, LanguageDTO>(language);
        }

        /// <summary>
        /// Create language
        /// </summary>
        /// <param name="languageDTO">Language model</param>
        public void Create(LanguageDTO languageDTO)
        {
            if (languageDTO == null)
                throw new Exception($"languageDTO is null");

            var mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<Language, LanguageDTO>()).CreateMapper();
            var mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<LanguageDTO, Language>()).CreateMapper();

            Language newLanguage = mapperFromDTO.Map<LanguageDTO, Language>(languageDTO);

            _db.Languages.Add(newLanguage);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit language
        /// </summary>
        /// <param name="id">Id of language</param>
        /// <param name="languageDTO">Language model</param>
        public void Edit(int id, LanguageDTO languageDTO)
        {
            if (languageDTO == null)
                throw new Exception($"languageDTO is null");

            Language language = _db.Languages.FirstOrDefault(x => x.Id == id);
            if (language == null)
                throw new Exception($"Language with id {id} not found");

            language.NameEng = languageDTO.NameEng;
            language.NameRus = languageDTO.NameRus;
            language.NameKir = languageDTO.NameKir;
            language.Code = languageDTO.Code;
            language.SortNum = languageDTO.SortNum;

            _db.Languages.Update(language);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete language by id
        /// </summary>
        /// <param name="id">Id of language</param>
        public void Delete(int id)
        {
            Language language = _db.Languages.FirstOrDefault(x => x.Id == id);
            if (language == null)
                throw new Exception($"Language with id {id} not found");

            _db.Languages.Remove(language);
            _db.SaveChanges();
        }

        /// <summary>
        /// Get language SelectList
        /// </summary>
        /// <param name="selectedLanguage">Selected language id</param>
        /// <returns>SelectList of languages</returns>
        public List<SelectListItem> GetLanguageSelectList(int? selectedLanguage)
        {
            return new SelectList(_db.Languages.OrderBy(x => x.NameEng), "Id", "NameEng", selectedLanguage).ToList();
        }
        
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
