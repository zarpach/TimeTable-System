using iuca.Application.DTO.Users.Instructors;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Enums;
using iuca.Application.Interfaces.ExportData;
using iuca.Application.ViewModels.Common;
using iuca.Application.ViewModels.ExportData;
using iuca.Domain.Entities.Users.Instructors;
using iuca.Domain.Entities.Users.UserInfo;
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

namespace iuca.Application.Services.ExportData
{
    public class ExportInstructorService : IExportInstructorService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public ExportInstructorService(IApplicationDbContext db,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Export instructor info to old DB
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="instructorBasicInfoId">Instructor basic info id</param>
        /// <param name="connection">Connection string</param>
        /// <returns>Instructor import code and result</returns>
        /// <exception cref="Exception"></exception>
        public (int importCode, ResultViewModel result) ExportInstructor(int organizationId, int instructorBasicInfoId,
            string connection)
        {
            int importCode = 0;

            ResultViewModel result = new ResultViewModel(true);
            try
            {
                importCode = Export(organizationId, instructorBasicInfoId, connection);
            }
            catch (Exception ex) 
            {
                result = new ResultViewModel(false, ex.Message);
            }

            return (importCode, result);
        }

        private int Export(int organizationId, int instructorBasicInfoId, string connection) 
        {
            var instructorBasicInfo = _db.InstructorBasicInfo
                    .Include(x => x.InstructorOrgInfo).ThenInclude(x => x.Department)
                    .Include(x => x.InstructorOtherJobInfo)
                    .Include(x => x.InstructorEducationInfo).ThenInclude(x => x.University)
                    .Include(x => x.InstructorEducationInfo).ThenInclude(x => x.EducationType)
                    .Include(x => x.InstructorContactInfo).ThenInclude(x => x.Country)
                    .Include(x => x.InstructorContactInfo).ThenInclude(x => x.CitizenshipCountry)
                    .FirstOrDefault(x => x.Id == instructorBasicInfoId);

            if (instructorBasicInfo == null)
                throw new Exception($"Instructor basic info not found");

            var orgInfo = instructorBasicInfo.InstructorOrgInfo.FirstOrDefault(x => x.OrganizationId == organizationId);
            if (orgInfo == null)
                throw new Exception($"Instructor org info not found");

            var user = _userManager.Users.FirstOrDefault(x => x.Id == instructorBasicInfo.InstructorUserId);
            if (user == null)
                throw new Exception($"User not found");

            var userBasicInfo = _db.UserBasicInfo
                .Include(x => x.Nationality)
                .Include(x => x.Citizenship)
                .FirstOrDefault(x => x.ApplicationUserId == instructorBasicInfo.InstructorUserId);

            CheckImportCodes(userBasicInfo, orgInfo, instructorBasicInfo.InstructorEducationInfo, 
                instructorBasicInfo.InstructorContactInfo);

            var instructorGeneralInfo = GetInstructorGeneralInfo(instructorBasicInfo, user, userBasicInfo, orgInfo);

            int importCode = 0;

            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                conn.Open();
                if (orgInfo.ImportCode == 0)
                {
                    importCode = CreateInstructorInfo(instructorGeneralInfo, instructorBasicInfo, orgInfo, conn);
                }
                else
                {
                    importCode = orgInfo.ImportCode;
                    ProcessInstructorInfo(importCode, instructorGeneralInfo, instructorBasicInfo, conn);
                    instructorBasicInfo.IsChanged = false;
                    _db.SaveChanges();
                }
            }

            return importCode;
        }

        private void CheckImportCodes(UserBasicInfo userBasicInfo, InstructorOrgInfo orgInfo,
            List<InstructorEducationInfo> educationInfoList, InstructorContactInfo contactInfo) 
        {
            if (orgInfo.Department.ImportCode == 0)
                throw new Exception("Department import code is wrong");

            if (userBasicInfo != null) 
            {
                if (userBasicInfo.Nationality?.ImportCode == 0) 
                    throw new Exception("Nationality import code is wrong");

                if (userBasicInfo.Citizenship?.ImportCode == 0)
                    throw new Exception("Citizenship import code is wrong");
            }

            if (contactInfo != null) 
            {
                if (contactInfo.Country?.ImportCode == 0)
                    throw new Exception("Contact country import code is wrong");

                if (contactInfo.CitizenshipCountry?.ImportCode == 0)
                    throw new Exception("Contact citizenship country import code is wrong");
            }

            if (educationInfoList != null && educationInfoList.Count > 0) 
            {
                foreach (var educationInfo in educationInfoList) 
                {
                    if (educationInfo.University?.ImportCode == 0)
                        throw new Exception("University import code is wrong");

                    if (educationInfo.EducationType?.ImportCode == 0)
                        throw new Exception("Education type import code is wrong");
                }
            }
        }

