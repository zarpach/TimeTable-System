using iuca.Application.Interfaces.ImportData;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.ImportData
{
    public class ImportCourseService : IImportCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly IImportHelperService _importHelperService;

        public ImportCourseService(IApplicationDbContext db,
            IImportHelperService importHelperService)
        {
            _db = db;
            _importHelperService = importHelperService;
        }

        /// <summary>
        /// Import courses from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        /// <param name="organizationId">Organization id</param>
        public void ImportCourses(string connection, bool overwrite, int organizationId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.courses_for_import";
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
                                ProcessCourses(sdr, overwrite, organizationId);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void ProcessCourses(NpgsqlDataReader sdr, bool overwrite, int organizationId)
        {
            int importCode = int.Parse(sdr["importcode"].ToString());
            var course = _db.Courses.FirstOrDefault(x => x.ImportCode == importCode && x.OrganizationId == organizationId);
            if (course != null)
            {
                if (overwrite)
                    EditCourses(sdr, course, organizationId);
            }
            else
                CreateCourses(sdr, organizationId);
        }

        private void CreateCourses(NpgsqlDataReader sdr, int organizationId)
        {
            Course course = new Course();
            course.ImportCode = int.Parse(sdr["importcode"].ToString());
            course.NameEng = sdr["nameeng"].ToString();
            course.NameRus = sdr["namerus"].ToString();
            course.NameKir = sdr["namekir"].ToString();
            course.Abbreviation = sdr["abbreviation"].ToString();
            course.Number = sdr["number"].ToString();
            course.DepartmentId = _importHelperService.GetDepartmentId(sdr["departmentid"].ToString(), organizationId);
            course.LanguageId = _importHelperService.GetLanguageId(sdr["languageid"].ToString());
            course.OrganizationId = organizationId;

            _db.Courses.Add(course);
        }

        private void EditCourses(NpgsqlDataReader sdr, Course course, int organizationId)
        {
            course.NameEng = sdr["nameeng"].ToString();
            course.NameRus = sdr["namerus"].ToString();
            course.NameKir = sdr["namekir"].ToString();
            course.Abbreviation = sdr["abbreviation"].ToString();
            course.Number = sdr["number"].ToString();
            course.DepartmentId = _importHelperService.GetDepartmentId(sdr["departmentid"].ToString(), organizationId);
            course.LanguageId = _importHelperService.GetLanguageId(sdr["languageid"].ToString());

            _db.Courses.Update(course);
        }

    }
}
