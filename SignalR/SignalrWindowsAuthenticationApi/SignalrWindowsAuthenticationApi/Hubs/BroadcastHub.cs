using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalrWindowsAuthenticationApi.Models;
using System;
using System.Threading.Tasks;

namespace SignalrWindowsAuthenticationApi.Hubs
{
    [Authorize]
    public class BroadcastHub : Hub
    {
        public BroadcastHub() { }

        public Task SubmitMessage(ChatMessage message)
        {
            message.TimeStamp = DateTime.UtcNow;
            message.UserName = Context.User?.Identity?.Name ?? "[unknown]";
            return Clients.All.SendAsync("BroadcastMessage", message);
        }
    }
}
