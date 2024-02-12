namespace CinemaBooking.Theaters.Features.Theaters;

public class TheaterResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? SeatingArrangement { get; set; }
}