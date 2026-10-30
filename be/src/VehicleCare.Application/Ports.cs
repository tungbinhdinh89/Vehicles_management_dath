namespace VehicleCare.Application;

// Business ports will be added with their first use case; do not expose DbContext or IQueryable here.
public interface IBusinessClock
{
    DateOnly Today { get; }
    DateTimeOffset UtcNow { get; }
}
