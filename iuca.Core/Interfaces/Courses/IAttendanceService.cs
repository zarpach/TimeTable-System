using iuca.Application.DTO.Courses;
using iuca.Application.ViewModels.Courses;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Courses
{
    public interface IAttendanceService
    {
        /// <summary>
        /// Get announcement sections that has attendance
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Announcement sections</returns>
        IEnumerable<AnnouncementSectionDTO> GetAnnouncementSections(int semesterId);

        /// <summary>
        /// Get announcement section attendance
        /// </summary>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <returns>Attendance</returns>
        IEnumerable<AttendanceDTO> GetAnnouncementSectionAttendance(int announcementSectionId);

        /// <summary>
        /// Get adviser students attendance
        /// </summary>
        /// <param name="adviserUserId">Adviser user id</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Students attendance</returns>
        IEnumerable<StudentAttendanceViewModel> GetAdvisorStudentsAttendance(string adviserUserId, int semesterId);

        /// <summary>
        /// Get student courses attendance
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Student attendance</returns>
        IEnumerable<StudentAttendanceDetailsViewModel> GetStudentCoursesAttendance(string studentUserId, int semesterId);

        /// <summary>
        /// Get attendance
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <returns>Attendance</returns>
        AttendanceDTO GetAttendance(string studentUserId, int announcementSectionId);

        /// <summary>
        /// Generate attendance spreadsheets for all active announcements
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        void GenerateAttendanceSpreadsheets(int semesterId);

        /// <summary>
        /// Generate attendance spreadsheet for announcement
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        void GenerateAttendanceSpreadsheet(int announcementId);

        /// <summary>
        /// Parse attendance spreadsheets for all announcements
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        void ParseAttendanceSpreadsheets(int semesterId);

        /// <summary>
        /// Parse attendance spreadsheet for announcement
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        void ParseAttendanceSpreadsheet(int announcementId);

        /// <summary>
        /// Get spreadsheet link for announcement
        /// </summary>
        /// <param name="spreadsheetId">Spreadsheet id</param>
        string GetSpreadsheetLink(string spreadsheetId);

        /// <summary>
        /// Delete attendance spreadsheet
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        void DeleteAttendanceSpreadsheet(int announcementId);

        /// <summary>
        /// Delete attendance
        /// </summary>
        /// <param name="announcementSectionId">Announcement section id</param>
        void DeleteAttendance(int announcementSectionId);
    }
}
