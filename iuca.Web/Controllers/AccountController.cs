using iuca.Application.Interfaces.Common;
using iuca.Application.Interfaces.Users;
using iuca.Application.Interfaces.Users.UserInfo;
using iuca.Application.ViewModels;
using iuca.Application.ViewModels.Users;
using iuca.Application.ViewModels.Users.UserInfo;
using iuca.Infrastructure.Identity;
using iuca.Infrastructure.Identity.Claims;
using iuca.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace iuca.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationUserManager<ApplicationUser> _userManager;
        private readonly IUserInfoService _userInfoService;
        private readonly IOrganizationService _organizationService;
        private readonly IUserTypeOrganizationService _userTypeOrganizationService;

        public AccountController(SignInManager<ApplicationUser> signInManager, 
            ApplicationUserManager<ApplicationUser> userManager,
            IUserInfoService userInfoService,
            IOrganizationService organizationService,
            IUserTypeOrganizationService userTypeOrganizationService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userInfoService = userInfoService;
            _organizationService = organizationService;
            _userTypeOrganizationService = userTypeOrganizationService;
        }

        [Authorize]
        public IActionResult AccountInfo() 
        {
            int selectedOrganization = _organizationService.GetSelectedOrganization(User);
            ApplicationUser user = null;
            Task.Run(() => user = _userManager.GetUserAsync(User).GetAwaiter().GetResult()).Wait();
            if (user == null)
                return NotFound();

            AccountInfoViewModel model = _userInfoService.GetUserAccountInfo(selectedOrganization, user.Id);
            
            return View(model);
        }

        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction(nameof(GoogleLogin));

            string email = info.Principal.FindFirst(ClaimTypes.Email).Value;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction(nameof(GoogleLogin));

            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.IsActive)
                return RedirectToAction("AccessDenied");

            if (!_userManager.HasSelectedOrganization(user)) 
            {
                if(!SetUserDefaultSelectedOrganization(user))
                    return RedirectToAction("AccessDenied");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
            {
                IdentityResult identResult = await _userManager.AddLoginAsync(user, info);
                if (identResult.Succeeded)
                {
                    result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
                    if (result.Succeeded)
                        return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("AccessDenied");
        }

        public async Task<IActionResult> GoogleLogout() 
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login() 
        {
            return RedirectToAction(nameof(GoogleLogin));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private bool SetUserDefaultSelectedOrganization(ApplicationUser user) 
        {
            bool success = false;
            var organizations = _userTypeOrganizationService.GetUserOrganizations(user.Id).ToList();
            if (organizations.Count > 0) 
            {
                Task.Run(() => _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.SelectedOrganizationId,
                        organizations[0].Id.ToString())).GetAwaiter().GetResult()).Wait();
                success = true;
            }
                
            return success;
        }
    }
}
