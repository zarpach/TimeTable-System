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
    public class ImportCountryService : IImportCountryService
    {
        private readonly IApplicationDbContext _db;

        public ImportCountryService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Import countries from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        public void ImportCountries(string connection, bool overwrite)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.ccode_info";
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
                                ProcessCountry(sdr, overwrite);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void ProcessCountry(NpgsqlDataReader sdr, bool overwrite)
        {
            int importCode = int.Parse(sdr["ccodeid"].ToString());
            var country = _db.Countries.FirstOrDefault(x => x.ImportCode == importCode);
            if (country != null)
            {
                if (overwrite)
                    EditCountry(sdr, country);
            }
            else
                CreateCountry(sdr);
        }

        private void CreateCountry(NpgsqlDataReader sdr)
        {
            Country country = new Country();

            country.ImportCode = int.Parse(sdr["ccodeid"].ToString());
            country.Code = sdr["ccode"].ToString();
            country.NameEng = sdr["countryeng"].ToString();
            country.NameRus = sdr["countryrus"].ToString();
            country.NameKir = sdr["countrykyr"].ToString();

            _db.Countries.Add(country);
        }

        private void EditCountry(NpgsqlDataReader sdr, Country country)
        {
            country.Code = sdr["ccode"].ToString();
            country.NameEng = sdr["countryeng"].ToString();
            country.NameRus = sdr["countryrus"].ToString();
            country.NameKir = sdr["countrykyr"].ToString();

            _db.Countries.Update(country);
        }
    }
}
