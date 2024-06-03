using iuca.Application.Interfaces.ImportData;
using iuca.Application.ViewModels.Courses;
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
    public class ImportTransferCourseService : IImportTransferCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly IImportHelperService _importHelperService;

        public ImportTransferCourseService(IApplicationDbContext db,
            IImportHelperService importHelperService)
        {
            _db = db;
            _importHelperService = importHelperService;
        }

        /// <summary>
        /// Import transfer courses from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        /// <param name="organizationId">Organization id</param>
        public void ImportTransferCourses(string connection, bool overwrite, int organizationId)
        {
            var organization = _db.Organizations.FirstOrDefault(x => x.Id == organizationId);
            if (organization == null)
                throw new Exception("Organization not found");

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.courses_students_transfer";

                //13 - college
                if (organization.IsMain)
                    query += " WHERE program != 13";
                else
                    query += " WHERE program = 13";

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
                                ProcessTransferCourses(sdr, overwrite, organizationId);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void ProcessTransferCourses(NpgsqlDataReader sdr, bool overwrite, int organizationId)
        {
            var importTransferCourseVM = new ImportTransferCourseViewModel();
            importTransferCourseVM.StudentUserId = _importHelperService.GetStudentUserIdByStudentId(organizationId,
                                                    int.Parse(sdr["sid"].ToString()));
            importTransferCourseVM.UniversityId = _importHelperService.GetUniversityId(sdr["uni"].ToString());
            importTransferCourseVM.Year = int.Parse(sdr["triyear"].ToString());
            importTransferCourseVM.Season = _importHelperService.GetSeason(sdr["triseason"].ToString());
            importTransferCourseVM.NameEng = sdr["course_nameeng"].ToString();

            var transferCourse = _db.TransferCourses
                .FirstOrDefault(x => x.StudentUserId == importTransferCourseVM.StudentUserId
                                    && x.OrganizationId == organizationId 
                                    && x.UniversityId == importTransferCourseVM.UniversityId
                                    && x.Year == importTransferCourseVM.Year 
                                    && x.Season == importTransferCourseVM.Season
                                    && x.NameEng == importTransferCourseVM.NameEng);

            if (transferCourse != null)
            {
                if (overwrite)
                    EditTransferCourses(sdr, organizationId, transferCourse, importTransferCourseVM);
            }
            else
                CreateTransferCourses(sdr, organizationId, importTransferCourseVM);
        }

        private void CreateTransferCourses(NpgsqlDataReader sdr, int organizationId, 
            ImportTransferCourseViewModel importTransferCourseVM)
        {
            TransferCourse transferCourse = new TransferCourse();
            transferCourse.StudentUserId = importTransferCourseVM.StudentUserId;
            transferCourse.UniversityId = importTransferCourseVM.UniversityId;
            transferCourse.NameEng = importTransferCourseVM.NameEng;
            transferCourse.NameRus = sdr["course_namerus"].ToString();
            transferCourse.NameKir = sdr["course_namekyr"].ToString();
            transferCourse.Season = importTransferCourseVM.Season;
            transferCourse.Year = importTransferCourseVM.Year;
            transferCourse.Points = float.Parse(sdr["points"].ToString());
            transferCourse.OrganizationId = organizationId;

            _db.TransferCourses.Add(transferCourse);
        }

        private void EditTransferCourses(NpgsqlDataReader sdr, int organizationId, TransferCourse transferCourse,
            ImportTransferCourseViewModel importTransferCourseVM)
        {
            transferCourse.StudentUserId = importTransferCourseVM.StudentUserId;
            transferCourse.UniversityId = importTransferCourseVM.UniversityId;
            transferCourse.NameEng = importTransferCourseVM.NameEng;
            transferCourse.NameRus = sdr["course_namerus"].ToString();
            transferCourse.NameKir = sdr["course_namekyr"].ToString();
            transferCourse.Season = importTransferCourseVM.Season;
            transferCourse.Year = importTransferCourseVM.Year;
            transferCourse.Points = float.Parse(sdr["points"].ToString());

            _db.TransferCourses.Update(transferCourse);
        }
    }
}
