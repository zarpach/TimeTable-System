using iuca.Application.Interfaces.ImportData;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Persistence;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.ImportData
{
    public class ImportDepartmentService : IImportDepartmentService
    {
        private readonly IApplicationDbContext _db;

        public ImportDepartmentService(IApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Import departments from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        public void ImportDepartments(string connection, bool overwrite, int organizationId)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.departments";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                ProcessDepartment(sdr, overwrite, organizationId);
                            }
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void ProcessDepartment(NpgsqlDataReader sdr, bool overwrite, int organizationId)
        {
            int importCode = int.Parse(sdr["deptid"].ToString());
            var department = _db.Departments.FirstOrDefault(x => x.ImportCode == importCode && x.OrganizationId == organizationId);
            if (department != null)
            {
                if (overwrite)
                    EditDepartment(sdr, department);
            }
            else
                CreateDepartment(sdr, organizationId);
        }

        private void CreateDepartment(NpgsqlDataReader sdr, int organizationId)
        {
            Department department = new Department();
            department.ImportCode = int.Parse(sdr["deptid"].ToString());
            department.OrganizationId = organizationId;
            department.Code = sdr["dcode"].ToString();
            department.NameEng = sdr["department_nameeng"].ToString();
            department.NameRus = sdr["department_namerus"].ToString();
            department.NameKir = sdr["department_namekyr"].ToString();

            _db.Departments.Add(department);
        }

        private void EditDepartment(NpgsqlDataReader sdr, Department department)
        {
            department.Code = sdr["dcode"].ToString();
            department.NameEng = sdr["department_nameeng"].ToString();
            department.NameRus = sdr["department_namerus"].ToString();
            department.NameKir = sdr["department_namekyr"].ToString();

            _db.Departments.Update(department);
        }

    }
}
