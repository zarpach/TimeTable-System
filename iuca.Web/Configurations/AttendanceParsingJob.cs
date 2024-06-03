using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Courses;
using Microsoft.Extensions.Logging;
using System;

namespace iuca.Web.Configurations
{
    public class AttendanceParsingJob
    {
        private readonly ILogger<AttendanceParsingJob> _logger;
        private readonly IAttendanceService _attendanceService;
        private readonly IEnvarSettingService _envarSettingService;

        public AttendanceParsingJob(ILogger<AttendanceParsingJob> logger,
            IAttendanceService attendanceService,
            IEnvarSettingService envarSettingService)
        {
            _logger = logger;
            _attendanceService = attendanceService;
            _envarSettingService = envarSettingService;
        }

        public void Execute()
        {
            _logger.LogWarning("Attendance Parsing Job running.");

            try
            {
                var semesterId = _envarSettingService.GetCurrentSemester(1);
                _attendanceService.ParseAttendanceSpreadsheets(semesterId);

                _logger.LogWarning("Attendance Parsing Job completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Attendance Parsing Job failed.");
            }
        }
    }
}
