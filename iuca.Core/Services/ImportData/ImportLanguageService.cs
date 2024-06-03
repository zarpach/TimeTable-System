using iuca.Application.Interfaces.ImportData;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.ImportData
{
    public class ImportLanguageService : IImportLanguageService
    {
        private readonly IApplicationDbContext _db;

        public ImportLanguageService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Import languages from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        public void ImportLanguages(string connection, bool overwrite)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.languages";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                ProcessLanguage(sdr, overwrite);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void ProcessLanguage(NpgsqlDataReader sdr, bool overwrite)
        {
            int importCode = int.Parse(sdr["langid"].ToString());
            var language = _db.Languages.FirstOrDefault(x => x.ImportCode == importCode);
            if (language != null)
            {
                if (overwrite)
                    EditLanguage(sdr, language);
            }
            else
                CreateLanguage(sdr);
        }

        private void CreateLanguage(NpgsqlDataReader sdr)
        {
            Language language = new Language();
            language.ImportCode = int.Parse(sdr["langid"].ToString());
            language.Code = sdr["lcode"].ToString();
            language.NameEng = sdr["languageseng"].ToString();
            language.NameRus = sdr["languagesrus"].ToString();
            language.NameKir = sdr["languageskyr"].ToString();

            _db.Languages.Add(language);
        }

        private void EditLanguage(NpgsqlDataReader sdr, Language language)
        {
            language.Code = sdr["lcode"].ToString();
            language.NameEng = sdr["languageseng"].ToString();
            language.NameRus = sdr["languagesrus"].ToString();
            language.NameKir = sdr["languageskyr"].ToString();

            _db.Languages.Update(language);
        }

    }
}
