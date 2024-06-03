using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.ImportData
{
    public interface IImportStudentEmailService
    {
        /// <summary>
        /// Update students emails by csv file
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="fileStream">File stream</param>
        void UpdateStudentEmails(int organizationId, Stream fileStream);
    }
}
