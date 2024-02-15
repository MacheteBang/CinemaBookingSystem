namespace CinemaBooking.Theaters.Contracts;

public class SeatResponse
{
    public required Guid Id { get; set; }
    public required Seat.OccupancyState Occupancy { get; set; }
    public required string Row { get; set; }
    public required ushort Number { get; set; }
}