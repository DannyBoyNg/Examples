using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExampleWebApi.SharedKernel.Modules.Jwt
{
    public class JwtService : IJwtService
    {
        JwtSettings Settings { get; set; }

        public JwtService(IOptions<JwtSettings> settings)
        {
            Settings = settings?.Value ?? new JwtSettings();
            if (string.IsNullOrWhiteSpace(Settings.Key)) throw new JwtSecretKeyNotSetException();
        }

        public string GenerateJwtAccessToken(IEnumerable<Claim> claims)
        {
            var issuedAt = DateTime.UtcNow;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Key!));
            var token = new JwtSecurityToken(
              issuer: Settings.Issuer,
              audience: Settings.Audience,
              claims: claims,
              notBefore: issuedAt,
              expires: issuedAt.AddMinutes(Settings.AccessTokenDurationInMinutes),
              signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateJwtRefreshToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray()).Replace('/','_').Replace('+', '-');
        }

        public bool IsRefreshTokenExpired(string token)
        {
            DateTime when = GetCreationTimeFromRefreshToken(token);
            return when < DateTime.UtcNow.AddHours(Settings.RefreshTokenDurationInHours * -1);
        }

        public DateTime GetCreationTimeFromRefreshToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));
            token = token.Replace('_', '/').Replace('-', '+');
            switch (token.Length % 4)
            {
                case 2: token += "=="; break;
                case 3: token += "="; break;
            }
            byte[] data = Convert.FromBase64String(token);
            return DateTime.FromBinary(BitConverter.ToInt64(data, 0));
        }

        public string? GetClaim(ClaimsPrincipal claimsIdentity, string claimType)
        {
            if (claimsIdentity == null) throw new ArgumentNullException(nameof(claimsIdentity));
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);
            if (claim != null)
            {
                return claim.Value;
            }
            return null;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Key!)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid access token");

            return principal;
        }

    }
}
