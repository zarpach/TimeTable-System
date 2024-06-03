
namespace iuca.Domain.Entities.Courses
{
    public class StudyCardCourse
    {
        public int Id { get; set; }
        public int StudyCardId { get; set; }
        public StudyCard StudyCard { get; set; }
        public int AnnouncementSectionId { get; set; }
        public AnnouncementSection AnnouncementSection { get; set; }
        public string Comment { get; set; }
    }
}
