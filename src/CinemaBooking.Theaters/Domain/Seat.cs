namespace CinemaBooking.Theaters.Domain;

public class Seat
{
    public required Guid Id { get; set; }
    public SeatOccupancy Occupancy { get; set; } = SeatOccupancy.Vacant;
    public required string Row { get; set; }
    public required ushort SeatNumber { get; set; }
}