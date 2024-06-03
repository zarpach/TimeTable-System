using Google.Apis.Sheets.v4.Data;
using iuca.Application.DTO.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class AnnouncementDTO
    {
        public int Id { get; set; }

        [Display(Name = "Course")]
        public CourseDTO Course { get; set; }

        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Display(Name = "Semester")]
        public SemesterDTO Semester { get; set; }

        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        [Display(Name = "For all")]
        public bool IsForAll { get; set; }

        [Display(Name = "Activated")]
        public bool IsActivated { get; set; }

        [Display(Name = "Attendance spreadsheet id")]
        public string AttendanceSpreadsheetId { get; set; }

        [Display(Name = "Attendance spreadsheet link")]
        public string AttendanceSpreadsheetLink
        {
            get
            {
                if (string.IsNullOrEmpty(AttendanceSpreadsheetId))
                    return "";

                return $"https://docs.google.com/spreadsheets/d/{AttendanceSpreadsheetId}/edit";
            }
        }

        [Display(Name = "Proposal course")]
        public ProposalCourseDTO ProposalCourse { get; set; }

        public IEnumerable<AnnouncementSectionDTO> AnnouncementSections { get; set; }
    }
}
