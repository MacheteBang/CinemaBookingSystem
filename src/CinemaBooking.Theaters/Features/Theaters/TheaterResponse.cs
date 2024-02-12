namespace CinemaBooking.Theaters.Features.Theaters;

public class TheaterResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? Name { get; set; }

    public List<Seat>? Seats { get; set; }
}