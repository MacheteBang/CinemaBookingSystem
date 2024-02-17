namespace CinemaBooking.Theaters.Contracts;

public class TheaterResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required List<Seat> Seats { get; set; }
}