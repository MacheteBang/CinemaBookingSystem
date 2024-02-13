namespace CinemaBooking.Theaters.Features.Showings.Seats;

public class SeatResponse
{
    public required Guid Id { get; set; }
    public required SeatOccupancy Occupancy { get; set; }
    public required string Row { get; set; }
    public required ushort SeatNumber { get; set; }
}