        private int CreateInstructorInfo(ExportInstructorGeneralnfoViewModel instructorGeneralInfo, 
            InstructorBasicInfo instructorBasicInfo, InstructorOrgInfo orgInfo, NpgsqlConnection conn) 
        {
            //Create new instructor general info 
            int importCode = InsertInstructorGeneralInfo(instructorGeneralInfo, conn);
            if (importCode == 0)
                throw new Exception("New import code was not set");

            //Set new import code
            SetImportCodeToInstructorOrgInfo(orgInfo, importCode);

            InsertOtherJobInfo(importCode, instructorBasicInfo.InstructorOtherJobInfo, conn);
            InsertEducationInfo(importCode, instructorBasicInfo.InstructorEducationInfo, conn);
            InsertContactInfo(importCode, instructorBasicInfo.InstructorContactInfo, conn);

            return importCode;
        }

        private void ProcessInstructorInfo(int importCode, ExportInstructorGeneralnfoViewModel instructorGeneralInfo,
            InstructorBasicInfo instructorBasicInfo, NpgsqlConnection conn)
        {
            ProcessInstructorGeneralInfo(importCode, instructorGeneralInfo, conn);
            ProcessInstructorOtherJobInfo(importCode, instructorBasicInfo.InstructorOtherJobInfo, conn);
            ProcessInstructorEducationInfo(importCode, instructorBasicInfo.InstructorEducationInfo, conn);
            ProcessInstructorContactInfo(importCode, instructorBasicInfo.InstructorContactInfo, conn);
        }

        #region General info

        private ExportInstructorGeneralnfoViewModel GetInstructorGeneralInfo(InstructorBasicInfo instrucorBasicInfo,
            ApplicationUser user, UserBasicInfo userBasicInfo, InstructorOrgInfo orgInfo)
        {
            ExportInstructorGeneralnfoViewModel model = new ExportInstructorGeneralnfoViewModel();
            model.ImportCode = orgInfo.ImportCode;
            model.LastNameEng = user.LastNameEng;
            model.FirstNameEng = user.FirstNameEng;
            model.MiddleNameEng = user.MiddleNameEng;

            if (userBasicInfo != null)
            {
                model.LastNameRus = userBasicInfo.LastNameRus;
                model.FirstNameRus = userBasicInfo.FirstNameRus;
                model.MiddleNameRus = userBasicInfo.MiddleNameRus;
                model.Sex = (enu_Sex)userBasicInfo.Sex == enu_Sex.Male;
                model.DateOfBirth = userBasicInfo.DateOfBirth.ToString("yyyy-MM-dd");
                model.NationalityImportCode = userBasicInfo.Nationality?.ImportCode;
                model.CitizenshipImportCode = userBasicInfo.Citizenship?.ImportCode;
            }

            model.IsMarried = instrucorBasicInfo.IsMarried;
            model.ChildrenQty = instrucorBasicInfo.ChildrenQty;
            model.Status = "f";
            model.DepartmentImportCode = orgInfo.Department?.ImportCode;
            model.FullPart = !orgInfo.PartTime;

            return model;
        }

        private void ProcessInstructorGeneralInfo(int importCode, ExportInstructorGeneralnfoViewModel instructorGeneralInfo, 
            NpgsqlConnection conn) 
        {
            if (!GeneralInfoExists(importCode, conn))
                InsertInstructorGeneralInfo(instructorGeneralInfo, conn);
            else
                UpdateInstructorGeneralInfo(importCode, instructorGeneralInfo, conn);
        }

        private int InsertInstructorGeneralInfo(ExportInstructorGeneralnfoViewModel instructorGeneralInfo,
            NpgsqlConnection conn) 
        {
            var newImportCode = GetLastIid(conn)+1;
            string query = InsertGeneralInfoQuery(newImportCode, instructorGeneralInfo);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                if (cmd.ExecuteNonQuery() <= 0)
                    throw new Exception("General info was not created");
            }

