using iuca.Application.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.ExportData
{
    public interface IExportStudentCourseService
    {
        /// <summary>
        /// Export student courses to old DB by registration course id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="registrationCourseId">Registration course id</param>
        /// <param name="connection">Connection</param>
        /// <returns>Added and dropped courses count</returns>
        ExportCourseResultViewModel ExportStudentCoursesByRegistrationCourse(int organizationId, int registrationCourseId,
            string connection);

        /// <summary>
        /// Export student courses to old DB by semester id
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="connection">Connection</param>
        /// <param name="force">If true deletes unnecessary rows from old DB</param>
        /// <returns>Added and dropped courses count</returns>
        ExportCourseResultViewModel ExportStudentCoursesBySemester(int organizationId, int semesterId,
            string connection, bool force);
    }
}
