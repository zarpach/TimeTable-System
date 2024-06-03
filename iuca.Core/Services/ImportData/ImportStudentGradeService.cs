using iuca.Application.Enums;
using iuca.Application.Interfaces.ImportData;
using iuca.Domain.Entities.Common;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.ImportData
{
    public class ImportStudentGradeService : IImportStudentGradeService
    {
        private readonly IApplicationDbContext _db;
        private readonly IImportHelperService _importHelperService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        private List<Course> coursesCash;
        private List<Grade> gradesCash;
        private List<StudentOrgInfo> studentOrgInfoCash;
        private List<StudentCourseGrade> studentCourseGradesCash;

        public ImportStudentGradeService(IApplicationDbContext db,
            IImportHelperService importHelperService,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _importHelperService = importHelperService;
            _userManager = userManager;
        }

        /// <summary>
        /// Import students grades from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        /// <param name="organizationId">Organization id</param>
        public void ImportStudentsGrades(string connection, bool overwrite, int organizationId)
        {
            var organization = _db.Organizations.FirstOrDefault(x => x.Id == organizationId);
            if (organization == null)
                throw new Exception("Organization not found");

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.student_grades_for_import";

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
                            coursesCash = _db.Courses.Where(x => x.OrganizationId == organizationId).ToList();
                            gradesCash = _db.Grades.ToList();
                            studentOrgInfoCash = _db.StudentOrgInfo
                                .Include(x => x.StudentBasicInfo)
                                .Where(x => x.OrganizationId == organizationId).ToList();

                            studentCourseGradesCash = _db.StudentCourseGrades.Where(x => x.OrganizationId == organizationId).ToList();
                            while (sdr.Read())
                            {
                                ProcessStudentsGrades(sdr, overwrite, organizationId);
                            }

                            //Delete removed courses
                            if (studentCourseGradesCash.Count > 0) 
                                _db.StudentCourseGrades.RemoveRange(studentCourseGradesCash);

                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void ProcessStudentsGrades(NpgsqlDataReader sdr, bool overwrite, int organizationId)
        {
            if (string.IsNullOrEmpty(sdr["sid"].ToString()))
                return;

            int importCode = int.Parse(sdr["importcode"].ToString());
            var studentCourseGrade = studentCourseGradesCash.FirstOrDefault(x => x.ImportCode == importCode);
            if (studentCourseGrade != null)
            {
                if (overwrite)
                    EditStudentsGrade(sdr, studentCourseGrade);
                studentCourseGradesCash.Remove(studentCourseGrade);
            }
            else
                CreateStudentsGrade(sdr, organizationId);
        }

        private void CreateStudentsGrade(NpgsqlDataReader sdr, int organizationId)
        {
            StudentCourseGrade studentCourseGrade = new StudentCourseGrade();

            studentCourseGrade.ImportCode = int.Parse(sdr["importcode"].ToString());
            studentCourseGrade.StudentUserId = GetStudentUserId(int.Parse(sdr["sid"].ToString()));
            studentCourseGrade.CourseId = GetCourseId(int.Parse(sdr["cid"].ToString()));
            studentCourseGrade.Season = _importHelperService.GetSeason(sdr["season"].ToString());
            studentCourseGrade.Year = int.Parse(sdr["year"].ToString());
            studentCourseGrade.Points = float.Parse(sdr["points"].ToString());
            studentCourseGrade.GradeId = _importHelperService.GetGradeId(sdr["grade"].ToString());
            studentCourseGrade.OrganizationId = organizationId;

            _db.StudentCourseGrades.Add(studentCourseGrade);
        }

        private void EditStudentsGrade(NpgsqlDataReader sdr, StudentCourseGrade studentCourseGrade)
        {
            studentCourseGrade.StudentUserId = GetStudentUserId(int.Parse(sdr["sid"].ToString()));
            studentCourseGrade.CourseId = GetCourseId(int.Parse(sdr["cid"].ToString()));
            studentCourseGrade.Season = _importHelperService.GetSeason(sdr["season"].ToString());
            studentCourseGrade.Year = int.Parse(sdr["year"].ToString());
            studentCourseGrade.Points = float.Parse(sdr["points"].ToString());
            studentCourseGrade.GradeId = _importHelperService.GetGradeId(sdr["grade"].ToString());

            _db.StudentCourseGrades.Update(studentCourseGrade);
        }

       private string GetStudentUserId(int studentId) 
        {
            var orgInfo = studentOrgInfoCash.FirstOrDefault(x => x.StudentId == studentId);
            if (orgInfo == null)
                throw new Exception($"Student with id {studentId} not found");

            if (orgInfo.StudentBasicInfo == null)
                throw new Exception($"Student basic info not found");

            return orgInfo.StudentBasicInfo.ApplicationUserId;
        }

        private int GetCourseId(int importCode)
        {
            var course = coursesCash.FirstOrDefault(x => x.ImportCode == importCode);
            if (course == null)
                throw new Exception($"Course with import code {importCode} not found");
            return course.Id;
        }

        


    }
}
