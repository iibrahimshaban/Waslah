using Waslah.Abstraction.Consts;

namespace Waslah.Presistence.EntitiesConfiguration;

public class RoleClaimsConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        var permissions = Permissions.GetAllPermissions();
        var adminPermissions = new List<IdentityRoleClaim<string>>();

        for (int i = 0; i < permissions.Count; i++)
        {
            adminPermissions.Add(new IdentityRoleClaim<string>
            {
                ClaimType = Permissions.Type,
                ClaimValue = permissions[i],
                Id = i + 1,
                RoleId = DefaultRoles.AdminRoleId
            });
        }
        builder.HasData(adminPermissions);
    }
}
