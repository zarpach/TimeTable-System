using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.DTO.Users.UserInfo;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Persistence;
using System;
using System.Linq;
using System.Text.Json;
using iuca.Application.DTO.Users.Students;
using Microsoft.EntityFrameworkCore;
using static iuca.Application.Constants.Permissions;

namespace iuca.Application.Services.Common
{
    public class EnvarSettingService : IEnvarSettingService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;

        public EnvarSettingService(IApplicationDbContext db,
            IMapper mapper,
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Get EnvarSettings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>EnvarSettings model. Creates new if does not exist</returns>
        public EnvarSettingDTO GetEnvarSettings(int organizationId) 
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.OrganizationId == organizationId);
            if (settings == null) 
            {
                EnvarSetting newEnvarSettings = new EnvarSetting();
                newEnvarSettings.OrganizationId = organizationId;
                _db.EnvarSettings.Add(newEnvarSettings);
                _db.SaveChanges();

                settings = newEnvarSettings;
            }

            return _mapper.Map<EnvarSettingDTO>(settings);
        }

        /// <summary>
        /// Get maximum registration settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns> Maximum registration credits </returns>
        public int GetMaxRegistrationCredits(int organizationId)
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.OrganizationId == organizationId);
            if (settings == null)
                throw new Exception($"Settings not found");

            return settings.MaxRegistrationCredits;
        }

        /// <summary>
        /// Set maximum registration settings
        /// </summary>
        /// <param name="id">Settings id</param>
        /// <param name="maxRegistrationCredits">Max registrstion credits value</param>
        public void SetMaxRegistrationCredits(int id, int maxRegistrationCredits) 
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.Id == id);

            if (settings == null)
                throw new Exception($"Settings with id {id} not found");

            settings.MaxRegistrationCredits = maxRegistrationCredits;
            _db.SaveChanges();
        }

        /// <summary>
        /// Get default instructor settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Default instructor</returns>
        public UserDTO GetDefaultInstructor(int organizationId)
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.OrganizationId == organizationId);
            if (settings == null)
                throw new Exception($"Settings not found");

            var instructor = new UserDTO
            {
                Id = settings.DefaultInstructor,
                FullName = _userManager.GetUserFullName(settings.DefaultInstructor)
            };

            return instructor;
        }

        /// <summary>
        /// Set default instructor settings
        /// </summary>
        /// <param name="id">Settings id</param>
        /// <param name="defaultInstructorId">Default instructor id value</param>
        public void SetDefaultInstructor(int id, string defaultInstructorId)
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.Id == id);

            if (settings == null)
                throw new Exception($"Settings with id {id} not found");

            settings.DefaultInstructor = defaultInstructorId;
            _db.SaveChanges();
        }

        /// <summary>
        /// Get current semester settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Current semester</returns>
        public int GetCurrentSemester(int organizationId)
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.OrganizationId == organizationId);
            if (settings == null)
                throw new Exception($"Settings not found");

            return settings.CurrentSemester;
        }

        /// <summary>
        /// Set current semester settings
        /// </summary>
        /// <param name="id">Settings id</param>
        /// <param name="semesterId">Current semester id value</param>
        public void SetCurrentSemester(int id, int semesterId)
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.Id == id);

            if (settings == null)
                throw new Exception($"Settings with id {id} not found");

            settings.CurrentSemester = semesterId;
            _db.SaveChanges();
        }

        /// <summary>
        /// Get upcoming semester settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Upcoming semester</returns>
        public int GetUpcomingSemester(int organizationId)
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.OrganizationId == organizationId);
            if (settings == null)
                throw new Exception($"Settings not found");

            return settings.UpcomingSemester;
        }

        /// <summary>
        /// Set upcoming semester settings
        /// </summary>
        /// <param name="id">Settings id</param>
        /// <param name="semesterId">Upcoming semester id value</param>
        public void SetUpcomingSemester(int id, int semesterId)
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.Id == id);

            if (settings == null)
                throw new Exception($"Settings with id {id} not found");

            settings.UpcomingSemester = semesterId;
            _db.SaveChanges();
        }

        /// <summary>
        /// Get last attendance update settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <returns>Upcoming semester</returns>
        public DateTime GetLastAttendanceUpdate(int organizationId)
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.OrganizationId == organizationId);
            if (settings == null)
                throw new Exception($"Settings not found");

            return settings.LastAttendanceUpdate;
        }

        /// <summary>
        /// Set last attendance update settings
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="lastAttendanceUpdate">Last attendance update date and time</param>
        public void SetLastAttendanceUpdate(int organizationId, DateTime lastAttendanceUpdate)
        {
            var settings = _db.EnvarSettings.FirstOrDefault(x => x.OrganizationId == organizationId);
            if (settings == null)
                throw new Exception($"Settings not found");

            settings.LastAttendanceUpdate = lastAttendanceUpdate;
            _db.SaveChanges();
        }
    }
}
