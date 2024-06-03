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
    public class ImportUniversityService : IImportUniversityService
    {
        private readonly IApplicationDbContext _db;
        private readonly IImportHelperService _importHelperService;

        public ImportUniversityService(IApplicationDbContext db,
            IImportHelperService importHelperService)
        {
            _db = db;
            _importHelperService = importHelperService;
        }

        /// <summary>
        /// Import universities from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        public void ImportUniversities(string connection, bool overwrite)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.unicode_info";
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
                                ProcessUniversity(sdr, overwrite);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void ProcessUniversity(NpgsqlDataReader sdr, bool overwrite)
        {
            int importCode = int.Parse(sdr["uniid"].ToString());
            var univercity = _db.Universities.FirstOrDefault(x => x.ImportCode == importCode);
            if (univercity != null)
            {
                if (overwrite)
                    EditUniversity(sdr, univercity);
            }
            else
                CreateUniversity(sdr);
        }

        private void CreateUniversity(NpgsqlDataReader sdr)
        {
            University university = new University();

            university.ImportCode = int.Parse(sdr["uniid"].ToString());
            university.Code = sdr["ucode"].ToString();
            university.NameEng = sdr["unieng"].ToString();
            university.NameRus = sdr["unirus"].ToString();
            university.NameKir = sdr["unikyr"].ToString();
            university.CountryId = _importHelperService.GetCountryId(sdr["ucode"].ToString());

            _db.Universities.Add(university);
        }

        private void EditUniversity(NpgsqlDataReader sdr, University university)
        {
            university.Code = sdr["ucode"].ToString();
            university.NameEng = sdr["unieng"].ToString();
            university.NameRus = sdr["unirus"].ToString();
            university.NameKir = sdr["unikyr"].ToString();
            university.CountryId = _importHelperService.GetCountryId(sdr["ucode"].ToString());

            _db.Universities.Update(university);
        }
    }
}
