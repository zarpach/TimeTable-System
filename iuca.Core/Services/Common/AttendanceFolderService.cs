using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static iuca.Application.Constants.Permissions;

namespace iuca.Application.Services.Common
{
    public class AttendanceFolderService : IAttendanceFolderService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;

        public AttendanceFolderService(IApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Get attendance folders
        /// </summary>
        /// <returns>Attendance folders</returns>
        public IEnumerable<AttendanceFolderDTO> GetAttendanceFolders()
        {
            var attendanceFolders = _db.AttendanceFolders
                .Include(x => x.Semester)
                .ToList();

            return _mapper.Map<IEnumerable<AttendanceFolderDTO>>(attendanceFolders);
        }

        /// <summary>
        /// Get attendance folder
        /// </summary>
        /// <param name="attendanceFolderId">Semester id</param>
        /// <returns>Attendance folder</returns>
        public AttendanceFolderDTO GetAttendanceFolder(int attendanceFolderId)
        {
            if (attendanceFolderId <= 0)
                throw new ArgumentException("Invalid attendance folder ID.", nameof(attendanceFolderId));

            var attendanceFolder = _db.AttendanceFolders.Find(attendanceFolderId);

            if (attendanceFolder == null)
                throw new ArgumentException("Attendance folder not found.", nameof(attendanceFolder));

            return _mapper.Map<AttendanceFolderDTO>(attendanceFolder);
        }

        /// <summary>
        /// Get attendance folder id
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Attendance folder id</returns>
        public string GetAttendanceFolderId(int semesterId)
        {
            if (semesterId <= 0)
                throw new ArgumentException("Invalid semester ID.", nameof(semesterId));

            var attendanceFolder = _db.AttendanceFolders.FirstOrDefault(x => x.SemesterId == semesterId);

            if (attendanceFolder == null)
                throw new ArgumentException("Attendance folder not found.", nameof(attendanceFolder));

            return attendanceFolder.FolderId;
        }

        /// <summary>
        /// Get attendance main spreadsheet link
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <returns>Attendance main spreadsheet link</returns>
        public string GetAttendanceMainSpreadsheetLink(int semesterId)
        {
            if (semesterId <= 0)
                throw new ArgumentException("Invalid semester ID.", nameof(semesterId));

            var attendanceFolder = _db.AttendanceFolders.FirstOrDefault(x => x.SemesterId == semesterId);

            if (attendanceFolder == null)
                return string.Empty;

            return $"https://docs.google.com/spreadsheets/d/{attendanceFolder.MainSpreadsheetId}/edit";
        }

        /// <summary>
        /// Create attendance folder
        /// </summary>
        /// <param name="attendanceFolderDTO">Attendance folder</param>
        public void CreateAttendanceFolder(AttendanceFolderDTO attendanceFolderDTO)
        {
            if (attendanceFolderDTO == null)
                throw new ArgumentException("The attendance folder is null.", nameof(attendanceFolderDTO));

            var newAttendanceFolder = _mapper.Map<AttendanceFolder>(attendanceFolderDTO);

            _db.AttendanceFolders.Add(newAttendanceFolder);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit attendance folder
        /// </summary>
        /// <param name="attendanceFolderId">Attendance folder id</param>
        /// <param name="attendanceFolderDTO">Attendance folder</param>
        public void EditAttendanceFolder(int attendanceFolderId, AttendanceFolderDTO attendanceFolderDTO)
        {
            if (attendanceFolderId <= 0)
                throw new ArgumentException("Invalid attendance folder ID.", nameof(attendanceFolderId));

            if (attendanceFolderDTO == null)
                throw new ArgumentException("The attendance folder is null.", nameof(attendanceFolderDTO));

            var attendanceFolder = _db.AttendanceFolders.Find(attendanceFolderId);

            if (attendanceFolder == null)
                throw new ArgumentException("Attendance folder not found.", nameof(attendanceFolder));

            attendanceFolder.SemesterId = attendanceFolderDTO.SemesterId;
            attendanceFolder.FolderId = attendanceFolderDTO.FolderId;
            attendanceFolder.MainSpreadsheetId = attendanceFolderDTO.MainSpreadsheetId;

            _db.SaveChanges();
        }

        /// <summary>
        /// Delete attendance folder
        /// </summary>
        /// <param name="attendanceFolderId">Attendance folder id</param>
        public void DeleteAttendanceFolder(int attendanceFolderId)
        {
            if (attendanceFolderId <= 0)
                throw new ArgumentException("Invalid attendance folder ID.", nameof(attendanceFolderId));

            var attendanceFolder = _db.AttendanceFolders.Find(attendanceFolderId);

            if (attendanceFolder == null)
                throw new ArgumentException("Attendance folder not found.", nameof(attendanceFolder));

            _db.AttendanceFolders.Remove(attendanceFolder);
            _db.SaveChanges();
        }
    }
}
