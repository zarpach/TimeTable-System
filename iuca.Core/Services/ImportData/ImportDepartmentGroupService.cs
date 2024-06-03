using iuca.Application.Interfaces.ImportData;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Persistence;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.ImportData
{
    public class ImportDepartmentGroupService : IImportDepartmentGroupService
    {
        private readonly IApplicationDbContext _db;
        private readonly IImportHelperService _importHelperService;

        private List<DepartmentGroup> departmentGroupList;

        public ImportDepartmentGroupService(IApplicationDbContext db,
            IImportHelperService importHelperService)
        {
            _db = db;
            _importHelperService = importHelperService;
        }


        /// <summary>
        /// Import departments from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        /// <param name="organizationId">Organization id</param>
        public void ImportDepartmentGroups(string connection, bool overwrite, int organizationId)
        {
            var organization = _db.Organizations.FirstOrDefault(x => x.Id == organizationId);
            if (organization == null)
                throw new Exception("Organization not found");

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.departments_for_import";

                //13 - college
                if (organization.IsMain)
                    query += " WHERE organization != 13";
                else
                    query += " WHERE organization = 13";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            departmentGroupList = new List<DepartmentGroup>();
                            while (sdr.Read())
                            {
                                AddDepartmentGroupToList(sdr, organizationId);
                            }

                            foreach (var departmentGroup in departmentGroupList) 
                            {
                                ProcessDepartmentGroup(departmentGroup, overwrite);
                            }

                            _db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void AddDepartmentGroupToList(NpgsqlDataReader sdr, int organizationId) 
        {
            DepartmentGroup departmentGroup = new DepartmentGroup();

            departmentGroup.Code = sdr["code"].ToString();
            departmentGroup.Year = GetYearByDepartmentCode(departmentGroup.Code);
            departmentGroup.DepartmentId = _importHelperService.GetDepartmentId(sdr["deptid"].ToString(), organizationId);
            departmentGroup.OrganizationId = organizationId;

            if (departmentGroupList.FirstOrDefault(x => x.Code == departmentGroup.Code &&
                                                        x.Year == departmentGroup.Year &&
                                                        x.DepartmentId == departmentGroup.DepartmentId &&
                                                        x.OrganizationId == departmentGroup.OrganizationId) == null) 
            {
                departmentGroupList.Add(departmentGroup);
            }
        }

        private void ProcessDepartmentGroup(DepartmentGroup departmentGroup, bool overwrite)
        {
            var dbDepartmentGroup = _db.DepartmentGroups.FirstOrDefault(x => x.Code == departmentGroup.Code 
                        && x.DepartmentId == departmentGroup.DepartmentId);
            if (dbDepartmentGroup != null)
            {
                if (overwrite)
                    EditDepartmentGroup(dbDepartmentGroup, departmentGroup);
            }
            else
                CreateDepartmentGroup(departmentGroup);
        }

        private void CreateDepartmentGroup(DepartmentGroup departmentGroup)
        {
            _db.DepartmentGroups.Add(departmentGroup);
        }

        private void EditDepartmentGroup(DepartmentGroup dbDepartmentGroup, DepartmentGroup departmentGroup)
        {
            dbDepartmentGroup.Code = departmentGroup.Code;
            dbDepartmentGroup.Year = departmentGroup.Year;
            dbDepartmentGroup.DepartmentId = departmentGroup.DepartmentId;

            _db.DepartmentGroups.Update(dbDepartmentGroup);
        }

        private int GetYearByDepartmentCode(string code)
        {
            int year = 0;
            if (!string.IsNullOrEmpty(code) && code.Length == 3)
            {
                int.TryParse("20" + code.Substring(1), out year);
            }

            return year;
        }

    }
}
