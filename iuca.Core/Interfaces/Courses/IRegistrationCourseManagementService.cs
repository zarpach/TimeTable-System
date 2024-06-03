using iuca.Application.DTO.Common;
using iuca.Application.DTO.Courses;
using iuca.Application.ViewModels.Courses;
using System.Collections.Generic;

namespace iuca.Application.Interfaces.Courses
{
    public interface IRegistrationCourseManagementService
    {
        /// <summary>
        /// Get registration course students
        /// </summary>
        /// <param name="registrationCourseId">Registration course id</param>
        List<TransferCourseStudentViewModel> GetRegistrationCourseStudents(int registrationCourseId);

        /// <summary>
        /// Save transfered students for registration courses
        /// </summary>
        /// <param name="courseIdFrom">Registration course id from</param>
        /// <param name="courseIdTo">Registration course id to</param>
        /// <param name="transferStudentUserIds">Student user ids to transfer</param>
        void SaveTransferCourseStudents(int courseIdFrom, int courseIdTo, string[] transferStudentUserIds);

        /// <summary>
        /// Get announcement list for assigning
        /// </summary>
        /// <param name="semesterId">Semester id</param>
        /// <param name="deanDepartments">Dean departments</param>
        /// <returns>Announcement list</returns>
        IEnumerable<AnnouncementDTO> GetAnnouncementsForAssigning(int semesterId, List<DepartmentDTO> deanDepartments);

        /// <summary>
        /// Get sections with students
        /// </summary>
        /// <param name="announcementId">Announcement id</param>
        /// <returns>Sections with students</returns>
        List<StudentsInSectionViewModel> GetSectionsWithStudents(int announcementId);

        /// <summary>
        /// Set student section
        /// </summary>
        /// <param name="studentUserId">Student user id</param>
        /// <param name="oldAnnouncementSectionId">Old announcement section id</param>
        /// <param name="newAnnouncementSectionId">New announcement section id</param>
        void SetStudentSection(string studentUserId, int oldAnnouncementSectionId, int newAnnouncementSectionId);
    }
}
