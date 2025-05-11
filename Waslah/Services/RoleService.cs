namespace Waslah.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default)
    {
        var roles = await _roleManager.Roles
            .Where(x => !x.IsDefault && (!x.IsDisabled || (includeDisabled.HasValue && includeDisabled.Value)))
            .AsNoTracking()
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);

        return roles;
    }
    public async Task<Result<RoleDetailsResponse>> GetDetailsAsync(string roleId)
    {
        if (await _roleManager.FindByIdAsync(roleId) is not { } Role)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

        var rolePermissions = await _roleManager.GetClaimsAsync(Role);

        var response = new RoleDetailsResponse(
            Role.Id,
            Role.Name!,
            Role.IsDisabled,
            rolePermissions.Select(x => x.Value)
            );

        return Result.Success(response);
    }
    public async Task<Result<RoleDetailsResponse>> CreateAsyc(RoleRequest request, CancellationToken cancellationToken = default)
    {
        var roleExists = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name, cancellationToken);

        if (roleExists)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.DoublicatedName);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailsResponse>(RoleErrors.InvalidPermissions);


        var Role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = "01967cc2-5bee-7bf5-93db-c5fed9b450fa",
            NormalizedName = request.Name.ToUpper(),
        };

        var result = await _roleManager.CreateAsync(Role);

        if (result.Succeeded)
        {
            var rolePermissions = request.Permissions
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = Role.Id,
                });

            await _context.AddRangeAsync(rolePermissions, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new RoleDetailsResponse(Role.Id, Role.Name, Role.IsDisabled, request.Permissions);

            return Result.Success(response);

        }
        var error = result.Errors.FirstOrDefault();

        return Result.Failure<RoleDetailsResponse>(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> UpdateAsync(string RoleId, RoleRequest request, CancellationToken cancellationToken)
    {
        if (await _roleManager.FindByIdAsync(RoleId) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);

        var NameExists = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != RoleId, cancellationToken);

        if (NameExists)
            return Result.Failure(RoleErrors.DoublicatedName);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure(RoleErrors.InvalidPermissions);


        role.Name = request.Name;
        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            var CurrentPermissions = await _context.RoleClaims
                .Where(x => x.RoleId == role.Id && x.ClaimType == Permissions.Type)
                .Select(x => x.ClaimValue)
                .ToListAsync(cancellationToken);

            var NewPermissions = request.Permissions.Except(CurrentPermissions)
                .Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                });

            var RemovedPermissions = CurrentPermissions.Except(request.Permissions);

            await _context.RoleClaims
                .Where(x => x.RoleId == role.Id && RemovedPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync(cancellationToken);

            await _context.RoleClaims.AddRangeAsync(NewPermissions, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        var error = result.Errors.FirstOrDefault();

        return Result.Failure(new Error(error!.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<Result> ToggleStatusAsync(string RoleId)
    {
        if (await _roleManager.FindByIdAsync(RoleId) is not { } Role)
            return Result.Failure(RoleErrors.RoleNotFound);

        Role.IsDisabled = !Role.IsDisabled;
        await _roleManager.UpdateAsync(Role);

        return Result.Success();
    }
}
