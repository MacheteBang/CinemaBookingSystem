namespace CinemaBooking.Theaters.Contracts;

public class SeatResponse
{
    public required Guid Id { get; set; }
    public required OccupancyState Occupancy { get; set; }
    public DateTime? OccupancyStateExpiration { get; set; } = null;
    public bool IsAvailable { get; set; }
    public required string Row { get; set; }
    public required ushort Number { get; set; }
}