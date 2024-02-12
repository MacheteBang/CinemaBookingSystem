namespace CinemaBooking.Theaters.Domain;

public class Theater
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required List<Seat> Seats { get; set; }
}