using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iuca.Infrastructure.Identity.Claims
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            //((ClaimsIdentity)principal.Identity).AddClaim(new Claim("ProjectReader", "true"));
            return Task.FromResult(principal);
        }
    }
}
