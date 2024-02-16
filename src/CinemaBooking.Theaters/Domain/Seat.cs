namespace CinemaBooking.Theaters.Domain;

public class Seat
{
    public required Guid Id { get; set; }
    public OccupancyState Occupancy { get; set; } = OccupancyState.Vacant;
    public DateTime? OccupancyStateExpiration { get; set; } = null;
    public required string Row { get; set; }
    public required ushort Number { get; set; }

    public Result Reserve(ushort pendingTimeoutInSeconds)
    {
        if (Occupancy != OccupancyState.Vacant) return SeatError.InvalidAction;

        Occupancy = OccupancyState.Pending;
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
        if (Occupancy != OccupancyState.Pending) return SeatError.InvalidAction;

        Occupancy = OccupancyState.Reserved;
        OccupancyStateExpiration = null;

        return Result.Success();
    }

    public enum OccupancyState
    {
        Vacant,
        Pending,
        Reserved
    }

    public enum OccupancyAction
    {
        Reserve,
        Release,
        Confirm
    }
}