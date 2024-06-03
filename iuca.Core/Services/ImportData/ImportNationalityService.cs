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
    public class ImportNationalityService : IImportNationalityService
    {
        private readonly IApplicationDbContext _db;

        public ImportNationalityService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Import nationalities from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        public void ImportNationalities(string connection, bool overwrite)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.ncode_info";
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
                                ProcessNationality(sdr, overwrite);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void ProcessNationality(NpgsqlDataReader sdr, bool overwrite)
        {
            int importCode = int.Parse(sdr["ncodeid"].ToString());
            var nationality = _db.Nationalities.FirstOrDefault(x => x.ImportCode == importCode);
            if (nationality != null)
            {
                if (overwrite)
                    EditNationality(sdr, nationality);
            }
            else
                CreateNationality(sdr);
        }

        private void CreateNationality(NpgsqlDataReader sdr)
        {
            Nationality nationality = new Nationality();

            nationality.ImportCode = int.Parse(sdr["ncodeid"].ToString());
            nationality.NameEng = sdr["nationalityeng"].ToString();
            nationality.NameRus = sdr["nationalityrus"].ToString();
            nationality.NameKir = sdr["nationalitykyr"].ToString();

            _db.Nationalities.Add(nationality);
        }

        private void EditNationality(NpgsqlDataReader sdr, Nationality nationality)
        {
            nationality.NameEng = sdr["nationalityeng"].ToString();
            nationality.NameRus = sdr["nationalityrus"].ToString();
            nationality.NameKir = sdr["nationalitykyr"].ToString();

            _db.Nationalities.Update(nationality);
        }

    }
}
