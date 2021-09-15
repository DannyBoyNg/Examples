using System.Collections.Generic;

namespace SignalrJwtAuthenticationApi.Models
{
    public class GetTokenArgs
    {
        public string? UserName { get; set; }
        public string[]? Roles { get; set; }
        public Dictionary<string, string>? UserDefinedClaims { get; set; }
    }
}
