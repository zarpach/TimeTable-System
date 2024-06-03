using iuca.Application.DTO.Courses;
using iuca.Application.Enums;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Courses
{
    public interface IAnnouncementService
    {
        /// <summary>
        /// Get announcement list
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="withAnnouncementSections">Announcement section include</param>
        /// <param name="withSemester">Semester include</param>
        /// <returns>Announcement list</returns>
        IEnumerable<AnnouncementDTO> GetAnnouncements(int semesterId, bool withAnnouncementSections = false, bool withSemester = false);

        /// <summary>
        /// Get announcement list for announcement info
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="departmentId">Department id</param>
        /// <param name="courseType">Course type</param>
        /// <param name="isForAll">For all</param>
        /// <param name="isActivated">Activated</param>
        /// <returns>Announcement list</returns>
        IEnumerable<AnnouncementForAnnouncementInfoDTO> GetAnnouncementsForAnnouncementInfo(int semesterId, int departmentId = -1, int courseType = -1, int isForAll = (int)enu_Status.All, int isActivated = (int)enu_Status.All);

        /// <summary>
        /// Get announcement by id for announcement controls
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <returns>Announcement</returns>
        AnnouncementForAnnouncementControlsDTO GetAnnouncementForAnnouncementControls(int announcementId);

        /// <summary>
        /// Get announcement by id
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <returns>Announcement</returns>
        AnnouncementDTO GetAnnouncement(int announcementId);

        /// <summary>
        /// Edit announcement sections by announcement id
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <param name="announcementSectionDTOs">Announcement sections</param>
        void EditAnnouncementSections(int announcementId, IEnumerable<AnnouncementSectionDTO> announcementSectionDTOs);

        /// <summary>
        /// Replace announcememnt instructor for all sections
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="courseId">Course id</param>
        /// <param name="previousInstructorId">Previous instructor id</param>
        /// <param name="futureInstructorId">Future instructor id</param>
        void ReplaceAnnouncementInstructor(int semesterId, int courseId, string previousInstructorId, string futureInstructorId);

        /// <summary>
        /// Remove announcememnt instructor for all sections
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="courseId">Course id</param>
        /// <param name="instructorId">Instructor id</param>
        void RemoveAnnouncementInstructor(int semesterId, int courseId, string instructorId);

        /// <summary>
        /// Set announcement for all value
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <param name="isForAll">For all value</param>
        void SetAnnouncementForAllValue(int announcementId, bool isForAll);

        /// <summary>
        /// Set announcement status
        /// </summary>
        /// <param name="announcementIds">Announcement ids</param>
        /// <param name="isActivated">Announcement status</param>
        void SetAnnouncementStatuses(int[] announcementIds, bool isActivated);

        /// <summary>
        /// Create announcements
        /// </summary>
        /// <param name="proposalCourseIds">Proposal course ids</param>
        void CreateAnnouncements(int[] proposalCourseIds);

        /// <summary>
        /// Delete announcement
        /// </summary>
        /// <param name="proposalCourseId">Proposal course id</param>
        void DeleteAnnouncement(int proposalCourseId);
    }
}
