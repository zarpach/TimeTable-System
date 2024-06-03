using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using iuca.Application.Interfaces.ImportData;
using System.IO;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using iuca.Application.Interfaces.Common;
using Microsoft.Extensions.Logging;

namespace iuca.Application.Services.ImportData
{
    public class ImportStudentEmailService : IImportStudentEmailService
    {
        private readonly IApplicationDbContext _db;
        private readonly IOrganizationService _organizationService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly ILogger<ImportStudentEmailService> _logger;

        public ImportStudentEmailService(IApplicationDbContext db,
            IOrganizationService organizationService,
            ApplicationUserManager<ApplicationUser> userManager,
            ILogger<ImportStudentEmailService> logger) 
        {
            _db = db;
            _organizationService = organizationService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Update students emails by csv file
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="fileStream">File stream</param>
        public void UpdateStudentEmails(int organizationId, Stream fileStream) 
        {
            var organization = _organizationService.GetOrganization(organizationId);

            using (TextFieldParser csvReader = new TextFieldParser(fileStream))
            {
                int fileOrganization;
                int sid;
                string email;
                string[] fields;

                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;
                bool columnsPassed = false;
                while (columnsPassed == false)
                {
                    csvReader.ReadFields();
                    columnsPassed = true;
                }
                while (!csvReader.EndOfData)
                {
                    fields = csvReader.ReadFields();

                    fileOrganization = int.Parse(fields[0]);
                    if (fileOrganization == 13 && organization.IsMain)
                        continue;

                    sid = int.Parse(fields[1]);
                    email = fields[4];
                    if (string.IsNullOrEmpty(email))
                        continue;

                    ProcessStudentEmail(organizationId, sid, email);
                }
            }
        }

        private void ProcessStudentEmail(int organizationId, int studentId, string email) 
        {
            var studentOrgInfo = _db.StudentOrgInfo.Include(x => x.StudentBasicInfo)
                .FirstOrDefault(x => x.OrganizationId == organizationId && x.StudentId == studentId);
            if (studentOrgInfo == null)
                _logger.Log(LogLevel.Information, $"Student with id {studentId} not found in organization with id {organizationId}");
            else
                EditApplicationUser(email, studentOrgInfo.StudentBasicInfo.ApplicationUserId);
        }

        private void EditApplicationUser(string email, string applicationUserId)
        {
            ApplicationUser user = _userManager.Users.FirstOrDefault(x => x.Id == applicationUserId);
            if (user != null)
            {
                if (user.Email != email)
                {
                    user.UserName = email;
                    user.Email = email;

                    IdentityResult result = null;
                    Task.Run(() => result = _userManager.UpdateAsync(user).GetAwaiter().GetResult()).Wait();
                    if (result == null || !result.Succeeded)
                    {
                        throw new Exception("User cannot be updated:\n" +
                            (result != null ? string.Join(",", result.Errors.Select(x => x.Description).ToList()) : ""));
                    }
                }
            }
        }
    }
}
