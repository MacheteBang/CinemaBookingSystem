namespace CinemaBooking.Theaters.Features.Showings;

public class ShowingResponse
{
    public required Guid Id { get; set; }
    public required Guid TheaterId { get; set; }
    public required Guid MovieId { get; set; }
    public required DateTime Showtime { get; set; }
    public required List<Seat> Seats { get; set; }
}