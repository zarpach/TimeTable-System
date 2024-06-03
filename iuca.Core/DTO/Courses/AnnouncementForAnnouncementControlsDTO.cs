using iuca.Application.Enums;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.DTO.Courses
{
    public class AnnouncementForAnnouncementControlsDTO
    {
        public int Id { get; set; }
        public int SemesterId { get; set; }
        public int CourseId { get; set; }
        public bool IsActivated { get; set; }
        public bool IsForAll { get; set; }
        public string AttendanceSpreadsheetId { get; set; }

        public string AttendanceSpreadsheetLink { 
            get 
            { 
                return $"https://docs.google.com/spreadsheets/d/{AttendanceSpreadsheetId}/edit"; 
            } 
        }

        public CourseForAnnouncementControlsDTO Course { get; set; }
        public ProposalCourseForAnnouncementControlsDTO ProposalCourse { get; set; }

        public IEnumerable<AnnouncementSectionForAnnouncementControlsDTO> AnnouncementSections { get; set; }
    }

    public class CourseForAnnouncementControlsDTO
    {
        public int Id { get; set; }
        public string NameEng { get; set; }
    }

    public class ProposalCourseForAnnouncementControlsDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int ProposalId { get; set; }
        public int Status { get; set; }

        public int InstructorsCount { 
            get 
            {
                return InstructorsJson?.Count() ?? 0;
            } 
        }

        public ProposalForAnnouncementControlsDTO Proposal { get; set; }

        public IEnumerable<string> InstructorsJson { get; set; }
    }

    public class ProposalForAnnouncementControlsDTO
    {
        public int Id { get; set; }
        public int SemesterId { get; set; }
    }

    public class AnnouncementSectionForAnnouncementControlsDTO
    {
        public int Id { get; set; }
        public string Section { get; set; }

        public int StudentsCount {
            get
            {
                return StudentCourses?.Count(sc => sc.State != (int)enu_CourseState.Dropped) ?? 0;
            }
        }

        public IEnumerable<StudentCourseTempForAnnouncementControlsDTO> StudentCourses { get; set; }
    }

    public class StudentCourseTempForAnnouncementControlsDTO
    {
        public int Id { get; set; }
        public int State { get; set; }
    }
}