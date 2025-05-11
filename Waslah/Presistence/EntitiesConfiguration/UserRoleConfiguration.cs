using Waslah.Abstraction.Consts;

namespace Waslah.Presistence.EntitiesConfiguration;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(new IdentityUserRole<string>
        {
            UserId = DefaultUsers.UserId,
            RoleId = DefaultRoles.AdminRoleId
        });
    }
}
