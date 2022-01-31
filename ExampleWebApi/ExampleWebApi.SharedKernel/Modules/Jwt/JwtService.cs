using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ExampleWebApi.SharedKernel.Modules.Jwt
{
    public class JwtService : IJwtService
    {
        JwtSettings Settings { get; set; }

        public JwtService(IOptions<JwtSettings> settings)
        {
            Settings = settings?.Value ?? new JwtSettings();
            if (Settings.TokenValidationParameters?.IssuerSigningKey == null) throw new JwtSecretKeyNotSetException();
        }

        public string GenerateJwtAccessToken(IEnumerable<Claim> claims)
        {
            var issuedAt = DateTime.UtcNow;
            var key = Settings.TokenValidationParameters?.IssuerSigningKey;
            var tokenDescriptor = new SecurityTokenDescriptor {
              Issuer = Settings.TokenValidationParameters?.ValidIssuer,
              Audience = Settings.TokenValidationParameters?.ValidAudience,
              Subject = new ClaimsIdentity(claims),
              NotBefore = issuedAt,
              Expires = issuedAt.AddMinutes(Settings.AccessTokenExpirationInMinutes),
              SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
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
            return when < DateTime.UtcNow.AddHours(Settings.RefreshTokenExpirationInHours * -1);
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
                IssuerSigningKey = Settings.TokenValidationParameters?.IssuerSigningKey,
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken 
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid access token");

            return claimsPrincipal;
        }

    }
}
