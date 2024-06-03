using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using System.Collections.Generic;
using System.Linq;

namespace iuca.Application.DTO.Courses
{
    public class AnnouncementForAnnouncementInfoDTO
    {
        public int Id { get; set; }
        public int SemesterId { get; set; }
        public int CourseId { get; set; }
        public bool IsActivated { get; set; }
        public bool IsForAll { get; set; }

        public int AnnouncementSectionsCount
        {
            get
            {
                return AnnouncementSections?.Count() ?? 0;
            }
        }

        public List<UserDTO> Instructors { get; set; }
        public List<GroupDTO> Groups { get; set; }

        public SemesterForAnnouncementInfoDTO Semester { get; set; }
        public CourseForAnnouncementInfoDTO Course { get; set; }
        public ProposalCourseForAnnouncementInfoDTO ProposalCourse { get; set; }

        public IEnumerable<AnnouncementSectionForAnnouncementInfoDTO> AnnouncementSections { get; set; }
    }

    public class SemesterForAnnouncementInfoDTO
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
    }

    public class CourseForAnnouncementInfoDTO
    {
        public int Id { get; set; }
        public string NameEng { get; set; }
        public string NameRus { get; set; }
        public int CourseType { get; set; }
        public string Abbreviation { get; set; }
        public string Number { get; set; }
        public int ImportCode { get; set; }
        public int DepartmentId { get; set; }
        public int LanguageId { get; set; }

        public DepartmentForAnnouncementInfoDTO Department { get; set; }
        public LanguageForAnnouncementInfoDTO Language { get; set; }
    }

    public class ProposalCourseForAnnouncementInfoDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int ProposalId { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }
        public string Schedule { get; set; }

        public IEnumerable<int> YearsOfStudyJson { get; set; }

        public ProposalForAnnouncementInfoDTO Proposal { get; set; }
    }

    public class ProposalForAnnouncementInfoDTO
    {
        public int Id { get; set; }
        public int SemesterId { get; set; }
    }

    public class AnnouncementSectionForAnnouncementInfoDTO
    {
        public int Id { get; set; }
        public string InstructorUserId { get; set; }

        public UserDTO Instructor { get; set; }

        public IEnumerable<string> ExtraInstructorsJson { get; set; }
        public IEnumerable<string> GroupsJson { get; set; }

        public IEnumerable<UserDTO> ExtraInstructorsList { get; set; }
        public IEnumerable<GroupDTO> Groups { get; set; }
    }

    public class DepartmentForAnnouncementInfoDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
    }

    public class LanguageForAnnouncementInfoDTO
    {
        public int Id { get; set; }
        public string NameEng { get; set; }
    }
}