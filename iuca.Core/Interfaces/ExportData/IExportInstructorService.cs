using iuca.Application.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.ExportData
{
    public interface IExportInstructorService
    {
        /// <summary>
        /// Export instructor info to old DB
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <param name="connection">Connection string</param>
        /// <returns>Instructor import code and result</returns>
        /// <exception cref="Exception"></exception>
        (int importCode, ResultViewModel result) ExportInstructor(int organizationId, int instructorBasicInfoId,
            string connection);
    }
}
