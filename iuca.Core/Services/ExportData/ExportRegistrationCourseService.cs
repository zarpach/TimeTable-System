using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.ExportData;
using iuca.Application.ViewModels.Settings;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace iuca.Application.Services.ExportData
{
    public class ExportRegistrationCourseService : IExportRegistrationCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly ISemesterService _semesterService;
        private readonly ILogger<ExportRegistrationCourseService> _logger;

        public ExportRegistrationCourseService(IApplicationDbContext db,
            ISemesterService semesterService,
            ILogger<ExportRegistrationCourseService> logger)
        {
            _db = db;
            _semesterService = semesterService;
            _logger = logger;
        }

        /// <summary>
        /// Export announcement sections
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="force">Force update</param>
        /// <param name="connection">Connection</param>
        public void ExportAnnouncementSections(int organizationId, int semesterId, bool force, string connection) 
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                conn.Open();
                var contextTransaction = _db.Database.BeginTransaction();
                var externalDbTransaction = conn.BeginTransaction();

                try 
                {
                    ProcessAnnouncementSections(organizationId, semesterId, force, conn);
                    externalDbTransaction.Commit();
                    contextTransaction.Commit();
                }
                catch (Exception ex) 
                {
                    externalDbTransaction.Rollback();
                    contextTransaction.Rollback();
                    throw;
                }
            }
        }

        private void ProcessAnnouncementSections(int organizationId, int semesterId, bool force, NpgsqlConnection conn)
        {
            var announcementSections = _db.AnnouncementSections.Include(x => x.Announcement).ThenInclude(x => x.Course)
                .Include(x => x.Announcement).ThenInclude(x => x.Semester)
                .Where(x => x.OrganizationId == organizationId && x.Announcement.SemesterId == semesterId)
                .ToList();

            var semester = _semesterService.GetSemester(semesterId);
            var existingCourseDetailIds = GetExistingCourseDetailIds(GetOldDbSeason((enu_Season)semester.Season), semester.Year, conn);

            foreach (var announcementSection in announcementSections)
            {
                if (announcementSection.CourseDetId == 0)
                    InsertAnnouncementSection(announcementSection, conn);
                else
                {
                    existingCourseDetailIds.Remove(announcementSection.CourseDetId);
                    if (AnnoucementSectionExists(announcementSection.CourseDetId, conn))
                        UpdateAnnouncementSection(announcementSection, force, conn);
                    else
                        InsertAnnouncementSection(announcementSection, conn);
                }
            }

            if (existingCourseDetailIds.Count > 0)
            {
                foreach (var courseDetId in existingCourseDetailIds) 
                {
                    if (StudentCoursesExist(courseDetId, conn)) 
                        DeleteStudentCourses(courseDetId, conn);

                    DeleteAnnouncementSection(courseDetId, conn);
                }
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// Update announcement section data in old DB
        /// </summary>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <param name="connection">Connection</param>
        public void ExportRegistrationCourseData(int announcementSectionId, string connection)
        {
            var announcementSection = GetAnnouncementSection(announcementSectionId);

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                conn.Open();
                var contextTransaction = _db.Database.BeginTransaction();
                var externalDbTransaction = conn.BeginTransaction();

                try 
                {
                    if (announcementSection.CourseDetId == 0)
                        InsertAnnouncementSection(announcementSection, conn);
                    else
                    {
                        if (AnnoucementSectionExists(announcementSection.CourseDetId, conn))
                            UpdateAnnouncementSection(announcementSection, false, conn);
                        else
                            InsertAnnouncementSection(announcementSection, conn);
                    }

                    _db.SaveChanges();
                    externalDbTransaction.Commit();
                    contextTransaction.Commit();
                }
                catch (Exception ex) 
                {
                    contextTransaction.Rollback();
                    externalDbTransaction.Rollback();
                    throw;
                }
                
            }
        }

        private AnnouncementSection GetAnnouncementSection(int announcementSectionId) 
        {
            if (announcementSectionId == 0)
                throw new Exception("Announcement section id is 0");

            var announcementSection = _db.AnnouncementSections
                .Include(x => x.Announcement)
                .ThenInclude(x => x.Course)
                .Include(x => x.Announcement)
                .ThenInclude(x => x.Semester)
                .Include(x => x.ExtraInstructors)
                .FirstOrDefault(x => x.Id == announcementSectionId);

            if (announcementSection == null)
                throw new Exception($"Announcement section with id {announcementSectionId} not found");

            return announcementSection;
        }

        private ExportAnnoucementSectionViewModel GetAnnouncementSectionModel(AnnouncementSection announcementSection) 
        {
            var instructor = _db.InstructorOrgInfo.Include(x => x.InstructorBasicInfo)
                .FirstOrDefault(x => x.OrganizationId == announcementSection.OrganizationId &&
                    x.InstructorBasicInfo.InstructorUserId == announcementSection.InstructorUserId);

            if (instructor == null)
                throw new Exception($"Instructor with id {announcementSection.InstructorUserId} not found");

            if (instructor.ImportCode == 0)
                throw new Exception($"Instructor ImportCode is wrong");

            if (announcementSection.Announcement.Course.ImportCode == 0)
                throw new Exception($"Course ImportCode is wrong");

            ExportAnnoucementSectionViewModel model = new ExportAnnoucementSectionViewModel();
            model.CourseDetId = announcementSection.CourseDetId;
            model.CourseImportCode = announcementSection.Announcement.Course.ImportCode;
            model.InstructorImportCode = instructor.ImportCode;
            model.Season = GetOldDbSeason((enu_Season)announcementSection.Announcement.Semester.Season);
            model.Year = announcementSection.Announcement.Semester.Year;
            model.Section = announcementSection.Section;
            model.Points = announcementSection.Credits;

            if (announcementSection.ExtraInstructorsJson != null)
            {
                model.ExtraInstructors.AddRange(GetExtraInstructorsIds(announcementSection.OrganizationId,
                    announcementSection.ExtraInstructorsJson.ToList()));
            }
            else if (announcementSection.ExtraInstructors != null && announcementSection.ExtraInstructors.Any()) 
            {
                model.ExtraInstructors.AddRange(GetExtraInstructorsIds(announcementSection.OrganizationId,
                    announcementSection.ExtraInstructors.Select(x => x.InstructorUserId).ToList()));
            }

            return model;
        }

        private List<int> GetExtraInstructorsIds(int organizationId, List<string> instructorUserIds) 
        {
            List<int> ids = new List<int>();
            var extraInstructors = _db.InstructorOrgInfo.Include(x => x.InstructorBasicInfo)
                    .Where(x => x.OrganizationId == organizationId && 
                    instructorUserIds.Contains(x.InstructorBasicInfo.InstructorUserId)).ToList();

            foreach (var extra in extraInstructors)
            {
                if (extra.ImportCode == 0)
                    throw new Exception($"Instructor with id {extra.InstructorBasicInfo.InstructorUserId} ImportCode is wrong");
                ids.Add(extra.ImportCode);
            }

            return ids;
        }

        private List<int> GetExistingCourseDetailIds(int season, int year, NpgsqlConnection conn)
        {
            var courseDetailIds = new List<int>();
            string query = SelectCourseDetailIdsQuery(season, year);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                            courseDetailIds.Add(int.Parse(sdr["coursedetid"].ToString()));
                    }
                }
            }

            return courseDetailIds;
        }

        private int GetLastCourseDetId(NpgsqlConnection conn)
        {
            int coruseDetId = 0;
            string selectQuery = SelectLastCourseDetIdQuery();
            using (NpgsqlCommand cmd = new NpgsqlCommand(selectQuery))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        coruseDetId = int.Parse(sdr["max"].ToString());
                    }
                }
            }

            return coruseDetId;
        }

        private void InsertAnnouncementSection(AnnouncementSection announcementSection, NpgsqlConnection conn)
        {
            var announcementSectionModel = GetAnnouncementSectionModel(announcementSection);
            announcementSectionModel.CourseDetId = GetLastCourseDetId(conn) + 1;

            string query = InsertQuery(announcementSectionModel);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                try
                {
                    var result = cmd.ExecuteNonQuery();

                    if (result <= 0)
                        throw new Exception($"Announcement section with CourseImportCode {announcementSectionModel.CourseImportCode} was not inserted. Query: {query}");

                    InsertSecondInstructors(announcementSectionModel, conn);

                    announcementSection.CourseDetId = announcementSectionModel.CourseDetId;
                }
                catch (PostgresException ex)
                {
                    _logger.LogError($"query: {cmd.CommandText}");
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private void UpdateAnnouncementSection(AnnouncementSection announcementSection, bool force, NpgsqlConnection conn) 
        {
            if (force || announcementSection.IsChanged) 
            {
                var announcementSectionModel = GetAnnouncementSectionModel(announcementSection);
                string updateQuery = UpdateQuery(announcementSectionModel);
                using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery))
                {
                    cmd.Connection = conn;
                    var result = cmd.ExecuteNonQuery();

                    if (result <= 0) 
                        throw new Exception($"Course with CourseImportCode ({announcementSectionModel.CourseImportCode}) was not updated. Query: {updateQuery}");

                    UpdateSecondInstructors(announcementSectionModel, conn);

                    announcementSection.IsChanged = false;
                }
            }
        }

        private bool AnnoucementSectionExists(int courseDetId, NpgsqlConnection conn)
        {
            bool exists = false;
            string query = SelectQuery(courseDetId);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                        exists = true;
                }
            }

            return exists;
        }

        private bool StudentCoursesExist(int courseDetId, NpgsqlConnection conn)
        {
            bool courseExists = false;
            string query = SelectStudentCoursesQuery(courseDetId);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
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

        private void DeleteStudentCourses(int courseDetId, NpgsqlConnection conn)
        {
            string query = DeleteStudentCoursesQuery(courseDetId);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                var result = cmd.ExecuteNonQuery();

                if (result <= 0)
                    throw new Exception("Student courses were not deleted");
            }
        }

        private void DeleteAnnouncementSection(int courseDetId, NpgsqlConnection conn) 
        {
            string deleteQuery = DeleteQuery(courseDetId);
            using (NpgsqlCommand cmd = new NpgsqlCommand(deleteQuery))
            {
                cmd.Connection = conn;
                var result = cmd.ExecuteNonQuery();

                if (result <= 0)
                    throw new Exception($"Announcement section was not deleted. Query: {deleteQuery}");
            }
        }

        private void InsertSecondInstructors(ExportAnnoucementSectionViewModel model, NpgsqlConnection conn)
        {
            if (model.ExtraInstructors.Count > 0)
                foreach (var extraInstructorId in model.ExtraInstructors)
                    InsertSecondInstructor(model.CourseDetId, extraInstructorId, conn);
        }

        private void UpdateSecondInstructors(ExportAnnoucementSectionViewModel model, NpgsqlConnection conn) 
        {
            var existingSecondInstructors = GetExistingSecondInstructors(model.CourseDetId, conn);
            
            foreach (var extraInstructorId in model.ExtraInstructors) 
            {
                if (!existingSecondInstructors.Contains(extraInstructorId)) 
                    InsertSecondInstructor(model.CourseDetId, extraInstructorId, conn);

                existingSecondInstructors.Remove(extraInstructorId);
            }
            
            foreach (var iid in existingSecondInstructors) 
                DeleteSecondInstructor(model.CourseDetId, iid, conn);
        }

        private List<int> GetExistingSecondInstructors(int courseDetId, NpgsqlConnection conn) 
        {
            List<int> iidList = new List<int>();
            string query = SelectSecondInstructorQuery(courseDetId);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            int iid;
                            if (int.TryParse(sdr["iid2"].ToString(), out iid))
                                iidList.Add(iid);
                        }
                    }
                }
            }

            return iidList;
        }

        private void InsertSecondInstructor(int courseDetId, int iid, NpgsqlConnection conn) 
        {
            string query = InsertSecondInstructorQuery(courseDetId, iid);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteSecondInstructor(int courseDetId, int iid, NpgsqlConnection conn)
        {
            string query = DeleteSecondInstructorQuery(courseDetId, iid);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private string SelectLastCourseDetIdQuery()
        {
            return $"SELECT MAX(coursedetid) FROM auca.course_details";
        }

        private string SelectCourseDetailIdsQuery(int season, int year)
        {
            return $"SELECT coursedetid FROM auca.course_details WHERE triseason = { season } AND triyear = { year }";
        }

        private string SelectQuery(int courseDetId)
        {
            return $"SELECT 1 FROM auca.course_details WHERE coursedetid = {courseDetId}";
        }

        private string InsertQuery(ExportAnnoucementSectionViewModel model)
        {
            return $"INSERT INTO auca.course_details " +
                $"(coursedetid, cid, iid, triseason, triyear, section, number_of_groups, whenhour, room, points, labsection) " +
                $"VALUES ({model.CourseDetId}, {model.CourseImportCode}, {model.InstructorImportCode}, " +
                $"{model.Season}, {model.Year}, '{model.Section}', {1}, '{""}', '{""}', {model.Points.ToString("G", CultureInfo.InvariantCulture)}, {false});";
        }

        private string UpdateQuery(ExportAnnoucementSectionViewModel exportRegistrationCourse)
        {
            return $"UPDATE auca.course_details SET cid = {exportRegistrationCourse.CourseImportCode}, " +
                $"iid = {exportRegistrationCourse.InstructorImportCode}, " +
                $"triseason = {exportRegistrationCourse.Season}, " +
                $"triyear = {exportRegistrationCourse.Year}, " +
                $"section = '{exportRegistrationCourse.Section}', " +
                $"points = {exportRegistrationCourse.Points} " +
                $"WHERE coursedetid = {exportRegistrationCourse.CourseDetId}";
        }

        private string SelectStudentCoursesQuery(int courseDetId) 
        {
            return $"SELECT 1 FROM auca.courses_students WHERE coursedetid = {courseDetId}";
        }

        private string DeleteStudentCoursesQuery(int courseDetId)
        {
            return $"DELETE FROM auca.courses_students WHERE coursedetid = {courseDetId}";
        }

        private string DeleteQuery(int courseDetId)
        {
            return $"DELETE FROM auca.course_details WHERE coursedetid = {courseDetId}";
        }

        private string SelectSecondInstructorQuery(int courseDetId)
        {
            return $"SELECT iid2 FROM auca.course_second_instructor WHERE coursedetid = {courseDetId}";
        }

        private string InsertSecondInstructorQuery(int courseDetId, int iid) 
        {
            return $"INSERT INTO auca.course_second_instructor (coursedetid, iid2) VALUES ({courseDetId}, {iid})";
        }

        private string DeleteSecondInstructorQuery(int courseDetId, int iid)
        {
            return $"DELETE FROM auca.course_second_instructor WHERE coursedetid = {courseDetId} and iid2 = { iid }";
        }

        private int GetOldDbSeason(enu_Season season) 
        {
            return season switch
            {
                enu_Season.Fall => 1,
                enu_Season.Spring => 2,
                enu_Season.Summer => 3,
                enu_Season.Winter => 29,
                _ => 1
            };
        }
        
    }
}
