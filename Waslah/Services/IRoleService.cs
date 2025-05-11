namespace Waslah.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default);
    Task<Result<RoleDetailsResponse>> GetDetailsAsync(string roleId);
    Task<Result<RoleDetailsResponse>> CreateAsyc(RoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(string RoleId, RoleRequest request, CancellationToken cancellationToken);
    Task<Result> ToggleStatusAsync(string RoleId);
}
