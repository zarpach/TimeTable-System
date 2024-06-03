using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.ExportData;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Settings;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace iuca.Application.Services.ExportData
{
    public class ExportStudentCourseService : IExportStudentCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly IStudentOrgInfoService _studentOrgInfoService;
        private readonly IOrganizationService _organizationService;
        private readonly ISemesterService _semesterService;

        public ExportStudentCourseService(IApplicationDbContext db,
            IStudentOrgInfoService studentOrgInfoService,
            IOrganizationService organizationService,
            ISemesterService semesterService)
        {
            _db = db;
            _studentOrgInfoService = studentOrgInfoService;
            _organizationService = organizationService;
            _semesterService = semesterService;
        }

        /// <summary>
        /// Export student courses to old DB by registration course id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <param name="connection">Connection</param>
        /// <returns>Added and dropped courses count</returns>
        public ExportCourseResultViewModel ExportStudentCoursesByRegistrationCourse(int organizationId, int registrationCourseId,
            string connection)
        {
            List<ExportCourseViewModel> exportCourses = GetExportCoursesByRegistrationCourse(organizationId, registrationCourseId);
            var registrationCourse = _db.AnnouncementSections.FirstOrDefault(x => x.Id == registrationCourseId);
            if (registrationCourse == null)
                throw new Exception($"Registration course with id {registrationCourseId} not found");
            
            List<int> courseDetIds = new List<int> { registrationCourse.CourseDetId };
            return ProcessExportCourses(organizationId, courseDetIds, exportCourses, connection, false);
        }

        /// <summary>
        /// Export student courses to old DB by semester id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="connection">Connection</param>
        /// <param name="force">If true deletes unnecessary rows from old DB</param>
        /// <returns>Added and dropped courses count</returns>
        public ExportCourseResultViewModel ExportStudentCoursesBySemester(int organizationId, int semesterId, 
            string connection, bool force) 
        {
            var semester = _semesterService.GetSemester(organizationId, semesterId);
            var registrationCourseDetIds = _db.AnnouncementSections.Where(x => x.OrganizationId == organizationId
                && x.Year == semester.Year && x.Season == semester.Season).Select(x => x.CourseDetId).ToList();

            List<ExportCourseViewModel> exportCourses = GetExportCoursesBySemester(organizationId, semesterId);

            return ProcessExportCourses(organizationId, registrationCourseDetIds, exportCourses, connection, force);
        }

        private ExportCourseResultViewModel ProcessExportCourses(int organizationId, List<int> courseDetIds, List<ExportCourseViewModel> exportCourses, 
            string connection, bool force) 
        {
            ExportCourseResultViewModel result = new ExportCourseResultViewModel();

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                conn.Open();

                int programId = GetProgramId(organizationId);

                foreach (int courseDetId in courseDetIds)
                {
                    var exportCourseStudents = exportCourses.Where(x => x.CourseDetId == courseDetId).ToList();

                    //Find all students for the course in old DB
                    var oldDbCourseStudents = GetOldDbCourseStudents(courseDetId, programId, conn);
                    
                    foreach (var exportCourseStudent in exportCourseStudents) 
                    {
                        //Get student from old db course list
                        var oldDbCourseStudent = oldDbCourseStudents.FirstOrDefault(x => x.StudentId == exportCourseStudent.StudentId
                                && x.CourseDetId == exportCourseStudent.CourseDetId);

                        bool studentCourseExists = oldDbCourseStudent != null;
                        
                        if (exportCourseStudent.IsDismissed || exportCourseStudent.IsDeleted || exportCourseStudent.State == enu_CourseState.Dropped)
                        {
                            if (studentCourseExists)
                            {
                                DeleteCourse(exportCourseStudent, programId, conn);
                                result.DroppedCoursesQty++;
                            }

                            if (exportCourseStudent.IsDeleted)
                                DeleteStudentCourseFromNewDB(exportCourseStudent.StudentCourseId);
                        }
                        else if (RegistrationCourseExists(exportCourseStudent.CourseDetId, conn) && !studentCourseExists)
                        {
                            InsertCourse(exportCourseStudent, programId, conn);
                            result.AddedCoursesQty++;
                        }
                        
                        if (studentCourseExists)
                            oldDbCourseStudents.Remove(oldDbCourseStudent);
                    }

                    //Delete unnecessary rows from old DB
                    if (force && oldDbCourseStudents.Count > 0) 
                    {
                        foreach (var oldDbCourseStudent in oldDbCourseStudents) 
                        {
                            DeleteCourse(oldDbCourseStudent.StudentId, oldDbCourseStudent.CourseDetId, 
                                programId, conn);
                            result.DroppedCoursesQty++;
                        }   
                    }
                }
            }
            return result;
        }

        private int GetProgramId(int organizationId) 
        {
            var programId = 0;
            var organization = _organizationService.GetMainOrganization();
            if (organizationId == organization.Id)
                programId = organizationId;
            else
                programId = 13; // College

            return programId;
        }

        private List<ExportCourseViewModel> GetExportCoursesByRegistrationCourse(int organizationId, int registrationCourseId)
        {
            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.StudentCourseRegistration)
                .Include(x => x.AnnouncementSection)
                .Include(x => x.Grade)
                .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId &&
                    x.AnnouncementSectionId == registrationCourseId &&
                    ((x.StudentCourseRegistration.State == (int)enu_RegistrationState.Submitted &&
                    x.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.NotSent) ||
                    x.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.Submitted)).ToList();

            return GetExportCoursesFromStudentCourses(organizationId, studentCourses);
        }

        private List<ExportCourseViewModel> GetExportCoursesBySemester(int organizationId, int semesterId) 
        {
            var studentCourses = _db.StudentCoursesTemp
                .Include(x => x.StudentCourseRegistration)
                .Include(x => x.AnnouncementSection)
                .Include(x => x.Grade)
                .Where(x => x.StudentCourseRegistration.OrganizationId == organizationId &&
                    x.StudentCourseRegistration.SemesterId == semesterId &&
                    ((x.StudentCourseRegistration.State == (int)enu_RegistrationState.Submitted && 
                    x.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.NotSent) ||
                    x.StudentCourseRegistration.AddDropState == (int)enu_RegistrationState.Submitted)).ToList();

            return GetExportCoursesFromStudentCourses(organizationId, studentCourses);
        }

        private List<ExportCourseViewModel> GetExportCoursesFromStudentCourses(int organizationId, List<StudentCourseTemp> studentCourses) 
        {
            List<ExportCourseViewModel> exportCourses = new List<ExportCourseViewModel>();

            var grade = _db.Grades.FirstOrDefault(x => x.GradeMark == "*");
            if (grade == null)
                throw new Exception("Grade * not found");

            foreach (var studentCourse in studentCourses)
            {
                ExportCourseViewModel exportCourse = new ExportCourseViewModel();

                var studentOrgInfo = _studentOrgInfoService.GetStudentOrgInfoByUserId(organizationId,
                    studentCourse.StudentCourseRegistration.StudentUserId);

                exportCourse.StudentCourseId = studentCourse.Id;
                exportCourse.StudentId = studentOrgInfo.StudentId;
                exportCourse.CourseDetId = studentCourse.AnnouncementSection.CourseDetId;
                exportCourse.GradeImportCode = studentCourse.GradeId == null ? grade.ImportCode : studentCourse.Grade.ImportCode;
                exportCourse.State = (enu_CourseState)studentCourse.State;
                exportCourse.IsDismissed = (studentOrgInfo.State == (int)enu_StudentState.Dismissed ||
                    studentOrgInfo.State == (int)enu_StudentState.AcadLeave) && studentCourse.GradeId == null;
                exportCourse.IsDeleted = studentCourse.MarkedDeleted;

                exportCourses.Add(exportCourse);
            }

            return exportCourses;
        }

        private List<ExportCourseViewModel> GetOldDbCourseStudents(int courseDetId, int programId, NpgsqlConnection conn) 
        {
            List<ExportCourseViewModel> oldDbCourses = new List<ExportCourseViewModel>();

            string selectQuery = SelectStudentCoursesQuery(courseDetId, programId);
            using (NpgsqlCommand cmd = new NpgsqlCommand(selectQuery))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            ExportCourseViewModel row = new ExportCourseViewModel();
                            row.StudentId = int.Parse(sdr["sid"].ToString());
                            row.CourseDetId = int.Parse(sdr["coursedetId"].ToString());
                            //row.GradeImportCode = int.Parse(sdr["grade"].ToString());
                            oldDbCourses.Add(row);
                        }
                    }
                }
            }

            return oldDbCourses;
        }

        private bool RegistrationCourseExists(int courseDetId, NpgsqlConnection conn)
        {
            bool courseExists = false;
            string selectQuery = SelectRegistrationCourseQuery(courseDetId);
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

        private void DeleteStudentCourseFromNewDB(int studentCourseId) 
        {
            var studentCourse = _db.StudentCoursesTemp.FirstOrDefault(x => x.Id == studentCourseId);
            if (studentCourse == null)
                throw new Exception($"Student course with id {studentCourseId} not found");

            _db.StudentCoursesTemp.Remove(studentCourse);
            _db.SaveChanges();
        }

        private void InsertCourse(ExportCourseViewModel exportCourse, int programId, NpgsqlConnection conn)
        {
            string insertQuery = InsertQuery(exportCourse, programId);
            using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteCourse(ExportCourseViewModel exportCourse, int programId, NpgsqlConnection conn)
        {
            string deleteQuery = DeleteQuery(exportCourse, programId);
            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteQuery))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteCourse(int studentId, int courseDetId, int programId, NpgsqlConnection conn)
        {
            string deleteQuery = DeleteQuery(studentId, courseDetId, programId);
            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteQuery))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private string SelectRegistrationCourseQuery(int courseDetId)
        {
            return $"SELECT 1 FROM auca.course_details WHERE coursedetid = {courseDetId}";
        }

        private string SelectStudentCoursesQuery(int courseDetId, int programId)
        {
            return $"SELECT * FROM auca.courses_students WHERE coursedetid = {courseDetId} AND program = {programId}";
        }

        private string InsertQuery(ExportCourseViewModel exportCourse, int programId)
        {
            return $"INSERT INTO auca.courses_students (sid, coursedetid, grade, program) VALUES ({exportCourse.StudentId}, {exportCourse.CourseDetId}, {exportCourse.GradeImportCode}, {programId}); ";
        }

        private string DeleteQuery(ExportCourseViewModel exportCourse, int programId)
        {
            return $"DELETE FROM auca.courses_students WHERE sid = {exportCourse.StudentId} AND coursedetid = {exportCourse.CourseDetId} AND program = {programId}";
        }

        private string DeleteQuery(int studentId, int courseDetId, int programId)
        {
            return $"DELETE FROM auca.courses_students WHERE sid = {studentId} AND coursedetid = {courseDetId} AND program = {programId}";
        }
    }
}
