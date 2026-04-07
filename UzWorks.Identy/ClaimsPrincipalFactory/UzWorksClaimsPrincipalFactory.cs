using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using UzWorks.Identity.Constants;
using UzWorks.Identity.Models;

namespace UzWorks.Identity.ClaimsPrincipalFactory;

public class UzWorksClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
{
    public UzWorksClaimsPrincipalFactory(
        UserManager<User> userManager, 
        IOptions<IdentityOptions> optionsAccessor) 
            : base(userManager, optionsAccessor) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        var claims = new[]
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            new Claim(ClaimNames.UserName, user.UserName),
            new Claim(ClaimNames.Email, user.Email),
            new Claim(ClaimNames.UserId, user.Id),
            new Claim(ClaimNames.FirstName, user.FirstName),
            new Claim(ClaimNames.LastName, user.LastName)
        };

        identity.AddClaims(claims);

        return identity;
    }
}
