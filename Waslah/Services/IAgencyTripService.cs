namespace Waslah.Services;

public interface IAgencyTripService
{
    Task<IEnumerable<AgencyResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<AgencyResponse>> GetByIdAsync(int Id,CancellationToken cancellationToken);
    Task<Result<AgencyResponse>> CreateAsync(AgencyRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(int Id, AgencyRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateAgencyPhotoAsync(int Id, AgencyPhotoRequest request, CancellationToken cancellationToken);
    Task<Result> ToggleStatusAsync(int Id, CancellationToken cancellationToken);
}
