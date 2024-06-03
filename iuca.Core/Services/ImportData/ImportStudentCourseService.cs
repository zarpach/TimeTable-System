using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.ImportData;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.ImportData
{
    public  class ImportStudentCourseService : IImportStudentCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly ISemesterService _semesterService;
        private readonly IImportHelperService _importHelperService;

        private List<StudentOrgInfo> studentsCash;
        private List<AnnouncementSection> registrationCoursesCash;
        private List<StudentCourseDetGrade> studentCourses;

        public ImportStudentCourseService(IApplicationDbContext db,
            ISemesterService semesterService,
            IImportHelperService importHelperService)
        { 
            _db = db;
            _semesterService = semesterService;
            _importHelperService = importHelperService;
        }

        /// <summary>
        /// Import student registration courses from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        public void ImportStudentCourses(string connection, bool overwrite, int organizationId, int semesterId)
        {
            var organization = _db.Organizations.FirstOrDefault(x => x.Id == organizationId);
            if (organization == null)
                throw new Exception("Organization not found");

            var semester = _semesterService.GetSemester(organizationId, semesterId);
            if (semester == null)
                throw new Exception("Semester not found");

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = $"SELECT * FROM auca.student_courses_{getSeasonName((enu_Season)semester.Season)}_{semester.Year}_myiuca";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            studentsCash = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                                            .Where(x => x.OrganizationId == organizationId).ToList();

                            registrationCoursesCash = _db.AnnouncementSections.Where(x => x.OrganizationId == organizationId
                                && x.Season == semester.Season && x.Year == semester.Year).ToList();

                            studentCourses = new List<StudentCourseDetGrade>();

                            while (sdr.Read())
                            {
                                studentCourses.Add(new StudentCourseDetGrade(sdr));
                            }
                            if (studentCourses.Count > 0) 
                            {
                                using (var transaction = _db.Database.BeginTransaction()) 
                                {
                                    try
                                    {
                                        ProcessStudentCourses(overwrite, organizationId, semesterId);
                                        _db.SaveChanges();
                                        transaction.Commit();
                                    }
                                    catch (Exception ex) 
                                    {
                                        transaction.Rollback();
                                        throw;
                                    }
                                }
                                    
                            }
                            
                        }
                    }
                }
            }
        }

        private string getSeasonName(enu_Season season)
        {
            string name = "";

            if (season == enu_Season.Fall)
                name = "fall";
            else if (season == enu_Season.Spring)
                name = "spring";
            else if (season == enu_Season.Winter)
                name = "winter";
            else if (season == enu_Season.Summer)
                name = "summer";

            return name;
        }

        private void ProcessStudentCourses(bool overwrite, int organizationId, int semesterId)
        {

            foreach (var studentId in studentCourses.GroupBy(x => x.StudentId)) 
            {
                var student = studentsCash.FirstOrDefault(x => x.StudentId == studentId.Key);
                if (student != null) 
                {
                    var studentCourseRegistration = _db.StudentCourseRegistrations.FirstOrDefault(x => x.OrganizationId == organizationId
                        && x.SemesterId == semesterId && x.StudentUserId == student.StudentBasicInfo.ApplicationUserId);

                    if (studentCourseRegistration != null)
                    {
                        if (overwrite)
                            EditStudentRegistration(studentCourseRegistration, studentId.ToList());
                    }
                    else
                        CreateStudentRegistration(student.StudentBasicInfo.ApplicationUserId, 
                            semesterId, organizationId, studentId.ToList());
                }
            }
        }

        private void CreateStudentRegistration(string studentUserId, int semesterId, int organizationId,
                List<StudentCourseDetGrade> studentCourses) 
        {
            StudentCourseRegistration studentCourseRegistration = new StudentCourseRegistration();
            studentCourseRegistration.StudentUserId = studentUserId;
            studentCourseRegistration.SemesterId = semesterId;
            studentCourseRegistration.IsApproved = true;
            studentCourseRegistration.OrganizationId = organizationId;
            studentCourseRegistration.State = (int)enu_RegistrationState.Submitted;
            studentCourseRegistration.DateCreate = DateTime.Now;

            _db.StudentCourseRegistrations.Add(studentCourseRegistration);
            _db.SaveChanges();

            foreach (var studentCourse in studentCourses) 
            {
                var registrationCourse = registrationCoursesCash.FirstOrDefault(x => x.CourseDetId == studentCourse.CourseDetId);
                if (registrationCourse == null)
                    throw new Exception($"Registration course with courseDetId {studentCourse.CourseDetId} not found");
            
                StudentCourseTemp studentCourseTemp = new StudentCourseTemp();
                studentCourseTemp.StudentCourseRegistrationId = studentCourseRegistration.Id;
                studentCourseTemp.AnnouncementSectionId = registrationCourse.Id;
                studentCourseTemp.GradeId = _importHelperService.GetGradeId(studentCourse.GradeImportCode);
                studentCourseTemp.IsApproved = true;
                studentCourseTemp.IsProcessed = true;

                _db.StudentCoursesTemp.Add(studentCourseTemp);
            }
            _db.SaveChanges();
        }

        private void EditStudentRegistration(StudentCourseRegistration studentCourseRegistration,
            List<StudentCourseDetGrade> studentCourses) 
        {
            var dbStudentCoursesTemp = _db.StudentCoursesTemp.Include(x => x.AnnouncementSection)
                .Where(x => x.StudentCourseRegistrationId == studentCourseRegistration.Id).ToList();

            foreach (var studentCourse in studentCourses) 
            {
                var studentCourseTemp = dbStudentCoursesTemp
                    .FirstOrDefault(x => x.AnnouncementSection.CourseDetId == studentCourse.CourseDetId);

                if (studentCourseTemp != null)
                {
                    studentCourseTemp.GradeId = _importHelperService.GetGradeId(studentCourse.GradeImportCode);
                    _db.StudentCoursesTemp.Update(studentCourseTemp);

                    dbStudentCoursesTemp.Remove(studentCourseTemp);
                }
                else 
                {
                    var registrationCourse = registrationCoursesCash.FirstOrDefault(x => x.CourseDetId == studentCourse.CourseDetId);
                    if (registrationCourse == null)
                        throw new Exception($"Registration course with courseDetId {studentCourse.CourseDetId} not found");

                    StudentCourseTemp newStudentCourseTemp = new StudentCourseTemp();
                    newStudentCourseTemp.StudentCourseRegistrationId = studentCourseRegistration.Id;
                    newStudentCourseTemp.AnnouncementSectionId = registrationCourse.Id;
                    newStudentCourseTemp.GradeId = _importHelperService.GetGradeId(studentCourse.GradeImportCode);
                    newStudentCourseTemp.IsApproved = true;
                    newStudentCourseTemp.IsProcessed = true;

                    _db.StudentCoursesTemp.Add(studentCourseTemp);
                }

                if (dbStudentCoursesTemp.Count > 0) 
                {
                    foreach (var dbStudentCourse in dbStudentCoursesTemp) 
                    {
                        _db.StudentCoursesTemp.Remove(dbStudentCourse);
                    }
                }
            }
            _db.SaveChanges();
        }

        public class StudentCourseDetGrade 
        {
            public StudentCourseDetGrade() { }

            public StudentCourseDetGrade(NpgsqlDataReader sdr) 
            {
                StudentId = int.Parse(sdr["sid"].ToString());
                CourseDetId = int.Parse(sdr["coursedetid"].ToString());
                GradeImportCode = sdr["grade"].ToString();
            }

            public int StudentId { get; set; }
            public int CourseDetId { get; set; }
            public string GradeImportCode { get; set; }
        }
    }

}
