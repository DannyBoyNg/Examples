#nullable disable

namespace JwtAuthenticationApi.Models
{
    public partial class SimpleToken
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
