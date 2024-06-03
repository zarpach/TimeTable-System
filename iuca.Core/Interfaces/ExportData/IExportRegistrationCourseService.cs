namespace iuca.Application.Interfaces.ExportData
{
    public interface IExportRegistrationCourseService
    {
        /// <summary>
        /// Export announcement sections
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="force">Force update</param>
        /// <param name="connection">Connection</param>
        void ExportAnnouncementSections(int organizationId, int semesterId, bool force, string connection);

        /// <summary>
        /// Update announcement section data in old DB
        /// </summary>
        /// <param name="announcementSectionId">Announcement section id</param>
        /// <param name="connection">Connection</param>
        void ExportRegistrationCourseData(int announcementSectionId, string connection);
    }
}
