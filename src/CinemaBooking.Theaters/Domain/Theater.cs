namespace CinemaBooking.Theaters.Domain;

public class Theater : Entity
{
    public required string Name { get; set; }
    public required List<Seat> Seats { get; set; }
}