using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.ExportData;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Settings;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.ExportData
{
    public class ExportStudentGradeService : IExportStudentGradeService
    {
        private readonly IApplicationDbContext _db;
        private readonly IStudentOrgInfoService _studentOrgInfoService;
        private readonly IOrganizationService _organizationService;

        public ExportStudentGradeService(IApplicationDbContext db,
            IStudentOrgInfoService studentOrgInfoService,
            IOrganizationService organizationService)
        {
            _db = db;
            _studentOrgInfoService = studentOrgInfoService;
            _organizationService = organizationService;
        }

        /// <summary>
        /// Export student grades to old DB
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="connection">Connection</param>
        /// <returns>Added courses count</returns>
        public void ExportStudentGrades(int organizationId, int semesterId, string connection)
        {
            List<ExportCourseViewModel> exportCourses = GetCourses(organizationId, semesterId);

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                conn.Open();
                foreach (var exportCourse in exportCourses)
                {
                    if (exportCourse.GradeImportCode != 0 && exportCourse.GradeImportCode != 20)
                        UpdateCourse(exportCourse, conn);
                }
            }
        }

        private List<ExportCourseViewModel> GetCourses(int organizationId, int semesterId)
        {
            List<ExportCourseViewModel> exportCourses = new List<ExportCourseViewModel>();

            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.StudentCourseRegistration)
                .Include(x => x.AnnouncementSection)
                .Include(x => x.Grade)
                .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId &&
                    x.StudentCourseRegistration.SemesterId == semesterId &&
                    x.StudentCourseRegistration.State == (int)enu_RegistrationState.Submitted).ToList();

            var defaultGrade = _db.Grades.FirstOrDefault(x => x.GradeMark == "*");
            if (defaultGrade == null)
                throw new Exception("Grade * not found");

            var programId = 0;
            var organization = _organizationService.GetMainOrganization();
            if (organizationId == organization.Id)
                programId = organizationId;
            else
                programId = 13; // College

            foreach (var studentCourse in studentCourses)
            {
                if (studentCourse.Grade != null && studentCourse.Grade != defaultGrade) 
                {
                    ExportCourseViewModel exportCourse = new ExportCourseViewModel();

                    var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(organizationId,
                        studentCourse.StudentCourseRegistration.StudentUserId);

                    exportCourse.StudentId = studentOrgInfo.StudentId;
                    exportCourse.CourseDetId = studentCourse.AnnouncementSection.CourseDetId;
                    exportCourse.GradeImportCode = studentCourse.Grade.ImportCode;
                    exportCourse.ProgramId = programId;

                    exportCourses.Add(exportCourse);
                }
            }

            return exportCourses;
        }

        private void UpdateCourse(ExportCourseViewModel exportCourse, NpgsqlConnection conn)
        {
            string updateQuery = UpdateQuery(exportCourse);
            using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private string UpdateQuery(ExportCourseViewModel exportCourse)
        {
            return $"UPDATE auca.courses_students SET grade = {exportCourse.GradeImportCode} " +
                $"WHERE sid = {exportCourse.StudentId} AND coursedetid = {exportCourse.CourseDetId} AND program = {exportCourse.ProgramId}";
        }

    }
}
