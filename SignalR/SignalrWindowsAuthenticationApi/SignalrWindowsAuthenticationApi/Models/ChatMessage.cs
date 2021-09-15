using System;

namespace SignalrWindowsAuthenticationApi.Models
{
    public class ChatMessage
    {
        public string UserName { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime TimeStamp { get; set; }
    }
}