            return newImportCode;
        }

        private int GetLastIid(NpgsqlConnection conn)
        {
            int iid = 0;
            string query = SelectLastIidQuery();
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        sdr.Read();
                        iid = int.Parse(sdr["max"].ToString());
                    }
                }
            }

            return iid;
        }

        private void SetImportCodeToInstructorOrgInfo(InstructorOrgInfo orgInfo, int importCode)
        {
            orgInfo.ImportCode = importCode;
            _db.SaveChanges();
        }

        private void UpdateInstructorGeneralInfo(int instructorImportCode, 
            ExportInstructorGeneralnfoViewModel instructorGeneralInfo, NpgsqlConnection conn)
        {
            string query = UpdateGeneralInfoQuery(instructorImportCode, instructorGeneralInfo);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private bool GeneralInfoExists(int instructorImportCode, NpgsqlConnection conn)
        {
            bool exists = false;
            string query = SelectGeneralInfoQuery(instructorImportCode);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                        exists = true;
                }
            }

            return exists;
        }

        #endregion

        #region Other job info

        private void ProcessInstructorOtherJobInfo(int instructorImportCode, List<InstructorOtherJobInfo> otherJobInfoList,
            NpgsqlConnection conn)
        {
            //Get oids of existing rows in old DB
            List<string> oidList = GetOtherJobInfoOids(instructorImportCode, conn);

            foreach (var otherJobInfo in otherJobInfoList)
            {
                if (oidList.Count > 0)
                {
                    //Update each existing row of old DB
                    UpdateOtherJobInfo(oidList[0], otherJobInfo, conn);
                    oidList.Remove(oidList[0]);
                }
                else //Insert new row if there are no rows in old DB
                    InsertOtherJobInfo(instructorImportCode, otherJobInfo, conn);
            }

            //Delete the rest old DB rows 
            if (oidList.Count > 0)
            {
                DeleteOtherJobInfo(oidList, conn);
            }
        }

        private List<string> GetOtherJobInfoOids(int instructorImportCode, NpgsqlConnection conn)
        {
            List<string> oidList = new List<string>();
            string query = SelectOtherJobInfoQuery(instructorImportCode);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            oidList.Add(sdr["oid"].ToString());
                        }
                    }
                }
            }

            return oidList;
        }

        private void InsertOtherJobInfo(int instructorImportCode, List<InstructorOtherJobInfo> otherJobInfoList, 
            NpgsqlConnection conn) 
        {
            if (otherJobInfoList != null && otherJobInfoList.Count > 0)
            {
                foreach (var otherJobInfo in otherJobInfoList)
                {
                    InsertOtherJobInfo(instructorImportCode, otherJobInfo, conn);
                }
            }
        }

        private void InsertOtherJobInfo(int instructorImportCode, InstructorOtherJobInfo otherJobInfo, 
            NpgsqlConnection conn)
        {
            string query = InsertOtherJobInfoQuery(instructorImportCode, otherJobInfo);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateOtherJobInfo(string oid, InstructorOtherJobInfo otherJobInfo, NpgsqlConnection conn)
        {
            string query = UpdateOtherJobInfoQuery(oid, otherJobInfo);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteOtherJobInfo(List<string> oidList, NpgsqlConnection conn)
        {
            foreach (var oid in oidList)
            {
                DeleteOtherJobInfo(oid, conn);
            }
        }

        private void DeleteOtherJobInfo(string oid, NpgsqlConnection conn)
        {
            string query = DeleteOtherJobInfoQuery(oid);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Education info

        private void ProcessInstructorEducationInfo(int instructorImportCode, List<InstructorEducationInfo> educationInfoList,
            NpgsqlConnection conn)
        {
            //Get oids of existing rows in old DB
            List<string> oidList = GetEducationInfoOids(instructorImportCode, conn);

            foreach (var educationInfo in educationInfoList)
            {
                if (oidList.Count > 0)
                {
                    //Update each existing row of old DB
                    UpdateEducationInfo(oidList[0], educationInfo, conn);
                    oidList.Remove(oidList[0]);
                }
                else //Insert new row if there are no rows in old DB
                    InsertEducationInfo(instructorImportCode, educationInfo, conn);
            }

            //Delete the rest old DB rows 
            if (oidList.Count > 0)
            {
                DeleteEducationInfo(oidList, conn);
            }
        }

        private List<string> GetEducationInfoOids(int instructorImportCode, NpgsqlConnection conn)
        {
            List<string> oidList = new List<string>();
            string query = SelectEducationInfoQuery(instructorImportCode);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            oidList.Add(sdr["oid"].ToString());
                        }
                    }
                }
            }

            return oidList;
        }

        private void InsertEducationInfo(int instructorImportCode, List<InstructorEducationInfo> educationInfoList,
            NpgsqlConnection conn)
        {
            if (educationInfoList != null && educationInfoList.Count > 0)
            {
                foreach (var educationInfo in educationInfoList)
                {
                    InsertEducationInfo(instructorImportCode, educationInfo, conn);
                }
            }
        }

        private void InsertEducationInfo(int instructorImportCode, InstructorEducationInfo educationInfo,
            NpgsqlConnection conn)
        {
            string query = InsertEducationInfoQuery(instructorImportCode, educationInfo);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateEducationInfo(string oid, InstructorEducationInfo educationInfo, NpgsqlConnection conn)
        {
            string query = UpdateEducationInfoQuery(oid, educationInfo);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteEducationInfo(List<string> oidList, NpgsqlConnection conn)
        {
            foreach (var oid in oidList)
            {
                DeleteEducationInfo(oid, conn);
            }
        }

        private void DeleteEducationInfo(string oid, NpgsqlConnection conn)
        {
            string query = DeleteEducationInfoQuery(oid);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Contact info

        private void ProcessInstructorContactInfo(int instructorImportCode, InstructorContactInfo contactInfo,
            NpgsqlConnection conn) 
        {
            if (ContactInfoExists(instructorImportCode, conn))
                UpdateContactInfo(instructorImportCode, contactInfo, conn);
            else 
                InsertContactInfo(instructorImportCode, contactInfo, conn);
        }

        private bool ContactInfoExists(int instructorImportCode, NpgsqlConnection conn)
        {
            bool exists = false;
            string query = SelectContactInfoQuery(instructorImportCode);
            using (NpgsqlCommand cmd = new NpgsqlCommand(query))
            {
                cmd.Connection = conn;
                using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                        exists = true;
                }
            }

            return exists;
        }

        private void InsertContactInfo(int instructorImportCode, InstructorContactInfo contactInfo,
            NpgsqlConnection conn)
        {
            if (contactInfo != null)
            {
                string query = InsertContactInfoQuery(instructorImportCode, contactInfo);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateContactInfo(int instructorImportCode, InstructorContactInfo contactInfo,
            NpgsqlConnection conn)
        {
            if (contactInfo != null)
            {
                string query = UpdateContactInfoQuery(instructorImportCode, contactInfo);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Queries

        private string SelectGeneralInfoQuery(int importCode)
        {
            return $"SELECT 1 FROM auca.instructor_general_info WHERE iid = {importCode}";
        }

        private string InsertGeneralInfoQuery(int importCode, ExportInstructorGeneralnfoViewModel generalInfo)
        {
            var querySB = new StringBuilder("INSERT INTO auca.instructor_general_info ");
            querySB.Append("(iid, lname, fname, mname, lnamerus, fnamerus, mnamerus, sex, dob, ncode, married, ");
            querySB.Append("children, ccode, status, department, full_part) ");
            querySB.Append($"VALUES ({importCode},'{generalInfo.LastNameEng}','{generalInfo.FirstNameEng}',");
            querySB.Append($"'{generalInfo.MiddleNameEng}','{generalInfo.LastNameRus}','{generalInfo.FirstNameRus}',");
            querySB.Append($"'{generalInfo.MiddleNameRus}',{generalInfo.Sex}, '{generalInfo.DateOfBirth}',");
            querySB.Append($"{generalInfo.NationalityImportCode},{generalInfo.IsMarried},{generalInfo.ChildrenQty},");
            querySB.Append($"{generalInfo.CitizenshipImportCode},'{generalInfo.Status}',");
            querySB.Append($"{generalInfo.DepartmentImportCode},{generalInfo.FullPart})");
            return querySB.ToString();
        }

        private string UpdateGeneralInfoQuery(int instructorImportCode, ExportInstructorGeneralnfoViewModel generalInfo)
        {
            var querySB = new StringBuilder("UPDATE auca.instructor_general_info ");
            querySB.Append($"SET lname = '{generalInfo.LastNameEng}', fname = '{generalInfo.FirstNameEng}', mname = '{generalInfo.MiddleNameEng}', ");
            querySB.Append($"lnamerus = '{generalInfo.LastNameRus}', fnamerus = '{generalInfo.FirstNameRus}', mnamerus = '{generalInfo.MiddleNameRus}', ");
            querySB.Append($"sex = {generalInfo.Sex}, dob = '{generalInfo.DateOfBirth}', ncode = {(generalInfo.NationalityImportCode != null ? generalInfo.NationalityImportCode : "NULL")}, ");
            querySB.Append($"married = {generalInfo.IsMarried}, children = {generalInfo.ChildrenQty}, ccode = {(generalInfo.CitizenshipImportCode != null ? generalInfo.CitizenshipImportCode : "NULL")}, ");
            querySB.Append($"status = '{generalInfo.Status}', department = {generalInfo.DepartmentImportCode}, full_part = {generalInfo.FullPart} ");
            querySB.Append($"WHERE iid = {instructorImportCode}");
            
            return querySB.ToString();
        }

        private string SelectLastIidQuery()
        {
            return $"SELECT MAX(iid) FROM auca.instructor_general_info";
        }

        private string SelectOtherJobInfoQuery(int importCode)
        {
            return $"SELECT oid FROM auca.instructor_otherjob_info WHERE iid = {importCode}";
        }

        private string InsertOtherJobInfoQuery(int instructorImportCode, InstructorOtherJobInfo otherJobInfo)
        {
            var querySB = new StringBuilder("INSERT INTO auca.instructor_otherjob_info ");
            querySB.Append("(iid, placeeng, placerus, placekyr, positioneng, positionrus, positionkyr, ph) ");
            querySB.Append($"VALUES ({instructorImportCode},'{otherJobInfo.PlaceNameEng}','{otherJobInfo.PlaceNameRus}',");
            querySB.Append($"'{otherJobInfo.PlaceNameKir}','{otherJobInfo.PositionEng}','{otherJobInfo.PositionRus}',");
            querySB.Append($"'{otherJobInfo.PositionKir}','{otherJobInfo.Phone}')");

            return querySB.ToString();
        }

        private string UpdateOtherJobInfoQuery(string oid, InstructorOtherJobInfo otherJobInfo)
        {
            var querySB = new StringBuilder("UPDATE auca.instructor_otherjob_info ");
            querySB.Append($"SET placeeng = '{otherJobInfo.PlaceNameEng}', placerus = '{otherJobInfo.PlaceNameRus}', ");
            querySB.Append($"placekyr = '{otherJobInfo.PlaceNameKir}', positioneng = '{otherJobInfo.PositionEng}', ");
            querySB.Append($"positionrus = '{otherJobInfo.PositionRus}', positionkyr = '{otherJobInfo.PositionKir}', ");
            querySB.Append($"ph = '{otherJobInfo.Phone}' ");
            querySB.Append($"WHERE oid = {oid}");

            return querySB.ToString();
        }

        private string DeleteOtherJobInfoQuery(string oid)
        {
            return $"DELETE FROM auca.instructor_otherjob_info WHERE oid = {oid}";
        }

        private string SelectEducationInfoQuery(int importCode)
        {
            return $"SELECT oid FROM auca.instructor_education_info WHERE iid = {importCode}";
        }

        private string InsertEducationInfoQuery(int instructorImportCode, InstructorEducationInfo educationInfo)
        {
            var querySB = new StringBuilder("INSERT INTO auca.instructor_education_info ");
            querySB.Append("(iid, unicode, majoreng, majorrus, majorkyr, dograd, degree) ");
            querySB.Append($"VALUES ({instructorImportCode},{(educationInfo.University != null ? educationInfo.University.ImportCode : "NULL")},'{educationInfo.MajorEng}',");
            querySB.Append($"'{educationInfo.MajorRus}','{educationInfo.MajorKir}',{educationInfo.GraduateYear},");
            querySB.Append($"{(educationInfo.EducationType != null ? educationInfo.EducationType.ImportCode : "NULL")})");

            return querySB.ToString();
        }

        private string UpdateEducationInfoQuery(string oid, InstructorEducationInfo educationInfo)
        {
            var querySB = new StringBuilder("UPDATE auca.instructor_education_info ");
            querySB.Append($"SET unicode = {(educationInfo.University != null ? educationInfo.University.ImportCode : "NULL")}, majoreng = '{educationInfo.MajorEng}', ");
            querySB.Append($"majorrus = '{educationInfo.MajorRus}', majorkyr = '{educationInfo.MajorKir}', ");
            querySB.Append($"dograd = {educationInfo.GraduateYear}, degree = {(educationInfo.EducationType != null ? educationInfo.EducationType.ImportCode : "NULL")} ");
            querySB.Append($"WHERE oid = {oid}");

            return querySB.ToString();
        }

        private string DeleteEducationInfoQuery(string oid)
        {
            return $"DELETE FROM auca.instructor_education_info WHERE oid = {oid}";
        }

        private string SelectContactInfoQuery(int importCode)
        {
            return $"SELECT 1 FROM auca.instructor_contact_info WHERE iid = {importCode}";
        }

        private string InsertContactInfoQuery(int instructorImportCode, InstructorContactInfo contactInfo)
        {
            var querySB = new StringBuilder("INSERT INTO auca.instructor_contact_info ");
            querySB.Append("(iid, pccode, pcity, pstreet, paddress, pcityrus, pstreetrus, paddressrus, pzip, pph, ");
            querySB.Append("cccode, ccity, cstreet, caddress, ccityrus, cstreetrus, caddressrus, czip, cph, ");
            querySB.Append("contact_nameeng, contact_namerus, contactph, relationeng, relationrus, relationkyr) ");
            querySB.Append($"VALUES ({instructorImportCode},{(contactInfo.Country != null ? contactInfo.Country.ImportCode : "NULL")},'{contactInfo.CityEng}',");
            querySB.Append($"'{contactInfo.StreetEng}','{contactInfo.AddressEng}','{contactInfo.CityRus}',");
            querySB.Append($"'{contactInfo.StreetRus}','{contactInfo.AddressRus}','{contactInfo.ZipCode}','{contactInfo.Phone}',");
            querySB.Append($"{(contactInfo.CitizenshipCountry != null ? contactInfo.CitizenshipCountry.ImportCode : "NULL")},'{contactInfo.CitizenshipCityEng}',");
            querySB.Append($"'{contactInfo.CitizenshipStreetEng}','{contactInfo.CitizenshipAddressEng}','{contactInfo.CitizenshipCityRus}',");
            querySB.Append($"'{contactInfo.CitizenshipStreetRus}','{contactInfo.CitizenshipAddressRus}','{contactInfo.CitizenshipZipCode}',");
            querySB.Append($"'{contactInfo.CitizenshipPhone}','{contactInfo.ContactNameEng}','{contactInfo.ContactNameRus}',");
            querySB.Append($"'{contactInfo.ContactPhone}','{contactInfo.RelationEng}','{contactInfo.RelationRus}','{contactInfo.RelationKir}')");

            return querySB.ToString();
        }

        private string UpdateContactInfoQuery(int importCode, InstructorContactInfo contactInfo)
        {
            var querySB = new StringBuilder("UPDATE auca.instructor_contact_info ");
            querySB.Append($"SET pccode = {(contactInfo.Country != null ? contactInfo.Country.ImportCode : "NULL")}, pcity = '{contactInfo.CityEng}', pstreet = '{contactInfo.StreetEng}', ");
            querySB.Append($"paddress = '{contactInfo.AddressEng}', pcityrus = '{contactInfo.CityRus}', pstreetrus = '{contactInfo.StreetRus}', ");
            querySB.Append($"paddressrus = '{contactInfo.AddressRus}', pzip = '{contactInfo.ZipCode}', pph = '{contactInfo.Phone}', ");
            querySB.Append($"cccode = {(contactInfo.CitizenshipCountry != null ? contactInfo.CitizenshipCountry.ImportCode : "NULL")}, ccity = '{contactInfo.CitizenshipCityEng}', ");
            querySB.Append($"cstreet = '{contactInfo.CitizenshipStreetEng}', caddress = '{contactInfo.CitizenshipAddressEng}', ");
            querySB.Append($"ccityrus = '{contactInfo.CitizenshipCityRus}', cstreetrus = '{contactInfo.CitizenshipStreetRus}', ");
            querySB.Append($"caddressrus = '{contactInfo.CitizenshipAddressRus}', czip = '{contactInfo.CitizenshipZipCode}', cph = '{contactInfo.CitizenshipPhone}', ");
            querySB.Append($"contact_nameeng = '{contactInfo.ContactNameEng}', contact_namerus = '{contactInfo.ContactNameRus}', ");
            querySB.Append($"contactph = '{contactInfo.ContactPhone}', relationeng = '{contactInfo.RelationEng}', ");
            querySB.Append($"relationrus = '{contactInfo.RelationRus}', relationkyr = '{contactInfo.RelationKir}'");
            querySB.Append($"WHERE iid = {importCode}");

            return querySB.ToString();
        }

        #endregion
    }
}
