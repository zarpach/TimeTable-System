using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using iuca.Application.Exceptions;
using iuca.Application.Interfaces.Courses;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.ViewModels.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using iuca.Application.Interfaces.Common;
using System.Threading;

namespace iuca.Application.Services.Courses
{
    public class AttendanceService : IAttendanceService
    {
        public IConfiguration Configuration { get; }
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IStudentInfoService _studentInfoService;
        private readonly IAttendanceClassService _attendanceClassService;
        private readonly IAttendanceFolderService _attendanceFolderService;
        private readonly ISemesterService _semesterService;
        private readonly IEnvarSettingService _envarSettingService;

        public AttendanceService(IConfiguration configuration,
            IApplicationDbContext db,
            IMapper mapper,
            ApplicationUserManager<ApplicationUser> userManager,
            IStudentInfoService studentInfoService,
            IAttendanceClassService attendanceClassService,
            IAttendanceFolderService attendanceFolderService,
            ISemesterService semesterService,
            IEnvarSettingService envarSettingService)
        {
            Configuration = configuration;
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _studentInfoService = studentInfoService;
            _attendanceClassService = attendanceClassService;
            _attendanceFolderService = attendanceFolderService;
            _semesterService = semesterService;
            _envarSettingService = envarSettingService;
        }

        /// <summary>
        /// Get announcement sections that has attendance
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Announcement sections</returns>
        public IEnumerable<AnnouncementSectionDTO> GetAnnouncementSections(int semesterId)
        {
            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            var semester = _semesterService.GetSemester(semesterId);

            var announcementSections = _db.AnnouncementSections
                .AsNoTracking()
                .Include(x => x.Announcement)
                .ThenInclude(x => x.Course)
                .Include(x => x.Attendances)
                .ThenInclude(x => x.AttendanceClasses)
                .Where(x => x.Announcement.SemesterId == semesterId && x.Attendances.Any());

            var announcementSectionsDTOs = _mapper.Map<IEnumerable<AnnouncementSectionDTO>>(announcementSections.OrderBy(x => x.Course.NameEng).ThenBy(x => x.Section));

            foreach (var announcementSection in announcementSectionsDTOs)
            {
                var announcementSectionStudentsIds = _db.StudentCoursesTemp
                    .AsNoTracking()
                    .Include(x => x.StudentCourseRegistration)
                    .Where(x => x.AnnouncementSectionId == announcementSection.Id && x.State != (int)enu_CourseState.Dropped)
                    .Select(x => x.StudentCourseRegistration.StudentUserId)
                    .ToList();

                var attendanceStudentsIds = announcementSection.Attendances.Select(x => x.StudentUserId);

                var missingStudents = announcementSectionStudentsIds.Except(attendanceStudentsIds);
                var surplusStudents = attendanceStudentsIds.Except(announcementSectionStudentsIds);

                announcementSection.AttendanceInconsistencies = new AttendanceInconsistencyViewModel
                {
                    UndefinedMarkCount = announcementSection.Attendances
                    .SelectMany(x => x.AttendanceClasses)
                    .Count(x => x.Mark == (int)enu_AttendanceMark.undefined),
                    MissingStudents = missingStudents
                    .Select(x => _studentInfoService.GetStudentMinimumInfo(semester.OrganizationId, x)),
                    SurplusStudents = surplusStudents
                    .Select(x => _studentInfoService.GetStudentMinimumInfo(semester.OrganizationId, x))
                };
            }

            return announcementSectionsDTOs;
        }

        /// <summary>
        /// Get announcement section attendance
        /// </summary>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <returns>Attendance</returns>
        public IEnumerable<AttendanceDTO> GetAnnouncementSectionAttendance(int announcementSectionId)
        {
            if (announcementSectionId == 0)
                throw new ArgumentException("The announcement section id is 0.", nameof(announcementSectionId));

            var attendances = _db.Attendances
                .Include(x => x.AnnouncementSection)
                .ThenInclude(x => x.Announcement)
                .ThenInclude(x => x.Semester)
                .Include(x => x.AnnouncementSection)
                .ThenInclude(x => x.Course)
                .Include(x => x.AttendanceClasses)
                .Where(x => x.AnnouncementSectionId == announcementSectionId);

            var attendanceDTOs = _mapper.Map<IEnumerable<AttendanceDTO>>(attendances);

            foreach(var attendance in attendanceDTOs)
                attendance.Student = _studentInfoService.GetStudentMinimumInfo(attendance.AnnouncementSection.Announcement.Semester.OrganizationId, attendance.StudentUserId);

            return attendanceDTOs;
        }

