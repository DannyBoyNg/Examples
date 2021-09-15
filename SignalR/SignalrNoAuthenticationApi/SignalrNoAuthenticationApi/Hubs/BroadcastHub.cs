using Microsoft.AspNetCore.SignalR;
using SignalrNoAuthenticationApi.Models;
using System;
using System.Threading.Tasks;

namespace SignalrNoAuthenticationApi.Hubs
{
    public class BroadcastHub : Hub
    {
        public BroadcastHub() { }

        public Task SubmitMessage(ChatMessage message)
        {
            message.TimeStamp = DateTime.UtcNow;
            return Clients.All.SendAsync("BroadcastMessage", message);
        }
    }
}
