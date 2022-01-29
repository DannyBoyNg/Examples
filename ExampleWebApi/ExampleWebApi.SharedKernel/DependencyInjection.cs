using ExampleWebApi.SharedKernel.Modules.Jwt;
using ExampleWebApi.SharedKernel.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ExampleWebApi.SharedKernel;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernelServices(this IServiceCollection services, IConfiguration configuration)
    {
        //Add and configure JWT authentication
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = configuration["JwtSettings:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])),
            ValidateLifetime = true,
            SaveSigninToken = true,
        };
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = tokenValidationParameters);
        services.AddScoped<IJwtService, JwtService>();
        services.Configure<JwtSettings>(options => options.TokenValidationParameters = tokenValidationParameters);

        //Add other services
        services.AddScoped<UserService>();

        return services;
    }
}
