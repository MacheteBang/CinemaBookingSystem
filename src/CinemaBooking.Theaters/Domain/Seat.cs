namespace CinemaBooking.Theaters.Domain;

public class Seat : Entity
{
    public OccupancyState Occupancy { get; set; } = OccupancyState.Vacant;
    public DateTime? OccupancyStateExpiration { get; set; } = null;
    public required string Row { get; set; }
    public required ushort Number { get; set; }

    public bool IsAvailable => Occupancy == OccupancyState.Vacant
        && (OccupancyStateExpiration ?? DateTime.MinValue) < DateTime.UtcNow;

    public Result Reserve(ushort pendingTimeoutInSeconds)
    {
        if (Occupancy != OccupancyState.Vacant) return SeatError.InvalidAction;

        OccupancyStateExpiration = DateTime.UtcNow.AddSeconds(pendingTimeoutInSeconds);

        return Result.Success();
    }
    public Result Release()
    {
        Occupancy = OccupancyState.Vacant;
        OccupancyStateExpiration = null;
        return Result.Success();
    }
    public Result Confirm()
    {
        if (Occupancy == OccupancyState.Vacant && OccupancyStateExpiration > DateTime.UtcNow) return SeatError.InvalidAction;

        Occupancy = OccupancyState.Confirmed;
        OccupancyStateExpiration = null;

        return Result.Success();
    }
}