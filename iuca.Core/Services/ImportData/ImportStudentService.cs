using iuca.Application.DTO.Users.Students;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Interfaces.ImportData;
using iuca.Application.Interfaces.Users.Students;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels.Users.Students;
using iuca.Application.ViewModels.Users.UserInfo;
using iuca.Domain.Entities.Users.Students;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Application.Services.ImportData
{
    public class ImportStudentService : IImportStudentService
    {
        private readonly IApplicationDbContext _db;
        private readonly IUserInfoService _userInfoService;
        private readonly IUserBasicInfoService _userBasicInfoService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IStudentInfoService _studentInfoService;
        private readonly IImportHelperService _importHelperService;
        private readonly IStudentOrgInfoService _studentOrgInfoService;
        
        private DepartmentGroup defaultDepartmentGroup;

        public ImportStudentService(IApplicationDbContext db,
            IUserInfoService userInfoService,
            IUserBasicInfoService userBasicInfoService,
            ApplicationUserManager<ApplicationUser> userManager,
            IStudentInfoService studentInfoService,
            IImportHelperService importHelperService,
            IStudentOrgInfoService studentOrgInfoService)
        {
            _db = db;
            _userInfoService = userInfoService;
            _userBasicInfoService = userBasicInfoService;
            _userManager = userManager;
            _studentInfoService = studentInfoService;
            _importHelperService = importHelperService;
            _studentOrgInfoService = studentOrgInfoService;

            defaultDepartmentGroup = _db.DepartmentGroups.Include(x => x.Department).FirstOrDefault(x => x.Code == "NA");
            if (defaultDepartmentGroup == null)
                throw new Exception("Default department group not found");
        }

        /// <summary>
        /// Import students from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        /// <param name="organizationId">Organization id</param>
        public void ImportStudents(string connection, bool overwrite, int organizationId)
        {
            var organization = _db.Organizations.FirstOrDefault(x => x.Id == organizationId);
            if (organization == null)
                throw new Exception("Organization not found");

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.students_for_import";

                //13 - college
                if (organization.IsMain)
                    query += " WHERE program != 13";
                else
                    query += " WHERE program = 13";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            List<StudentGroupInfoViewModel> groupInfoList = GetStudentsGroupInfo(connection, organization.IsMain);
                            while (sdr.Read())
                            {
                                ProcessStudent(sdr, overwrite, organizationId, organization.IsMain, groupInfoList);
                            }
                        }
                    }
                }
            }
        }

        private void ProcessStudent(NpgsqlDataReader sdr, bool overwrite, int organizationId, bool isMain,
            List<StudentGroupInfoViewModel> groupInfoList)
        {
            int studentId = int.Parse(sdr["sid"].ToString());
            DepartmentGroup departmentGroup = GetDepartmentGroup(studentId, groupInfoList, organizationId);
            var studentOrgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.StudentId == studentId && x.OrganizationId == organizationId);

            if (studentOrgInfo != null)
            {
                if (overwrite)
                    EditStudent(sdr, organizationId, studentOrgInfo.StudentBasicInfo,
                        departmentGroup, isMain);
            }
            else
                CreateStudent(sdr, organizationId, departmentGroup, isMain);
        }

        private void CreateStudent(NpgsqlDataReader sdr, int organizationId, DepartmentGroup departmentGroup, 
            bool isMain)
        {
            string email = CreateUser(sdr, organizationId);
            ApplicationUser user = _userManager.Users.Include(x => x.UserBasicInfo)
                .Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.Email == email);
            if (user == null)
                throw new Exception("User not found");
            if (user.UserBasicInfo == null)
                CreateUserBasicInfo(sdr, organizationId, user.Id);
            else
                EditUserBasicInfo(sdr, organizationId, user.UserBasicInfo.ApplicationUserId, user.UserBasicInfo.Id);

            if (user.StudentBasicInfo == null)
                CreateStudentInfo(sdr, organizationId, user.Id, departmentGroup);
            else
                EditStudentInfo(sdr, organizationId, departmentGroup, user.StudentBasicInfo.Id, isMain);
        }

        private string CreateUser(NpgsqlDataReader sdr, int organizationId)
        {
            UserInfoViewModel userInfo = new UserInfoViewModel();
            userInfo.ApplicationUser = new ApplicationUser();
            userInfo.ApplicationUser.Email = "_" + new String(sdr["lnameeng"].ToString().Where(char.IsLetter).ToArray()).Split()[0].Trim()
                                       + sdr["sid"].ToString() + "@iuca.kg";
            userInfo.ApplicationUser.UserName = userInfo.ApplicationUser.Email;
            userInfo.ApplicationUser.IsActive = true;
            userInfo.ApplicationUser.LastNameEng = sdr["lnameeng"].ToString();
            userInfo.ApplicationUser.FirstNameEng = sdr["fnameeng"].ToString();
            userInfo.ApplicationUser.MiddleNameEng = sdr["mnameeng"].ToString();
            userInfo.IsStudent = true;

            _userInfoService.Create(organizationId, userInfo);

            return userInfo.ApplicationUser.Email;
        }

        private void CreateUserBasicInfo(NpgsqlDataReader sdr, int organizationId, string applicationUserId) 
        {
            UserBasicInfoDTO userBasicInfo = new UserBasicInfoDTO();
            userBasicInfo.ApplicationUserId = applicationUserId;
            userBasicInfo.LastNameRus = sdr["lnamerus"].ToString();
            userBasicInfo.FirstNameRus = sdr["fnamerus"].ToString();
            userBasicInfo.MiddleNameRus = sdr["mnamerus"].ToString();
            userBasicInfo.Sex = bool.Parse(sdr["sex"].ToString()) ? (int)enu_Sex.Male : (int)enu_Sex.Female;
            userBasicInfo.DateOfBirth = DateTime.Parse(sdr["dob"].ToString());
            userBasicInfo.NationalityId = _importHelperService.GetNationalityId(sdr["ncode"].ToString());
            userBasicInfo.CitizenshipId = _importHelperService.GetCountryId(sdr["ccode"].ToString());

            _userBasicInfoService.Create(organizationId, userBasicInfo);
        }

        private void CreateStudentInfo(NpgsqlDataReader sdr, int organizationId, string applicationUserId,
            DepartmentGroup departmentGroup)
        {
            StudentBasicInfoDTO studentBasicInfo = new StudentBasicInfoDTO();
            studentBasicInfo.ApplicationUserId = applicationUserId;

            StudentOrgInfoDTO studentOrgInfo = new StudentOrgInfoDTO();
            studentOrgInfo.State = int.Parse(sdr["status"].ToString());
            studentOrgInfo.StudentId = int.Parse(sdr["sid"].ToString());
            studentOrgInfo.DepartmentGroupId = departmentGroup.Id;
            studentOrgInfo.IsPrep = departmentGroup.Code.StartsWith("PREP");

            StudentInfoDetailsViewModel studentInfo = new StudentInfoDetailsViewModel();
            studentInfo.StudentBasicInfo = studentBasicInfo;
            studentInfo.StudentOrgInfo = studentOrgInfo;
            _studentInfoService.Create(organizationId, studentInfo);
        }

        private void EditStudent(NpgsqlDataReader sdr, int organizationId, StudentBasicInfo studentBasicInfo,
            DepartmentGroup departmentGroup, bool isMain)
        {
            var user = _userManager.Users.Include(x => x.UserBasicInfo)
                .FirstOrDefault(x => x.Id == studentBasicInfo.ApplicationUserId);

            if (user == null)
                throw new Exception("User not found");

            EditUserInfo(sdr, organizationId, studentBasicInfo.ApplicationUserId, user);

            if (user.UserBasicInfo != null)
                EditUserBasicInfo(sdr, organizationId, studentBasicInfo.ApplicationUserId, user.UserBasicInfo.Id);
            else
                CreateUserBasicInfo(sdr, organizationId, studentBasicInfo.ApplicationUserId);

            EditStudentInfo(sdr, organizationId, departmentGroup, studentBasicInfo.Id, isMain);
        }

        private void EditUserInfo(NpgsqlDataReader sdr, int organizationId, string applicationUserId,
            ApplicationUser user)
        {
            UserInfoViewModel userInfo = new UserInfoViewModel();
            userInfo.ApplicationUser = user;
            userInfo.ApplicationUser.LastNameEng = sdr["lnameeng"].ToString();
            userInfo.ApplicationUser.FirstNameEng = sdr["fnameeng"].ToString();
            userInfo.ApplicationUser.MiddleNameEng = sdr["mnameeng"].ToString();
            userInfo.ApplicationUser.IsActive = true;
            userInfo.IsStudent = true;

            _userInfoService.Edit(organizationId, applicationUserId, userInfo);
        }

        private void EditUserBasicInfo(NpgsqlDataReader sdr, int organizationId, string applicationUserId,
            int userBasicInfoId) 
        {
            UserBasicInfoDTO userBasicInfo = new UserBasicInfoDTO();
            userBasicInfo.Id = userBasicInfoId;
            userBasicInfo.ApplicationUserId = applicationUserId;
            userBasicInfo.LastNameRus = sdr["lnamerus"].ToString();
            userBasicInfo.FirstNameRus = sdr["fnamerus"].ToString();
            userBasicInfo.MiddleNameRus = sdr["mnamerus"].ToString();
            userBasicInfo.Sex = bool.Parse(sdr["sex"].ToString()) ? (int)enu_Sex.Male : (int)enu_Sex.Female;
            userBasicInfo.DateOfBirth = DateTime.Parse(sdr["dob"].ToString());
            userBasicInfo.NationalityId = _importHelperService.GetNationalityId(sdr["ncode"].ToString());
            userBasicInfo.CitizenshipId = _importHelperService.GetCountryId(sdr["ccode"].ToString());

            _userBasicInfoService.Edit(organizationId, userBasicInfo);
        }

        private void EditStudentInfo(NpgsqlDataReader sdr, int organizationId, DepartmentGroup departmentGroup,
                    int studentBasicInfoId, bool isMain)
        {
            StudentBasicInfoDTO studentBasicInfo = new StudentBasicInfoDTO();
            studentBasicInfo.Id = studentBasicInfoId;
            studentBasicInfo.IsMainOrganization = isMain;

            StudentOrgInfoDTO studentOrgInfo = new StudentOrgInfoDTO();
            studentOrgInfo.StudentBasicInfoId = studentBasicInfoId;
            studentOrgInfo.State = int.Parse(sdr["status"].ToString());
            studentOrgInfo.StudentId = int.Parse(sdr["sid"].ToString());
            studentOrgInfo.DepartmentGroupId = departmentGroup.Id;
            studentOrgInfo.IsPrep = departmentGroup.Department.Code.StartsWith("PREP");

            StudentInfoDetailsViewModel studentInfo = new StudentInfoDetailsViewModel();
            studentInfo.StudentBasicInfo = studentBasicInfo;
            studentInfo.StudentOrgInfo = studentOrgInfo;
            _studentInfoService.Edit(organizationId, studentInfo);
        }

        private List<StudentGroupInfoViewModel> GetStudentsGroupInfo(string connection, bool isMainOrganization)
        {
            List<StudentGroupInfoViewModel> groupInfoList = new List<StudentGroupInfoViewModel>();

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.students_groups_for_import";

                //13 - college
                if (isMainOrganization)
                    query += " WHERE program != 13";
                else
                    query += " WHERE program = 13";

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
                                StudentGroupInfoViewModel groupInfo = new StudentGroupInfoViewModel();
                                groupInfo.StudentId = int.Parse(sdr["sid"].ToString());
                                groupInfo.DepartmentImportId = int.Parse(sdr["deptid"].ToString());
                                groupInfo.Code = sdr["code"].ToString();
                                groupInfo.Semester = int.Parse(sdr["semester"].ToString());
                                groupInfo.Year = int.Parse(sdr["year"].ToString());
                                groupInfo.CurrentGroup = bool.Parse(sdr["current"].ToString());
                                groupInfoList.Add(groupInfo);
                            }
                            _db.SaveChanges();
                        }
                    }
                }

                return groupInfoList;
            }
        }

        private DepartmentGroup GetDepartmentGroup(int studentId, List<StudentGroupInfoViewModel> groupInfoList, int organizationId)
        {
            DepartmentGroup _departmentGroup = defaultDepartmentGroup;

            var studentGroupInfoList = groupInfoList.Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.Year).ThenByDescending(x => x.Semester).ToList();

            if (studentGroupInfoList.Count > 0)
            {
                var studentGroupInfo = studentGroupInfoList.FirstOrDefault(x => x.CurrentGroup);
                if (studentGroupInfo == null)
                    studentGroupInfo = studentGroupInfoList.FirstOrDefault();

                var department = _db.Departments.FirstOrDefault(x => x.ImportCode == studentGroupInfo.DepartmentImportId
                                    && x.OrganizationId == organizationId);
                if (department != null)
                {
                    var departmentGroup = _db.DepartmentGroups.Include(x => x.Department)
                            .FirstOrDefault(x => x.DepartmentId == department.Id && x.Code == studentGroupInfo.Code);
                    if (departmentGroup != null)
                        _departmentGroup = departmentGroup;
                }

            }

            return _departmentGroup;
        }

        /// <summary>
        /// Syncronize students states with old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="organizationId">Organization id</param>
        public void SyncStudentsStates(string connection, int organizationId)
        {
            var organization = _db.Organizations.FirstOrDefault(x => x.Id == organizationId);
            if (organization == null)
                throw new Exception("Organization not found");

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.students_for_import";

                //13 - college
                if (organization.IsMain)
                    query += " WHERE program != 13";
                else
                    query += " WHERE program = 13";

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
                                ProcessStudentState(sdr, organizationId);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Syncronize students groups with old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="organizationId">Organization id</param>
        public void SyncStudentsGroups(string connection, int organizationId)
        {
            var organization = _db.Organizations.FirstOrDefault(x => x.Id == organizationId);
            if (organization == null)
                throw new Exception("Organization not found");

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.students_for_import";

                //13 - college
                if (organization.IsMain)
                    query += " WHERE program != 13";
                else
                    query += " WHERE program = 13";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            List<StudentGroupInfoViewModel> groupInfoList = GetStudentsGroupInfo(connection, organization.IsMain);
                            while (sdr.Read())
                            {
                                ProcessStudentGroup(sdr, organizationId, groupInfoList);
                            }
                        }
                    }
                }
            }
        }

        private void ProcessStudentState(NpgsqlDataReader sdr, int organizationId)
        {
            int studentId = int.Parse(sdr["sid"].ToString());
            var studentOrgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.StudentId == studentId && x.OrganizationId == organizationId);

            if (studentOrgInfo != null) 
            {
                EditStudentState(studentOrgInfo, int.Parse(sdr["status"].ToString()));
            }
        }

        private void ProcessStudentGroup(NpgsqlDataReader sdr, int organizationId,
            List<StudentGroupInfoViewModel> groupInfoList)
        {
            int studentId = int.Parse(sdr["sid"].ToString());
            var studentOrgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.StudentId == studentId && x.OrganizationId == organizationId);

            if (studentOrgInfo != null)
            {
                DepartmentGroup departmentGroup = GetDepartmentGroup(studentId, groupInfoList, organizationId);
                if (departmentGroup.Id != studentOrgInfo.DepartmentGroupId)
                    EditStudentGroup(studentOrgInfo, departmentGroup);
            }
        }

        private void EditStudentState(StudentOrgInfo orgInfo, int state)
        {
            StudentOrgInfoDTO studentOrgInfo = new StudentOrgInfoDTO();
            studentOrgInfo.StudentBasicInfoId = orgInfo.StudentBasicInfoId;
            studentOrgInfo.OrganizationId = orgInfo.OrganizationId;
            studentOrgInfo.StudentId = orgInfo.StudentId;
            studentOrgInfo.DepartmentGroupId = orgInfo.DepartmentGroupId;
            studentOrgInfo.PrepDepartmentGroupId = orgInfo.PrepDepartmentGroupId;
            studentOrgInfo.IsPrep = orgInfo.IsPrep;
            studentOrgInfo.State = state;

            _studentOrgInfoService.Edit(orgInfo.OrganizationId, studentOrgInfo);
        }

        private void EditStudentGroup(StudentOrgInfo orgInfo, DepartmentGroup departmentGroup)
        {
            StudentOrgInfoDTO studentOrgInfo = new StudentOrgInfoDTO();
            studentOrgInfo.StudentBasicInfoId = orgInfo.StudentBasicInfoId;
            studentOrgInfo.OrganizationId = orgInfo.OrganizationId;
            studentOrgInfo.StudentId = orgInfo.StudentId;
            studentOrgInfo.State = orgInfo.State;
            studentOrgInfo.PrepDepartmentGroupId = orgInfo.PrepDepartmentGroupId;
            studentOrgInfo.DepartmentGroupId = departmentGroup.Id;
            studentOrgInfo.IsPrep = departmentGroup.Department.Code.StartsWith("PREP");

            _studentOrgInfoService.Edit(orgInfo.OrganizationId, studentOrgInfo);
        }
    }
}
