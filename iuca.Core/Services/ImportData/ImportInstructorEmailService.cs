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
    public class ImportInstructorEmailService : IImportInstructorEmailService
    {
        private readonly IApplicationDbContext _db;
        private readonly IOrganizationService _organizationService;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly ILogger<ImportInstructorEmailService> _logger;

        public ImportInstructorEmailService(IApplicationDbContext db,
            IOrganizationService organizationService,
            ApplicationUserManager<ApplicationUser> userManager,
            ILogger<ImportInstructorEmailService> logger) 
        {
            _db = db;
            _organizationService = organizationService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Update instructors emails by csv file
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="fileStream">File stream</param>
        public void UpdateInstructorEmails(int organizationId, Stream fileStream) 
        {
            var organization = _organizationService.GetOrganization(organizationId);

            using (TextFieldParser csvReader = new TextFieldParser(fileStream))
            {
                int iid;
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
                    email = fields[4];
                    if (string.IsNullOrEmpty(email))
                        continue;

                    iid = int.Parse(fields[0]);
                    
                    ProcessInstructorEmail(organizationId, iid, email);
                }
            }
        }

        private void ProcessInstructorEmail(int organizationId, int iid, string email) 
        {
            var instructorOrgInfo = _db.InstructorOrgInfo.Include(x => x.InstructorBasicInfo)
                .FirstOrDefault(x => x.OrganizationId == organizationId && x.InstructorBasicInfo.ImportCode == iid);
            if (instructorOrgInfo == null)
                _logger.Log(LogLevel.Information, $"Instructor not found in organization with id {organizationId}");
            else
                EditApplicationUser(email, instructorOrgInfo.InstructorBasicInfo.InstructorUserId);
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
