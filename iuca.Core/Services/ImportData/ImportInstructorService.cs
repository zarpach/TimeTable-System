using iuca.Application.DTO.Users.Instructors;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Interfaces.ImportData;
using iuca.Application.Interfaces.Users.Instructors;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels.Users.UserInfo;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iuca.Application.Services.ImportData
{
    public class ImportInstructorService : IImportInstructorService
    {
        private readonly IApplicationDbContext _db;
        private readonly IUserInfoService _userInfoService;
        private readonly IUserBasicInfoService _userBasicInfoService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IInstructorBasicInfoService _instructorBasicInfoService;
        private readonly IInstructorOrgInfoService _instructorOrgInfoService;
        private readonly IInstructorOtherJobInfoService _instructorOtherJobInfoService;
        private readonly IInstructorEducationInfoService _instructorEducationInfoService;
        private readonly IInstructorContactInfoService _instructorContactInfoService;
        private readonly IImportHelperService _importHelperService;
        
        private List<InstructorOtherJobInfoDTO> cashedInstructorOtherJobInfo = new List<InstructorOtherJobInfoDTO>();
        private List<InstructorEducationInfoDTO> cashedInstructorEducationInfo = new List<InstructorEducationInfoDTO>();
        private List<InstructorContactInfoDTO> cashedInstructorContactInfo = new List<InstructorContactInfoDTO>();

        public ImportInstructorService(IApplicationDbContext db,
            IUserInfoService userInfoService,
            IUserBasicInfoService userBasicInfoService,
            ApplicationUserManager<ApplicationUser> userManager,
            IInstructorBasicInfoService instructorBasicInfoService,
            IInstructorOrgInfoService instructorOrgInfoService,
            IInstructorOtherJobInfoService instructorOtherJobInfoService,
            IInstructorEducationInfoService instructorEducationInfoService,
            IInstructorContactInfoService instructorContactInfoService,
            IImportHelperService importHelperService)
        {
            _db = db;
            _userInfoService = userInfoService;
            _userBasicInfoService = userBasicInfoService;
            _userManager = userManager;
            _instructorBasicInfoService = instructorBasicInfoService;
            _instructorOrgInfoService = instructorOrgInfoService;
            _instructorOtherJobInfoService = instructorOtherJobInfoService;
            _instructorEducationInfoService = instructorEducationInfoService;
            _instructorContactInfoService = instructorContactInfoService;
            _importHelperService = importHelperService;
        }

        /// <summary>
        /// Import instructors from old database
        /// </summary>
        /// <param name="connection">Connection string of old database</param>
        /// <param name="overwrite">Overwrite data if exists</param>
        /// <param name="organizationId">Organization id</param>
        public void ImportInstructors(string connection, bool overwrite, int organizationId)
        {
            var organization = _db.Organizations.FirstOrDefault(x => x.Id == organizationId);
            if (organization == null)
                throw new Exception("Organization not found");
            
            FillInstructorOtherJobInfo(connection);
            FillInstructorEducationInfo(connection);
            FillInstructorContactInfo(connection);

            using (var transaction = _db.Database.BeginTransaction()) 
            {
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connection))
                    {
                        string query = "SELECT * FROM auca.instructor_general_info";

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
                                        int importCode = int.Parse(sdr["iid"].ToString());
                                        ProcessInstructorMainInfo(sdr, overwrite, organizationId, organization.IsMain, importCode);
                                    }
                                }
                            }
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex) 
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #region Cashing instructor data

        private void FillInstructorOtherJobInfo(string connection) 
        {
            cashedInstructorOtherJobInfo = new List<InstructorOtherJobInfoDTO>();
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.instructor_otherjob_info";

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
                                AddInstructorOtherJobInfo(sdr);
                            }
                        }
                    }
                }
            }
        }

        private void AddInstructorOtherJobInfo(NpgsqlDataReader sdr) 
        { 
            InstructorOtherJobInfoDTO otherJobInfo = new InstructorOtherJobInfoDTO();
            otherJobInfo.PlaceNameEng = sdr["placeeng"].ToString();
            otherJobInfo.PlaceNameRus = sdr["placerus"].ToString();
            otherJobInfo.PlaceNameKir = sdr["placekyr"].ToString();
            otherJobInfo.PositionEng = sdr["positioneng"].ToString();
            otherJobInfo.PositionRus = sdr["positionrus"].ToString();
            otherJobInfo.PositionKir = sdr["positionkyr"].ToString();
            otherJobInfo.Phone = sdr["ph"].ToString();
            otherJobInfo.InstructorImportCode = int.Parse(sdr["iid"].ToString());

            cashedInstructorOtherJobInfo.Add(otherJobInfo);
        }

        private void FillInstructorEducationInfo(string connection)
        {
            cashedInstructorEducationInfo = new List<InstructorEducationInfoDTO>();
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.instructor_education_info";

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
                                AddInstructorEducationInfo(sdr);
                            }
                        }
                    }
                }
            }
        }

        private void AddInstructorEducationInfo(NpgsqlDataReader sdr)
        {
            int graduateYear = 0;
            int.TryParse(sdr["dograd"].ToString(), out graduateYear);

            InstructorEducationInfoDTO educationInfo = new InstructorEducationInfoDTO();
            educationInfo.MajorEng = sdr["majoreng"].ToString();
            educationInfo.MajorRus = sdr["majorrus"].ToString();
            educationInfo.MajorKir = sdr["majorkyr"].ToString();
            educationInfo.GraduateYear = graduateYear;
            educationInfo.UniversityId = _importHelperService.GetUniversityId(sdr["unicode"].ToString());
            educationInfo.EducationTypeId = _importHelperService.GetEducationTypeId(sdr["degree"].ToString());
            educationInfo.InstructorImportCode = int.Parse(sdr["iid"].ToString());

            cashedInstructorEducationInfo.Add(educationInfo);
        }

        private void FillInstructorContactInfo(string connection)
        {
            cashedInstructorContactInfo = new List<InstructorContactInfoDTO>();
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                string query = "SELECT * FROM auca.instructor_contact_info";

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
                                AddInstructorContactInfo(sdr);
                            }
                        }
                    }
                }
            }
        }

        private void AddInstructorContactInfo(NpgsqlDataReader sdr)
        {
            InstructorContactInfoDTO contactInfo = new InstructorContactInfoDTO();
            contactInfo.CountryId = _importHelperService.GetCountryId(sdr["pccode"].ToString());
            contactInfo.CityEng = sdr["pcity"].ToString();
            contactInfo.StreetEng = sdr["pstreet"].ToString();
            contactInfo.AddressEng = sdr["paddress"].ToString();
            contactInfo.CityRus = sdr["pcityrus"].ToString();
            contactInfo.StreetRus = sdr["pstreetrus"].ToString();
            contactInfo.AddressRus = sdr["paddressrus"].ToString();
            contactInfo.ZipCode = sdr["pzip"].ToString();
            contactInfo.Phone = sdr["pph"].ToString();
            contactInfo.CitizenshipCountryId = _importHelperService.GetCountryId(sdr["cccode"].ToString());
            contactInfo.CitizenshipCityEng = sdr["ccity"].ToString();
            contactInfo.CitizenshipStreetEng = sdr["cstreet"].ToString();
            contactInfo.CitizenshipAddressEng = sdr["caddress"].ToString();
            contactInfo.CitizenshipCityRus = sdr["ccityrus"].ToString();
            contactInfo.CitizenshipStreetRus = sdr["cstreetrus"].ToString();
            contactInfo.CitizenshipAddressRus = sdr["caddressrus"].ToString();
            contactInfo.CitizenshipZipCode = sdr["czip"].ToString();
            contactInfo.CitizenshipPhone = sdr["cph"].ToString();
            contactInfo.ContactNameEng = sdr["contact_nameeng"].ToString();
            contactInfo.ContactNameRus = sdr["contact_namerus"].ToString();
            contactInfo.ContactPhone = sdr["contactph"].ToString();
            contactInfo.RelationEng = sdr["relationeng"].ToString();
            contactInfo.RelationRus = sdr["relationrus"].ToString();
            contactInfo.RelationKir = sdr["relationkyr"].ToString();
            contactInfo.InstructorImportCode = int.Parse(sdr["iid"].ToString());

            cashedInstructorContactInfo.Add(contactInfo);
        }

        #endregion

        private void ProcessInstructorMainInfo(NpgsqlDataReader sdr, bool overwrite, 
            int organizationId, bool isMain, int importCode)
        {
            var instructorOrgInfo = _db.InstructorOrgInfo.Include(x => x.InstructorBasicInfo)
                .FirstOrDefault(x => x.InstructorBasicInfo.ImportCode == importCode && x.OrganizationId == organizationId);
            
            if (instructorOrgInfo != null)
            {
                //Temp work around
                var user = _userManager.Users.FirstOrDefault(x => x.Id == instructorOrgInfo.InstructorBasicInfo.InstructorUserId);
                if (instructorOrgInfo.ImportCode == 0) 
                {
                    if (user.LastNameEng.Trim() != sdr["lname"].ToString().Trim())
                        CreateInstructorMainInfo(sdr, organizationId, isMain);
                    else
                    {
                        instructorOrgInfo.ImportCode = instructorOrgInfo.InstructorBasicInfo.ImportCode;
                        _db.InstructorOrgInfo.Update(instructorOrgInfo);
                        _db.SaveChanges();
                    }
                }
                else if (overwrite)
                    EditInstructorMainInfo(sdr, organizationId, instructorOrgInfo.InstructorBasicInfo, isMain);
            }
            else
                CreateInstructorMainInfo(sdr, organizationId, isMain);
        }

        #region Create

        private void CreateInstructorMainInfo(NpgsqlDataReader sdr, int organizationId, bool isMain)
        {
            string email = CreateUser(sdr, organizationId);
            ApplicationUser user = _userManager.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
                throw new Exception("User not found");

            var userBasicInfo = _db.UserBasicInfo.FirstOrDefault(x => x.ApplicationUserId == user.Id);
            if (userBasicInfo == null)
                CreateUserBasicInfo(sdr, organizationId, user.Id);

            CreateInstructorInfo(sdr, organizationId, user.Id, isMain);
        }

        private string CreateUser(NpgsqlDataReader sdr, int organizationId)
        {
            string email = Regex.Replace(sdr["lname"].ToString(), @"[^A-Za-z]", string.Empty);
            email = "_" + new String(email.ToArray()).Split()[0].Trim() + sdr["iid"].ToString() + "@iuca.kg";

            UserInfoViewModel userInfo = _userInfoService.GetUserInfoByEmail(organizationId, email, false);

            if (userInfo == null)
            {
                userInfo = new UserInfoViewModel();
                userInfo.ApplicationUser = new ApplicationUser();
                userInfo.ApplicationUser.IsActive = true;
                userInfo.ApplicationUser.LastNameEng = sdr["lname"].ToString();
                userInfo.ApplicationUser.FirstNameEng = sdr["fname"].ToString();
                userInfo.ApplicationUser.MiddleNameEng = sdr["mname"].ToString();
                userInfo.ApplicationUser.Email = email;
                userInfo.ApplicationUser.UserName = userInfo.ApplicationUser.Email;
                userInfo.IsInstructor = true;
                _userInfoService.Create(organizationId, userInfo, false);
            }
            else 
            {
                userInfo.IsInstructor = true;
                _userInfoService.Edit(organizationId, userInfo.ApplicationUser.Id, userInfo, false);
            }

            return email;
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

        private void CreateInstructorInfo(NpgsqlDataReader sdr, int organizationId, string applicationUserId,
            bool isMain)
        {
            int intstructorImportCode = int.Parse(sdr["iid"].ToString());
            var instructorBasicInfoId = CreateInstructorBasicInfo(sdr, organizationId, applicationUserId, isMain);
            CreateInstructorOrgInfo(sdr, organizationId, instructorBasicInfoId);

            //Should delete existing and create new because there is no import code in tables
            EditInstructorOtherJobInfo(intstructorImportCode, instructorBasicInfoId);
            EditInstructorEducationInfo(intstructorImportCode, instructorBasicInfoId);
            EditInstructorContactInfo(intstructorImportCode, instructorBasicInfoId);
        }

        private int CreateInstructorBasicInfo(NpgsqlDataReader sdr, int organizationId, string applicationUserId,
            bool isMain) 
        {
            var instructorBasicInfo = _db.InstructorBasicInfo.FirstOrDefault(x => x.InstructorUserId == applicationUserId);
            if (instructorBasicInfo != null)
            {
                return instructorBasicInfo.Id;
            }
            else 
            {
                InstructorBasicInfoDTO instructorBasicInfoDTO = new InstructorBasicInfoDTO();
                instructorBasicInfoDTO.InstructorUserId = applicationUserId;
                instructorBasicInfoDTO.IsMainOrganization = isMain;
                instructorBasicInfoDTO.IsMarried = bool.Parse(sdr["married"].ToString());
                instructorBasicInfoDTO.ChildrenQty = int.Parse(sdr["children"].ToString());
                instructorBasicInfoDTO.ImportCode = int.Parse(sdr["iid"].ToString());
                instructorBasicInfoDTO = _instructorBasicInfoService.Create(organizationId, instructorBasicInfoDTO);

                return instructorBasicInfoDTO.Id;
            }
        }

        private void CreateInstructorOrgInfo(NpgsqlDataReader sdr, int organizationId, int instructorBasicInfoId) 
        {
            InstructorOrgInfoDTO instructorOrgInfo = new InstructorOrgInfoDTO();
            instructorOrgInfo.InstructorBasicInfoId = instructorBasicInfoId;
            instructorOrgInfo.OrganizationId = organizationId;
            instructorOrgInfo.DepartmentId =
                _importHelperService.GetDepartmentId(sdr["department"].ToString(), organizationId);
            instructorOrgInfo.State = (int)enu_InstructorState.Active;
            instructorOrgInfo.PartTime = !bool.Parse(sdr["full_part"].ToString());
            instructorOrgInfo.ImportCode = int.Parse(sdr["iid"].ToString());
            _instructorOrgInfoService.Create(instructorOrgInfo);
        }

        #endregion

        #region Edit

        private void EditInstructorMainInfo(NpgsqlDataReader sdr, int organizationId, 
            InstructorBasicInfo instructorBasicInfo, bool isMain)
        {
            var user = _userManager.Users.Include(x => x.UserBasicInfo)
                .FirstOrDefault(x => x.Id == instructorBasicInfo.InstructorUserId);

            if (user == null)
                throw new Exception("User not found");

            EditUserInfo(sdr, organizationId, instructorBasicInfo.InstructorUserId, user);
            if (user.UserBasicInfo != null)
                EditUserBasicInfo(sdr, organizationId, instructorBasicInfo.InstructorUserId, user.UserBasicInfo.Id);
            else
                CreateUserBasicInfo(sdr, organizationId, instructorBasicInfo.InstructorUserId);
            EditInstructorInfo(sdr, organizationId, instructorBasicInfo, isMain);
        }

        private void EditUserInfo(NpgsqlDataReader sdr, int organizationId, string applicationUserId,
            ApplicationUser user)
        {
            /*string email = Regex.Replace(sdr["lname"].ToString(), @"[^A-Za-z]", string.Empty);
            email = "_" + new String(email.ToArray()).Split()[0].Trim() + sdr["iid"].ToString() + "@iuca.kg";*/

            UserInfoViewModel userInfo = _userInfoService.GetUserInfoByEmail(organizationId, user.Email);

            /*userInfo.ApplicationUser.Email = email;
            userInfo.ApplicationUser.UserName = userInfo.ApplicationUser.Email;
            userInfo.ApplicationUser.IsActive = true;*/
            userInfo.ApplicationUser.LastNameEng = sdr["lname"].ToString();
            userInfo.ApplicationUser.FirstNameEng = sdr["fname"].ToString();
            userInfo.ApplicationUser.MiddleNameEng = sdr["mname"].ToString();
            userInfo.IsInstructor = true;

            _userInfoService.Edit(organizationId, applicationUserId, userInfo, false);
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

        private void EditInstructorInfo(NpgsqlDataReader sdr, int organizationId,
                    InstructorBasicInfo instructorBasicInfo, bool isMain)
        {
            int intstructorImportCode = int.Parse(sdr["iid"].ToString());
            EditInstructorBasicInfo(sdr, organizationId, instructorBasicInfo, isMain);
            EditInstructorOrgInfo(sdr, organizationId, instructorBasicInfo);
            EditInstructorOtherJobInfo(intstructorImportCode, instructorBasicInfo.Id);
            EditInstructorEducationInfo(intstructorImportCode, instructorBasicInfo.Id);
            EditInstructorContactInfo(intstructorImportCode, instructorBasicInfo.Id);
        }

        private void EditInstructorBasicInfo(NpgsqlDataReader sdr, int organizationId,
                    InstructorBasicInfo instructorBasicInfo, bool isMain) 
        {
            InstructorBasicInfoDTO instructorBasicInfoDTO = new InstructorBasicInfoDTO();
            instructorBasicInfoDTO.Id = instructorBasicInfo.Id;
            instructorBasicInfoDTO.InstructorUserId = instructorBasicInfo.InstructorUserId;
            instructorBasicInfoDTO.IsMainOrganization = isMain;
            instructorBasicInfoDTO.IsMarried = bool.Parse(sdr["married"].ToString());
            instructorBasicInfoDTO.ChildrenQty = int.Parse(sdr["children"].ToString());
            _instructorBasicInfoService.Edit(organizationId, instructorBasicInfoDTO);
        }

        private void EditInstructorOrgInfo(NpgsqlDataReader sdr, int organizationId,
                    InstructorBasicInfo instructorBasicInfo) 
        {
            InstructorOrgInfoDTO instructorOrgInfo = new InstructorOrgInfoDTO();
            instructorOrgInfo.InstructorBasicInfoId = instructorBasicInfo.Id;
            instructorOrgInfo.OrganizationId = organizationId;
            instructorOrgInfo.DepartmentId =
                _importHelperService.GetDepartmentId(sdr["department"].ToString(), organizationId);
            instructorOrgInfo.State = (int)enu_InstructorState.Active;
            instructorOrgInfo.PartTime = !bool.Parse(sdr["full_part"].ToString());
            instructorOrgInfo.ImportCode = int.Parse(sdr["iid"].ToString());
            _instructorOrgInfoService.Edit(organizationId, instructorOrgInfo);
        }

        private void EditInstructorOtherJobInfo(int instructorImportCode, int instructorBasicInfoId)
        {
            var dbOtherJobInfoList = _db.InstructorOtherJobInfo
                .Where(x => x.InstructorBasicInfoId == instructorBasicInfoId).ToList();
            if (dbOtherJobInfoList.Count > 0) 
            {
                foreach (var dbOtherJobInfo in dbOtherJobInfoList)
                    _instructorOtherJobInfoService.Delete(dbOtherJobInfo.Id);
            }

            var otherJobInfoList = cashedInstructorOtherJobInfo
                .Where(x => x.InstructorImportCode == instructorImportCode).ToList();

            foreach (var instructorOtherJobInfo in otherJobInfoList)
            {
                instructorOtherJobInfo.InstructorBasicInfoId = instructorBasicInfoId;
                _instructorOtherJobInfoService.Create(instructorOtherJobInfo);
            }
        }

        private void EditInstructorEducationInfo(int instructorImportCode, int instructorBasicInfoId)
        {
            var dbEducationInfoList = _db.InstructorEducationInfo
                .Where(x => x.InstructorBasicInfoId == instructorBasicInfoId).ToList();
            if (dbEducationInfoList.Count > 0)
            {
                foreach (var dbEducationInfo in dbEducationInfoList)
                    _instructorEducationInfoService.Delete(dbEducationInfo.Id);
            }

            var educationInfoList = cashedInstructorEducationInfo
                .Where(x => x.InstructorImportCode == instructorImportCode).ToList();

            foreach (var instructoEducationInfo in educationInfoList)
            {
                instructoEducationInfo.InstructorBasicInfoId = instructorBasicInfoId;
                _instructorEducationInfoService.Create(instructoEducationInfo);
            }
        }

        private void EditInstructorContactInfo(int instructorImportCode, int instructorBasicInfoId)
        {
            var dbContactInfoList = _db.InstructorContactInfo
                .Where(x => x.InstructorBasicInfoId == instructorBasicInfoId).ToList();
            if (dbContactInfoList.Count > 0)
            {
                foreach (var dbContactInfo in dbContactInfoList)
                    _instructorContactInfoService.Delete(dbContactInfo.Id);
            }

            var contactInfo = cashedInstructorContactInfo
                .FirstOrDefault(x => x.InstructorImportCode == instructorImportCode);

            if (contactInfo != null)
            {
                contactInfo.InstructorBasicInfoId = instructorBasicInfoId;
                _instructorContactInfoService.Create(contactInfo);
            }
        }

        #endregion
    }
}
