using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.ExportData;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Linq;
using System.Text;

namespace iuca.Application.Services.ExportData
{
    public class ExportCourseService : IExportCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly IOrganizationService _organizationService;

        public ExportCourseService(IApplicationDbContext db,
            IOrganizationService organizationService)
        {
            _db = db;
            _organizationService = organizationService;
        }

        public void ExportCourse(int organizationId, int courseId, string connection)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                conn.Open();

                var course = GetCourse(organizationId, courseId);

                if (course.ImportCode == 0)
                {
                    //Insert new course and save cid to ImportCode
                    InsertCourseDescription(course, conn);
                    var cid = GetLastCid(conn);
                    InsertCourseInfo(cid, course, conn);
                    course.ImportCode = cid;
                    _db.SaveChanges();
                }
                else if (course.IsChanged) 
                {
                    //Update existing course
                    UpdateCourseDescription(course.ImportCode, course, conn);
                    UpdateCourseInfo(course.ImportCode, course, conn);
                    course.IsChanged = false;
                    _db.SaveChanges();
                }
            }
        }

        private Course GetCourse(int organizationId, int courseId) 
        {
            var course = _db.Courses
                .Include(x => x.Department)
                .Include(x => x.Language)
                .FirstOrDefault(x => x.Id == courseId);


            if (course is null)
                throw new Exception($"Course with id {courseId} not found");

            if (course.Department is null || course.Department.ImportCode == 0)
                throw new Exception($"Department errors occured");

            if (course.Language is null || course.Language.ImportCode == 0)
                throw new Exception($"Language errors occured");

            return course;
        }

        private bool CourseExists(int cid, NpgsqlConnection conn)
        {
            bool courseExists = false;
            string selectQuery = SelectQuery(cid);
            using (NpgsqlCommand cmd = new NpgsqlCommand(selectQuery))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                        courseExists = true;
                }
            }

            return courseExists;
        }

        private int GetLastCid(NpgsqlConnection conn)
        {
            var cid = 0;
            string query = SelectLastCidQuery();
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        cid = int.Parse(sdr["max"].ToString());
                    }
                }
            }

            if (cid == 0)
                throw new Exception("Last course id not found");

            return cid;
        }

        private void InsertCourseDescription(Course course, NpgsqlConnection conn)
        {
            string insertQuery = InsertCourseDescriptionQuery(course);
            using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private void InsertCourseInfo(int cid, Course course, NpgsqlConnection conn)
        {
            string insertQuery = InsertCourseInfoQuery(cid, course);
            using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateCourseDescription(int cid, Course course, NpgsqlConnection conn)
        {
            string query = UpdateCourseDescriptionQuery(cid, course);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateCourseInfo(int cid, Course course, NpgsqlConnection conn)
        {
            string query = UpdateCourseInfoQuery(cid, course);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private string SelectLastCidQuery()
        {
            return $"SELECT MAX(cid) FROM auca.course_description";
        }

        private string SelectQuery(int cid)
        {
            return $"SELECT 1 FROM auca.course_description WHERE cid = {cid}";
        }

        private string InsertCourseDescriptionQuery(Course course)
        {
            return $"INSERT INTO auca.course_description (abbreviation, coursenumber, coursenameeng, department, general_education, langid) " +
                $"VALUES ('{course.Abbreviation}', '{course.Number}', '{course.NameEng}', {course.Department.ImportCode}, {true}, {course.Language.ImportCode});";
        }

        private string InsertCourseInfoQuery(int cid, Course course)
        {
            return $"INSERT INTO auca.course_descript_catalog_info (cid, russian_title, kyrgyz_title) " +
                $"VALUES ({cid}, '{course.NameRus}', '{course.NameKir}');";
        }

        private string UpdateCourseDescriptionQuery(int cid, Course course)
        {
            var querySB = new StringBuilder("UPDATE auca.course_description ");
            querySB.Append($"SET abbreviation = '{course.Abbreviation}', coursenumber = '{course.Number}', ");
            querySB.Append($"coursenameeng = '{course.NameEng}', department = {course.Department.ImportCode}, ");
            querySB.Append($"langid = {course.Language.ImportCode} ");
            querySB.Append($"WHERE cid = {cid}");

            return querySB.ToString();
        }

        private string UpdateCourseInfoQuery(int cid, Course course)
        {
            var querySB = new StringBuilder("UPDATE auca.course_descript_catalog_info ");
            querySB.Append($"SET russian_title = '{course.NameRus}', kyrgyz_title = '{course.NameKir}' ");
            querySB.Append($"WHERE cid = {cid}");

            return querySB.ToString();
        }
    }
}
