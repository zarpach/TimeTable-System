namespace iuca.Application.Interfaces.ExportData
{
    public interface IExportCourseService
    {
        void ExportCourse(int organizationId, int courseId, string connection);
    }
}