        /// <summary>
        /// Get adviser students attendance
        /// </summary>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Students attendance</returns>
        public IEnumerable<StudentAttendanceViewModel> GetAdvisorStudentsAttendance(string adviserUserId, int semesterId)
        {
            var adviserStudentsAttendance = new List<StudentAttendanceViewModel>();

            if (string.IsNullOrEmpty(adviserUserId))
                return adviserStudentsAttendance;

            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            var semester = _semesterService.GetSemester(semesterId);

            var adviserStudentIds = _db.AdviserStudents
                .AsNoTracking()
                .Where(x => x.InstructorUserId == adviserUserId && x.OrganizationId == semester.OrganizationId)
                .Select(x => x.StudentUserId)
                .ToList();

            var adviserActiveStudentIds = _db.StudentOrgInfo
                .AsNoTracking()
                .Where(x => adviserStudentIds.Contains(x.StudentBasicInfo.ApplicationUserId) && 
                x.OrganizationId == semester.OrganizationId && x.State == (int)enu_StudentState.Active)
                .Select(x => x.StudentBasicInfo.ApplicationUserId)
                .ToList();

            if (!adviserActiveStudentIds.Any())
                return adviserStudentsAttendance;

            foreach (var studentId in adviserActiveStudentIds)
            {
                var studentAttendance = new StudentAttendanceViewModel();

                studentAttendance.StudentUserId = studentId;
                studentAttendance.Student = _studentInfoService.GetStudentMinimumInfo(semester.OrganizationId, studentId);


                var studentRegisteredCourseIds = _db.StudentCourseRegistrations
                    .AsNoTracking()
                    .Where(x => x.StudentUserId == studentId && x.SemesterId == semesterId)
                    .SelectMany(x => x.StudentCoursesTemp)
                    .Where(x => x.State != (int)enu_CourseState.Dropped)
                    .Select(x => x.AnnouncementSectionId)
                    .ToList();

                var studentAttendanceTrackedCourseIds = _db.Attendances
                    .AsNoTracking()
                    .Where(x => x.StudentUserId == studentId && studentRegisteredCourseIds.Contains(x.AnnouncementSectionId))
                    .Select(x => x.AnnouncementSectionId)
                    .ToList();

                var studentTotalClasses = _db.AttendanceClasses
                    .AsNoTracking()
                    .Where(x => x.Attendance.StudentUserId == studentId && studentAttendanceTrackedCourseIds.Contains(x.Attendance.AnnouncementSectionId))
                    .Select(x => x.Mark)
                    .Where(x => x == (int)enu_AttendanceMark.blank || x == (int)enu_AttendanceMark.late || x == (int)enu_AttendanceMark.abs)
                    .ToList();

                float studentBlankOrLateClasses = (float)studentTotalClasses
                    .Sum(x => x == (int)enu_AttendanceMark.blank ? 1 : (x == (int)enu_AttendanceMark.late ? 0.5 : 0));

                studentAttendance.RegisteredСoursesCount = studentRegisteredCourseIds.Count();
                studentAttendance.AttendanceTrackedCoursesCount = studentAttendanceTrackedCourseIds.Count();
                studentAttendance.OverallAttendancePercentage = studentTotalClasses.Count() == 0 ? 100 : (studentBlankOrLateClasses / (float)studentTotalClasses.Count()) * 100;

                adviserStudentsAttendance.Add(studentAttendance);
            }

            return adviserStudentsAttendance.OrderBy(x => x.Student.ShortNameEng);
        }

        /// <summary>
        /// Get student courses attendance
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Student attendance</returns>
        public IEnumerable<StudentAttendanceDetailsViewModel> GetStudentCoursesAttendance(string studentUserId, int semesterId)
        {
            if (string.IsNullOrEmpty(studentUserId))
                throw new ArgumentException("The student user id is 0.", nameof(studentUserId));

            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            var studentCoursesAttendance = _db.Attendances
                .AsNoTracking()
                .Include(x => x.AnnouncementSection.Course)
                .Include(x => x.AttendanceClasses)
                .Include(x => x.AnnouncementSection.Syllabus.CourseRequirements)
                .Where(x => x.StudentUserId == studentUserId && x.AnnouncementSection.Announcement.SemesterId == semesterId)
                .ToList()
                .Select(x => new StudentAttendanceDetailsViewModel
                {
                    Course = x.AnnouncementSection.Course,
                    AttendanceWeightedGrade = GetAttendanceWeightedGrade(x.AnnouncementSection.Syllabus),
                    CourseAttendance = GetCourseAttendanceViewModels(x.AttendanceClasses)
                })
                .ToList();

            return _mapper.Map<IEnumerable<StudentAttendanceDetailsViewModel>>(studentCoursesAttendance);
        }

        private float GetAttendanceWeightedGrade(Syllabus syllabus)
        {
            var courseRequirements = syllabus?.CourseRequirements;
            if (courseRequirements != null)
            {
                var attendanceRequirement = courseRequirements.FirstOrDefault(y => y.Syllabus.Status == (int)enu_SyllabusStatus.Approved && y.Name == (int)enu_CourseRequirement_EN.Attendance);
                if (attendanceRequirement != null)
                {
                    return attendanceRequirement.Points;
                }
            }
            return 0;
        }

        private IEnumerable<CourseAttendanceViewModel> GetCourseAttendanceViewModels(IEnumerable<AttendanceClass> attendanceClasses)
        {
            return attendanceClasses
                .GroupBy(x => x.Date)
                .Select(x => new CourseAttendanceViewModel
                {
                    Date = x.Key,
                    Marks = x.Select(y => y.Mark),
                    TotalClasses = x.Count(y => y.Mark == (int)enu_AttendanceMark.blank || y.Mark == (int)enu_AttendanceMark.late || y.Mark == (int)enu_AttendanceMark.abs),
                    BlankOrLateClasses = (float)x.Sum(y => y.Mark == (int)enu_AttendanceMark.blank ? 1 : (y.Mark == (int)enu_AttendanceMark.late ? 0.5 : 0))
                })
                .OrderBy(x => x.Date);
        }

        /// <summary>
        /// Get attendance
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <returns>Attendance</returns>
        public AttendanceDTO GetAttendance(string studentUserId, int announcementSectionId)
        {
            if (string.IsNullOrEmpty(studentUserId))
                throw new ArgumentException("The student user id is null.", nameof(studentUserId));

            if (announcementSectionId == 0)
                throw new ArgumentException("The announcement section id is 0.", nameof(announcementSectionId));

            var attendance = _db.Attendances
                .Include(x => x.AttendanceClasses)
                .FirstOrDefault(x => x.StudentUserId == studentUserId && x.AnnouncementSectionId == announcementSectionId);

            return _mapper.Map<AttendanceDTO>(attendance);
        }

