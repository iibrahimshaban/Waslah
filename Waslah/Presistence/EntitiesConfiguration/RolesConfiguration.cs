
using Waslah.Abstraction.Consts;

namespace Waslah.Presistence.EntitiesConfiguration;

public class RolesConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
       [
           new ApplicationRole
            {
                Id = DefaultRoles.AdminRoleId,
                ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
                Name = DefaultRoles.Admin,
                NormalizedName = DefaultRoles.Admin.ToUpper()
            },
            new ApplicationRole
            {
                Id = DefaultRoles.MemberRoleId,
                Name = DefaultRoles.Member,
                ConcurrencyStamp= DefaultRoles.MemberRoleConcurrencyStamp,
                NormalizedName= DefaultRoles.Member.ToUpper(),
                IsDefault = true
            }
       ]);
    }
}
