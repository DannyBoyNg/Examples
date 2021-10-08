#nullable disable

namespace JwtAuthenticationApi.Models
{
    public partial class RefreshToken
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }
}
