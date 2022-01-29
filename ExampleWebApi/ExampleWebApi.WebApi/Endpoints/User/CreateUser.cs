using ExampleWebApi.SharedKernel.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExampleWebApi.WebApi.Endpoints.User;

[ApiController]
[Route("api")]
public class CreateUser : ControllerBase
{
    public CreateUser(UserService userService) { }

    [HttpPost("users")]
    [SwaggerOperation(Summary = "Creates an user", Description = "Creates an user", OperationId = "User.Create", Tags = new[] { "UserEndpoint" })]
    public async Task<IActionResult> HandleAsync(CreateUserRequest user, CancellationToken cancellationToken = default)
    {
        //UserService.createUser();
        return NoContent();
    }
}

