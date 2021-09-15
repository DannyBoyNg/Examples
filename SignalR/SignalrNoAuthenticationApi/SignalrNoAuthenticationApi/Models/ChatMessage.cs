using System;

namespace SignalrNoAuthenticationApi.Models
{
    public class ChatMessage
    {
        public string UserName { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime TimeStamp { get; set; }
    }
}