        /// <summary>
        /// Generate attendance spreadsheets for all active announcements
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        public void GenerateAttendanceSpreadsheets(int semesterId)
        {
            if (semesterId == 0)
                throw new ArgumentException("The semester id is 0.", nameof(semesterId));

            var data = GetAttendanceInputDataForSemester(semesterId);

            if (data == null || !data.Any())
                throw new ModelValidationException("Data acquisition issue.", "");

            var announcements = _db.Announcements.Where(x => data.Select(y => y.AnnouncementId).Contains(x.Id)).ToList();

            var driveService = GetDriveService();
            var sheetService = GetSheetsService();

            string folderId = _attendanceFolderService.GetAttendanceFolderId(semesterId);

            string mainFileName = $"Attendance_{data.First().Season}_{data.First().Year}";
            string mainSpreadsheetId = CreateSpreadsheet(driveService, folderId, mainFileName);

            foreach (var announcement in data)
            {
                string fileName = $"{announcement.CourseAbbreviation}_{announcement.CourseNumber}_ID{announcement.CourseId}_{announcement.Season}_{announcement.Year}";
                string spreadsheetId = CreateSpreadsheet(driveService, folderId, fileName);

                try
                {
                    var sheetNames = announcement.AttendanceSheetsData.Select(x => $"{x.Section} section").ToList();
                    var (existingSheetTitles, addedSheetNames) = AddSheets(sheetService, spreadsheetId, sheetNames);

                    if (addedSheetNames != null && addedSheetNames.Any())
                    {
                        foreach (var sheetName in addedSheetNames)
                        {
                            var sheetData = announcement.AttendanceSheetsData.FirstOrDefault(x => $"{x.Section} section" == sheetName);
                            InsertDataIntoSheet(sheetService, spreadsheetId, sheetName, sheetData);
                        }

                        announcements.FirstOrDefault(x => x.Id == announcement.AnnouncementId).AttendanceSpreadsheetId = spreadsheetId;
                        _db.SaveChanges();
                    }

                    UpdateMainSpreadsheet(sheetService, mainSpreadsheetId, spreadsheetId, announcement);
                }
                catch
                {
                    DeleteFile(driveService, spreadsheetId);
                    throw;
                }
            }
        }

        /// <summary>
        /// Generate attendance spreadsheet for announcement
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        public void GenerateAttendanceSpreadsheet(int announcementId)
        {
            if (announcementId == 0)
                throw new ArgumentException("The announcement id is 0.", nameof(announcementId));

            var announcement = _db.Announcements.Find(announcementId);

            if (announcement == null)
                throw new ArgumentException($"The announcement with id {announcementId} does not exist.", nameof(announcementId));

            var data = GetAttendanceInputDataForAnnouncement(announcementId);

            if (data == null)
                throw new ModelValidationException("Data acquisition issue.", "");

            var driveService = GetDriveService();
            var sheetsService = GetSheetsService();

            string folderId = _attendanceFolderService.GetAttendanceFolderId(announcement.SemesterId);

            if (string.IsNullOrEmpty(announcement.AttendanceSpreadsheetId))
            {
                string fileName = $"{data.CourseAbbreviation}_{data.CourseNumber}_ID{data.CourseId}_{data.Season}_{data.Year}";
                string spreadsheetId = CreateSpreadsheet(driveService, folderId, fileName);

                announcement.AttendanceSpreadsheetId = spreadsheetId;
                _db.SaveChanges();

                string mainFileName = $"Attendance_{data.Season}_{data.Year}";
                string mainSpreadsheetId = CreateSpreadsheet(driveService, folderId, mainFileName);
                UpdateMainSpreadsheet(sheetsService, mainSpreadsheetId, spreadsheetId, data);
            }

            var attendanceSpreadsheetId = announcement.AttendanceSpreadsheetId;

            var sheetTitles = data.AttendanceSheetsData.Select(x => $"{x.Section} section").ToList();
            var (existingSheetTitles, addedSheetTitles) = AddSheets(sheetsService, attendanceSpreadsheetId, sheetTitles);

            sheetTitles = existingSheetTitles.Concat(addedSheetTitles).ToList();

            if (sheetTitles != null && sheetTitles.Any())
            {
                foreach (var sheetTitle in sheetTitles)
                {
                    var sheetData = data.AttendanceSheetsData.FirstOrDefault(x => $"{x.Section} section" == sheetTitle);
                    UpdateDataInSheet(sheetsService, attendanceSpreadsheetId, sheetTitle, sheetData);
                }
            }
        }

