namespace Waslah.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } =string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<RefreshToken> RefreshTokens { get; set; } = [];
        public ICollection<ChainedRoute> ChainedRoutes { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<UserPoints> Points { get; set; } = [];
        public ICollection<Report> Reports { get; set; } = [];
        public ICollection<MyRoute> Routes { get; set; } = [];
    }
}
