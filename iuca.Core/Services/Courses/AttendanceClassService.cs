using AutoMapper;
using iuca.Application.DTO.Courses;
using iuca.Application.Interfaces.Courses;
using iuca.Domain.Entities.Courses;
using iuca.Infrastructure.Persistence;
using System;

namespace iuca.Application.Services.Courses
{
    public class AttendanceClassService : IAttendanceClassService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;

        public AttendanceClassService(IApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Create attendance class
        /// </summary>
        /// <param name="attendanceClassDTO">Attendance class</param>
        public void CreateAttendanceClass(AttendanceClassDTO attendanceClassDTO)
        {
            if (attendanceClassDTO == null)
                throw new ArgumentException("The attendance class is null.");

            if (attendanceClassDTO.AttendanceId == 0)
                throw new ArgumentException("The attendance id is null.");

            var newAttendanceClass = _mapper.Map<AttendanceClass>(attendanceClassDTO);

            _db.AttendanceClasses.Add(newAttendanceClass);
        }

        /// <summary>
        /// Edit attendance class by id
        /// </summary>
        /// <param name="attendanceClassId">Attendance class id</param>
        /// <param name="attendanceClassDTO">Attendance class</param>
        public void EditAttendanceClass(int attendanceClassId, AttendanceClassDTO attendanceClassDTO)
        {
            if (attendanceClassDTO == null)
                throw new ArgumentNullException(nameof(attendanceClassDTO), "The attendance class is null.");

            if (attendanceClassId == 0)
                throw new ArgumentException("The attendance class id is 0.", nameof(attendanceClassId));

            var attendanceClass = _db.AttendanceClasses.Find(attendanceClassId);

            if (attendanceClass == null)
                throw new ArgumentException($"The attendance class with id {attendanceClassId} does not exist.", nameof(attendanceClassId));

            attendanceClass.Mark = attendanceClassDTO.Mark;
            attendanceClass.Number = attendanceClassDTO.Number;
            attendanceClass.Date = attendanceClassDTO.Date;
            attendanceClass.Data = attendanceClassDTO.Data;

            _db.SaveChanges();
        }

        /// <summary>
        /// Delete attendance class by id
        /// </summary>
        /// <param name="attendanceClassId">Attendance class id</param>
        public void DeleteAttendanceClass(int attendanceClassId)
        {
            if (attendanceClassId == 0)
                throw new ArgumentException($"The attendance class id is 0.");

            var attendanceClass = _db.AttendanceClasses.Find(attendanceClassId);

            if (attendanceClass == null)
                throw new ArgumentException($"The attendance class with id {attendanceClassId} does not exist.", nameof(attendanceClassId));

            _db.AttendanceClasses.Remove(attendanceClass);
            _db.SaveChanges();
        }
    }
}
