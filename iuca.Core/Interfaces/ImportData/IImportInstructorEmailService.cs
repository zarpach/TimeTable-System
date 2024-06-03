using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.ImportData
{
    public interface IImportInstructorEmailService
    {
        /// <summary>
        /// Update instructors emails by csv file
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="fileStream">File stream</param>
        void UpdateInstructorEmails(int organizationId, Stream fileStream);
    }
}
