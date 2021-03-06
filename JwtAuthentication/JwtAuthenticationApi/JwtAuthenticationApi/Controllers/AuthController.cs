using Ng.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Services.UserServ;
using System.Threading;
using JwtAuthenticationApi.Models;
using System.Globalization;

namespace JwtAuthenticationApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/user/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IJwtTokenService jwtTokenService;
        private readonly IPasswordHashingService hashingService;

        public AuthController(
            IUserService userService,
            IJwtTokenService jwtTokenService,
            IPasswordHashingService hashingService)
        {
            this.userService = userService;
            this.jwtTokenService = jwtTokenService;
            this.hashingService = hashingService;
        }

        [Route("/Token")]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Token(string username, string password)
        {
            //sleep timer to demonstrate the loading bar
            Thread.Sleep(2000);
            //get user from data store
            var user = userService.GetByName(username);
            //validate password
            if (user == null) return BadRequest("Invalid credentials");
            if (user.Active != true) return BadRequest("Account not active");
            if (hashingService.VerifyHashedPassword(user.PasswordHash, password) == PasswordVerificationResult.Failed) return BadRequest("Invalid credentials");
            //create claims
            var claims = new List<Claim> { new Claim("uid", user.Id.ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer) };
            //create tokens
            var accessToken = jwtTokenService.GenerateAccessToken(user.UserName, null, claims);
            var refreshToken = jwtTokenService.GenerateRefreshToken();
            //store refresh token
            jwtTokenService.StoreRefreshToken(user.Id, refreshToken);
            return Ok(new JwtToken { AccessToken = accessToken, RefreshToken = refreshToken, TokenType = "bearer" });
        }

        [Route("/Refresh")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Refresh(string accessToken, string refreshToken)
        {
            //get userId from access token
            var claimsPrincipal = jwtTokenService.GetClaimsFromExpiredAccessToken(accessToken);
            var uid = jwtTokenService.GetClaim(claimsPrincipal, "uid");
            if (!int.TryParse(uid, out int userId)) return Unauthorized("Invalid access token");
            //validate refresh token (Optionally delete refreshToken after validation)
            jwtTokenService.ValidateRefreshToken(userId, refreshToken);
            //get user from data store
            var user = userService.GetById(userId);
            if (user == null) return NotFound("User not found");
            //create new tokens
            var claims = new List<Claim> { new Claim("uid", uid) };
            var newAccessToken = jwtTokenService.GenerateAccessToken(user.UserName, null, claims);
            var newRefreshToken = jwtTokenService.GenerateRefreshToken();
            //store refresh token in data store
            jwtTokenService.StoreRefreshToken(userId, newRefreshToken);
            return Ok(new JwtToken { AccessToken = newAccessToken, RefreshToken = newRefreshToken, TokenType = "bearer" });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(string username, string email, string password)
        {
            //Input validation
            var errors = new List<string>();
            if (!userService.IsEmailUnique(email)) errors.Add("Email already exists");
            if (!userService.IsNameUnique(username)) errors.Add("Username already exists");
            if (errors.Any()) return BadRequest(errors);
            //Create new user
            var passwordHash = hashingService.HashPassword(password);
            IUser user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = passwordHash,
                Active = true,
            };
            userService.Create(user);
            //Send email to confirm email
            // ...
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(int userId, string token, string password, string confirmPassword)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            //Validate token, if valid continue
            //...
            var user = userService.GetById(userId);
            if (user == null) return BadRequest();
            if (user.Active == false) return BadRequest("Account is not active.");
            if (password != confirmPassword) return BadRequest("Password is not equal to confirm password.");
            userService.UpdatePassword(userId, hashingService.HashPassword(password));
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotUsername(string email)
        {
            var user = userService.GetByEmail(email);
            if (user == null) return NotFound();
            if (user.Active == false) return BadRequest("Unable to retrieve username. Account is not active.");
            //Send Email with the username
            //...
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(string email)
        {
            var user = userService.GetByEmail(email);
            if (user == null) return NotFound();
            if (user.Active == false) return BadRequest("Cannot reset password. Account is not active.");
            //Send an email with a reset link to the user
            //...
            return NoContent();
        }

        [HttpGet("{userId}/{token}")]
        [AllowAnonymous]
        public ActionResult ConfirmEmail(int userId, string token)
        {
            if (userService.IsEmailConfirmed(userId)) return BadRequest("Your email has already been confirmed");
            //Validate token, if valid set email confirmed to true in database
            userService.SetEmailConfirmed(userId);
            return NoContent();
        }

        [HttpPost]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var user = userService.GetByName(User?.Identity?.Name ?? "");
            if (newPassword != confirmPassword) return BadRequest("New password and confirm password are not equal.");
            if (user == null) return BadRequest("User not found.");
            if (!user.Active) return BadRequest("User is not active.");
            if (hashingService.VerifyHashedPassword(user.PasswordHash, currentPassword) == PasswordVerificationResult.Failed) return BadRequest("Current password is not correct.");
            userService.UpdatePassword(user.Id, hashingService.HashPassword(newPassword));
            return NoContent();
        }

    }
}
