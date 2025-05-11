using Microsoft.Data.SqlClient;

namespace Waslah.Services;

public class AgencyTripService(ApplicationDbContext context, IWebHostEnvironment environment) : IAgencyTripService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IWebHostEnvironment _webEnvironment = environment;

    public async Task<IEnumerable<AgencyResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.AgenciesTrip
            .Where(x => !x.IsDiasabled)
            .ProjectToType<AgencyResponse>()
            .ToListAsync(cancellationToken);
    }
    public async Task<Result<AgencyResponse>> GetByIdAsync(int Id, CancellationToken cancellationToken)
    {
        var Trip = await _context.AgenciesTrip
            .Where(x => x.Id == Id)
            .ProjectToType<AgencyResponse>()
            .FirstOrDefaultAsync(cancellationToken);

        if (Trip == null)
            return Result.Failure<AgencyResponse>(AgencyTripErrors.TripNotFound);

        return Result.Success(Trip);
    }

    public async Task<Result<AgencyResponse>> CreateAsync(AgencyRequest request, CancellationToken cancellationToken)
    {
        var relativePath = string.Empty;

        var trip = new AgencyTrip
        {
            AgencyName = request.CompanyName,
            PickUpCity = request.PickUpCity,
            DestinationCity = request.DestinationCity,
            PickUpDate = request.PickUpDate,
            Price = request.Price,
            NumberOfDays = request.NumberOfDays,
            Details = request.Details,
            PhoneNumber = request.PhoneNumber,
            PhotoPath = relativePath,
            IsDiasabled = false,
            Links = request.RelatedLinks.Select(linkDto => new AgencyLink
            {
                Name = linkDto.Name,
                Url = linkDto.Url
            }).ToList()
        };

        try
        {
            await _context.AgenciesTrip.AddAsync(trip, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            return Result.Failure<AgencyResponse>(AgencyTripErrors.DoublicatedTrip);
        }

        return Result.Success(trip.Adapt<AgencyResponse>());
    }
    public async Task<Result> UpdateAsync(int Id , AgencyRequest request, CancellationToken cancellationToken)
    {
        var trip = await _context.AgenciesTrip.FirstOrDefaultAsync(trip => trip.Id == Id,cancellationToken);

        if (trip == null)
            return Result.Failure(AgencyTripErrors.TripNotFound);

        var TripExists = await _context.AgenciesTrip
            .Where(x => x.AgencyName.ToLower().Trim() == request.CompanyName.ToLower().Trim() && 
                x.PickUpCity.ToLower().Trim() == request.PickUpCity.ToLower().Trim() &&
                x.DestinationCity.ToLower().Trim() == request.DestinationCity.ToLower().Trim() && 
                x.PickUpDate == request.PickUpDate && 
                x.Id != Id).AnyAsync(cancellationToken);

        if (TripExists)
            return Result.Failure(AgencyTripErrors.DoublicatedTrip);

        trip.AgencyName = request.CompanyName;
        trip.PickUpCity = request.PickUpCity;
        trip.DestinationCity = request.DestinationCity;
        trip.Price = request.Price;
        trip.NumberOfDays = request.NumberOfDays;
        trip.PhoneNumber = request.PhoneNumber;
        trip.Details = request.Details;
        trip.PickUpDate = request.PickUpDate;

        await _context.Links
            .Where(x => x.AgencyId == trip.Id)
            .ExecuteDeleteAsync(cancellationToken);

        var NewLinks = request.RelatedLinks.Select(x => new AgencyLink
        {
            AgencyId = trip.Id,
            Name = x.Name,
            Url = x.Url,
        });
        
        await _context.Links.AddRangeAsync(NewLinks,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }
    public async Task<Result> UpdateAgencyPhotoAsync(int Id,AgencyPhotoRequest request,CancellationToken cancellationToken)
    {
        var trip = await _context.AgenciesTrip.FirstOrDefaultAsync(trip => trip.Id == Id, cancellationToken);

        if (trip == null)
            return Result.Failure(AgencyTripErrors.TripNotFound);

        var relativePath = string.Empty;

        if (request.Photo is not null)
        {
            DeleteOldProfilePhoto(trip.PhotoPath);

            // Now upload the new photo
            var uploadsFolder = Path.Combine(_webEnvironment.WebRootPath, "Agency-Trip-photos");
            Directory.CreateDirectory(uploadsFolder); // Make sure the folder exists

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Photo.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Photo.CopyToAsync(stream, cancellationToken);
            }

            relativePath = $"/Agency-Trip-photos/{uniqueFileName}";

        }
        else if (request.Photo is null)
            DeleteOldProfilePhoto(trip.PhotoPath);

        trip.PhotoPath = relativePath;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result> ToggleStatusAsync(int Id,CancellationToken cancellationToken)
    {
        if ( await _context.AgenciesTrip.FindAsync(Id,cancellationToken) is not { } Trip)
            return Result.Failure(AgencyTripErrors.CompanyNotFound);

        Trip.IsDiasabled = !Trip.IsDiasabled;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private void DeleteOldProfilePhoto(string? photoPath)
    {
        if (string.IsNullOrWhiteSpace(photoPath)) return;

        var fullPath = Path.Combine(_webEnvironment.WebRootPath, photoPath.TrimStart('/'));
        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }
    }
}
