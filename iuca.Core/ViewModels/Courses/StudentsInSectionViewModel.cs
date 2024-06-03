using iuca.Application.DTO.Courses;
using System.Collections.Generic;

namespace iuca.Application.ViewModels.Courses
{
    public class StudentsInSectionViewModel
    {
        public int AnnouncementSectionId { get; set; }
        public AnnouncementSectionDTO AnnouncementSection { get; set; }
        public IEnumerable<StudentInfoViewModel> Students { get; set; }
    }

    public class StudentInfoViewModel
    {
        public string UserId { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string Group { get; set; }
        public int StudentId { get; set; }
    }
}
