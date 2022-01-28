using System.Security.Claims;

namespace ExampleWebApi.SharedKernel.Modules.Jwt
{
    public interface IJwtService
    {
        string GenerateJwtAccessToken(IEnumerable<Claim> claims);
        string GenerateJwtRefreshToken();
        bool IsRefreshTokenExpired(string token);
        DateTime GetCreationTimeFromRefreshToken(string token);
        string? GetClaim(ClaimsPrincipal claimsIdentity, string claimType);
        ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string token);
    }
}
