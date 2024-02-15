namespace CinemaBooking.Theaters.Domain;

public class Seat
{
    public required Guid Id { get; set; }
    public OccupancyState Occupancy { get; set; } = OccupancyState.Vacant;
    public required string Row { get; set; }
    public required ushort Number { get; set; }

    public Result Reserve()
    {
        if (Occupancy != OccupancyState.Vacant)
        {
            // FIX THIS
            return Result.Failure(new Error(""));
        }

        Occupancy = OccupancyState.Pending;

        return Result.Success();
    }
    public Result Release()
    {
        Occupancy = OccupancyState.Vacant;
        return Result.Success();
    }
    public Result Confirm()
    {
        if (Occupancy != OccupancyState.Pending)
        {
            // FIX THIS
            return Result.Failure(new Error(""));
        }

        Occupancy = OccupancyState.Reserved;

        return Result.Success();
    }

    public enum OccupancyState
    {
        Vacant = 1,
        Pending = 2,
        Reserved = 3
    }

    public enum OccupancyAction
    {
        Reserve = 1,
        Release = 2,
        Confirm = 3
    }

}