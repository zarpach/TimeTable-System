using iuca.Application.DTO.Courses;

namespace iuca.Application.Interfaces.Courses
{
    public interface IAttendanceClassService
    {
        /// <summary>
        /// Create attendance class
        /// </summary>
        /// <param name="attendanceClassDTO">Attendance class</param>
        void CreateAttendanceClass(AttendanceClassDTO attendanceClassDTO);

        /// <summary>
        /// Edit attendance class by id
        /// </summary>
        /// <param name="attendanceClassId">Attendance class id</param>
        /// <param name="attendanceClassDTO">Attendance class</param>
        void EditAttendanceClass(int attendanceClassId, AttendanceClassDTO attendanceClassDTO);

        /// <summary>
        /// Delete attendance class by id
        /// </summary>
        /// <param name="attendanceClassId">Attendance class id</param>
        void DeleteAttendanceClass(int attendanceClassId);
    }
}
