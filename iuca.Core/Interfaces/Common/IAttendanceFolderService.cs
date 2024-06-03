using iuca.Application.DTO.Common;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Common
{
    public interface IAttendanceFolderService
    {
        /// <summary>
        /// Get attendance folders
        /// </summary>
        /// <returns>Attendance folders</returns>
        IEnumerable<AttendanceFolderDTO> GetAttendanceFolders();

        /// <summary>
        /// Get attendance folder
        /// </summary>
        /// <param name="attendanceFolderId">Semester id</param>
        /// <returns>Attendance folder</returns>
        AttendanceFolderDTO GetAttendanceFolder(int attendanceFolderId);

        /// <summary>
        /// Get attendance folder id
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Attendance folder id</returns>
        string GetAttendanceFolderId(int semesterId);

        /// <summary>
        /// Get attendance main spreadsheet link
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Attendance main spreadsheet link</returns>
        string GetAttendanceMainSpreadsheetLink(int semesterId);

        /// <summary>
        /// Create attendance folder
        /// </summary>
        /// <param name="attendanceFolderDTO">Attendance folder</param>
        void CreateAttendanceFolder(AttendanceFolderDTO attendanceFolderDTO);

        /// <summary>
        /// Edit attendance folder
        /// </summary>
        /// <param name="attendanceFolderId">Attendance folder id</param>
        /// <param name="attendanceFolderDTO">Attendance folder</param>
        void EditAttendanceFolder(int attendanceFolderId, AttendanceFolderDTO attendanceFolderDTO);

        /// <summary>
        /// Delete attendance folder
        /// </summary>
        /// <param name="attendanceFolderId">Attendance folder id</param>
        void DeleteAttendanceFolder(int attendanceFolderId);
    }
}
