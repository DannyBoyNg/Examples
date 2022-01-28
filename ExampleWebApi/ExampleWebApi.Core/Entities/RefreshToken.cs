namespace ExampleWebApi.Core.Entities
{
    public partial class RefreshToken
    {
        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
