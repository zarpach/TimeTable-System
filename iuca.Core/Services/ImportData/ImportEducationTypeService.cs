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
    public class ImportEducationTypeService : IImportEducationTypeService
    {
        private readonly IApplicationDbContext _db;

        public ImportEducationTypeService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Import education type from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        public void ImportEducationTypes(string connection, bool overwrite)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.educations";
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
                                ProcessEducationType(sdr, overwrite);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void ProcessEducationType(NpgsqlDataReader sdr, bool overwrite)
        {
            int importCode = int.Parse(sdr["educid"].ToString());
            var educationType = _db.EducationTypes.FirstOrDefault(x => x.ImportCode == importCode);
            if (educationType != null)
            {
                if (overwrite)
                    EditEducationType(sdr, educationType);
            }
            else
                CreateEducationType(sdr);
        }

        private void CreateEducationType(NpgsqlDataReader sdr)
        {
            EducationType educationType = new EducationType();

            educationType.ImportCode = int.Parse(sdr["educid"].ToString());
            educationType.NameEng = sdr["educationeng"].ToString();
            educationType.NameRus = sdr["educationrus"].ToString();
            educationType.NameKir = sdr["educationkyr"].ToString();

            _db.EducationTypes.Add(educationType);
        }

        private void EditEducationType(NpgsqlDataReader sdr, EducationType educationType)
        {
            educationType.NameEng = sdr["educationeng"].ToString();
            educationType.NameRus = sdr["educationrus"].ToString();
            educationType.NameKir = sdr["educationkyr"].ToString();

            _db.EducationTypes.Update(educationType);
        }
    }
}
