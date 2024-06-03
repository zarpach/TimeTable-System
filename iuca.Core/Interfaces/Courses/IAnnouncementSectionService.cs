
using iuca.Application.DTO.Courses;

namespace iuca.Application.Interfaces.Courses
{
    public interface IAnnouncementSectionService
    {
        /// <summary>
        /// Get announcement section by course import code, semester id and section number
        /// </summary>
        /// <param name="importCode">Course import code</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="sectionNumber">Section number</param>
        /// <returns>Announcement section</returns>
        AnnouncementSectionDTO GetAnnouncementSection(int importCode, int semesterId, string sectionNumber);

        /// <summary>
        /// Create announcement section
        /// </summary>
        /// <param name="announcementSectionDTO">Announcement section</param>
        void CreateAnnouncementSection(AnnouncementSectionDTO announcementSectionDTO);

        /// <summary>
        /// Edit announcement section by id
        /// </summary>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <param name="announcementSectionDTO">Announcement section</param>
        void EditAnnouncementSection(int announcementSectionId, AnnouncementSectionDTO announcementSectionDTO);

        /// <summary>
        /// Delete announcement section by id
        /// </summary>
        /// <param name="announcementSectionId">Announcement section id</param>
        void DeleteAnnouncementSection(int announcementSectionId);
    }
}
