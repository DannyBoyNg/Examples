using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalrWindowsAuthenticationApi.WindowsAuth
{
    //This class handles authorization
    public class ClaimsTransformer : IClaimsTransformation
    {
        public IConfiguration Configuration { get; }

        public ClaimsTransformer(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal p)
        {
            //Get authorized users
            var authorizedUsers = Configuration.GetSection("AuthorizedUsers").Get<string[]>();

            //Check if user is authorized
            if (authorizedUsers?.Contains(p.Identity?.Name) ?? false)
            {
                //If the user is authorized add a claimsIdentity with any number of claims to the claimsPrincipal
                var claimsIdentity = new ClaimsIdentity();
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                p.AddIdentity(claimsIdentity);
            }
            return Task.FromResult(p);
        }
    }
}
