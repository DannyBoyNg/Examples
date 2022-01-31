using ExampleWebApi.SharedKernel.Modules.Jwt;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExampleWebApi.WebApi.Endpoints.Token;

[ApiController]
public class CreateToken : ControllerBase
{
    private readonly IJwtService jwtService;

    public CreateToken(IJwtService jwtService)
    {
        this.jwtService = jwtService;
    }

    [HttpPost("token")]
    public async Task<IActionResult> HandleAsync(CreateTokenRequest r, CancellationToken cancellationToken)
    {
        var claims = new List<Claim> {
            new Claim("claim1","value1"),
            new Claim("claim2","value2"),
            new Claim("claim3","value3"),
        };

        var t = r;
        return Ok(jwtService.GenerateJwtAccessToken(claims));
    }

}
