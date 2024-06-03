using iuca.Application.DTO.Common;
using iuca.Application.Enums;
using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.ImportData;
using iuca.Application.ViewModels.ImportData;
using iuca.Domain.Entities.Courses;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.Services.ImportData
{
    public class ImportRegistrationCourseService : IImportRegistrationCourseService
    {
        private readonly IApplicationDbContext _db;
        private readonly IImportHelperService _importHelperService;
        private readonly ISemesterService _semesterService;

        private List<Course> coursesCash;
        private List<InstructorOrgInfo> instructorsCash;

        public ImportRegistrationCourseService(IApplicationDbContext db,
            IImportHelperService importHelperService,
            ISemesterService semesterService)
        {
            _db = db;
            _importHelperService = importHelperService;
            _semesterService = semesterService;
        }

        /// <summary>
        /// Import announcement sections from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        public void ImportAnnouncementSections(string connection, bool overwrite, int organizationId, int semesterId)
        {
            var semester = _semesterService.GetSemester(semesterId);

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                var announcementSectionsModel = GetImportAnnouncementSectionsModel(semester, organizationId, conn);
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var announcementSectionModel in announcementSectionsModel.GroupBy(x => new { x.Cid, x.Season, x.Year }))
                        {
                            ProcessAnnouncementSections(organizationId, announcementSectionModel.Key.Cid,
                                announcementSectionModel.Key.Season, announcementSectionModel.Key.Year,
                                announcementSectionModel.ToList(), overwrite, conn);
                        }
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

        private List<ImportAnnouncementSectionViewModel> GetImportAnnouncementSectionsModel(SemesterDTO semester, 
            int organizationId, NpgsqlConnection conn)
        {
            var announcementSectionsModel = new List<ImportAnnouncementSectionViewModel>();
            string query = $"SELECT * FROM auca.courses4{getSeasonName((enu_Season)semester.Season)}_{semester.Year}_myiuca";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                conn.Open();
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        coursesCash = _db.Courses.Where(x => x.OrganizationId == organizationId).ToList();
                        instructorsCash = _db.InstructorOrgInfo.Include(x => x.InstructorBasicInfo).Where(x => x.OrganizationId == organizationId).ToList();
                        while (sdr.Read())
                        {
                            announcementSectionsModel.Add(GetImportAccouncementSectionModel(sdr));
                        }
                    }
                }
            }

            return announcementSectionsModel;
        }

        private ImportAnnouncementSectionViewModel GetImportAccouncementSectionModel(NpgsqlDataReader sdr) 
        {
            var announcementSectionModel = new ImportAnnouncementSectionViewModel();

            announcementSectionModel.CourseDetId = int.Parse(sdr["coursedetid"].ToString());
            announcementSectionModel.Cid = int.Parse(sdr["cid"].ToString());
            announcementSectionModel.Iid = int.Parse(sdr["iid"].ToString());
            announcementSectionModel.Section = sdr["section"].ToString();
            announcementSectionModel.Points = float.Parse(sdr["points"].ToString());
            announcementSectionModel.Season = _importHelperService.GetSeason(sdr["triseason"].ToString());
            announcementSectionModel.Year = int.Parse(sdr["triyear"].ToString());

            return announcementSectionModel;
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

        private void ProcessAnnouncementSections(int organizationId, int cid, int season, int year, 
            List<ImportAnnouncementSectionViewModel> announcementSections, bool overwrite, 
            NpgsqlConnection conn)
        {
            var courseId = GetCourseId(cid);
            var semester = _semesterService.GetSemester(organizationId, year, season);
            var announcement = _db.Announcements.Include(x => x.AnnouncementSections).ThenInclude(x => x.ExtraInstructors)
                .FirstOrDefault(x => x.SemesterId == semester.Id && x.CourseId == courseId);

            if (announcement != null)
            {
                if (overwrite)
                    EditAnnouncement(organizationId, announcement, announcementSections);
            }
            else 
                announcement = CreateAnnouncement(organizationId, semester.Id, courseId, announcementSections);

            ImportSecondInstructors(announcement, organizationId, conn);
        }

        private Announcement CreateAnnouncement(int organizationId, int semesterId, int courseId, 
            List<ImportAnnouncementSectionViewModel> announcementSections) 
        {
            Announcement announcement = new Announcement();
            announcement.CourseId = courseId;
            announcement.SemesterId = semesterId;
            announcement.IsActivated = true;
            announcement.AnnouncementSections = GetAnnouncementSections(organizationId, courseId, announcementSections);
            _db.Announcements.Add(announcement);
            _db.SaveChanges();

            return announcement;
        }

        private void EditAnnouncement(int organizationId, Announcement announcement, 
            List<ImportAnnouncementSectionViewModel> announcementSections) 
        {
            foreach (var announcementSectionModel in announcementSections)
            {
                int courseDetId = announcementSectionModel.CourseDetId;
                var announcementSection = announcement.AnnouncementSections.FirstOrDefault(x => x.CourseDetId == courseDetId);
                if (announcementSection != null)
                    EditAnnouncementSection(organizationId, announcement.CourseId, 
                        announcementSectionModel, announcementSection);
                else
                    CreateAnnouncementSection(organizationId, announcement, announcementSectionModel);
            }
        }

        private void CreateAnnouncementSection(int organizationId, Announcement announcement, 
            ImportAnnouncementSectionViewModel announcementSectionModel) 
        {
            var announcementSection = GetAnnouncementSection(organizationId, announcement.CourseId, 
                announcementSectionModel);
            announcementSection.AnnouncementId = announcement.Id;
            _db.AnnouncementSections.Add(announcementSection);
        }

        private List<AnnouncementSection> GetAnnouncementSections(int organizationId, int courseId, 
            List<ImportAnnouncementSectionViewModel> announcementSectionModelList)
        {
            var announcementSections = new List<AnnouncementSection>();
            foreach (var announcementSectionModel in announcementSectionModelList) 
                announcementSections.Add(GetAnnouncementSection(organizationId, courseId, announcementSectionModel));

            return announcementSections;
        }

        private AnnouncementSection GetAnnouncementSection(int organizationId, int courseId,
            ImportAnnouncementSectionViewModel announcementSectionModel) 
        {
            AnnouncementSection announcementSection = new AnnouncementSection();
            announcementSection.OrganizationId = organizationId;
            announcementSection.CourseDetId = announcementSectionModel.CourseDetId;
            announcementSection.CourseId = courseId;
            announcementSection.InstructorUserId = GetInstructorUserId(organizationId, announcementSectionModel.Iid);
            announcementSection.Section = announcementSectionModel.Section;
            announcementSection.Credits = announcementSectionModel.Points;
            announcementSection.Season = announcementSectionModel.Season;
            announcementSection.Year = announcementSectionModel.Year;
            announcementSection.Places = 25;

            return announcementSection;
        }

        private void EditAnnouncementSection(int organizationId, int courseId,
            ImportAnnouncementSectionViewModel announcementSectionModel, 
            AnnouncementSection announcementSection)
        {
            announcementSection.CourseId = courseId;
            announcementSection.InstructorUserId = GetInstructorUserId(organizationId, announcementSectionModel.Iid);
            announcementSection.Section = announcementSectionModel.Section;
            announcementSection.Credits = announcementSectionModel.Points;
            announcementSection.Season = announcementSectionModel.Season;
            announcementSection.Year = announcementSectionModel.Year;
            announcementSection.Places = 25;

            _db.AnnouncementSections.Update(announcementSection);
        }

        private void ImportSecondInstructors(Announcement announcement, int organizationId, NpgsqlConnection conn) 
        {
            if (announcement.AnnouncementSections != null) 
            {
                foreach (var announcementSection in announcement.AnnouncementSections)
                {
                    var existingSecondInstructors = GetExistingSecondInstructors(announcementSection.CourseDetId, conn);
                    var dbExtraInstructorIds = announcementSection.ExtraInstructors.Select(x => x.InstructorUserId).ToList();

                    foreach (var iid in existingSecondInstructors)
                    {
                        if (!string.IsNullOrEmpty(iid))
                        {
                            var instructorUserId = GetInstructorUserId(organizationId, int.Parse(iid));

                            var extraInstructor = announcementSection.ExtraInstructors.FirstOrDefault(x => x.InstructorUserId == instructorUserId);

                            if (extraInstructor == null)
                            {
                                _db.ExtraInstructors.Add(new ExtraInstructor
                                {
                                    AnnouncementSectionId = announcementSection.Id,
                                    InstructorUserId = instructorUserId
                                });
                            }

                            dbExtraInstructorIds.Remove(instructorUserId);
                        }
                    }

                    if (dbExtraInstructorIds.Any())
                    {
                        foreach (var instuctorUserId in dbExtraInstructorIds)
                        {
                            //Delete if instructor belongs to current organization
                            if (_db.InstructorOrgInfo.Any(x => x.OrganizationId == organizationId &&
                                 x.InstructorBasicInfo.InstructorUserId == instuctorUserId))
                            {
                                var extraInstructor = announcementSection.ExtraInstructors
                                   .FirstOrDefault(x => x.InstructorUserId == instuctorUserId);

                                if (extraInstructor != null)
                                    announcementSection.ExtraInstructors.Remove(extraInstructor);
                            }
                        }
                    }
                }
            }
        }

        private string GetInstructorUserId(int instructorImportCode)
        {
            var instructorBasicInfo = instructorsCash.FirstOrDefault(x => x.ImportCode == instructorImportCode);
            if (instructorBasicInfo == null)
                throw new Exception($"Instructor with import code {instructorImportCode} not found");

            return instructorBasicInfo.InstructorBasicInfo.InstructorUserId;
        }

        private int GetCourseId(int importCode)
        {
            var course = coursesCash.FirstOrDefault(x => x.ImportCode == importCode);
            if (course == null)
                throw new Exception($"Course with import code {importCode} not found");
            return course.Id;
        }

        /// <summary>
        /// Import announcement section data from old DB to new DB
        /// </summary>
        /// <param name="connection">Connection string</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="courseDetId">Course details id</param>
        public void ImportAnnouncementSectionData(string connection, int organizationId, int courseDetId) 
        {
            var announcementSection = GetAnnouncementSection(organizationId, courseDetId);
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                UpdateAnnouncementSection(announcementSection, conn);
                ImportSecondInstructors(announcementSection.Announcement, organizationId, conn);
                _db.SaveChanges();
            }
        }

        private AnnouncementSection GetAnnouncementSection(int organizationId, int courseDetId)
        {
            if (courseDetId == 0)
                throw new Exception("CourseDetId is 0");

            var announcementSection = _db.AnnouncementSections.Include(x => x.Announcement).Include(x => x.ExtraInstructors)
                .FirstOrDefault(x => x.OrganizationId == organizationId && x.CourseDetId == courseDetId);

            if (announcementSection == null)
                throw new Exception($"Announcement section with CourseDetId {courseDetId} not found");

            if (announcementSection.MarkedDeleted)
                throw new Exception("Announcement section is marked as deleted");

            return announcementSection;
        }

        private void UpdateAnnouncementSection(AnnouncementSection announcementSection, NpgsqlConnection conn)
        {
            string selectQuery = SelectQuery(announcementSection.CourseDetId);
            using (NpgsqlCommand cmd = new NpgsqlCommand(selectQuery))
            {
                cmd.Connection = conn;
                conn.Open();
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            announcementSection.Credits = float.Parse(sdr["points"].ToString());
                            announcementSection.InstructorUserId = GetInstructorUserId(announcementSection.OrganizationId,
                                    int.Parse(sdr["iid"].ToString()));
                            announcementSection.IsChanged = false;
                            _db.AnnouncementSections.Update(announcementSection);
                        }
                    }
                }
            }
        }

        private string GetInstructorUserId(int organizationId, int instructorImportCode)
        {
            var instructor = _db.InstructorOrgInfo.Include(x => x.InstructorBasicInfo)
                .FirstOrDefault(x => x.OrganizationId == organizationId &&
                    x.ImportCode == instructorImportCode);

            if (instructor == null)
                throw new Exception($"Instructor with import code {instructorImportCode} not found");

            return instructor.InstructorBasicInfo.InstructorUserId;
        }

        private List<string> GetExistingSecondInstructors(int courseDetId, NpgsqlConnection conn)
        {
            List<string> iidList = new List<string>();
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
                            iidList.Add(sdr["iid2"].ToString());
                        }
                    }
                }
            }

            return iidList;
        }

        private string SelectQuery(int courseDetId)
        {
            return $"SELECT * FROM auca.course_details WHERE coursedetid = {courseDetId}";
        }

        private string SelectSecondInstructorQuery(int courseDetId)
        {
            return $"SELECT iid2 FROM auca.course_second_instructor WHERE coursedetid = {courseDetId}";
        }
        
    }
}
