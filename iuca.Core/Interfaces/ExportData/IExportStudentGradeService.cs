using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.ExportData
{
    public interface IExportStudentGradeService
    {
        /// <summary>
        /// Export student grades to old DB
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="semesterId">Semester id</param>
        /// <param name="connection">Connection</param>
        /// <returns>Added courses count</returns>
        void ExportStudentGrades(int organizationId, int semesterId, string connection);
    }
}
