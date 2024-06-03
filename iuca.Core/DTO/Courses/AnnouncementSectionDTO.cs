using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.ViewModels.Courses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iuca.Application.DTO.Courses
{
    public class AnnouncementSectionDTO
    {
        public int Id { get; set; }

        [Display(Name = "Announcement")]
        public AnnouncementDTO Announcement { get; set; }

        [Display(Name = "Announcement")]
        public int AnnouncementId { get; set; }

        [Display(Name = "Instructor")]
        [Required]
        public string InstructorUserId { get; set; }
        public UserDTO Instructor { get; set; }

        [Display(Name = "Instructor user name")]
        public string InstructorUserName { get; set; } // ???

        [Display(Name = "Syllabus")]
        public SyllabusDTO Syllabus { get; set; }

        [Display(Name = "Places")]
        public int Places { get; set; } = 25;

        [Display(Name = "Schedule")]
        public string Schedule { get; set; } = "Undefined";

        [Display(Name = "Credits")]
        [Range(0, 50, ErrorMessage = "Value must be equal or greater than 0")]
        public float Credits { get; set; } = -1;

        [Display(Name = "Section")]
        [MaxLength(2)]
        [Required]
        public string Section { get; set; }

        [Display(Name = "Grade sheet submitted")]
        public bool GradeSheetSubmitted { get; set; }
        
        public IEnumerable<AttendanceDTO> Attendances { get; set; }

        [Display(Name = "Extra instructors")]
        public IEnumerable<string> ExtraInstructorsJson { get; set; }
        public IEnumerable<UserDTO> ExtraInstructorsList { get; set; }

        [Display(Name = "Groups")]
        public IEnumerable<string> GroupsJson { get; set; }
        public IEnumerable<GroupDTO> Groups { get; set; }

        [Display(Name = "Attendance inconsistencies")]
        public AttendanceInconsistencyViewModel AttendanceInconsistencies { get; set; }

        // to delete
        [Display(Name = "Organization")]
        public OrganizationDTO Organization { get; set; }

        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Course")]
        public CourseDTO Course { get; set; }

        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Display(Name = "Season")]
        public int Season { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Changed")]
        public bool IsChanged { get; set; }

        [Display(Name = "Deleted")]
        public bool MarkedDeleted { get; set; }

        [Display(Name = "Course det id")]
        public int CourseDetId { get; set; }
    }
}
