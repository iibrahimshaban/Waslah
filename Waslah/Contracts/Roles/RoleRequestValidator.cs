namespace Waslah.Contracts.Roles;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    public RoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role Name is required")
            .Length(3,50).WithMessage("Name must be between 3 & 50 characters");

        RuleFor(x => x.Permissions)
            .NotEmpty().NotNull().WithMessage("Role need at least one permission");

        RuleFor(x => x.Permissions)
            .Must(x => x.Distinct().Count() == x.Count()).WithMessage("doblicated permission for the same role")
            .When(x => x.Permissions is not null);   

    }
}
