namespace ExampleWebApi.WebApi.Endpoints.Token;

public class CreateTokenRequest
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
