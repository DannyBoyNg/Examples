using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SignalrJwtAuthenticationApi.Exceptions;
using SignalrJwtAuthenticationApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SignalrJwtAuthenticationApi.Controllers
{
    public class TokenController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public TokenController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost]
        [Route("Token")]
        public IActionResult Token([FromBody] GetTokenArgs args)
        {
            if (args.UserName == null) return BadRequest();
            try
            {
                var issuedAt = DateTime.UtcNow;
                var issuedAtUnix = ((DateTimeOffset)issuedAt).ToUnixTimeSeconds();
                var expiresAt = issuedAt.AddMinutes(60);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, issuedAtUnix.ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                    new Claim(ClaimTypes.Name, args.UserName),
                };
                if (args.Roles != null && args.Roles.Any()) foreach (var role in args.Roles) claims.Add(new Claim(ClaimTypes.Role, role));
                if (args.UserDefinedClaims != null && args.UserDefinedClaims.Any())
                {
                    claims.AddRange(args.UserDefinedClaims.Where(x => IsUserDefinedClaim(x.Key)).Select(x => new Claim(x.Key, x.Value)));
                }

                var key = Configuration["JwtAuth:key"] ?? throw new EncryptionKeyNotSetException();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var token = new JwtSecurityToken(
                  issuer: Configuration["JwtAuth:issuer"],
                  audience: Configuration["JwtAuth:audience"],
                  claims: claims,
                  notBefore: issuedAt,
                  expires: expiresAt,
                  signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );
                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(accessToken);
            }
            catch (ArgumentOutOfRangeException) { throw new EncryptionKeyIsTooShortException(); }
        }

        private static bool IsUserDefinedClaim(string x)
        {
            return x != JwtRegisteredClaimNames.Jti
            && x != JwtRegisteredClaimNames.Iat
            && x != JwtRegisteredClaimNames.Nbf
            && x != JwtRegisteredClaimNames.Exp
            && x != JwtRegisteredClaimNames.Iss
            && x != JwtRegisteredClaimNames.Aud
            && x != ClaimTypes.Name
            && x != ClaimTypes.Role;
        }
    }
}