        /// <summary>
        /// Parse attendance spreadsheets for all announcements
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        public void ParseAttendanceSpreadsheets(int semesterId)
        {
            if (semesterId <= 0)
                throw new ArgumentException("The semester id is invalid.", nameof(semesterId));

            var announcements = _db.Announcements
                .AsNoTracking()
                .Include(x => x.Semester)
                .Include(x => x.AnnouncementSections)
                .Where(x => x.SemesterId == semesterId && !string.IsNullOrEmpty(x.AttendanceSpreadsheetId) && x.IsActivated)
                .ToList();

            if (!announcements.Any())
                throw new ModelValidationException("No announcements.", "");

            var sheetsService = GetSheetsService();
            var attendanceDTOs = new List<AttendanceDTO>();

            foreach (var announcement in announcements)
            {
                if (announcement.AnnouncementSections == null || !announcement.AnnouncementSections.Any())
                    continue;

                try
                {
                    string spreadsheetId = announcement.AttendanceSpreadsheetId;

                    var spreadsheetRequest = sheetsService.Spreadsheets.Get(spreadsheetId);
                    var spreadsheetResponse = spreadsheetRequest.Execute();
                    Thread.Sleep(1000);

                    if (spreadsheetResponse == null)
                        throw new ModelValidationException("Spreadsheet experiencing issues.", "");

                    var existingSheetTitles = spreadsheetResponse.Sheets.Select(sheet => sheet.Properties.Title).ToList();

                    foreach (var section in announcement.AnnouncementSections)
                    {
                        var sectionTitle = $"{section.Section} section";

                        if (existingSheetTitles.Contains(sectionTitle))
                        {
                            string sheetRange = $"{sectionTitle}";

                            try
                            {
                                var sheetRequest = sheetsService.Spreadsheets.Values.Get(spreadsheetId, sheetRange);
                                var sheetResponse = sheetRequest.Execute();
                                Thread.Sleep(1000);

                                if (sheetResponse == null)
                                    throw new ModelValidationException("Sheet experiencing issues.", "");

                                var values = sheetResponse.Values;

                                attendanceDTOs.AddRange(GetAttendanceDataFromSheet(announcement.Semester.OrganizationId, section.Id, values));
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            if (attendanceDTOs.Any())
                SaveAttendanceDataForSheet(attendanceDTOs);
        }

        /// <summary>
        /// Parse attendance spreadsheet for announcement
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        public void ParseAttendanceSpreadsheet(int announcementId)
        {
            if (announcementId <= 0)
                throw new ArgumentException("The announcement id is invalid.", nameof(announcementId));

            var announcement = _db.Announcements
                .AsNoTracking()
                .Include(x => x.Semester)
                .Include(x => x.AnnouncementSections)
                .FirstOrDefault(x => x.Id == announcementId);

            if (announcement == null)
                throw new ArgumentException($"The announcement with id {announcementId} does not exist.", nameof(announcementId));

            if (announcement.AnnouncementSections == null || !announcement.AnnouncementSections.Any())
                return;

            var sheetsService = GetSheetsService();

            string spreadsheetId = announcement.AttendanceSpreadsheetId;
            if (string.IsNullOrEmpty(spreadsheetId))
                throw new ModelValidationException("No attendance spreadsheet created.", "");

            var spreadsheetRequest = sheetsService.Spreadsheets.Get(spreadsheetId);
            var spreadsheetResponse = spreadsheetRequest.Execute();

            if (spreadsheetResponse == null)
                throw new ModelValidationException("Spreadsheet experiencing issues.", "");

            var existingSheetTitles = spreadsheetResponse.Sheets.Select(sheet => sheet.Properties.Title).ToList();

            foreach (var section in announcement.AnnouncementSections)
            {
                var sectionTitle = $"{section.Section} section";

                if (existingSheetTitles.Contains(sectionTitle))
                {
                    string sheetRange = $"{sectionTitle}";

                    var sheetRequest = sheetsService.Spreadsheets.Values.Get(spreadsheetId, sheetRange);
                    var sheetResponse = sheetRequest.Execute();

                    if (sheetResponse == null)
                        throw new ModelValidationException("Sheet experiencing issues.", "");

                    var values = sheetResponse.Values;

                    var attendanceDTOs = GetAttendanceDataFromSheet(announcement.Semester.OrganizationId, section.Id, values).ToList();

                    if (attendanceDTOs.Any())
                        SaveAttendanceDataForSheet(attendanceDTOs);
                }
            }
        }

        /// <summary>
        /// Get spreadsheet link for announcement
        /// </summary>
        /// <param name="spreadsheetId">Spreadsheet id</param>
        public string GetSpreadsheetLink(string spreadsheetId)
        {
            return $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/edit";
        }

        /// <summary>
        /// Delete attendance spreadsheet
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        public void DeleteAttendanceSpreadsheet(int announcementId)
        {
            if (announcementId <= 0)
                throw new ArgumentException("Invalid announcement ID.", nameof(announcementId));

            var announcement = _db.Announcements.Find(announcementId);

            if (announcement == null)
                throw new ArgumentException("Announcement not found.", nameof(announcementId));

            if (string.IsNullOrEmpty(announcement.AttendanceSpreadsheetId))
                throw new InvalidOperationException("Attendance spreadsheet ID is null or empty.");

            var driveService = GetDriveService();

            try
            {
                DeleteFile(driveService, announcement.AttendanceSpreadsheetId);
            }
            finally
            {
                announcement.AttendanceSpreadsheetId = null;
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Delete attendance
        /// </summary>
        /// <param name="announcementSectionId">Announcement section id</param>
        public void DeleteAttendance(int announcementSectionId)
        {
            if (announcementSectionId <= 0)
                throw new ArgumentException("Invalid announcement section ID.", nameof(announcementSectionId));

            var attendancesToDelete = _db.Attendances.Where(x => x.AnnouncementSectionId == announcementSectionId).ToList();

            if (attendancesToDelete.Any())
            {
                _db.Attendances.RemoveRange(attendancesToDelete);
                _db.SaveChanges();
            }
        }





        /// <summary>
        /// Get google credential
        /// </summary>
        /// <param name="scopes">Scopes</param>
        /// <returns>Google credential</returns>
        private GoogleCredential GetCredential(string[] scopes)
        {
            string serviceAccountFilename = Configuration["GoogleCloud:Credentials:ServiceAccount"];

            using (var stream = new FileStream(serviceAccountFilename, FileMode.Open, FileAccess.Read))
            {
                return GoogleCredential.FromStream(stream)
                                      .CreateScoped(scopes);
            }
        }

        /// <summary>
        /// Get drive service
        /// </summary>
        /// <returns>Drive service</returns>
        private DriveService GetDriveService()
        {
            var credential = GetCredential(new[] { DriveService.Scope.DriveFile });

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });
        }

        /// <summary>
        /// Get sheets service
        /// </summary>
        /// <returns>Sheets service</returns>
        private SheetsService GetSheetsService()
        {
            var credential = GetCredential(new[] { SheetsService.Scope.Spreadsheets });

            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });
        }

        /// <summary>
        /// Get file id by file name and folder id
        /// </summary>
        /// <param name="driveService">Drive service</param>
        /// <param name="folderId">Folder id</param>
        /// <param name="fileName">File name</param>
        /// <returns>File id</returns>
        private string GetFileId(DriveService driveService, string folderId, string fileName)
        {
            var query = $"'{folderId}' in parents and trashed = false";
            var getFilesRequest = driveService.Files.List();
            getFilesRequest.Q = query;

            var files = getFilesRequest.Execute().Files;

            var existingFile = files.FirstOrDefault(file => file.Name == fileName);

            return existingFile?.Id;
        }

        /// <summary>
        /// Create spreadsheet
        /// </summary>
        /// <param name="driveService">Drive service</param>
        /// <param name="folderId">Folder id</param>
        /// <param name="fileName">File name</param>
        /// <returns>Spreadsheet id</returns>
        private string CreateSpreadsheet(DriveService driveService, string folderId, string fileName)
        {
            try
            {
                string mimeType = "application/vnd.google-apps.spreadsheet";

                // Get existing file id
                var existingFileId = GetFileId(driveService, folderId, fileName);

                if (!string.IsNullOrEmpty(existingFileId))
                    return existingFileId;

                // Create file
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    MimeType = mimeType,
                    Parents = new List<string> { folderId }
                };

                var createRequest = driveService.Files.Create(fileMetadata);
                createRequest.Fields = "id";
                var createdFile = createRequest.Execute();

                // Add permissions
                var permission = new Permission
                {
                    Type = Configuration["GoogleCloud:Permission:Type"],
                    Role = Configuration["GoogleCloud:Permission:Role"],
                    Domain = Configuration["GoogleCloud:Permission:Domain"]
                };

                var permissionRequest = driveService.Permissions.Create(permission, createdFile.Id);
                permissionRequest.Execute();

                return createdFile.Id;
            }
            catch
            {
                throw new ModelValidationException("Spreadsheet creation issue.", "");
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="driveService">Drive service</param>
        /// <param name="fileId">File id</param>
        private void DeleteFile(DriveService driveService, string fileId)
        {
            try
            {
                driveService.Files.Delete(fileId).Execute();
            }
            catch
            {
                throw new ModelValidationException($"Spreadsheet deletion issue.", "");
            }
        }

        /// <summary>
        /// Add sheets to spreadsheet
        /// </summary>
        /// <param name="sheetsService">Sheets service</param>
        /// <param name="spreadsheetId">Spreadsheet id</param>
        /// <param name="sheetNames">List of sheet names</param>
        /// <returns>List of existing sheet titles and list of added sheet titles</returns>
        private (List<string> existingSheetTitles, List<string> addedSheetTitles) AddSheets(SheetsService sheetsService, string spreadsheetId, List<string> sheetNames)
        {
            try
            {
                var spreadsheet = sheetsService.Spreadsheets.Get(spreadsheetId).Execute();
                var allSheetTitles = spreadsheet.Sheets.Select(sheet => sheet.Properties.Title).ToList();

                var requests = new List<Request>();
                var existingSheetTitles = new List<string>();
                var addedSheetTitles = new List<string>();

                foreach (var sheetName in sheetNames)
                {
                    if (!allSheetTitles.Contains(sheetName))
                    {
                        requests.Add(new Request
                        {
                            AddSheet = new AddSheetRequest
                            {
                                Properties = new SheetProperties
                                {
                                    Title = sheetName
                                }
                            }
                        });

                        addedSheetTitles.Add(sheetName);
                    }
                    else
                    {
                        existingSheetTitles.Add(sheetName);
                    }
                }

                // Delete "Sheet1"
                var sheet1Title = "Sheet1";
                var sheet1 = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == sheet1Title);

                if (sheetNames != null && sheetNames.Any() && sheet1 != null)
                {
                    requests.Add(new Request
                    {
                        DeleteSheet = new DeleteSheetRequest
                        {
                            SheetId = sheet1.Properties.SheetId
                        }
                    });
                }

                if (requests.Any())
                {
                    var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
                    {
                        Requests = requests
                    };

                    sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId).Execute();
                }

                return (existingSheetTitles, addedSheetTitles);
            }
            catch
            {
                throw new ModelValidationException("Sheet creation issue.", "");
            }
        }

        /// <summary>
        /// Insert data into sheet
        /// </summary>
        /// <param name="sheetsService">Sheets service</param>
        /// <param name="spreadsheetId">Spreadsheet id</param>
        /// <param name="sheetName">Sheet name</param>
        /// <param name="data">Inserting data</param>
        private void InsertDataIntoSheet(SheetsService sheetsService, string spreadsheetId, string sheetName, AttendanceSheetDataViewModel data)
        {
            try
            {
                var range = $"{sheetName}!A1:I";

                var values = new List<IList<object>>();

                // Course information
                values.Add(new List<object> { $"{data.CourseCode} {data.CourseName}" });
                values.Add(new List<object> { $"Section: {data.Section}" });
                values.Add(new List<object> { $"Instructor: {data.Instructor}" });
                values.Add(new List<object> { "Attendance" });
                values.Add(new List<object>());

                // Header for student information
                values.Add(new List<object> { "N", "ID", "Student Name", "Group" });

                // Populate student
                int num = 0;
                foreach (var student in data.StudentsData)
                    values.Add(new List<object> { ++num, student.StudentId, student.FullName, student.Group });

                // Add empty row and rules
                values.Add(new List<object>());
                values.Add(new List<object> { "Rules:" });

                // Rules details
                values.Add(new List<object> { "blank (пусто)", "был в классе / was in class" });
                values.Add(new List<object> { "number", "кол-во пропущенных занятий / number of classes missed that day" });
                values.Add(new List<object> { "sick", "отсутствие по причине болезни / sick leave due to illness" });
                values.Add(new List<object> { "exc", "отсутствие по уважительной причине, кроме болезни / excused from class for a valid reasons other than being sick" });
                values.Add(new List<object> { "na", "\"не учитывать\" - занятий в этот день не было / \"non applicable\" - there were no classes during this day" });
                values.Add(new List<object> { "late", "опоздал / was late" });

                var body = new ValueRange
                {
                    Values = values
                };

                var updateRequest = sheetsService.Spreadsheets.Values.Update(body, spreadsheetId, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                updateRequest.Execute();
            }
            catch
            {
                throw new ModelValidationException("Data insertion issue.", "");
            }
        }

        /// <summary>
        /// Update data in sheet
        /// </summary>
        /// <param name="sheetsService">Sheets service</param>
        /// <param name="spreadsheetId">Spreadsheet id</param>
        /// <param name="sheetName">Sheet name</param>
        /// <param name="data">Updating data</param>
        private void UpdateDataInSheet(SheetsService sheetsService, string spreadsheetId, string sheetName, AttendanceSheetDataViewModel data)
        {
            try
            {
                int firstRow = 7;

                var range = $"{sheetName}!A{firstRow}:D";

                var existingValues = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range).Execute().Values;

                if (existingValues == null || existingValues.Count == 0)
                {
                    InsertDataIntoSheet(sheetsService, spreadsheetId, sheetName, data);
                    return;
                }

                int emptyRow = existingValues.Count + 1;
                for (int i = 0; i < existingValues.Count; i++)
                {
                    if (existingValues[i].Count == 0)
                    {
                        emptyRow = i + 1;
                        break;
                    }
                }

                var clearRange = $"{sheetName}!A{firstRow + emptyRow}:D";

                foreach (var student in data.StudentsData)
                {
                    bool studentExists = false;

                    for (int i = 0; i < emptyRow - 1; i++)
                    {
                        var existingStudentId = existingValues[i][1]?.ToString();

                        if (existingStudentId == student.StudentId.ToString())
                        {
                            existingValues[i][2] = student.FullName;
                            existingValues[i][3] = student.Group;
                            studentExists = true;
                            break;
                        }
                    }

                    if (!studentExists)
                    {
                        if (emptyRow <= existingValues.Count)
                            existingValues.Insert(emptyRow - 1, new List<object> { emptyRow++, student.StudentId, student.FullName, student.Group });
                        else
                            existingValues.Add(new List<object> { emptyRow++, student.StudentId, student.FullName, student.Group });
                    }    
                }

                var body = new ValueRange
                {
                    Values = existingValues
                };

                sheetsService.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadsheetId, clearRange).Execute();

                var updateRequest = sheetsService.Spreadsheets.Values.Update(body, spreadsheetId, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                updateRequest.Execute();
            }
            catch
            {
                throw new ModelValidationException("Data update issue.", "");
            }
        }

        /// <summary>
        /// Update main spreadsheet
        /// </summary>
        /// <param name="sheetsService">Sheets service</param>
        /// <param name="mainSpreadsheetId">Main spreadsheet id</param>
        /// <param name="spreadsheetId">Spreadsheet id</param>
        /// <param name="data">Updating data</param>
        private void UpdateMainSpreadsheet(SheetsService sheetsService, string mainSpreadsheetId, string spreadsheetId, AttendanceSpreadsheetDataViewModel data)
        {
            try
            {
                var range = $"Sheet1!A1:D";

                var existingValues = sheetsService.Spreadsheets.Values.Get(mainSpreadsheetId, range).Execute().Values;

                if (existingValues == null || existingValues.Count == 0)
                    existingValues = new List<IList<object>> {
                        new List<object> { "Code", "ID", "Name", "Link" }
                    };

                int lastRowNum = existingValues.Count;

                bool courseExists = false;

                for (int i = 1; i < existingValues.Count; i++)
                {
                    var existingCourseId = existingValues[i][1]?.ToString();

                    if (existingCourseId == data.CourseId)
                    {
                        existingValues[i][3] = GetSpreadsheetLink(spreadsheetId);
                        courseExists = true;
                        break;
                    }
                }

                if (!courseExists)
                    existingValues.Add(new List<object> { $"{data.CourseAbbreviation} {data.CourseNumber}", data.CourseId, data.CourseName, GetSpreadsheetLink(spreadsheetId) });

                var body = new ValueRange
                {
                    Values = existingValues
                };

                var updateRequest = sheetsService.Spreadsheets.Values.Update(body, mainSpreadsheetId, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                updateRequest.Execute();
            }
            catch
            {
                throw new ModelValidationException("Main spreadsheet issue.", "");
            }
        }

        /// <summary>
        /// Get attendance input data for semester
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>List of attendance spreadsheet data</returns>
        private List<AttendanceSpreadsheetDataViewModel> GetAttendanceInputDataForSemester(int semesterId)
        {
            var attendanceSpreadsheetDataList = new List<AttendanceSpreadsheetDataViewModel>();

            var announcements = _db.Announcements
                .Include(x => x.Semester)
                .Include(x => x.Course)
                .Include(x => x.AnnouncementSections)
                .ThenInclude(x => x.StudentCourses)
                .ThenInclude(x => x.StudentCourseRegistration)
                .Where(x => x.SemesterId == semesterId && x.IsActivated && x.Course.CourseType != (int)enu_CourseType.TermPaper)
                .ToList();

            foreach (var announcement in announcements)
                attendanceSpreadsheetDataList.Add(GetAttendanceSpreadsheetData(announcement));

            return attendanceSpreadsheetDataList;
        }

        /// <summary>
        /// Get attendance input data for announcement
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <returns>Attendance spreadsheet data</returns>
        private AttendanceSpreadsheetDataViewModel GetAttendanceInputDataForAnnouncement(int announcementId)
        {
            var announcement = _db.Announcements
                .Include(x => x.Semester)
                .Include(x => x.Course)
                .Include(x => x.AnnouncementSections)
                .ThenInclude(x => x.StudentCourses)
                .ThenInclude(x => x.StudentCourseRegistration)
                .FirstOrDefault(x => x.Id == announcementId);

            return GetAttendanceSpreadsheetData(announcement);
        }

        /// <summary>
        /// Get attendance spreadsheet data
        /// </summary>
        /// <param name="announcement">Announcement</param>
        /// <returns>Attendance spreadsheet data</returns>
        private AttendanceSpreadsheetDataViewModel GetAttendanceSpreadsheetData(Announcement announcement)
        {
            var attendanceSpreadsheetData = new AttendanceSpreadsheetDataViewModel
            {
                Season = ((enu_Season)announcement.Semester.Season).ToString(),
                Year = announcement.Semester.Year.ToString(),
                CourseId = announcement.Course.ImportCode.ToString(),
                CourseAbbreviation = announcement.Course.Abbreviation,
                CourseNumber = announcement.Course.Number,
                CourseName = announcement.Course.NameEng,
                AnnouncementId = announcement.Id,
                AttendanceSheetsData = new List<AttendanceSheetDataViewModel>()
            };

            foreach (var section in announcement.AnnouncementSections)
            {
                var attendanceSheetData = new AttendanceSheetDataViewModel
                {
                    CourseName = announcement.Course.NameEng,
                    CourseCode = $"{announcement.Course.Abbreviation} {announcement.Course.Number}",
                    Section = section.Section,
                    Instructor = _userManager.GetUserFullName(section.InstructorUserId),
                    StudentsData = section.StudentCourses
                        .Where(student => student.State != (int)enu_CourseState.Dropped)
                        .Select(MapStudentInfo)
                        .OrderBy(x => x.FullName)
                        .ToList()
                };

                attendanceSpreadsheetData.AttendanceSheetsData.Add(attendanceSheetData);
            }

            return attendanceSpreadsheetData;
        }

        /// <summary>
        /// Map student info
        /// </summary>
        /// <param name="student">Student</param>
        /// <returns>Student info</returns>
        private StudentInfoViewModel MapStudentInfo(StudentCourseTemp student)
        {
            var studentInfo = _userManager.Users
                .Include(x => x.StudentBasicInfo)
                .ThenInclude(x => x.StudentOrgInfo)
                .ThenInclude(x => x.DepartmentGroup)
                .ThenInclude(x => x.Department)
                .FirstOrDefault(x => x.Id == student.StudentCourseRegistration.StudentUserId);

            if (studentInfo == null)
                throw new Exception($"Student with id {student.StudentCourseRegistration.StudentUserId} was not found");

            var orgInfo = studentInfo.StudentBasicInfo?.StudentOrgInfo
                .FirstOrDefault(x => x.OrganizationId == student.StudentCourseRegistration.OrganizationId);

            if (orgInfo == null)
                throw new Exception($"Organization info not found for student with id {student.StudentCourseRegistration.StudentUserId}");

            return new StudentInfoViewModel
            {
                // UserId = student.StudentCourseRegistration.StudentUserId,
                // ShortName = $"{studentInfo.LastNameEng} {studentInfo.FirstNameEng.Substring(0,1)}.",
                FullName = $"{studentInfo.LastNameEng} {studentInfo.FirstNameEng}",
                Group = $"{orgInfo.DepartmentGroup?.Department?.Code}-{orgInfo.DepartmentGroup?.Code}",
                StudentId = orgInfo.StudentId
            };
        }

        /// <summary>
        /// Extract file info from file name
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Import code, season and year of the course</returns>
        private (int importCode, int season, int year) ExtractFileInfo(string fileName)
        {
            // ^: Соответствует началу строки.
            // (?i): Опция, которая включает регистронезависимый поиск.
            // (?:.*): Негруппирующие скобки. Они обозначают начало и конец негруппирующей подмаски.
            // .* Соответствует любому количеству любых символов.
            // (\d+): Группа захвата, которая соответствует одной или более цифрам после "ID".
            // ([A-Za-z]+): Еще одна группа захвата - одна или более букв в верхнем или нижнем регистре.
            // (\d+): Последняя группа захвата - одна или более цифр.
            var match = Regex.Match(fileName, @"^(?i)(?:.*)ID(\d+)_([A-Za-z]+)_(\d+)");

            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int importCode) && int.TryParse(match.Groups[3].Value, out int year))
                {
                    if (Enum.TryParse(match.Groups[2].Value, out enu_Season seasonEnumValue))
                        return (importCode, (int)seasonEnumValue, year);
                    else
                        throw new ModelValidationException("Unable to convert a string to an enum (file name).", "");
                }
                else
                {
                    throw new ModelValidationException("Unable to convert string values to numbers (file name).", "");
                }
            }
            else
            {
                throw new ModelValidationException("The file naming format does not match the expected one.", "");
            }
        }

