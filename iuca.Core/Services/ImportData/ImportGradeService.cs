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
    public class ImportGradeService : IImportGradeService
    {
        private readonly IApplicationDbContext _db;

        public ImportGradeService(IApplicationDbContext db)
        {
            _db = db;

        }

        /// <summary>
        /// Import grades from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        public void ImportGrades(string connection, bool overwrite)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.balls";
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
                                ProcessGrade(sdr, overwrite);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void ProcessGrade(NpgsqlDataReader sdr, bool overwrite)
        {
            int importCode = int.Parse(sdr["ballid"].ToString());
            var grade = _db.Grades.FirstOrDefault(x => x.ImportCode == importCode);
            if (grade != null)
            {
                if (overwrite)
                    EditGrade(sdr, grade);
            }
            else
                CreateGrade(sdr);
        }

        private void CreateGrade(NpgsqlDataReader sdr)
        {
            Grade grade = new Grade();

            float gpa = 0;
            float.TryParse(sdr["ballgpa"].ToString(), out gpa);

            grade.ImportCode = int.Parse(sdr["ballid"].ToString());
            grade.GradeMark = sdr["ball"].ToString();
            grade.Gpa = gpa;
            grade.NameEng = sdr["balldesceng"].ToString();
            grade.NameRus = sdr["balldescrus"].ToString();
            grade.NameKir = sdr["balldesckyr"].ToString();

            _db.Grades.Add(grade);
        }

        private void EditGrade(NpgsqlDataReader sdr, Grade grade)
        {
            float gpa = 0;
            float.TryParse(sdr["ballgpa"].ToString(), out gpa);

            grade.GradeMark = sdr["ball"].ToString();
            grade.Gpa = gpa;
            grade.NameEng = sdr["balldesceng"].ToString();
            grade.NameRus = sdr["balldescrus"].ToString();
            grade.NameKir = sdr["balldesckyr"].ToString();

            _db.Grades.Update(grade);
        }
    }
}
