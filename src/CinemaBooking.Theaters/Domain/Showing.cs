namespace CinemaBooking.Theaters.Domain;

public class Showing
{
    public required Guid Id { get; set; }
    public required Guid TheaterId { get; set; }
    public required Guid MovieId { get; set; }
    public required DateTime Showtime { get; set; }
}