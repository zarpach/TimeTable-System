using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Interfaces.ImportData
{
    public interface IImportEducationTypeService
    {
        /// <summary>
        /// Import education type from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        void ImportEducationTypes(string connection, bool overwrite);
    }
}
