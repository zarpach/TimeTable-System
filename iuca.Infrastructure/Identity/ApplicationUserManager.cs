using iuca.Infrastructure.Identity.Claims;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace iuca.Infrastructure.Identity
{
    public class ApplicationUserManager<TUser> : UserManager<ApplicationUser> where TUser : class
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store,
                                        IOptions<IdentityOptions> optionsAccessor,
                                        IPasswordHasher<ApplicationUser> passwordHasher,
                                        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
                                        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
                                        ILookupNormalizer keyNormalizer,
                                        IdentityErrorDescriber errors,
                                        IServiceProvider services,
                                        ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        
        /// <summary>
        /// Set user active or not active
        /// </summary>
        /// <param name="userId">ApplicationUser Id</param>
        /// <param name="isActive">true - active, false - not active</param>
        /// <returns></returns>
        public async Task<IdentityResult> SetUserActive(string userId, bool isActive)
        {
            ApplicationUser user = Users.SingleOrDefault(x => x.Id == userId);
            if (user != null)
            {
                user.IsActive = isActive;
                return await UpdateAsync(user);
            }
            else
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        }

        /// <summary>
        /// Get full name of user from basic user info
        /// </summary>
        /// <param name="principal">User claims</param>
        /// <returns>Full name</returns>
        public string GetUserFullName(ClaimsPrincipal principal)
        {
            string userId = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = Users.SingleOrDefault(x => x.Id == userId);
            if (user == null)
                throw new Exception("User not found");

            return user.FullNameEng;
        }

        /// <summary>
        /// Check if selected organization assigned. Tries to assign default organization if not
        /// </summary>
        /// <param name="userId">Application user Id</param>
        /// <returns></returns>
        public bool HasSelectedOrganization(ApplicationUser user)
        {
            List<Claim> claimList = new List<Claim>();
            Task.Run(() => claimList = GetClaimsAsync(user).GetAwaiter().GetResult().ToList()).Wait();

            return claimList.Any(x => x.Type == CustomClaimTypes.SelectedOrganizationId);
        }

        /// <summary>
        /// Get user full name eng by user id
        /// </summary>
        /// <param name="applicationUserId">Application user id</param>
        /// <returns>User full name eng</returns>
        public string GetUserFullName(string applicationUserId) 
        {
            string fullName = "";
            var user = Users.FirstOrDefault(x => x.Id == applicationUserId);
            if (user != null) 
                fullName = user.FullNameEng;

            return fullName;
        }

    }
}
