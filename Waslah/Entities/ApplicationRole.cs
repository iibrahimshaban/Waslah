namespace Waslah.Entities;

public class ApplicationRole : IdentityRole
{
    public bool IsDefault { get; set; }
    public bool IsDisabled { get; set; }
}
