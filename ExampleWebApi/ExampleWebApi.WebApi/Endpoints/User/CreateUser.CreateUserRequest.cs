namespace ExampleWebApi.WebApi.Endpoints.User;

public class CreateUserRequest
{
    public string Name { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Email { get; set; }
}