        /// <summary>
        /// Extract sheet info from sheet name
        /// </summary>
        /// <param name="sheetName">Sheet name</param>
        /// <returns>Section number</returns>
        private string ExtractSheetInfo(string sheetName)
        {
            // ^: Соответствует началу строки.
            // (?i): Опция, которая включает регистронезависимый поиск.
            // (\d+|-): Это группа захвата, которая соответствует одной или более цифр(\d+) или(|) минусу(-).
            // \s*: Это ноль или более пробельных символов.
            var sheetMatch = Regex.Match(sheetName, @"^(?i)(\d+|-)\s*section");

            if (sheetMatch.Success)
                return sheetMatch.Groups[1].Value;
            else
                return null;
        }

        /// <summary>
        /// Get cell indices
        /// </summary>
        /// <param name="values">Sheet values</param>
        /// <param name="searchValue">Search value</param>
        /// <returns>Announcement sections</returns>
        private (int, int) GetCellIndices(IList<IList<object>> values, string searchValue)
        {
            if (values != null && values.Count > 0)
            {
                int rowCount = values.Count;
                int columnCount = values[0].Count;

                for (int sum = 0; sum <= rowCount + columnCount - 2; sum++)
                {
                    for (int row = 0; row <= sum; row++)
                    {
                        int col = sum - row;
                        if (row < rowCount && col < columnCount)
                        {
                            if (values[row][col].ToString().Trim().Equals(searchValue.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                return (row, col);
                            }
                        }
                    }
                }
            }

            throw new ModelValidationException($"Cell with the column of {searchValue} not found.", "");
        }

        private readonly Dictionary<string, enu_AttendanceMark> StringToEnumMapping = new Dictionary<string, enu_AttendanceMark>(StringComparer.OrdinalIgnoreCase)
        {
            { "blank", enu_AttendanceMark.blank },
            { "absent", enu_AttendanceMark.abs },
            { "abs", enu_AttendanceMark.abs },
            { "ab", enu_AttendanceMark.abs },
            { "sick", enu_AttendanceMark.sick },
            { "excused", enu_AttendanceMark.exc },
            { "exc", enu_AttendanceMark.exc },
            { "ex", enu_AttendanceMark.exc },
            { "not available", enu_AttendanceMark.na },
            { "na", enu_AttendanceMark.na },
            { "n/a", enu_AttendanceMark.na },
            { "late", enu_AttendanceMark.late },
            { "lt", enu_AttendanceMark.late }
        };

        /// <summary>
        /// Try parse attendance mark
        /// </summary>
        /// <param name="markData">Mark data</param>
        /// <returns>Attendance mark</returns>
        private bool TryParseAttendanceMark(string markData, out enu_AttendanceMark attendanceMark)
        {
            attendanceMark = enu_AttendanceMark.undefined;

            if (StringToEnumMapping.TryGetValue(markData, out enu_AttendanceMark result))
            {
                attendanceMark = result;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parse marks
        /// </summary>
        /// <param name="classCount">Class count</param>
        /// <param name="marksData">Marks data</param>
        /// <returns>Attendance marks list</returns>
        private List<enu_AttendanceMark> ParseMarks(int classCount, string marksData)
        {
            var marks = new List<enu_AttendanceMark>();

            if (string.IsNullOrWhiteSpace(marksData))
            {
                for (int i = 0; i < classCount; i++)
                {
                    marks.Add(enu_AttendanceMark.blank);
                }
            }
            else
            {
                string[] parts = marksData.ToLower().Split(',');

                foreach (string part in parts)
                {
                    string[] subParts = part.Trim().Split(new char[] { ' ', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);

                    if (subParts.Length == 2 && TryParseAttendanceMark(subParts[0], out enu_AttendanceMark mark) && int.TryParse(subParts[1], out int markCount))
                    {
                        for (int i = 0; i < markCount; i++)
                        {
                            marks.Add(mark);
                        }
                    }
                    else if (subParts.Length == 1)
                    {
                        if (int.TryParse(subParts[0], out int absCount))
                        {
                            for (int i = marks.Count; i < absCount; i++)
                            {
                                marks.Add(enu_AttendanceMark.abs);
                            }
                        }
                        else if (TryParseAttendanceMark(subParts[0], out enu_AttendanceMark aloneMark))
                        {
                            if (aloneMark != enu_AttendanceMark.late)
                            {
                                for (int i = marks.Count; i < classCount; i++)
                                {
                                    marks.Add(aloneMark);
                                }
                            }
                            else
                            {
                                marks.Add(aloneMark);
                            }

                        }
                        else
                        {
                            while (marks.Count < classCount)
                            {
                                marks.Add(enu_AttendanceMark.undefined);
                            }
                        }
                    }
                    else
                    {
                        while (marks.Count < classCount)
                        {
                            marks.Add(enu_AttendanceMark.undefined);
                        }
                    }
                }

                while (marks.Count < classCount)
                {
                    marks.Add(enu_AttendanceMark.blank);
                }
            }

            return marks;
        }

        /// <summary>
        /// Get dates from sheet
        /// </summary>
        /// <param name="values">Sheet values</param>
        /// <returns>Dates and class counts</returns>
        private Dictionary<int, (DateTime date, int classCount)> GetDates(IList<IList<object>> values)
        {
            var (checkboxRow, firstCheckboxCol) = (3, 4);
            var colCount = values[checkboxRow].Count;

            var dates = new Dictionary<int, (DateTime date, int classCount)>();

            for (int col = firstCheckboxCol; col < colCount; col += 2)
            {
                if (!(values.Count > checkboxRow))
                    break;

                string checkboxValue = values[checkboxRow][col].ToString();

                if (bool.TryParse(checkboxValue, out bool isChecked))
                {
                    if (!isChecked)
                        continue;

                    string dateValue = values[checkboxRow + 2][col].ToString(); // 2 строки ниже
                    string classCountValue = values[checkboxRow + 2][col + 1].ToString(); // 2 строки ниже и на 1 строку правее

                    if (DateTime.TryParse(dateValue, out DateTime date) && int.TryParse(classCountValue, out int classCount))
                    {
                        dates.Add(col, (date, classCount));
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    break;
                }
            }

            return dates;
        }

        /// <summary>
        /// Get attendance classes from sheet
        /// </summary>
        /// <param name="values">Sheet values</param>
        /// <param name="studentRow">Student row</param>
        /// <param name="dates">Dates</param>
        /// <returns>Attendance classes</returns>
        private List<AttendanceClassDTO> GetAttendanceClasses(IList<IList<object>> values, int studentRow, Dictionary<int, (DateTime date, int classCount)> dates)
        {
            var attendanceClasses = new List<AttendanceClassDTO>();

            foreach (var date in dates)
            {
                string dataValue = string.Empty;
                if (date.Key < values[studentRow].Count)
                    dataValue = values[studentRow][date.Key].ToString();

                var marks = ParseMarks(date.Value.classCount, dataValue);

                foreach (var (mark, index) in marks.Select((value, i) => (value, i)))
                {
                    var attendanceClass = new AttendanceClassDTO()
                    {
                        Date = date.Value.date,
                        Number = index + 1,
                        Mark = (int)mark,
                        Data = dataValue
                    };

                    attendanceClasses.Add(attendanceClass);
                }
            }

            return attendanceClasses;
        }

        /// <summary>
        /// Get attendance data from sheet
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <param name="values">Sheet values</param>
        /// <returns>Attendance data</returns>
        private IEnumerable<AttendanceDTO> GetAttendanceDataFromSheet(int organizationId, int announcementSectionId, IList<IList<object>> values)
        {
            var attendances = new List<AttendanceDTO>();

            if (values == null || !values.Any())
                return attendances;

            var rowCount = values.Count;
            var (firstStudentIdRow, studentIdCol) = (6, 1);

            var dates = GetDates(values);

            for (int row = firstStudentIdRow; row < rowCount; row++)
            {
                if (!(values[row].Count > studentIdCol))
                    break;

                string studentIdValue = values[row][studentIdCol].ToString();

                if (int.TryParse(studentIdValue, out int studentId))
                {
                    try
                    {
                        string studentUserId = _studentInfoService.GetUserIdByStudentId(organizationId, studentId);

                        var attendanceClasses = GetAttendanceClasses(values, row, dates);

                        if (attendanceClasses != null && attendanceClasses.Any())
                        {
                            var attendance = new AttendanceDTO()
                            {
                                AnnouncementSectionId = announcementSectionId,
                                StudentUserId = studentUserId,
                                AttendanceClasses = attendanceClasses
                            };

                            attendances.Add(attendance);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    break;
                }
            }

            return attendances;
        }

        /// <summary>
        /// Save attendance data for sheet
        /// </summary>
        /// <param name="attendanceDTOs">Attendances</param>
        private void SaveAttendanceDataForSheet(IEnumerable<AttendanceDTO> attendanceDTOs)
        {
            if (attendanceDTOs == null || !attendanceDTOs.Any())
                throw new ModelValidationException("No data in sheet.", "");

            foreach (var attendanceDTO in attendanceDTOs)
            {
                var existingAttendance = _db.Attendances
                    .Include(x => x.AttendanceClasses)
                    .FirstOrDefault(x => x.AnnouncementSectionId == attendanceDTO.AnnouncementSectionId &&
                                         x.StudentUserId == attendanceDTO.StudentUserId);

                if (existingAttendance != null)
                {
                    _db.AttendanceClasses.RemoveRange(existingAttendance.AttendanceClasses);

                    foreach (var attendanceClass in attendanceDTO.AttendanceClasses)
                    {
                        attendanceClass.AttendanceId = existingAttendance.Id;
                        _attendanceClassService.CreateAttendanceClass(attendanceClass);
                    }
                }
                else
                {
                    var newAttendance = _mapper.Map<Attendance>(attendanceDTO);
                    _db.Attendances.Add(newAttendance);
                }
            }

            _db.SaveChanges();
            _envarSettingService.SetLastAttendanceUpdate(1, DateTime.Now);
        }
    }
}
