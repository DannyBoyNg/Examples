using ExampleWebApi.SharedKernel.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExampleWebApi.WebApi.Endpoints.User;

[ApiController]
[Route("api")]
public class CreateUser : ControllerBase
{
    public CreateUser(UserService userService) { }

    [HttpPost("users")]
    public async Task<IActionResult> HandleAsync(CreateUserRequest user, CancellationToken cancellationToken = default)
    {
        //UserService.createUser();
        return Ok();
    }
}

