using Microsoft.AspNetCore.SignalR;

namespace SignalrWindowsAuthenticationApi.WindowsAuth
{
    //This class handles authentication
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity?.Name ?? "";
        }
    }
}
