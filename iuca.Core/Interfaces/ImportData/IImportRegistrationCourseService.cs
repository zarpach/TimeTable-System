namespace iuca.Application.Interfaces.ImportData
{
    public interface IImportRegistrationCourseService
    {
        /// <summary>
        /// Import announcement sections from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        void ImportAnnouncementSections(string connection, bool overwrite, int organizationId, int semesterId);

        /// <summary>
        /// Import announcement section data from old BB to new DB
        /// </summary>
        /// <param name="connection">Connection string</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="courseDetId">Course details id</param>
        void ImportAnnouncementSectionData(string connection, int organizationId, int courseDetId);
    }
}
