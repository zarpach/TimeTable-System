using AutoMapper;
using iuca.Application.DTO.Common;
using iuca.Application.Interfaces.Common;
using iuca.Domain.Entities.Common;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Claims;
using iuca.Infrastructure.Identity.Entities;
using iuca.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace iuca.Application.Services.Common
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IApplicationDbContext _db;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        public OrganizationService(IApplicationDbContext db, 
            ApplicationUserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Get organization list
        /// </summary>
        /// <returns>Organization list</returns>
        public IEnumerable<OrganizationDTO> GetOrganizations()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Organization, OrganizationDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Organization>, IEnumerable<OrganizationDTO>>(_db.Organizations);
        }

        /// <summary>
        /// Get organization by id
        /// </summary>
        /// <param name="id">Id of organization</param>
        /// <returns>Organization model</returns>
        public OrganizationDTO GetOrganization(int id)
        {
            Organization organization = _db.Organizations.FirstOrDefault(x => x.Id == id);
            if (organization == null)
                throw new Exception($"Organization with id {id} not found");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Organization, OrganizationDTO>()).CreateMapper();
            return mapper.Map<Organization, OrganizationDTO>(organization);
        }

        /// <summary>
        /// Create organization
        /// </summary>
        /// <param name="organizationDTO">Organization model</param>
        public void Create(OrganizationDTO organizationDTO)
        {
            if (organizationDTO == null)
                throw new Exception($"organizationDTO is null");
                
            Organization organization = new Organization();
            organization.Name = organizationDTO.Name;
            organization.IsMain = organizationDTO.IsMain;
            _db.Organizations.Add(organization);
            _db.SaveChanges();
        }

        /// <summary>
        /// Edit organization
        /// </summary>
        /// <param name="id">Id of organization</param>
        /// <param name="organizationDTO">Organization model</param>
        public void Edit(int id, OrganizationDTO organizationDTO)
        {
            if (organizationDTO == null)
                throw new Exception($"organizationDTO is null");

            Organization organization = _db.Organizations.FirstOrDefault(x => x.Id == id);
            if (organization == null)
                throw new Exception($"Organization with id {id} not found");

            organization.Name = organizationDTO.Name;
            organization.IsMain = organizationDTO.IsMain;
            _db.Organizations.Update(organization);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete organization by id
        /// </summary>
        /// <param name="id">Id of organization</param>
        public void Delete(int id)
        {
            Organization organization = _db.Organizations.FirstOrDefault(x => x.Id == id);
            if (organization == null)
                throw new Exception($"Organization with id {id} not found");

            using (var transaction = _db.Database.BeginTransaction()) 
            {
                try
                {
                    _db.Organizations.Remove(organization);
                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex) 
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

 
        /// <summary>
        /// Get selected organization of user
        /// </summary>
        /// <param name="userId">Application user id</param>
        /// <returns>Organization</returns>
        public OrganizationDTO GetUserSelectedOrganization(string userId) 
        {
            return GetUserSelectedOrganization(_userManager.Users.FirstOrDefault(x => x.Id == userId));
        }

        /// <summary>
        /// Get selected organization of user
        /// </summary>
        /// <param name="userId">Application user</param>
        /// <returns>Organization</returns>
        public OrganizationDTO GetUserSelectedOrganization(ApplicationUser applicationUser)
        {
            if (applicationUser == null)
                throw new Exception("User not found");

            OrganizationDTO org = null;
            Claim selectedOrganizationId = GetClaim(applicationUser, CustomClaimTypes.SelectedOrganizationId);
            if (selectedOrganizationId != null)
                org = GetOrganization(int.Parse(selectedOrganizationId.Value));
            
            return org;
        }

        /// <summary>
        /// Change user organization
        /// </summary>
        /// <param name="userId">Application user id</param>
        /// <param name="newOrganizationId">New Organization Id</param>
        public void ChangeUserOrganization(string userId, int newOrganizationId) 
        {
            ChangeUserOrganization(_userManager.Users.FirstOrDefault(x => x.Id == userId), newOrganizationId);
        }

        /// <summary>
        /// Change user organization
        /// </summary>
        /// <param name="applicationUser">Application user</param>
        /// <param name="newOrganizationId">New Organization Id</param>
        public void ChangeUserOrganization(ApplicationUser applicationUser, int newOrganizationId)
        {
            if (applicationUser == null)
                throw new Exception("User not found");

            Claim oldSelectedOrganizationId = GetClaim(applicationUser, CustomClaimTypes.SelectedOrganizationId);
            using (var transaction = _db.Database.BeginTransaction()) 
            {
                try
                {
                    //Remove precious claim
                    if (oldSelectedOrganizationId != null)
                        _userManager.RemoveClaimAsync(applicationUser, oldSelectedOrganizationId).Wait();

                    //Add updated claim
                    Claim newSelectedOrganizationId = new Claim(CustomClaimTypes.SelectedOrganizationId, newOrganizationId.ToString());
                    _userManager.AddClaimAsync(applicationUser, newSelectedOrganizationId).Wait();
                    transaction.Commit();
                }
                catch (Exception ex) 
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        private List<Claim> GetClaims(ApplicationUser applicationUser)
        {
            List<Claim> userClaims = new List<Claim>();
            Task.Run(() => userClaims = _userManager.GetClaimsAsync(applicationUser).GetAwaiter().GetResult().ToList()).Wait();
            return userClaims;
        }

        private Claim GetClaim(ApplicationUser applicationUser, string claimType)
        {
            return GetClaims(applicationUser).FirstOrDefault(x => x.Type == claimType);
        }


        /// <summary>
        /// Get selected organization of user
        /// </summary>
        /// <param name="principal">User claims</param>
        /// <returns>Selected organization id</returns>
        public int GetSelectedOrganization(ClaimsPrincipal principal)
        {
            int selectedOrganization;
            Claim selectedOrganizationClaim = principal.FindFirst(x => x.Type == CustomClaimTypes.SelectedOrganizationId);
            if (selectedOrganizationClaim == null || string.IsNullOrEmpty(selectedOrganizationClaim.Value))
                throw new Exception("Selected organization not defined");

            if (!int.TryParse(selectedOrganizationClaim.Value, out selectedOrganization))
                throw new Exception("Selected organization not defined");

            return selectedOrganization;
        }

        /// <summary>
        /// Get main organization
        /// </summary>
        /// <returns>Organization DTO</returns>
        public OrganizationDTO GetMainOrganization() 
        {
            Organization organization = _db.Organizations.FirstOrDefault(x => x.IsMain);
            if (organization == null)
                throw new Exception($"Main organization not defined");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Organization, OrganizationDTO>()).CreateMapper();
            return mapper.Map<Organization, OrganizationDTO>(organization);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
